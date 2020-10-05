using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
   
    public static Vector2 NearestPointOnSegment(Vector2 point, Vector2 min, Vector2 max)
    {
        //Get heading
        Vector2 heading = (max - min);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();

        //Do projection from the point but clamp it
        Vector2 lhs = point - min;
        float dotP = Vector2.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);

        return min + heading * dotP;
    }
}
