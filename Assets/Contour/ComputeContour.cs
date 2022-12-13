using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class ComputeContour : MonoBehaviour
{
    struct ResultBufferStruct : ISegment
    {
        public Vector3 start { set; get; }
        public Vector3 end { set; get; }
        public int containsSegment;
    }

    // Start is called before the first frame update
    MeshFilter mf;
    public ComputeShader cs;
    private int kernelId;
    private ComputeBuffer triBuffer;
    private ComputeBuffer vertBuffer;
    private ComputeBuffer normalBuffer;


    private ComputeBuffer resultBuffer;

    int containsSegment;
    void Start()
    {
        mf = GetComponent<MeshFilter>();

        if (mf == null)
        {
            Debug.LogError("No MeshFilter found");
        }


        kernelId = cs.FindKernel("GenerateContourSegments");

        //create Buffers
        triBuffer = new(mf.mesh.triangles.Length, sizeof(int));
        vertBuffer = new(mf.mesh.vertices.Length, sizeof(float) * 3);
        normalBuffer = new(mf.mesh.normals.Length, sizeof(float) * 3);
        resultBuffer = new(mf.mesh.triangles.Length / 3, sizeof(int) + sizeof(float) * 3 * 2);


        //set buffer data
        triBuffer.SetData(mf.mesh.triangles);
        vertBuffer.SetData(mf.mesh.vertices);
        normalBuffer.SetData(mf.mesh.normals);

        //assign buffers to the compute shader
        cs.SetBuffer(kernelId, "triangles", triBuffer);
        cs.SetBuffer(kernelId, "vertices", vertBuffer);
        cs.SetBuffer(kernelId, "normals", normalBuffer);
        cs.SetBuffer(kernelId, "results", resultBuffer);

        cs.SetInt("numTriangles", triBuffer.count / 3);

        Debug.Log(triBuffer.stride);
        Debug.Log(vertBuffer.stride);
        Debug.Log(resultBuffer.stride);

    }

    // Update is called once per frame

    private BentleyOttmann.BentleyOttman bo;
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.Space))
        {
            CalcContourSegments();


            bo = new BentleyOttmann.BentleyOttman(contours);

        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            bo.testStep();
        }
        if (Input.GetKey(KeyCode.RightShift))
        {
            if (bo != null)
            {
                for (int i = 0; i < 300; i++)
                    bo.testStep();
            }
        }
        
        //CalcContourSegments();
    }


    void CalcContourSegments()
    {
        uint threadGroupSizes;


        int numTriangles = triBuffer.count / 3;

        cs.SetVector("cameraPos", transform.InverseTransformPoint(Camera.main.transform.position));
        cs.GetKernelThreadGroupSizes(kernelId, out threadGroupSizes, out _, out _);

        int numGroups = 1 + (int)(numTriangles / threadGroupSizes);

        cs.Dispatch(kernelId, numGroups, 1, 1);
        ResultBufferStruct[] results = new ResultBufferStruct[numTriangles];
        resultBuffer.GetData(results);

        contours = results.Where(x => x.containsSegment != 0).Cast<ISegment>().ToList();


    }
    private List<ISegment> contours;


    private void OnDrawGizmos()
    {
        if (contours != null)
        {
            Handles.matrix = transform.localToWorldMatrix;
            foreach (var c in contours)
            {
                Handles.color = Color.black;
                Handles.DrawLine(c.start, c.end);
            }

        }

        if (bo != null)
        {
            bo.debugDraw();
        }

    }

    private void OnDestroy()
    {
        vertBuffer.Release();
        triBuffer.Release();
        normalBuffer.Release();
        resultBuffer.Release();
    }

}
