using UnityEngine;

namespace TheOneStudio.UITemplate.UITemplate.Utils
{
    public class MotionProjectilesUtil
    {
        /// <summary>
        /// This function calculate and return 2d velocity vector for an object being throw
        /// Params:
        /// - top: highest position in the air that the object will reach
        /// - source: source position where the object is thrown from
        /// - target: target position where the object is thrown to
        /// - gravity: gravity vector of the object being thrown. Obtain from Physics2d.gravity
        /// </summary>
        public static Vector2 CalculateProjectileVelocityVector2D(Vector2 top, Vector2 source, Vector2 target, Vector2 gravity)
        {
            float l = Mathf.Abs(target.x - source.x);
            float h0 = source.y - target.y;
            float h = top.y - target.y;
            float g = gravity.magnitude;
            
            // calculate t
            float t = (Mathf.Sqrt(2 * g * (h - h0)) + Mathf.Sqrt(2 * g * h)) / g;

            // Calculate alpha
            float alpha = Mathf.Atan(t * (Mathf.Sqrt(2 * g * (h - h0)) / l));
            float v = Mathf.Sqrt(((l/t) * (l/t)) + (2 * g * (h - h0)));

            // Calculate vx, vy
            float vx = Mathf.Cos(alpha) * v;
            float vy = Mathf.Sin(alpha) * v;
            
            // Construct Unity velocity vector
            if (target.x - source.x < 0)
            {
                vx = -vx;
            }
            Vector2 initialVelocity = new Vector2(vx, vy);
            
            return initialVelocity;
        }
    }
}