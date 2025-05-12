using FpsScreenPosition = UITemplate.Scripts.Enum.FpsScreenPosition;

namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using GameFoundation.DI;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using UnityEngine;

    public class Fps : MonoBehaviour
    {
        private float deltaTime = 0.0f;
        public  Color c         = Color.red;

        private FpsScreenPosition   fpsScreenPosition;
        private GameFeaturesSetting gameFeaturesSetting;

        // Use this for initialization
        private void Start()
        {
            this.gameFeaturesSetting = this.GetCurrentContainer().Resolve<GameFeaturesSetting>();
            this.fpsScreenPosition   = this.gameFeaturesSetting.FpsScreenPosition;
            #if !THEONE_SHOW_FPS
            this.gameObject.SetActive(false);
            #endif
        }

        // Update is called once per frame
        private void Update() => this.deltaTime += (Time.deltaTime - this.deltaTime) * 0.1f;

        private void OnGUI()
        {
            int w = Screen.width, h = Screen.height;

            var style = new GUIStyle
            {
                alignment = TextAnchor.UpperLeft, fontSize = h * 2 / 100, normal = { textColor = this.c },
            };

            var msec = this.deltaTime * 1000.0f;
            var fps  = 1.0f / this.deltaTime;
            var text = $"{msec:0.0} ms ({fps:0.} fps)";

            var rect = this.GetRect(w, h);

            GUI.Label(rect, text, style);
        }

        private Rect GetRect(int screenWidth, int screenHeight)
        {
            const int MARGIN = 10;
            var       width  = screenWidth / 4;
            var       height = screenHeight * 2 / 100;

            return this.fpsScreenPosition switch
            {
                FpsScreenPosition.TopLeft     => new(MARGIN, MARGIN, width, height),
                FpsScreenPosition.TopRight    => new(screenWidth - 200, MARGIN, -width, height),
                FpsScreenPosition.BottomLeft  => new(MARGIN, screenHeight - height - MARGIN, width, height),
                FpsScreenPosition.BottomRight => new(screenWidth - 200, screenHeight - height - MARGIN, -width, height),
                _                             => new(MARGIN, MARGIN, width, height),
            };
        }
    }
}