using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BoundingVolumeHierarchy;
using UnityEditor;

public class IntersectionTester : MonoBehaviour
{
    private GameObject lineRendererParent;
    public LineRenderer MyLineRenderer;
    List<List<Vector2>> segments = new();

    LineRenderer cursorLineRenderer;

    [SerializeField]
    private Color[] depthColours;

    BoundingVolumeHierarchy<SegmentAABB> bvh;

    // Start is called before the first frame update
    void Start()
    {
        bvh = new BoundingVolumeHierarchy<SegmentAABB>();
        lineRendererParent = gameObject.transform.Find("AllLineRenderers").gameObject;
        Random.InitState(5);
        for (int i = 0; i < 15; i++)
        {
            Vector2 a = Random.insideUnitCircle * 5;
            Vector2 b = Random.insideUnitCircle * 5;
            segments.Add(new List<Vector2> { a, b });
            bvh.Add(new SegmentAABB(a, b));
        }

        for (int i = 0; i < 7; i++)
        {
            Vector2 a = new Vector2(6, -1.5f + 0.5f * i);
            Vector2 b = a + new Vector2(1, 0) + Random.insideUnitCircle * new Vector2(0.5f, 4);

            segments.Add(new List<Vector2> { a, b });
            bvh.Add(new SegmentAABB(a, b));
        }
        for (int i = 0; i < 0; i++)
        {
            Vector2 a = Random.insideUnitCircle * new Vector2(1.5f, 0.1f) + new Vector2(-8, -3f + 0.5f * i);
            Vector2 b = a + Vector2.right * 2 + Random.insideUnitCircle * new Vector2(1, 0.1f);

            segments.Add(new List<Vector2> { a, b });
            bvh.Add(new SegmentAABB(a, b));
        }
        Vector2 av = new Vector2(6.5f, -3);
        Vector2 bv = new Vector2(6.5f, 3);
        segments.Add(new List<Vector2> { av, bv });
        bvh.Add(new SegmentAABB(av, bv));
        //segments.Add(new List<Vector2> { new Vector2(0, 0), new Vector2(1, -1) });
        //segments.Add(new List<Vector2> { new Vector2(1, 1), new Vector2(2, 1), new Vector2(2, 2) });
        cursorLineRenderer = Instantiate(MyLineRenderer);
        cursorLineRenderer.transform.parent = transform;
        //cursorLineRenderer.SetPositions(new Vector3[]{ Vector2.zero,Vector2.zero}); 

        updateLineRenderers();
    }

    // Update is called once per frame
    Vector2 pressStartRmb;
    Vector2 pressStartLmb;

    BentleyOttmann.BentleyOttman bo;

    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            pressStartRmb = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1))
        {
            cursorLineRenderer.enabled = true;
            cursorLineRenderer.SetPosition(0, pressStartRmb);
            cursorLineRenderer.SetPosition(1, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        else
        {

            {
                cursorLineRenderer.enabled = false;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            Vector2 pressEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            segments.Add(new List<Vector2> { pressStartRmb, pressEnd });
            bvh.Add(new SegmentAABB(pressStartRmb, pressEnd));
        }

        if (Input.GetMouseButtonDown(0))
        {
            pressStartLmb = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Debug.Log("ayyy+");
            Vector2 press = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (segments.Count == 0)
            {
                segments.Add(new());
            }

            var lastList = segments.Last();
            lastList.Add(press);

            if (lastList.Count >= 2)
            {
                Vector2 p1 = lastList[lastList.Count - 2];
                Vector2 p2 = lastList[lastList.Count - 1];
                bvh.Add(new SegmentAABB(p1, p2));
            }



            pressStartRmb = press;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            List<(Vector2, Vector2)> segmentsRaw = new();
            foreach (List<Vector2> l in segments)
            {

                for (int i = 1; i < l.Count; i++)
                {
                    segmentsRaw.Add((l[i - 1], l[i]));
                }
            }

            bo = new(segmentsRaw);
            bo.testColours = depthColours;

        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            bo.testStep();
        }
        if (Input.GetKey(KeyCode.RightShift))
        {
            if (bo != null)
            {
                    bo.testStep();
            }
        }
        updateLineRenderers();
    }


    void updateLineRenderers()
    {
        foreach (Transform t in lineRendererParent.transform)
        {
            GameObject.Destroy(t.gameObject);
        }

        foreach (List<Vector2> l in segments)
        {


            LineRenderer lr = Instantiate(MyLineRenderer);
            lr.transform.parent = lineRendererParent.transform;
            //   LineRenderer lr = LineRendererParent.AddComponent(typeof(LineRenderer))as LineRenderer;


            Vector3[] positions = new Vector3[l.Count];
            int i = 0;

            foreach (Vector2 v in l)
            {
                positions[i] = v;
                i++;
            }
            lr.positionCount = positions.Length;
            lr.SetPositions(positions);

        }

    }


    public void OnDrawGizmos()
    {
        if (bvh == null || bvh.RootIsNull)
            return;

        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;


        /*foreach ((BoundingVolumeHierarchy<Segment>.Node node, int depth) in bvh.EnumerateNodes())
        {
            int modDepth = depth % depthColours.Length;

            Color col = depthColours[modDepth];

            Handles.color = col;
            Handles.DrawWireCube(node.AABB.center, node.AABB.size);
        }*/

        if (Input.GetMouseButton(0))
        {
            cursorLineRenderer.enabled = true;
            cursorLineRenderer.SetPosition(0, pressStartLmb);
            cursorLineRenderer.SetPosition(1, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));


            SegmentAABB testingSegment = new SegmentAABB(pressStartLmb, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
            Handles.color = Color.green;
            Handles.DrawWireCube(testingSegment.GetBounds().center, testingSegment.GetBounds().size + (Vector3.one * 0.05f));

            foreach (var node in bvh.EnumerateOverlappingLeafNodes(testingSegment.GetBounds()))
            {
                if (!node.IsLeaf) { continue; }

                Handles.color = Color.blue;
                if (node.Object.intersects(testingSegment,out _)) { Handles.color = Color.red; }
                Handles.DrawWireCube(node.AABB.center, node.AABB.size + (Vector3.one * 0.05f));
            };
        }
        else
        {

            if (!Input.GetMouseButton(1))
            {
                cursorLineRenderer.enabled = false;
            }
        }

        if (bo != null)
        {

            bo.debugDraw();
        }

    }
    void findAllIntersections()
    {
        List<(Vector2, Vector2)> lineSegments = new();
        foreach (var l in segments)
        {
            if (l.Count < 2) { continue; }
            for (int i = 1; i < l.Count; i++)
            {
                lineSegments.Add((l[i - 1], l[i]));
            }
        }
    }

}
