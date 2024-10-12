using System.Collections.Generic;

namespace TheOneStudio.HyperCasual
{
    using UnityEngine;

    public class ResponsiveCamera : MonoBehaviour
    {
        private                  Camera         cam;
        private                  int            _lastScreenWidth  = 0;
        private                  int            _lastScreenHeight = 0;
        private                  EdgeCollider2D _edgeCollider2D;
        [SerializeField] private bool           _autoAddCameraEdgeCollider;

        protected virtual void Awake()
        {
            this.cam               = this.GetComponent<Camera>();
            this._lastScreenWidth  = Screen.width;
            this._lastScreenHeight = Screen.height;
            this.UpdateCamSize(this.cam.orthographicSize);
            if (this._autoAddCameraEdgeCollider)
            {
                this._edgeCollider2D = this.gameObject.GetComponent<EdgeCollider2D>();
                this.CreateEdgeCollider();
            }
        }

        private void UpdateCamSize(float currentSize)
        {
            var ratio   = 1f * this.cam.pixelWidth / this.cam.pixelHeight;
            var newSize = 0.5625f / ratio * currentSize;
            this.cam.orthographicSize = newSize < currentSize ? currentSize : newSize;
        }

        protected virtual void Update()
        {
            if (this._lastScreenWidth != Screen.width || this._lastScreenHeight != Screen.height)
            {
                this._lastScreenWidth  = Screen.width;
                this._lastScreenHeight = Screen.height;
                this.UpdateCamSize(this.cam.orthographicSize);
                if (this._autoAddCameraEdgeCollider) this.CreateEdgeCollider();
            }
        }

        private void CreateEdgeCollider()
        {
            var edges = new List<Vector2>();
            edges.Add(this.cam.ScreenToWorldPoint(Vector2.zero));
            edges.Add(this.cam.ScreenToWorldPoint(new Vector2(Screen.width, 0)));
            edges.Add(this.cam.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)));
            edges.Add(this.cam.ScreenToWorldPoint(new Vector2(0, Screen.height)));
            edges.Add(this.cam.ScreenToWorldPoint(Vector2.zero));
            this._edgeCollider2D.SetPoints(edges);
        }
    }
}