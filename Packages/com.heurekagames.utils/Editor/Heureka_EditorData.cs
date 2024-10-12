using System;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames.Utils
{
    public class Heureka_EditorData : ScriptableObject
    {
        public delegate void EditorDataRefreshDelegate();

        public static event EditorDataRefreshDelegate OnEditorDataRefresh;

        private static Heureka_EditorData m_instance;

        public static Heureka_EditorData Instance
        {
            get
            {
                if (!m_instance) m_instance = loadData();

                return m_instance;
            }
        }

        private static Heureka_EditorData loadData()
        {
            //LOGO ON WINDOW
            var configData = AssetDatabase.FindAssets("EditorData t:" + typeof(Heureka_EditorData).ToString(), null);
            if (configData.Length >= 1) return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(configData[0]), typeof(Heureka_EditorData)) as Heureka_EditorData;

            Debug.LogError("Failed to find config data");
            return null;
        }

        internal void RefreshData()
        {
            if (OnEditorDataRefresh != null) OnEditorDataRefresh();
        }

        public GUIStyle HeadlineStyle;

        public static class Links
        {
            private const string AffiliateID      = "1011l4Izm";
            private const string SlugSmartBuilder = "206777";
            private const string SlugAssetHunter  = "135296";
            private const string SlugAssetFinder  = "97772";

            public static string FromAHPToSmartBuilder => GenerateLink("AHPNews", SlugSmartBuilder);

            private static string GenerateLink(string pubref, string slug)
            {
                return $"https://prf.hn/click/camref:{AffiliateID}/pubref:{pubref}/destination:https%3A%2F%2Fassetstore.unity.com%2Fpackages%2Fslug%2F{slug}";
            }
        }
    }

    [Serializable]
    public class ConfigurableIcon
    {
        [SerializeField] private bool isUsingDarkSkin = false;

        [SerializeField] private string  buildInIconName = "";
        [SerializeField] private Texture iconCached      = null;

        [SerializeField] private Texture m_iconNormalOverride  = null;
        [SerializeField] private Texture m_iconProSkinOverride = null;

        public ConfigurableIcon()
        {
            Heureka_EditorData.OnEditorDataRefresh += this.onEditorDataRefresh;
        }

        private void onEditorDataRefresh()
        {
            this.iconCached = null;
        }

        public Texture Icon
        {
            get
            {
                //TODO A way to make sure we update, if the user have changed skin
                if (this.isUsingDarkSkin != EditorGUIUtility.isProSkin)
                {
                    this.iconCached      = null;
                    this.isUsingDarkSkin = EditorGUIUtility.isProSkin;
                }
                return this.iconCached != null ? this.iconCached : this.iconCached = this.GetInvertedForProSkin();
            }
        }

        [SerializeField] private bool m_darkSkinInvert = false;

        protected Texture GetInvertedForProSkin()
        {
            var imageToUse = EditorGUIUtility.isProSkin ? this.m_iconProSkinOverride : this.m_iconNormalOverride;

            //If we want to use default unity icons and nothing has been setup to override
            if (imageToUse == null && !string.IsNullOrEmpty(this.buildInIconName))
                if (EditorGUIUtility.IconContent(this.buildInIconName) != null)
                    imageToUse = EditorGUIUtility.IconContent(this.buildInIconName).image;

            //Return current image if we dont have proskin, or dont want to invert
            if (!EditorGUIUtility.isProSkin || (EditorGUIUtility.isProSkin && !this.m_darkSkinInvert)) return imageToUse;

            var readableTexture = this.getReadableTexture(imageToUse);
            var inverted        = new Texture2D(readableTexture.width, readableTexture.height, TextureFormat.ARGB32, false);
            for (var x = 0; x < readableTexture.width; x++)
            for (var y = 0; y < readableTexture.height; y++)
            {
                var origColor     = readableTexture.GetPixel(x, y);
                var invertedColor = new Color(1 - origColor.r, 1 - origColor.g, 1 - origColor.b, origColor.a);
                inverted.SetPixel(x, y, origColor.a > 0 ? invertedColor : origColor);
            }
            inverted.Apply();
            return inverted;
        }

        private Texture2D getReadableTexture(Texture imageToUse)
        {
            // Create a temporary RenderTexture of the same size as the texture
            var tmp = RenderTexture.GetTemporary(
                imageToUse.width,
                imageToUse.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);

            // Blit the pixels on texture to the RenderTexture
            Graphics.Blit(imageToUse, tmp);
            // Backup the currently set RenderTexture
            var previous = RenderTexture.active;
            // Set the current RenderTexture to the temporary one we created
            RenderTexture.active = tmp;
            // Create a new readable Texture2D to copy the pixels to it
            var myTexture2D = new Texture2D(imageToUse.width, imageToUse.height);
            // Copy the pixels from the RenderTexture to the new Texture
            myTexture2D.ReadPixels(new(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();
            // Reset the active RenderTexture
            RenderTexture.active = previous;
            // Release the temporary RenderTexture
            RenderTexture.ReleaseTemporary(tmp);

            return myTexture2D;
        }
    }
}