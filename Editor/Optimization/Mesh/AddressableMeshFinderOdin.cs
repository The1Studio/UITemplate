namespace TheOne.Tool.Optimization.Mesh
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using TheOne.Tool.Core;
    using UnityEditor;
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

        [MenuItem("TheOne/List And Optimize/Mesh List")]
        private static void OpenWindow() { GetWindow<AddressableMeshFinderOdin>().Show(); }

        [ButtonGroup("Action")]
        [Button(ButtonSizes.Medium)]
        private void FindAllMeshAndImporter() { this.FindMeshesInAddressables(); }

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

            var allMeshInAddressable = AssetSearcher.GetAllAssetInAddressable<Mesh>();

            foreach (var keyValuePair in allMeshInAddressable.Where(KeyValuePair => KeyValuePair.Key))
            {
                var mesh          = keyValuePair.Key;
                var assetPath     = AssetDatabase.GetAssetPath(mesh);
                var modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                if (modelImporter == null) continue;

                var meshInfo = new MeshInfo
                {
                    Mesh          = mesh,
                    Objects   = keyValuePair.Value.ToList(),
                    ModelImporter = modelImporter // Storing the reference
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

    [Serializable]
    public class MeshInfo
    {
        [InlineProperty] [ShowInInspector] public Mesh                              Mesh                 { get; set; }
        [InlineProperty] [ShowInInspector] public List<Object>                      Objects              { get; set; }
        [InlineProperty] [ShowInInspector] public ModelImporter                     ModelImporter        { get; set; }
        [InlineProperty] [ShowInInspector] public ModelImporterMeshCompression      MeshCompression      => this.ModelImporter.meshCompression;
        [InlineProperty] [ShowInInspector] public ModelImporterAnimationCompression AnimationCompression => this.ModelImporter.animationCompression;


        // Example method to get specific information from ModelImporter
        public string GetImporterInfo() { return this.ModelImporter != null ? this.ModelImporter.assetPath : "No Importer"; }
    }
}