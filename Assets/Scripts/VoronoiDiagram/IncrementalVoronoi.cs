using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementalVoronoi : MonoBehaviour
{
    private Rect boundingArea;

    private List<VoronoiCell> cells;

    void Start()
    {
        boundingArea = new Rect(0, 0, 512, 512);
        cells = new List<VoronoiCell>();

        StartCoroutine(addRandomPointEverySecond());
    }

    private IEnumerator addRandomPointEverySecond()
    {
        while (true)
        {
            AddRandomPoint();
            yield return new WaitForSeconds(10.0f);
        }
    }

    [ContextMenu("Add Random Point")]
    public void AddRandomPoint()
    {
        AddPoint(new Vector2(Random.Range(boundingArea.x, boundingArea.xMax), Random.Range(boundingArea.y, boundingArea.yMax)));
    }

    public void AddPoint(Vector2 point)
    {
        if (!boundingArea.Contains(point))
        {
            return;
        }

        VoronoiCell cell = new VoronoiCell();
        cell.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        cell.point = point;

        if (cells.Count == 0)
        {
            // add edges from bounding area
            Vector2 corner1 = new Vector2(boundingArea.x, boundingArea.y);
            Vector2 corner2 = new Vector2(boundingArea.xMax, boundingArea.y);
            Vector2 corner3 = new Vector2(boundingArea.xMax, boundingArea.yMax);
            Vector2 corner4 = new Vector2(boundingArea.x, boundingArea.yMax);
            VoronoiCellEdge edge1 = new VoronoiCellEdge(corner1, corner2);
            VoronoiCellEdge edge2 = new VoronoiCellEdge(corner2, corner3);
            VoronoiCellEdge edge3 = new VoronoiCellEdge(corner3, corner4);
            VoronoiCellEdge edge4 = new VoronoiCellEdge(corner4, corner1);
            cell.edges.Add(edge1);
            cell.edges.Add(edge2);
            cell.edges.Add(edge3);
            cell.edges.Add(edge4);
        }
        else
        {
            fitNewCell(cell);
        }

        cells.Add(cell);
    }

    private void fitNewCell(VoronoiCell cell)
    {
        float minDistance = float.MaxValue;
        VoronoiCell closestCell = null;
        foreach(VoronoiCell otherCell in cells) {
            float distance = Vector2.Distance(cell.point, otherCell.point);
            if (distance < minDistance)
            {
                closestCell = otherCell;
                minDistance = distance;
            }
        }

        Vector2 pointToPoint = closestCell.point - cell.point;
        Vector2 q = (closestCell.point - cell.point) * 0.5f + cell.point;
        Vector2 s = new Vector2(pointToPoint.normalized.y, -pointToPoint.normalized.x);

        for (int i = 0; i < closestCell.edges.Count; i++)
        {
            VoronoiCellEdge edge = closestCell.edges[i];
            Vector2 p = edge.point1;
            Vector2 r = edge.point2 - edge.point1;

            Vector2 intersection;
            if (doLineLineSegmentIntersect(q, s, p, r, out intersection, true))
            {

                cell.edges.Add(new VoronoiCellEdge(intersection, edge.point2));
                edge.point2 = intersection;

                VoronoiCellEdge fillerEdge = new VoronoiCellEdge(intersection, Vector2.zero);

                int next = (i + 1) % closestCell.edges.Count;
                VoronoiCellEdge checkEdge = closestCell.edges[next];

                while (!doLineLineSegmentIntersect(q, s, checkEdge.point1, checkEdge.point2 - checkEdge.point1, out intersection, false))
                {
                    closestCell.edges.Remove(checkEdge);
                    cell.edges.Add(checkEdge);
                    next = next % closestCell.edges.Count;
                    checkEdge = closestCell.edges[next];
                }

                cell.edges.Add(new VoronoiCellEdge(checkEdge.point1, intersection));
                checkEdge.point1 = intersection;

                cell.edges.Add(new VoronoiCellEdge(intersection, cell.edges[0].point1));

                fillerEdge.point2 = intersection;
                closestCell.edges.Insert(i, fillerEdge);

                break;
            }
        }
    }

    private bool doLineLineSegmentIntersect(Vector2 q, Vector2 s, Vector2 p, Vector2 r, out Vector2 intersection, bool clockwise)
    {
        // http://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect/565282#565282

        // t = (q − p) × s / (r × s)

        float denominator = crossProduct2D(r, s);
        float t = crossProduct2D((q - p), s) / denominator;

        // u = (q − p) × r / (r × s)

        float u = crossProduct2D((q - p), r) / denominator;

        bool clockwiseCheck = clockwise ? u > 0 : u < 0;

        if (denominator != 0 && !(t < 0 || t > 1) && clockwiseCheck)
        {
            intersection = p + t * r;
            return true;
        }

        intersection = Vector2.zero;
        return false;
    }

    private float crossProduct2D(Vector2 point1, Vector2 point2) {
	    return point1.x * point2.y - point1.y * point2.x;
    }

    void OnDrawGizmos()
    {
        // Points
        foreach (VoronoiCell cell in cells)
        {
            Gizmos.color = Color.red;
            Vector2 point = cell.point;
            Gizmos.DrawSphere(new Vector3(point.x, 0, point.y), 2f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(new Vector3(cell.debugPoint.x, 0, cell.debugPoint.y), 10f);

            Gizmos.color = cell.color;
            foreach (VoronoiCellEdge edge in cell.edges)
            {                
                Gizmos.DrawLine(new Vector3(edge.point1.x, 0, edge.point1.y), new Vector3(edge.point2.x, 0, edge.point2.y));
            }
        }

        // Bounds
        //Gizmos.color = Color.white;
        //Gizmos.DrawLine(new Vector3(boundingArea.x, 0, boundingArea.y), new Vector3(boundingArea.xMax, 0, boundingArea.y));
        //Gizmos.DrawLine(new Vector3(boundingArea.xMax, 0, boundingArea.y), new Vector3(boundingArea.xMax, 0, boundingArea.yMax));
        //Gizmos.DrawLine(new Vector3(boundingArea.xMax, 0, boundingArea.yMax), new Vector3(boundingArea.x, 0, boundingArea.yMax));
        //Gizmos.DrawLine(new Vector3(boundingArea.x, 0, boundingArea.yMax), new Vector3(boundingArea.x, 0, boundingArea.y));
    }
}

