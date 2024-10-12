namespace TheOne.Tool.Optimization.Texture.MenuItems
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public static class TextureContextMenu
    {
        [MenuItem("Assets/TheOne/TinyPNG Compress", true)]
        private static bool ValidateCompressTextures()
        {
            // This method returns true if the selected object is a Texture2D, enabling the menu item
            return Selection.activeObject is Texture;
        }

        [MenuItem("Assets/TheOne/TinyPNG Compress")]
        private static void CompressTextures()
        {
            var filePaths = new List<string>();
            foreach (var obj in Selection.objects)
            {
                if (obj is Texture)
                {
                    var path = AssetDatabase.GetAssetPath(obj);
                    filePaths.Add(path);
                }
            }
            TinyPngCompressor.CompressTextures(filePaths).Forget();
        }
    }
}