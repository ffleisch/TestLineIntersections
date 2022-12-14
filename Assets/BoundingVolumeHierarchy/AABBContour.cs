using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoundingVolumeHierarchy;
using static Math3d;

public class SegmentAABB:IBVHClientObject{
    public Vector2 start;
    public Vector2 end;
    public float dx;
    public float dy;
    Bounds myBounds;

    public Vector3 Position { get; }

    public Vector3 PreviousPosition { get; }
    private Vector2 Abs(Vector2 v2){
        return new Vector2(Mathf.Abs(v2.x), Mathf.Abs(v2.y));
    }

    public SegmentAABB(Vector2 start,Vector2 end) {
        this.start = start;
        this.end = end;

        dx = end.x - start.x;
        dy = end.y - start.y;
        Position = (start+end)/2;
        PreviousPosition = Position;
        myBounds = new Bounds(Position,Abs(end-start));
    }


   // public bool intersects(SegmentAABB other) {
    //    return AreLineSegmentsCrossing(start,end,other.start,other.end);
   // }

   public bool intersects(SegmentAABB other,out Vector2 isect) {
        //a.dx = p1_x - a.start.x; a.dy = p1_y - a.start.y;
        //b.dx = p3_x - b.start.x; b.dy = p3_y - b.start.y;

        float s, t;
        s = (-dy * (start.x - other.start.x) + dx * (start.y - other.start.y)) / (-other.dx * dy + dx * other.dy);
        t = (other.dx * (start.y - other.start.y) - other.dy * (start.x - other.start.x)) / (-other.dx * dy + dx * other.dy);

        if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
        {
            // Collision detected
            isect = new Vector2(start.x + (t * dx), start.y + (t * dy));
            return true;
        }
        isect = Vector2.zero;
        return false; // No collision
    }

    public Bounds GetBounds()
    {
        return myBounds;
    }
}