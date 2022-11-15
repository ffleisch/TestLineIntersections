using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
public class BentleyOttman
{





    public BentleyOttman(List<(Vector2, Vector2)> segmentsRaw)
    {


        PriorityQueue<SweepEvent, float> priorityQueue=new();


        SortedList<(Vector2, Vector2), int> sweepLine = new();


        var segments = new List<(Vector2, Vector2)>();
        foreach (var s in segmentsRaw)
        {
            if (s.Item1.x > s.Item2.x)
            {
                segments.Add((s.Item2, s.Item1));
            }
            else
            {
                segments.Add(s);
            }
        }


    }

    public enum EventType{ 
        Left,
        Right,
        Intersection
    }
    public class SweepEvent {
        Vector2 pos;
        int segmentIndex;
        EventType type;
        public SweepEvent(Vector2 pos,EventType type,int index=-1) {
            this.pos = pos;
            this.segmentIndex = index;
            this.type = type;
        }
    }
}


