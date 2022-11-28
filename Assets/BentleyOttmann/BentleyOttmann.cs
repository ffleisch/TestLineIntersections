using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utils;


namespace BentleyOttmann
{
    public class BentleyOttman
    {


        PriorityQueue<SweepEvent, SweepEvent> priorityQueue;
        List<Segment> segments = new();

        Tree<Segment> sweepLine;

        public BentleyOttman(List<(Vector2, Vector2)> segmentsRaw)
        {


            priorityQueue = new(new PriorityQueueOrdering());




            foreach (var s in segmentsRaw)
            {
                var ns = new Segment(s.Item1, s.Item2);
                segments.Add(ns);
                SweepEvent start = new(ns.start,ns);
                SweepEvent end = new(ns.end);
                priorityQueue.Enqueue(start, start);
                priorityQueue.Enqueue(end, end);
            }
            segments.Sort(new InitialSegmentOrdering());
            sweepLine = new();
        }



        public void testStep() {
            var currentEvent =priorityQueue.Dequeue();
            poslast = currentEvent.point;
            Segment requestSegment = new(poslast,poslast);
            sweeplineCanidates = sweepLine.FindAllEqual(requestSegment);
            Debug.Log(sweeplineCanidates);

            //step 6 
            //add all segments containing poslast to the sweepline
            if (currentEvent.segment != null) {
                sweepLine.Add(currentEvent.segment);
            }


        }


        Vector2 poslast=Vector2.positiveInfinity;
        List<Segment> sweeplineCanidates;

        public void debugDraw()

        {
            //Debug.Log("Debug");



            /*for(int i = 0; i < segments.Count; i++)
            {
                Color c = new Color((i%5)/4f,((int)(i/5f)%5)/4f,0);
                Handles.color = c;
                Segment s = segments[i];
                Handles.DrawWireCube(s.start,Vector3.one * 0.1f);
            }*/


            Handles.DrawWireCube(poslast, Vector3.one * 0.1f);

            
            Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (var s in segments)
            {
                float y = 0;
                bool intersects;
                if (GeometricPrimitives.GetLineIntersectionX(s, mouse, out y, out intersects))
                {
                    if (!intersects)
                    {
                        Handles.color = Color.blue;
                    }
                    else
                    {
                        Handles.color = Color.cyan;
                    }
                    Handles.DrawWireCube(new Vector2(mouse.x, y), Vector3.one * 0.1f);

                }

            }
            Handles.color = Color.red;
            foreach (var s in sweeplineCanidates) {
                Debug.Log(s);
                Handles.DrawLine(s.start,s.end);
            }


        }

    }






}






