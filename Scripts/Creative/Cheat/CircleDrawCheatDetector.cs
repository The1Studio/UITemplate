namespace TheOneStudio.UITemplate.UITemplate.Creative.Cheat
{
    using System.Collections.Generic;
    using UnityEngine;

    public class CircleDrawCheatDetector : ICheatDetector
    {
        private const float CircleRadiusThreshold     = 5f;   // Threshold for radius consistency
        private const float CircleClosureThreshold    = 5f;   // Threshold for closure distance
        private const float CircleCompletionThreshold = 0.8f; // Minimum percentage of circle completion to count

        private List<Vector2> points = new();
        private int           circleCount;
        private bool          isDrawing;

        public bool Check()
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.isDrawing = true;
                this.points.Clear();
                this.circleCount = 0;
            }

            if (this.isDrawing && Input.GetMouseButton(0) && Camera.main != null)
            {
                Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                this.points.Add(point);

                if (this.points.Count > 3 && this.CheckCircleCompletion(this.points))
                {
                    this.circleCount++;
                    this.points.Clear();

                    if (this.circleCount == 3)
                    {
                        this.isDrawing = false;
                        return true;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                this.isDrawing = false;
            }

            return false;
        }

        private bool CheckCircleCompletion(List<Vector2> checkPoints)
        {
            if (checkPoints.Count < 3)
                return false;

            var center          = this.GetCentroid(checkPoints);
            var radius          = Vector2.Distance(center, checkPoints[0]);
            var radiusVariance  = 0f;
            var completionRatio = 0f;

            for (var i = 1; i < checkPoints.Count; i++)
            {
                var currentRadius = Vector2.Distance(center, checkPoints[i]);
                radiusVariance += Mathf.Abs(currentRadius - radius);
                var angle = Vector2.Angle(checkPoints[i] - center, checkPoints[(i - 1 + checkPoints.Count) % checkPoints.Count] - center);
                completionRatio += Mathf.Abs(angle);
            }

            var closureDistance = Vector2.Distance(checkPoints[0], checkPoints[^1]);

            // Normalize completionRatio by dividing by 360 (full circle)
            completionRatio /= 360;

            return radiusVariance < CircleRadiusThreshold * checkPoints.Count
                && closureDistance < CircleClosureThreshold
                && completionRatio > CircleCompletionThreshold;
        }

        private Vector2 GetCentroid(List<Vector2> points)
        {
            Vector2 centroid = Vector2.zero;
            foreach (Vector2 point in points)
            {
                centroid += point;
            }

            return centroid / points.Count;
        }
    }
}