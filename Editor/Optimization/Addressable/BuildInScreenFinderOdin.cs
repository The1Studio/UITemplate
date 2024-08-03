namespace UITemplate.Editor.Optimization.Addressable
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.U2D;

    public class BuildInScreenFinderOdin : OdinEditorWindow
    {
        private static string BuildinScreenAssetGroupName    = "BuildInScreenAsset";
        private static string NotBuildinScreenAssetGroupName = "NotBuildInScreenAsset";


        [ShowInInspector] [TableList] [Title("Active Scenes", TitleAlignment = TitleAlignments.Centered)]
        private HashSet<SceneAsset> activeScenes = new();

        [ShowInInspector] [TableList] [Title("Dependencies asset", TitleAlignment = TitleAlignments.Centered)]
        private HashSet<Object> dependencyAssets = new();

        [ShowInInspector] [TableList] [Title("Not In Right atlas texture", TitleAlignment = TitleAlignments.Centered)]
        private HashSet<Texture> notInRightAtlasTexture = new();
        
        [ShowInInspector] [TableList] [Title("Build In Scenes Assets that not in right group", TitleAlignment = TitleAlignments.Centered)]
        private HashSet<Object> buildInSceneAssetsThatNotInRightGroup = new();

        [ShowInInspector] [TableList] [Title("Not Build In Scenes Assets that not in right group", TitleAlignment = TitleAlignments.Centered)]
        private HashSet<Object> notBuildInSceneAssetsThatNotInRightGroup = new();

        [MenuItem("TheOne/List And Optimize/Build-In Screens List")]
        private static void OpenWindow() { GetWindow<BuildInScreenFinderOdin>().Show(); }

        [ButtonGroup("Optimize Loading Screen")]
        [Button(ButtonSizes.Medium),  GUIColor(0, 1, 0)]
        private void AnalyzeBuildInScene() { this.FindActiveScreensAndAddressableReferences(); }

        [ButtonGroup("Optimize Loading Screen"), GUIColor(1, 1, 0)]
        [Button(ButtonSizes.Medium)]
        private void RegroupBuildInScreenAddressable()
        {
            this.MoveWrongAtlasTexture();
            this.ReGroup();
        }

        private void FindActiveScreensAndAddressableReferences()
        {
            (this.activeScenes, this.dependencyAssets, this.buildInSceneAssetsThatNotInRightGroup, this.notBuildInSceneAssetsThatNotInRightGroup) = AnalyzeProject();

            this.notInRightAtlasTexture = AnalyzeAtlases(this.dependencyAssets);
        }

        private static (HashSet<SceneAsset>, HashSet<Object>, HashSet<Object>, HashSet<Object>) AnalyzeProject()
        {
            // Find all active screens in the build
            var activeScenes = FindActiveScreens().ToHashSet();
            // Find all referenced assets in Addressable
            var dependencyAssets = activeScenes.SelectMany(sceneAsset => AssetSearcher.GetAllDependencies<Object>(sceneAsset)).ToHashSet();

            var buildInSceneAssetsThatNotInRightGroup = dependencyAssets.Where(asset => AssetSearcher.IsAssetAddressable(asset, out var group) && !group.name.Equals(BuildinScreenAssetGroupName))
                .ToHashSet();

            var allAssetsInGroup = AssetSearcher.GetAllAssetsInGroup(BuildinScreenAssetGroupName);
            var notBuildInSceneAssetsThatNotInRightGroup = allAssetsInGroup.Where(asset => !dependencyAssets.Contains(asset))
                .ToHashSet();
            
            return (activeScenes, dependencyAssets, buildInSceneAssetsThatNotInRightGroup, notBuildInSceneAssetsThatNotInRightGroup);
        }

        private static HashSet<Texture> AnalyzeAtlases(HashSet<Object> dependenciesAsset)
        {
            var result = new HashSet<Texture>();
            var atlases                   = AssetSearcher.GetAllAssetsOfType<SpriteAtlas>();
            foreach (var textureInAtlases in atlases.Select(spriteAtlas => AssetSearcher.GetAllDependencies<Texture>(spriteAtlas)))
            {
                result.AddRange(textureInAtlases.Where(dependenciesAsset.Contains));
            }

            return result;
        }

        private static IEnumerable<SceneAsset> FindActiveScreens() => EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path));

        public static void AutoOptimize()
        {
            var (activeScenes, dependenciesAssets, buildInSceneAssetsThatNotInRightGroup, notBuildInSceneAssetsThatNotInRightGroup) = AnalyzeProject();
            if (buildInSceneAssetsThatNotInRightGroup.Count > 0 || notBuildInSceneAssetsThatNotInRightGroup.Count > 0)
            {
                var window = GetWindow<BuildInScreenFinderOdin>();
                window.Show();
                window.activeScenes                             = activeScenes;
                window.dependencyAssets                         = dependenciesAssets;
                window.buildInSceneAssetsThatNotInRightGroup    = buildInSceneAssetsThatNotInRightGroup;
                window.notBuildInSceneAssetsThatNotInRightGroup = notBuildInSceneAssetsThatNotInRightGroup;
            }
        }

        private void MoveWrongAtlasTexture()
        {
            //Create folder Assets/Sprites/BuildInUI
            var assetsSpritesBuildinui = "Assets/Sprites/BuildInUI";
            AssetSearcher.CreateFolderIfNotExist(assetsSpritesBuildinui);
            //Move this.notInRightAtlasTexture to Assets/Sprites/BuildInUI
            foreach (var texture in AnalyzeAtlases(this.dependencyAssets))
            {
                AssetSearcher.MoveToNewFolder(texture, assetsSpritesBuildinui);
            }
            AssetDatabase.Refresh();
        }
        
        private void ReGroup()
        {
            // Information and Confirmation Dialog
            bool userConfirmed = EditorUtility.DisplayDialog("Optimizing BuildIn Scene Assets!!", "This will reorganize assets into the correct Addressable Asset Groups based on their usage. Do you want to proceed?", "Ok");
            
            if (!userConfirmed)
            {
                return; // Exit if the user does not confirm
            }

            var totalAssets     = this.buildInSceneAssetsThatNotInRightGroup.Count + this.notBuildInSceneAssetsThatNotInRightGroup.Count;
            var processedAssets = 0;

            // Start Progress Bar
            EditorUtility.DisplayProgressBar("Optimizing BuildIn Scene Assets!!", "Please wait...", 0f);

            var movedAssets = new List<string>();

            foreach (var asset in this.buildInSceneAssetsThatNotInRightGroup)
            {
                AssetSearcher.MoveAssetToGroup(asset, BuildinScreenAssetGroupName);
                processedAssets++;
                movedAssets.Add(AssetDatabase.GetAssetPath(asset));
                EditorUtility.DisplayProgressBar("ReGrouping Assets", $"Processing {AssetDatabase.GetAssetPath(asset)}", processedAssets / (float)totalAssets);
            }

            foreach (var asset in this.notBuildInSceneAssetsThatNotInRightGroup)
            {
                AssetSearcher.MoveAssetToGroup(asset, NotBuildinScreenAssetGroupName);
                processedAssets++;
                movedAssets.Add(AssetDatabase.GetAssetPath(asset));
                EditorUtility.DisplayProgressBar("ReGrouping Assets", $"Processing {AssetDatabase.GetAssetPath(asset)}", processedAssets / (float)totalAssets);
            }

            // Clear Progress Bar
            EditorUtility.ClearProgressBar();

            // Display Analysis Popup
            var movedAssetsSummary = string.Join("\n", movedAssets);
            EditorUtility.DisplayDialog("Optimizing BuildIn Scene Assets Summary", $"ReGrouping Complete. Moved Assets:\n{movedAssetsSummary}", "OK");
            
            this.FindActiveScreensAndAddressableReferences();
        }
    }
}