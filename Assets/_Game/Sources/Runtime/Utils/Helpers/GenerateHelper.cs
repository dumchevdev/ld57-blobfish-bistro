using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.Utils.Helpers
{
    public static partial class Helpers
    {
        public static class GenerateHelper
        {
            public static List<Vector2> GenerateArc(Vector2 origin, float radius, Vector2 direction, int numberOfPoints)
            {
                List<Vector2> points = new List<Vector2>();

                direction.Normalize();
                float angle = Mathf.Atan2(direction.y, direction.x);
                float verticalRange = radius / 2;

                float startAngle = angle - Mathf.Asin(verticalRange / radius);
                float endAngle = angle + Mathf.Asin(verticalRange / radius);

                float angleIncrement = (endAngle - startAngle) / (numberOfPoints - 1);

                for (int i = 0; i < numberOfPoints; i++)
                {
                    float currentAngle = startAngle + angleIncrement * i;
                    float x = origin.x + radius * Mathf.Cos(currentAngle);
                    float y = origin.y + radius * Mathf.Sin(currentAngle);

                    points.Add(new Vector2(x, y));
                }

                return points;
            }

            public static List<Vector2> FullCircle(Vector2 origin, int numberOfPoints, List<Vector2> points = null,
                float time = 0, float radius = 1)
            {
                points ??= new List<Vector2>();

                float angleIncrement = 2 * Mathf.PI / numberOfPoints;

                for (int i = 0; i < numberOfPoints; i++)
                {
                    float currentAngle = angleIncrement * i;

                    float x = origin.x + radius * Mathf.Cos(time + currentAngle);
                    float y = origin.y + radius * Mathf.Sin(time + currentAngle);

                    points.Add(new Vector2(x, y));
                }

                return points;
            }
        }
    }
}