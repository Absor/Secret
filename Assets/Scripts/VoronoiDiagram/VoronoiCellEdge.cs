using UnityEngine;

public class VoronoiCellEdge
{
    public Vector2 point1;
    public Vector2 point2;

    public VoronoiCellEdge(Vector2 point1, Vector2 point2)
    {
        this.point1 = point1;
        this.point2 = point2;
    }
}