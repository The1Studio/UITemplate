namespace TheOneStudio.UITemplate.UITemplate.Creative.Cheat
{
    using UnityEngine;

    public class TripleTapCheatDetector : ICheatDetector
    {
        private int   tapCount;
        private float lastTapTime;

        private readonly float tapThreshold;

        public TripleTapCheatDetector(float tapThreshold = 0.5f) { this.tapThreshold = tapThreshold; }

        public bool Check()
        {
            var result = false;
            if (Input.GetMouseButtonDown(0))
            {
                this.tapCount    = (Time.unscaledTime - this.lastTapTime < this.tapThreshold) ? this.tapCount + 1 : 1;
                this.lastTapTime = Time.unscaledTime;

                if (this.tapCount == 3)
                {
                    result        = true;
                    this.tapCount = 0;
                }
            }

            if (Time.unscaledTime - this.lastTapTime > this.tapThreshold) this.tapCount = 0;
            return result;
        }
    }
}