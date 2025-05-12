namespace TheOneStudio.UITemplate.UITemplate.Creative.Cheat
{
    using UnityEngine;

    public class TapOnCornerCheatDetector : ICheatDetector
    {
        private          int              currentTapCount;
        private          float            lastTapTime;
        private readonly int              tapCount;
        private readonly int              tapZoneSize;
        private readonly TapCheatPosition tapCheatPosition;
        private readonly float            tapThreshold;

        public TapOnCornerCheatDetector(TapCheatPosition tapCheatPosition, int tapZoneSize, int tapCount, float tapThreshold = 0.5f)
        {
            this.tapCheatPosition = tapCheatPosition;
            this.tapZoneSize      = tapZoneSize;
            this.tapCount         = tapCount;
            this.tapThreshold     = tapThreshold;
        }

        private bool IsWithinTapZone(Vector2 position)
        {
            var screenWidth  = Screen.width;
            var screenHeight = Screen.height;

            return this.tapCheatPosition switch
            {
                TapCheatPosition.TopLeft     => position.x <= this.tapZoneSize && position.y >= screenHeight - this.tapZoneSize,
                TapCheatPosition.TopRight    => position.x >= screenWidth - this.tapZoneSize && position.y >= screenHeight - this.tapZoneSize,
                TapCheatPosition.BottomLeft  => position.x <= this.tapZoneSize && position.y <= this.tapZoneSize,
                TapCheatPosition.BottomRight => position.x >= screenWidth - this.tapZoneSize && position.y <= this.tapZoneSize,
                _                            => false,
            };
        }

        public bool Check()
        {
            var result = false;

            if (Input.GetMouseButtonDown(0))
            {
                var mousePosition = Input.mousePosition;

                if (this.IsWithinTapZone(mousePosition))
                {
                    this.currentTapCount = Time.unscaledTime - this.lastTapTime < this.tapThreshold ? this.currentTapCount + 1 : 1;

                    this.lastTapTime = Time.unscaledTime;

                    if (this.currentTapCount == this.tapCount)
                    {
                        result               = true;
                        this.currentTapCount = 0;
                    }
                }
                else
                {
                    this.currentTapCount = 0;
                }
            }

            if (Time.unscaledTime - this.lastTapTime > this.tapThreshold) this.currentTapCount = 0;

            return result;
        }
    }
}