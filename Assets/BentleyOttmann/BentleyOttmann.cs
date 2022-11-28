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

		List<Vector2> GraphNodes = new();
		List<(int, int)> GraphEdges = new();
		public BentleyOttman(List<(Vector2, Vector2)> segmentsRaw)
		{


			priorityQueue = new(new PriorityQueueOrdering());




			foreach (var s in segmentsRaw)
			{
				var ns = new Segment(s.Item1, s.Item2);
				segments.Add(ns);
				SweepEvent start = new(ns.start, ns);
				SweepEvent end = new(ns.end);
				priorityQueue.Enqueue(start, start);
				priorityQueue.Enqueue(end, end);
			}
			segments.Sort(new InitialSegmentOrdering());
			sweepLine = new();
		}



		public void testStep()
		{
			var currentEvent = priorityQueue.Dequeue();
			poslast = currentEvent.point;

			//Step 1
			GraphNodes.Add(poslast);


			//Step 2
			Segment.currentSweepPosition = poslast;//very important
			Segment requestSegment = new(poslast, poslast);
			sweeplineCanidates = sweepLine.FindAllEqual(requestSegment);
			Debug.Log(sweeplineCanidates);

			//Step 3

			int current_index = GraphNodes.Count-1;
			foreach (Segment s in sweeplineCanidates) {
				if (s.last_intesection_index != -1)
				{
					GraphEdges.Add((s.last_intesection_index,current_index));
				}
			}

			//step 4
			sweepLine.DeleteAllEqual(requestSegment);
			sweeplineCanidates.RemoveAll(item => item.end==poslast);

			//step 5
			sweeplineCanidates.Reverse();

			//
			foreach (var s in sweeplineCanidates) { 
				sweepLine.Add(s);
			}
			//step 6 
			//add all segments containing poslast to the sweepline
			if (currentEvent.segment != null)
			{
				currentEvent.segment.last_intesection_index =current_index;
				sweepLine.Add(currentEvent.segment);
			}


			//step 7 create new Events
			

		}


		Vector2 poslast = Vector2.positiveInfinity;
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


			Handles.color = Color.yellow;
			Handles.DrawDottedLine(new Vector2(Segment.currentSweepPosition.x, -10), new Vector2(Segment.currentSweepPosition.x, 10), 1);

			Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			foreach (var s in sweepLine)
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

				/*if (GeometricPrimitives.GetLineIntersectionX(s, Segment.currentSweepPosition, out y, out intersects))
				{
					if (intersects)
					{

					Segment reqSeg = new(poslast, poslast);
					int comp = reqSeg.CompareTo(s);

					if (comp < 0)
						Handles.color = Color.red;
					if (comp == 0)
						Handles.color = Color.yellow;
					if (comp > 0)
						Handles.color = Color.green;
					Handles.DrawLine(s.start, s.end, 2);

					}
				}*/

				Handles.color = Color.green;
				Handles.DrawLine(s.start, s.end,2);
			}
			if (sweeplineCanidates != null)
			{
				Handles.color = Color.red;
				foreach (var s in sweeplineCanidates)
				{
					Debug.Log(s);
					Handles.DrawLine(s.start, s.end,2);
				}
			}

			foreach ((int a, int b) in GraphEdges) {
				Vector2 pa = GraphNodes[a];
				Vector2 pb = GraphNodes[b];
				Vector2 delta = pb - pa;
				delta = delta.normalized;
				Handles.color =Color.cyan;
				Handles.DrawLine(pa+delta*0.1f, pb-delta*0.1f,1);
			
			}

		}

	}






}






