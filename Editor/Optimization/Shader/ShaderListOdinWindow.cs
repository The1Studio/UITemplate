namespace TheOne.Tool.Optimization.Shader
{
    using System.Collections.Generic;
    using System.Linq;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEditor.AddressableAssets;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    public class ShaderListOdinWindow : OdinEditorWindow
    {
        [Button("Refresh Shaders andMaterials")]
        [GUIColor(0.3f, 0.8f, 0.3f)]
        public void RefreshShadersAndMaterials() { this.shaderInfos = this.FindAllShadersAndMaterials(); }

        [ListDrawerSettings(Expanded = true)] [TableList] [ShowInInspector]
        private List<ShaderMaterialInfo> shaderInfos = new List<ShaderMaterialInfo>();

        [MenuItem("TheOne/List And Optimize/Shader List")]
        private static void OpenWindow() { GetWindow<ShaderListOdinWindow>().Show(); }
    
        private List<ShaderMaterialInfo> FindAllShadersAndMaterials()
        {
            var shaderDict = new Dictionary<string, ShaderMaterialInfo>();

            // Store the original scene path so we can return to it later.
            var originalScenePath = EditorSceneManager.GetActiveScene().path;

            var scenes      = EditorBuildSettings.scenes;
            var totalSteps  = scenes.Length + 1; // +1 for addressables processing.
            var currentStep = 0;

            // Get all scenes from the build settings:
            foreach (var sceneInBuild in scenes)
            {
                currentStep++;
                EditorUtility.DisplayProgressBar("Refreshing Shaders and Materials",
                    $"Processing Scene {currentStep} out of {totalSteps}", currentStep / (float)totalSteps);

                if (!sceneInBuild.enabled) continue;

                if (string.IsNullOrEmpty(sceneInBuild.path))
                {
                    Debug.LogWarning("Invalid scene path in build settings.");
                    continue;
                }

                EditorSceneManager.OpenScene(sceneInBuild.path, OpenSceneMode.Single);

                var renderers = GameObject.FindObjectsOfType<Renderer>();
                foreach (var renderer in renderers)
                {
                    foreach (var mat in renderer.sharedMaterials)
                    {
                        if (!mat || !mat.shader) continue;
                        
                        shaderDict.TryAdd(mat.shader.name, new ShaderMaterialInfo { OriginalShader = mat.shader });

                        shaderDict[mat.shader.name].AddUniqueMaterial(mat, renderer.gameObject);
                    }
                }
            }

            // Handle addressable:
            EditorUtility.DisplayProgressBar("Refreshing Shaders and Materials", "Processing Addressables", currentStep / (float)totalSteps);
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings)
            {
                foreach (var group in settings.groups)
                {
                    foreach (var entry in group.entries)
                    {
                        var     path       = AssetDatabase.GUIDToAssetPath(entry.guid);
                        var mainObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                        var dependencies = this.GetAllDependencies(path);
                        foreach (var mat in dependencies.Select(depPath => AssetDatabase.LoadAssetAtPath<Material>(depPath)).Where(mat => mat))
                        {
                            shaderDict.TryAdd(mat.shader.name, new ShaderMaterialInfo { OriginalShader = mat.shader });

                            var info = shaderDict[mat.shader.name];
                            info.AddUniqueMaterial(mat, mainObject); // Assume the mainObject is using the material.
                        }
                    }
                }
            }

            // Return to the original scene:
            if (!string.IsNullOrEmpty(originalScenePath))
            {
                EditorSceneManager.OpenScene(originalScenePath);
            }
            else
            {
                Debug.LogWarning("Original scene path is null or empty. Cannot revert to the original scene.");
            }

            // Clear progress bar after completion:
            EditorUtility.ClearProgressBar();

            return new List<ShaderMaterialInfo>(shaderDict.Values);
        }

        private List<string> GetAllDependencies(string assetPath)
        {
            return new List<string>(AssetDatabase.GetDependencies(assetPath, true));
        }

        public static ValueDropdownList<Shader> GetAllShadersInProject()
        {
            var shaders  = AssetDatabase.FindAssets("t:Shader");
            var dropdown = new ValueDropdownList<Shader>();
            foreach (var shader in shaders)
            {
                var path = AssetDatabase.GUIDToAssetPath(shader);
                var s    = AssetDatabase.LoadAssetAtPath<Shader>(path);
                dropdown.Add(s.name, s);
            }

            return dropdown;
        }
    }

    [System.Serializable]
    public class ShaderMaterialInfo
    {
        [InlineProperty]
        [Title("Original Shader", TitleAlignment = TitleAlignments.Centered)]
        [ReadOnly]
        public Shader OriginalShader;

        [Title("Replacement", TitleAlignment = TitleAlignments.Centered)]
        [ValueDropdown("GetAllShadersForDropdown")]
        public Shader ReplacementShader;

        [ShowInInspector] [HideLabel] [TableList] [Title("Materials", TitleAlignment = TitleAlignments.Centered)]
        [TableColumnWidth(200)]
        private List<MaterialInfo> Materials = new List<MaterialInfo>();

        private ValueDropdownList<Shader> GetAllShadersForDropdown()
        {
            return ShaderListOdinWindow.GetAllShadersInProject();
        }

        public bool CanReplaceShader() { return this.ReplacementShader != null && this.ReplacementShader != this.OriginalShader; }


        private bool ContainsMaterial(Material material) { return this.Materials.Exists(m => m.Material == material); }

        public void AddUniqueMaterial(Material material, GameObject obj)
        {
            var existingInfo = this.Materials.FirstOrDefault(m => m.Material == material);

            if (existingInfo == null)
            {
                var newInfo = new MaterialInfo { Material = material };
                newInfo.UsingObjects.Add(obj);
                this.Materials.Add(newInfo);
            }
            else
            {
                if (!existingInfo.UsingObjects.Contains(obj))
                {
                    existingInfo.UsingObjects.Add(obj);
                }
            }
        }


        [ButtonGroup("Action")]
        [Button("Replace Shader", ButtonSizes.Medium)]
        [EnableIf("CanReplaceShader")]
        [GUIColor(0.3f, 0.8f, 0.3f)]
        public void ReplaceShaderForThis()
        {
            if (this.ReplacementShader != null)
            {
                foreach (var materialInfo in this.Materials)
                {
                    if (materialInfo.ShouldReplace)
                    {
                        materialInfo.Material.shader = this.ReplacementShader;
                    }
                }

                AssetDatabase.SaveAssets();
            }

            // After replacing shaders, refresh the shaderInfos list:
            var window = (ShaderListOdinWindow)EditorWindow.GetWindow(typeof(ShaderListOdinWindow));
            if (window != null)
            {
                window.RefreshShadersAndMaterials();
                window.Repaint();
            }
        }
        
        [ButtonGroup("Action")]
        [Button("Select All", ButtonSizes.Medium)]
        [GUIColor(0.6f, 0.8f, 1f)]
        public void SelectAllMaterials()
        {
            foreach (var materialInfo in this.Materials)
            {
                materialInfo.ShouldReplace = true;
            }
        }

        [ButtonGroup("Action")]
        [Button("Deselect All", ButtonSizes.Medium)]
        [GUIColor(1f, 0.6f, 0.6f)]
        public void DeselectAllMaterials()
        {
            foreach (var materialInfo in this.Materials)
            {
                materialInfo.ShouldReplace = false;
            }
        }
    }

    [System.Serializable]
    public class MaterialInfo
    {
        [ReadOnly]
        public Material Material;
        [ReadOnly]
        public List<GameObject> UsingObjects = new();
        private bool isImportedAsset;

        [ShowInInspector]
        public bool ShouldReplace 
        {
            get
            {
                // If the asset is imported (like materials in FBX), always return false.
                if (this.isImportedAsset)
                    return false;
                return this._shouldReplace;
            }
            set
            {
                if (!this.isImportedAsset)
                    this._shouldReplace = value;
            }
        }
        private bool _shouldReplace = true; 

        public MaterialInfo()
        {
            // Check if the material is part of an imported asset.
            var assetPath = AssetDatabase.GetAssetPath(this.Material);
            var importer  = AssetImporter.GetAtPath(assetPath);
            this.isImportedAsset = importer != null && importer.assetPath != assetPath;
        }
    }
}