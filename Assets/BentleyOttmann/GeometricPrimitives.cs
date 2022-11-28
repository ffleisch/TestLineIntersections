using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class SweepEvent
{
    public Vector2 point;
    public Segment segment = null;

    public SweepEvent(Vector2 pos, Segment segment = null)
    {
        point = pos;
        this.segment = segment;
    }
}
class Segment : IComparable<Segment>
{
    public Vector2 start;
    public Vector2 end;

    public int last_intesection_index=-1;

    public static Vector2 currentSweepPosition=Vector2.negativeInfinity;

    public float dx;
    public float dy;
    public Segment(Vector2 a, Vector2 b)
    {
        if (GeometricPrimitives.twoPointsCompare(a, b) < 0)
        {
            start = a;
            end = b;
        }
        else
        {
            start = b;
            end = a;
        }

        dx = end.x - start.x;
        dy = end.y - start.y;
    }

    public SweepEvent getPossibleSweepEvent(Segment b) {
        Vector2 isect;
        if (GeometricPrimitives.GetLineIntersection(this,b,out isect)) {
            if (GeometricPrimitives.twoPointsCompare(isect, currentSweepPosition)<=0) {
                return null;
            }
            return new SweepEvent(isect);
        }
        return null;
    }




    public int CompareTo(Segment other)
    {
        bool intersectsOther;
        bool intersects;
        float yOther;
        float y;
        GeometricPrimitives.GetLineIntersectionX(other,currentSweepPosition,out yOther,out intersectsOther);
        GeometricPrimitives.GetLineIntersectionX(this,currentSweepPosition,out y,out intersects);
        Debug.Log(y-yOther+" "+Mathf.Approximately(y,yOther));
        int comp = yOther.CompareTo(y);

        return Mathf.Approximately(y,yOther)?0:-comp;
    }
}
class PriorityQueueOrdering : Comparer<SweepEvent>
{
    public override int Compare(SweepEvent a, SweepEvent b)
    {
        int res = a.point.x.CompareTo(b.point.x);

        if (res == 0)
        {
            return a.point.y.CompareTo(b.point.y);
        }
        else
        {
            return res;
        }
    }
}

class InitialSegmentOrdering : Comparer<Segment>
{
    public override int Compare(Segment a, Segment b)
    {
        return (GeometricPrimitives.twoPointsCompare(a.start, b.start));
    }
}


class GeometricPrimitives
{
    public const float EPS =10e-15f;
    public static int twoPointsCompare(Vector2 a, Vector2 b)
    {
        int res = a.x.CompareTo(b.x);

        if (res == 0)
        {
            return a.y.CompareTo(b.y);
        }
        else
        {
            return res;
        }
    }

    //https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect/1968345#1968345

    // Returns 1 if the lines intersect, otherwise 0. In addition, if the lines 
    // intersect the intersection point may be stored in the floats i_x and i_y.
    public static bool GetLineIntersection(Segment a, Segment b,out Vector2 isect)
    {
        //a.dx = p1_x - a.start.x; a.dy = p1_y - a.start.y;
        //b.dx = p3_x - b.start.x; b.dy = p3_y - b.start.y;

        float s, t;
        s = (-a.dy * (a.start.x - b.start.x) + a.dx * (a.start.y - b.start.y)) / (-b.dx * a.dy + a.dx * b.dy);
        t = (b.dx * (a.start.y - b.start.y) - b.dy * (a.start.x - b.start.x)) / (-b.dx * a.dy + a.dx * b.dy);

        if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
        {
            // Collision detected
            isect=new Vector2(a.start.x + (t * a.dx),a.start.y + (t * a.dy));
            return true;
        }
        isect = Vector2.zero;
        return false; // No collision
    }


    public static bool GetLineIntersectionX(Segment a,Vector2 sweepPosition,out float y,out bool intersects) {//TODO
        if (a.dx == 0)
        {
            y = sweepPosition.y;
            intersects = Math.Abs(a.start.x - sweepPosition.x)<EPS;

            intersects &= a.start.y <= sweepPosition.y && sweepPosition.y <= a.end.y;
            return false;
        }
        else {

            //Segment is not vertical

            float ratio =(sweepPosition.x-a.start.x)/a.dx;


            if (Mathf.Approximately(ratio, 0))
                ratio = 0;
            if (Mathf.Approximately(ratio, 1))
                ratio = 1;
            intersects = 0 <= ratio && ratio <= 1;
            y = a.start.y*(1-ratio)+a.end.y*ratio;
            return true;


        }
    }
    

}

