namespace TheOneStudio.UITemplate.UITemplate.Utils
{
    using DG.Tweening;
    using UnityEngine;

    public static class MainCameraUtils
    {
        private static Camera mainCamera;

        private static Camera GetMainCam
        {
            get
            {
                if (mainCamera == null) mainCamera = Camera.main;
                return mainCamera;
            }
        }

        public static void SetCameraPosition(Vector2 position) { GetMainCam.transform.position = new Vector3(position.x, position.y, -10); }

        public static void SetOrthographicSize(float size) { GetMainCam.orthographicSize = size; }

        public static void ZoomCamera(float targetSize, float duration, Ease easing) { GetMainCam.DOOrthoSize(targetSize, duration).SetEase(easing).SetAutoKill(true); }

        public static void AdjustOrthographicHorizontalSize(float horizontalSize)
        {
            var unitsPerPixel    = horizontalSize / Screen.width;
            var desireHalfHeight = 0.5f * unitsPerPixel * Screen.height;
            SetOrthographicSize(desireHalfHeight);
            SetCameraPosition(new Vector2(0, -1 - (float)1920 / Screen.height));
        }
        
    }
}