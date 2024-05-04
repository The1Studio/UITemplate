namespace TheOneStudio.UITemplate.UITemplate.Extension
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using R3;
    using UnityEngine;
    using UnityEngine.UI;
    using Random = UnityEngine.Random;

    public static class UITemplateExtension
    {
        #region Cache

        private static Vector2 canvasSize;

        #endregion

        public static void GachaItemWithTimer<T>(this List<T> items, IDisposable randomTimerDispose, Action<T> onComplete, Action<T> everyCycle, float currentCooldownTime = 1f,
            float currentCycle = 0.5f, int finalItemIndex = -1)
        {
            randomTimerDispose = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(currentCycle)).Subscribe(_ =>
            {
                everyCycle?.Invoke(items[Random.Range(0, items.Count)]);

                if (currentCooldownTime <= 0)
                {
                    onComplete?.Invoke(finalItemIndex != -1 ? items[finalItemIndex] : items[Random.Range(0, items.Count)]);

                    randomTimerDispose.Dispose();

                    return;
                }

                currentCooldownTime -= currentCycle;
            });
        }
        
        public static T RandomGachaWithWeight<T>(this IDictionary<T, int> dictionary, int defaultElementIndex = 0)
        {
            return dictionary.Keys.ToList().RandomGachaWithWeight(dictionary.Values.Select(weight => weight * 1f).ToList());
        }
        
        public static T RandomGachaWithWeight<T>(this IDictionary<T, float> dictionary, int defaultElementIndex = 0)
        {
            return dictionary.Keys.ToList().RandomGachaWithWeight(dictionary.Values.ToList());
        }

        public static T RandomGachaWithWeight<T>(this IList<T> elements, IList<float> weights, int defaultElementIndex = 0)
        {
            // Validate input
            if (elements == null || weights == null || elements.Count != weights.Count || elements.Count == 0)
            {
                throw new ArgumentException("Invalid input");
            }

            // Normalize weights
            var sum               = weights.Sum();
            var normalizedWeights = weights.Select(w => w / sum).ToList();

            // Generate random number between 0 and 1
            var rnd          = new System.Random();
            var randomNumber = rnd.NextDouble();

            // Select element based on weights
            for (var i = 0; i < elements.Count; i++)
            {
                if (randomNumber < normalizedWeights[i])
                {
                    return elements[i];
                }

                randomNumber -= normalizedWeights[i];
            }

            return elements[defaultElementIndex];
        }

        public static bool IsNullOrEmpty(this string str) { return string.IsNullOrEmpty(str); }

        public static Vector3 RandomPointInBounds(this Bounds bounds)
        {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        public static Vector2 Random2DPointInBounds(this Bounds bounds)
        {
            return new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );
        }

        public static Vector2 GetRandomPointInRectTransform(this RectTransform rectTransform, Camera uiCamera)
        {
            // Convert the corners of the RectTransform to screen space
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            var bottomLeft = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[0]);
            var topRight   = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[2]);

            // Generate a random point within the RectTransform's bounds
            var randomPoint = new Vector2(Random.Range(bottomLeft.x, topRight.x), Random.Range(bottomLeft.y, topRight.y));

            // Convert the random point back to local space
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, randomPoint, uiCamera, out localPoint);

            return localPoint;
        }

        public static Vector3 GetUIPositionFromWorldPosition(RectTransform rootUIShow, Camera uiCamera, Vector3 worldPosition)
        {
            var mainCam = Camera.main;
            uiCamera.orthographicSize = mainCam.orthographicSize;
            var directionUiCamToMainCam                = uiCamera.transform.position - mainCam.transform.position;
            if (canvasSize == Vector2.zero) canvasSize = rootUIShow.GetComponentInParent<CanvasScaler>().referenceResolution;
            var canvasSizePerUnit                      = canvasSize / 10f;
            var screenPos                              = RectTransformUtility.WorldToScreenPoint(uiCamera, worldPosition);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rootUIShow, screenPos, uiCamera, out var anchorPos);
            var cameraScaleSize = mainCam.orthographicSize / uiCamera.orthographicSize;

            return anchorPos / cameraScaleSize + new Vector2(directionUiCamToMainCam.x, directionUiCamToMainCam.y) * canvasSizePerUnit * cameraScaleSize;
        }

        public static Vector3 GetRealRotationFromInSpector(this Transform transform)
        {
            var angle = transform.eulerAngles;
            var x     = angle.x;
            var y     = angle.y;
            var z     = angle.z;

            if (Vector3.Dot(transform.up, Vector3.up) >= 0f)
            {
                x = angle.x switch { >= 0f and <= 90f => angle.x, >= 270f and <= 360f => angle.x - 360f, _ => x };
            }

            if (Vector3.Dot(transform.up, Vector3.up) < 0f)
            {
                switch (angle.x)
                {
                    case >= 0f and <= 90f:
                    case >= 270f and <= 360f:
                        x = 180 - angle.x;

                        break;
                }
            }

            if (angle.y > 180)
            {
                y = angle.y - 360f;
            }

            if (angle.z > 180)
            {
                z = angle.z - 360f;
            }

            return new Vector3(x, y, z);
        }
    }
}