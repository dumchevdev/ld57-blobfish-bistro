using UnityEngine;

namespace Game.Runtime.Utils.Helpers
{
    public static partial class Helpers
    {
        public static class Math
        {
            public static float GetAngleOnCircle(Vector2 center, Vector2 point)
            {
                float opposite = Vector2.Distance(point, new Vector2(center.x, point.y));
                float adjacent = Vector2.Distance(center, new Vector2(center.x, point.y));

                int quadrant = GetQuadrant(center, point);
                float mouseAngle =
                    (quadrant == 2 || quadrant == 4
                        ? Mathf.PI * 0.5f - Mathf.Atan2(adjacent, opposite)
                        : Mathf.Atan2(adjacent, opposite)) + (quadrant - 1) * Mathf.PI * 0.5f;

                return mouseAngle;
            }

            public static int GetQuadrant(Vector2 circleCenter, Vector2 point)
            {
                if (point.x > circleCenter.x && point.y > circleCenter.y)
                    return 1;

                if (point.x > circleCenter.x && point.y < circleCenter.y)
                    return 4;

                if (point.x < circleCenter.x && point.y < circleCenter.y)
                    return 3;

                return 2;
            }

            public static float AngleDelta(float angle1, float angle2)
            {
                float w1 = angle1 > 180f ? angle1 - 360f : angle1;
                float w2 = angle2 > 180f ? angle2 - 360f : angle2;

                return Mathf.Abs(w1 - w2);
            }

            public static int DirectionLeftOrRight(Vector3 forward, Vector3 targetDir, Vector3 up)
            {
                Vector3 perp = Vector3.Cross(forward, targetDir);
                float dir = Vector3.Dot(perp, up);

                if (dir > 0f)
                {
                    return 1;
                }

                if (dir < 0f)
                    return -1;

                return 0;
            }

            public static Vector2 AngleToVector2(float angleDegrees)
            {
                float angleRadians = angleDegrees * Mathf.Deg2Rad;
                return new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));
            }
            
            public static float Inverse01(float value)
            {
                return 1f - Mathf.Clamp01(value);
            }
            
            public static float AngleToPos2D(Vector2 from, Vector2 to)
            {
                var direction = Direction(from, to);
                float angleRadians = Mathf.Atan2(direction.y, direction.x);
                float angleDegrees = angleRadians * Mathf.Rad2Deg;
                return (angleDegrees + 360) % 360;
            }
            
            public static Vector3 Direction(Vector3 origin, Vector3 target)
            {
                return (target - origin).normalized;
            }
            
            public static float Remap(float value, float from1, float to1, float from2, float to2)
            {
                return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
            }
        }
    }
}