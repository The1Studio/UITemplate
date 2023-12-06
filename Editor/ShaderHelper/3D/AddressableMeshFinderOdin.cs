namespace UITemplate.Editor.ShaderHelper._3D
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEditor.AddressableAssets;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class AddressableMeshFinderOdin : OdinEditorWindow
    {
        [ShowInInspector] [TableList] [Title("Off Compression Meshes", TitleAlignment = TitleAlignments.Centered)]
        private HashSet<MeshInfo> offCompressionMeshInfoList = new();

        [ShowInInspector] [TableList] [Title("Low Compression Meshes", TitleAlignment = TitleAlignments.Centered)]
        private HashSet<MeshInfo> lowCompressionMeshInfoList = new();

        [ShowInInspector] [TableList] [Title("Medium Compression Meshes", TitleAlignment = TitleAlignments.Centered)]
        private HashSet<MeshInfo> mediumCompressionMeshInfoList = new();

        [ShowInInspector] [TableList] [Title("Hih Compression Meshes", TitleAlignment = TitleAlignments.Centered)]
        private HashSet<MeshInfo> highCompressionMeshInfoList = new();

        [MenuItem("TheOne/3D/Mesh List")]
        private static void OpenWindow() { GetWindow<AddressableMeshFinderOdin>().Show(); }

        [ButtonGroup("Action")]
        [Button(ButtonSizes.Medium)]
        private void FindAllMeshAndImporter()
        {
            this.FindMeshesInAddressables();
        }

        [ButtonGroup("Action")]
        [Button(ButtonSizes.Medium)]
        [GUIColor(0.3f, 0.8f, 0.3f)]
        private void CompressAllToHigh()
        {
            var totalSteps  = this.offCompressionMeshInfoList.Count;
            var currentStep = 0;
            foreach (var meshInfo in this.offCompressionMeshInfoList)
            {
                EditorUtility.DisplayProgressBar("Refreshing Shaders and Materials", "Processing Addressables", currentStep / (float)totalSteps);

                meshInfo.ModelImporter.meshCompression      = ModelImporterMeshCompression.High;
                meshInfo.ModelImporter.animationCompression = ModelImporterAnimationCompression.Optimal;
                meshInfo.ModelImporter.SaveAndReimport();
            }
            EditorUtility.ClearProgressBar();
            this.FindMeshesInAddressables();
        }

        private void FindMeshesInAddressables()
        {
            this.offCompressionMeshInfoList.Clear();
            this.lowCompressionMeshInfoList.Clear();
            this.mediumCompressionMeshInfoList.Clear();
            this.highCompressionMeshInfoList.Clear();

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings)
            {
                var totalSteps  = settings.groups.Sum(group => group.entries.Count);
                var currentStep = 0;
                foreach (var group in settings.groups)
                {
                    foreach (var entry in group.entries)
                    {
                        EditorUtility.DisplayProgressBar("Refreshing Shaders and Materials", "Processing Addressables", currentStep / (float)totalSteps);

                        var path       = AssetDatabase.GUIDToAssetPath(entry.guid);
                        var mainObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                        var dependencies = this.GetAllDependencies(path);

                        foreach (var mesh in dependencies.Select(depPath => AssetDatabase.LoadAssetAtPath<Mesh>(depPath)).Where(mat => mat))
                        {
                            var assetPath     = AssetDatabase.GetAssetPath(mesh);
                            var modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;

                            var meshInfo = new MeshInfo
                            {
                                Mesh            = mesh,
                                GameObject      = mainObject,
                                ModelImporter   = modelImporter // Storing the reference
                            };

                            switch (modelImporter.meshCompression)
                            {
                                case ModelImporterMeshCompression.Off:
                                    this.offCompressionMeshInfoList.Add(meshInfo);
                                    break;
                                case ModelImporterMeshCompression.Low:
                                    this.lowCompressionMeshInfoList.Add(meshInfo);
                                    break;
                                case ModelImporterMeshCompression.Medium:
                                    this.mediumCompressionMeshInfoList.Add(meshInfo);
                                    break;
                                case ModelImporterMeshCompression.High:
                                    this.highCompressionMeshInfoList.Add(meshInfo);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }
                }
            }

            EditorUtility.ClearProgressBar();
        }

        private List<string> GetAllDependencies(string assetPath) { return new List<string>(AssetDatabase.GetDependencies(assetPath, true)); }
    }

    [Serializable]
    public class MeshInfo
    {
        [InlineProperty] [ShowInInspector] public Mesh                              Mesh                 { get; set; }
        [InlineProperty] [ShowInInspector] public Object                            GameObject           { get; set; }
        [InlineProperty] [ShowInInspector] public ModelImporter                     ModelImporter        { get; set; }
        [InlineProperty] [ShowInInspector] public ModelImporterMeshCompression      MeshCompression      => this.ModelImporter.meshCompression;
        [InlineProperty] [ShowInInspector] public ModelImporterAnimationCompression AnimationCompression => this.ModelImporter.animationCompression;


        // Example method to get specific information from ModelImporter
        public string GetImporterInfo() { return this.ModelImporter != null ? this.ModelImporter.assetPath : "No Importer"; }
    }
}