using System;
using UnityEngine;

public class FortuneEvent : IComparable<FortuneEvent>
{
    public readonly FortuneEventType type;
    public readonly Vector2 location;
    public readonly float radius;

    public FortuneEvent(FortuneEventType type, Vector2 location)
    {
        this.type = type;
        this.location = location;
    }

    public FortuneEvent(FortuneEventType type, Vector2 location, float radius)
    {
        this.type = type;
        this.location = location;
        this.radius = radius;
    }

    public int CompareTo(FortuneEvent other)
    {
        Vector2 a = this.location;
        Vector2 b = other.location;
        if (a.y > b.y)
        {
            return 1;
        }
        if (a.y < b.y)
        {
            return -1;
        }
        return 0;
    }
}
