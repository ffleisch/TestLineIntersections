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
    List<List<Vector2>> segments =new();

    LineRenderer cursorLineRenderer;

    [SerializeField]
    private Color[] depthColours;

    BoundingVolumeHierarchy<Segment> bvh;

    // Start is called before the first frame update
    void Start()
    {
        bvh = new BoundingVolumeHierarchy<Segment>();
        lineRendererParent = gameObject.transform.Find("AllLineRenderers").gameObject;
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

    void Update()
    {

        if (Input.GetMouseButtonDown(1)) {
            pressStartRmb = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1)) {
            cursorLineRenderer.enabled = true;
            cursorLineRenderer.SetPosition(0, pressStartRmb);
            cursorLineRenderer.SetPosition(1, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        else {
            if (!Input.GetMouseButton(0))
            {
                cursorLineRenderer.enabled = false;
            }
        }

        if (Input.GetMouseButtonUp(1)) { 
            Vector2 pressEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            segments.Add(new List<Vector2> { pressStartRmb,pressEnd });
            bvh.Add(new Segment(pressStartRmb,pressEnd));
        }

        if (Input.GetMouseButtonDown(0)) {
            pressStartLmb = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus)) {
            Debug.Log("ayyy+");
            Vector2 press = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (segments.Count == 0) {
                segments.Add(new());
            }

            var lastList = segments.Last();
            lastList.Add(press);

            if (lastList.Count >= 2) {
                Vector2 p1=lastList[lastList.Count-2];
                Vector2 p2=lastList[lastList.Count-1];
                bvh.Add(new Segment(p1,p2));
            }



            pressStartRmb = press;
        }


        updateLineRenderers();
    
    }


    void updateLineRenderers() {
        foreach (Transform t in lineRendererParent.transform) {
            GameObject.Destroy(t.gameObject);
        }

        foreach (List<Vector2> l in segments) {


            LineRenderer lr = Instantiate(MyLineRenderer);
            lr.transform.parent =lineRendererParent.transform;
            //   LineRenderer lr = LineRendererParent.AddComponent(typeof(LineRenderer))as LineRenderer;
            
            
            Vector3[] positions =new Vector3[l.Count];
            int i = 0;
            
            foreach (Vector2 v in l) {
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

        if (Input.GetMouseButton(0)) {
            cursorLineRenderer.enabled = true;
            cursorLineRenderer.SetPosition(0, pressStartLmb);
            cursorLineRenderer.SetPosition(1, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));

            Segment testingSegment = new Segment(pressStartLmb,(Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
            foreach ( var node in bvh.EnumerateOverlappingLeafNodes(testingSegment.GetBounds())) {
                if (!node.IsLeaf) { continue; }

                Handles.color = Color.blue;
                if (node.Object.intersects(testingSegment)){Handles.color=Color.red;}
                Handles.DrawWireCube(node.AABB.center, node.AABB.size+(Vector3.one*0.05f));
            };
        }
        else {
            
            if (!Input.GetMouseButton(1))
            {
                cursorLineRenderer.enabled = false;
            }
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