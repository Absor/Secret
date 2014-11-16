using System.Collections.Generic;
using UnityEngine;

public class VoronoiCell
{
    public Vector2 point;

    public List<VoronoiCellEdge> edges;
    public Vector2 debugPoint;
    public Color color;

    public VoronoiCell()
    {
        edges = new List<VoronoiCellEdge>();
    }
}

