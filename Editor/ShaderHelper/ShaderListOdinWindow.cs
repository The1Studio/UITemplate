namespace UITemplate.Editor.ShaderHelper
{
    using System.Collections.Generic;
    using System.Linq;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEditor.AddressableAssets;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class ShaderListOdinWindow : OdinEditorWindow
    {
        [Button("Refresh Shaders and Materials")]
        [GUIColor(0.3f, 0.8f, 0.3f)]
        public void RefreshShadersAndMaterials() { this.shaderInfos = this.FindAllShadersAndMaterials(); }

        [ListDrawerSettings(Expanded = true)] [TableList] [ShowInInspector]
        private List<ShaderMaterialInfo> shaderInfos = new List<ShaderMaterialInfo>();

        [MenuItem("Window/TheOne/Shader List")]
        private static void OpenWindow() { GetWindow<ShaderListOdinWindow>().Show(); }
    
        private List<ShaderMaterialInfo> FindAllShadersAndMaterials()
        {
            Dictionary<string, ShaderMaterialInfo> shaderDict = new Dictionary<string, ShaderMaterialInfo>();

            // Store the original scene path so we can return to it later.
            string originalScenePath = EditorSceneManager.GetActiveScene().path;

            var scenes      = EditorBuildSettings.scenes;
            int totalSteps  = scenes.Length + 1; // +1 for addressables processing.
            int currentStep = 0;

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

                Scene scene = EditorSceneManager.OpenScene(sceneInBuild.path, OpenSceneMode.Single);

                Renderer[] renderers = GameObject.FindObjectsOfType<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    foreach (Material mat in renderer.sharedMaterials)
                    {
                        if (mat && mat.shader)
                        {
                            if (!shaderDict.ContainsKey(mat.shader.name))
                            {
                                shaderDict[mat.shader.name] = new ShaderMaterialInfo { OriginalShader = mat.shader };
                            }

                            shaderDict[mat.shader.name].AddUniqueMaterial(mat, renderer.gameObject);
                        }
                    }
                }
            }

            // Handle addressables:
            EditorUtility.DisplayProgressBar("Refreshing Shaders and Materials", "Processing Addressables", currentStep / (float)totalSteps);
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings)
            {
                foreach (var group in settings.groups)
                {
                    foreach (var entry in group.entries)
                    {
                        string     path       = AssetDatabase.GUIDToAssetPath(entry.guid);
                        GameObject mainObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                        List<string> dependencies = this.GetAllDependencies(path);
                        foreach (var depPath in dependencies)
                        {
                            Material mat = AssetDatabase.LoadAssetAtPath<Material>(depPath);
                            if (mat)
                            {
                                shaderDict.TryAdd(mat.shader.name, new ShaderMaterialInfo { OriginalShader = mat.shader });

                                ShaderMaterialInfo info = shaderDict[mat.shader.name];
                                info.AddUniqueMaterial(mat, mainObject); // Assume the mainObject is using the material.
                            }
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
                string path = AssetDatabase.GUIDToAssetPath(shader);
                Shader s    = AssetDatabase.LoadAssetAtPath<Shader>(path);
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
            MaterialInfo existingInfo = this.Materials.FirstOrDefault(m => m.Material == material);

            if (existingInfo == null)
            {
                MaterialInfo newInfo = new MaterialInfo { Material = material };
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
            ShaderListOdinWindow window = (ShaderListOdinWindow)EditorWindow.GetWindow(typeof(ShaderListOdinWindow));
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
                if (isImportedAsset)
                    return false;
                return _shouldReplace;
            }
            set
            {
                if (!isImportedAsset)
                    _shouldReplace = value;
            }
        }
        private bool _shouldReplace = true; 

        public MaterialInfo()
        {
            // Check if the material is part of an imported asset.
            string        assetPath = AssetDatabase.GetAssetPath(Material);
            AssetImporter importer  = AssetImporter.GetAtPath(assetPath);
            isImportedAsset = importer != null && importer.assetPath != assetPath;
        }
    }
}