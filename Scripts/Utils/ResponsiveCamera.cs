using System.Collections.Generic;

namespace TheOneStudio.HyperCasual
{
    using UnityEngine;

    public class ResponsiveCamera : MonoBehaviour
    {
        private Camera cam;
        private int _lastScreenWidth = 0;
        private int _lastScreenHeight = 0;
        private EdgeCollider2D _edgeCollider2D;
        [SerializeField] private bool _autoAddCameraEdgeCollider;

        protected virtual void Awake()
        {
            cam = this.GetComponent<Camera>();
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;
            UpdateCamSize(cam.orthographicSize);
            if (_autoAddCameraEdgeCollider)
            {
                _edgeCollider2D = gameObject.GetComponent<EdgeCollider2D>();
                CreateEdgeCollider();
            }
          
        }

        private void UpdateCamSize(float currentSize)
        {
            var ratio  = 1f * cam.pixelWidth / cam.pixelHeight;
            var newSize = 0.5625f / ratio * currentSize;
            cam.orthographicSize = newSize < currentSize ? currentSize : newSize;
        }

        protected virtual void Update()
        {
            if (_lastScreenWidth != Screen.width || _lastScreenHeight != Screen.height)
            {
                _lastScreenWidth = Screen.width;
                _lastScreenHeight = Screen.height;
                UpdateCamSize(cam.orthographicSize);
                if (_autoAddCameraEdgeCollider)
                {
                    CreateEdgeCollider();
                }
            }
        }
        
        void CreateEdgeCollider()
        {
            List<Vector2> edges = new List<Vector2>();
            edges.Add(cam.ScreenToWorldPoint(Vector2.zero));
            edges.Add(cam.ScreenToWorldPoint(new Vector2(Screen.width, 0)));
            edges.Add(cam.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)));
            edges.Add(cam.ScreenToWorldPoint(new Vector2(0, Screen.height)));
            edges.Add(cam.ScreenToWorldPoint(Vector2.zero));
            _edgeCollider2D.SetPoints(edges);
        }
    }
}