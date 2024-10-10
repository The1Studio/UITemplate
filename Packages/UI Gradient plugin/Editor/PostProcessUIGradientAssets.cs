using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Case : If materials are created in editor and assigned to the gradient component.
/// The gradientmap in gradient shader is not serialized and is not also refrencing a saved map.
/// So when saving the scene, gradientmap in the material resets to null.
/// Saving properties in shader is actually not required since the gradient component assigns the properties in the scene.
/// So, what this script does is skip the saving of a gradient material.
/// The gradient shader/material is required only to support complex gradient rendering.
/// </summary>
namespace PolyAndCode.UI.effect
{
    public class PostProcessUIGradientAssets : UnityEditor.AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] paths)
        {
            UIGradientutility.UpdateAllGradients();
            List<string> newpaths = new List<string>();
            //if material is gradient material skip it.
            foreach (string path in paths)
            {
                if (!(string.Equals(Path.GetExtension(path), ".mat") && isGradientMat(path)))
                {
                    newpaths.Add(path);
                }
            }
            return newpaths.ToArray();
        }

        static bool isGradientMat(string path)
        {
            Material mat = (Material)AssetDatabase.LoadAssetAtPath(path, typeof(Material));
            Shader gradientshader = Shader.Find(UIGradient.GradientShaderPath);
            if (mat.shader == gradientshader)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Menu utiity to refresh all gradients.
    /// Not required in any cases so far.
    /// </summary>
    public class UIGradientutility : MonoBehaviour
    {
        [MenuItem("Window/UI Gradient/Refresh Gradients")]
        public static void UpdateAllGradients()
        {
            UIGradient[] allGradients = FindObjectsOfType<UIGradient>();
            foreach (var item in allGradients)
            {
                if (item.gameObject.activeInHierarchy)
                {
                    item.UpdateMaterial();
                }
            }
        }
    }
}