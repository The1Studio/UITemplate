namespace UITemplate.Editor.Screenshot
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEngine;

    public class ScreenshotEditorWindow : OdinEditorWindow
    {
        [Title("Screenshot Settings")] [BoxGroup("General Settings")] [LabelText("GameObjects to Capture")] [Required] [InlineEditor(InlineEditorModes.LargePreview)] public List<GameObject> targetObjects;

        [BoxGroup("General Settings")] [LabelText("Resolution Width")] public int resolutionWidth = 512;

        [BoxGroup("General Settings")] [LabelText("Resolution Height")] public int resolutionHeight = 512;

        [BoxGroup("Save Settings")] [FolderPath(AbsolutePath = false)] [LabelText("Save Folder in Assets")] public string savePath = "Screenshots";

        [BoxGroup("General Settings")] [LabelText("Camera Render")] public Camera screenshotCamera;

        [MenuItem("TheOne/Screenshot Capture")]
        public static void ShowWindow()
        {
            GetWindow<ScreenshotEditorWindow>("Screenshot Capture");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            screenshotCamera = Camera.main ?? FindFirstObjectByType<Camera>();
        }

        [Button("Capture Screenshot", ButtonSizes.Large)]
        [GUIColor(0.2f, 0.8f, 0.2f)]
        private void CaptureScreenshot()
        {
            if (this.targetObjects == null || this.targetObjects.Count == 0 || this.targetObjects.Any(x => x == null))
            {
                Debug.LogError("Please add valid GameObjects before capturing!");
                return;
            }

            if (this.screenshotCamera == null)
            {
                this.screenshotCamera = Camera.main ?? FindFirstObjectByType<Camera>();
                return;
            }

            // Disable all objects initially
            foreach (var obj in this.targetObjects)
            {
                obj.SetActive(false);
            }

            foreach (var target in this.targetObjects)
            {
                target.SetActive(true);

                // Set up Render Texture
                var renderTexture = new RenderTexture(this.resolutionWidth, this.resolutionHeight, 24);
                this.screenshotCamera.targetTexture = renderTexture;
                this.screenshotCamera.Render();

                // Create image
                var screenshot = new Texture2D(this.resolutionWidth, this.resolutionHeight, TextureFormat.RGBA32, false);
                RenderTexture.active = renderTexture;
                screenshot.ReadPixels(new Rect(0, 0, this.resolutionWidth, this.resolutionHeight), 0, 0);
                screenshot.Apply();

                var directory = Path.Combine(Application.dataPath, this.savePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Save image
                var filePath = Path.Combine(directory, $"Screenshot_{target.name}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");
                File.WriteAllBytes(filePath, screenshot.EncodeToPNG());

                Debug.Log($"Screenshot saved at: {filePath}");

                // Cleanup
                this.screenshotCamera.targetTexture = null;
                RenderTexture.active                = null;
                DestroyImmediate(renderTexture);

                AssetDatabase.Refresh();

                target.SetActive(false); // Disable after capturing
            }

            // Restore original active states
            foreach (var obj in this.targetObjects)
            {
                obj.SetActive(true);
            }

            Debug.Log($"All screenshots captured successfully! ({this.targetObjects.Count} items)");
        }
    }
}