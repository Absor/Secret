using UnityEngine;
using C5;
using System.Collections.Generic;
using UnityEditor;

public class FortuneVoronoi : MonoBehaviour {

    private IntervalHeap<FortuneEvent> events;
    private TreeSet<Vector2> beachLine;

    // DEBUG
    private List<Vector2> sites = new List<Vector2>();
    private Dictionary<Vector2, float> circles = new Dictionary<Vector2, float>();
    private Dictionary<Vector2, FortuneEvent> circleEventsByVectors = new Dictionary<Vector2, FortuneEvent>();

    public class Vector2Comparer : IComparer<Vector2>
    {
        public int Compare(Vector2 a, Vector2 b)
        {
            if (a.x > b.x)
            {
                return 1;
            }
            if (a.x < b.x)
            {
                return -1;
            }
            return 0;
        }
    }

	void Start () {
        events = new IntervalHeap<FortuneEvent>();
        beachLine = new TreeSet<Vector2>(new Vector2Comparer());

        List<Vector2> randomPoints = new List<Vector2>();
        for (int i = 0; i < 20; i++)
        {
            randomPoints.Add(new Vector2(Random.Range(0f, 512f), Random.Range(0f, 512f)));
        }

        SetPoints(randomPoints);
	}

    public void SetPoints(List<Vector2> points)
    {
        foreach (Vector2 point in points)
        {
            events.Add(new FortuneEvent(FortuneEventType.Site, point));
        }

        while (!events.IsEmpty)
        {
            FortuneEvent fortuneEvent = events.DeleteMax();
            switch (fortuneEvent.type)
            {
                case FortuneEventType.Circle:
                    handleCircleEvent(fortuneEvent.location, fortuneEvent.radius);
                    break;
                case FortuneEventType.Site:
                    handleSiteEvent(fortuneEvent.location);
                    break;
            }
        }
    }

    private void handleSiteEvent(Vector2 location)
    {
        sites.Add(location);

        if (beachLine.IsEmpty)
        {
            beachLine.Add(location);
            return;
        }


        Vector2 left;
        Vector2 leftLeft;
        if (beachLine.TryPredecessor(location, out left) && beachLine.TryPredecessor(left, out leftLeft))
        {
            checkTriple(leftLeft, left, location);
        }

        Vector2 right;
        Vector2 rightRight;
        if (beachLine.TrySuccessor(location, out right) && beachLine.TrySuccessor(right, out rightRight))
        {
            checkTriple(location, right, rightRight);
        }

        beachLine.Add(location);
    }

    private void checkTriple(Vector2 a, Vector2 b, Vector2 c)
    {
        float d = 2*(a.x*(b.y - c.y) + b.x*(c.y - a.y) + c.x*(a.y - b.y));

        float aSq = a.x * a.x + a.y * a.y;
        float bSq = b.x * b.x + b.y * b.y;
        float cSq = c.x * c.x + c.y * c.y;

        float x = (aSq * (b.y - c.y) + bSq * (c.y - a.y) + cSq * (a.y - b.y)) / d;
        float y = (aSq * (c.x - b.x) + bSq * (a.x - c.x) + cSq * (b.x - a.x)) / d;

        if (x < 0 || y < 0 || x > 512 || y > 512)
        {
            return;
        }
        Vector2 center = new Vector2(x, y);
        float radius = Vector2.Distance(center, a);

        events.Add(new FortuneEvent(FortuneEventType.Circle, new Vector2(x, y + radius), radius));
    }

    private void handleCircleEvent(Vector2 location, float radius)
    {
        circles.Add(new Vector2(location.x, location.y - radius), radius);
    }








    void OnDrawGizmos()
    {
        for (int i = 0; i < sites.Count; i++)
        {
            Vector2 siteOr = sites[i];
            Vector3 site = new Vector3(siteOr.x, 0, siteOr.y);
            Gizmos.DrawSphere(site, 2f);
            Handles.Label(site, "" + i);
        }

        foreach (Vector2 centerOr in circles.Keys)
        {
            Vector3 center = new Vector3(centerOr.x, 0, centerOr.y);
            Gizmos.DrawWireSphere(center, circles[centerOr]);
        }
    }
}
