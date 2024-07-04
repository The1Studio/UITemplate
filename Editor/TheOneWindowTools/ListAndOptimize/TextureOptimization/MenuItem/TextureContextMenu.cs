using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class TextureContextMenu
{
    [MenuItem("Assets/TheOne/TinyPNG Compress", true)]
    private static bool ValidateCompressTextures()
    {
        // This method returns true if the selected object is a Texture2D, enabling the menu item
        return Selection.activeObject is Texture2D;
    }

    [MenuItem("Assets/TheOne/TinyPNG Compress")]
    private static void CompressTextures()
    {
        // Get all selected Texture2D objects
        foreach (Object obj in Selection.objects)
        {
            if (obj is Texture2D texture)
            {
                // Assuming TinyPngCompressor.CompressTextures takes a list of Texture2D objects
                // Convert the single Texture2D to a list and pass it to the CompressTextures method
                TinyPngCompressor.CompressTextures(new List<Texture2D> { texture });
            }
        }
    }
}