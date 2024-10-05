namespace TheOne.Tool.Optimization.Addressable
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using TheOne.Tool.Core;
    using TheOne.Tool.Optimization.Texture.MenuItems;
    using UnityEditor;
    using UnityEngine;

    public class BuildInScreenFinderOdin : OdinEditorWindow
    {
        private static string NotBuildinScreenAssetGroupName = "NotBuildInScreenAsset";

        [ShowInInspector] [TableList] [Title("Dependencies asset", TitleAlignment = TitleAlignments.Centered)]
        private Dictionary<SceneAsset, HashSet<Object>> sceneToDependencyAsset = new();

        [ShowInInspector] [TableList] [Title("Not In Right atlas texture", TitleAlignment = TitleAlignments.Centered)]
        private Dictionary<SceneAsset, HashSet<Texture>> notInRightAtlasTexture = new();
        
        [ShowInInspector] [TableList] [Title("Build In Scenes Assets that not in right group", TitleAlignment = TitleAlignments.Centered)]
        private Dictionary<SceneAsset, HashSet<Object>> buildInSceneAssetsThatNotInRightGroup = new();

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

        private static string GetSceneAssetGroupName(SceneAsset sceneAsset) => $"BuildInScreenAsset_{sceneAsset.name}";

        private void FindActiveScreensAndAddressableReferences()
        {
            (this.sceneToDependencyAsset, this.buildInSceneAssetsThatNotInRightGroup, this.notBuildInSceneAssetsThatNotInRightGroup) = AnalyzeProject();

            this.notInRightAtlasTexture = AnalyzeAtlases(this.sceneToDependencyAsset);
        }

        private static (Dictionary<SceneAsset, HashSet<Object>>, Dictionary<SceneAsset, HashSet<Object>>, HashSet<Object>) AnalyzeProject()
        {
            // Find all active screens in the build
            var activeScenes = FindActiveScreens().ToList();
            // Find all referenced assets in Addressable
            var sceneToDependencyAssets = activeScenes.ToDictionary(scene => scene, scene =>
            {
                var hashSet = AssetSearcher.GetAllDependencies<Object>(scene).ToHashSet();
                return hashSet;
            });

            var countedAssets  = new HashSet<Object>();
            var buildInSceneAssetsThatNotInRightGroup = sceneToDependencyAssets.ToDictionary(kp => kp.Key, kp =>
            {
                var hashSet = kp.Value.Where(asset => IsAssetInNotRightBuildInGroup(asset, kp.Key))
                    .ToHashSet();
                hashSet.RemoveRange(countedAssets);
                countedAssets.AddRange(kp.Value);
                return hashSet;
            });
            
            var allAssetsInGroup = activeScenes.SelectMany(scene => AssetSearcher.GetAllAssetsInGroup(GetSceneAssetGroupName(scene))).ToHashSet();
            var dependencyAssets = sceneToDependencyAssets.SelectMany(kp => kp.Value).ToHashSet();
            var notBuildInSceneAssetsThatNotInRightGroup = allAssetsInGroup.Where(asset => !dependencyAssets.Contains(asset))
                .ToHashSet();
            
            return (sceneToDependencyAssets, buildInSceneAssetsThatNotInRightGroup, notBuildInSceneAssetsThatNotInRightGroup);
        }

        private static bool IsAssetInNotRightBuildInGroup(Object asset, SceneAsset sceneAsset)
        {
            return AssetSearcher.IsAssetAddressable(asset, out var group) && !group.name.Equals(GetSceneAssetGroupName(sceneAsset));
        }

        private static Dictionary<SceneAsset, HashSet<Texture>> AnalyzeAtlases(Dictionary<SceneAsset, HashSet<Object>> sceneToDependencyAsset)
        {
            var result            = new Dictionary<SceneAsset, HashSet<Texture>>();

            var countedTextures = new HashSet<Texture>();
            foreach (var (scene, assets) in sceneToDependencyAsset)
            {
                result.Add(scene, new HashSet<Texture>());
                var textures = assets.OfType<Texture>().ToHashSet();
                textures.RemoveRange(countedTextures);
                foreach (var texture in textures)
                {
                    var assetPath = AssetDatabase.GetAssetPath(texture);
                    var fileName  = Path.GetFileName(assetPath);
                    if (!assetPath.Equals($"{GetTextureBuildInPath(scene)}/{fileName}"))
                    {
                        result[scene].Add(texture);
                    }
                }
                countedTextures.AddRange(textures);
            }
            
            return result;
        }

        private static IEnumerable<SceneAsset> FindActiveScreens() => EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path));

        public static void AutoOptimize()
        {
            // TODO: fix this because it makes the editor got 2 white windows
            // var (dependenciesAssets, buildInSceneAssetsThatNotInRightGroup, notBuildInSceneAssetsThatNotInRightGroup) = AnalyzeProject();
            // if (buildInSceneAssetsThatNotInRightGroup.Count > 0 || notBuildInSceneAssetsThatNotInRightGroup.Count > 0)
            // {
            //     var window = GetWindow<BuildInScreenFinderOdin>();
            //     window.sceneToDependencyAsset                   = dependenciesAssets;
            //     window.buildInSceneAssetsThatNotInRightGroup    = buildInSceneAssetsThatNotInRightGroup;
            //     window.notBuildInSceneAssetsThatNotInRightGroup = notBuildInSceneAssetsThatNotInRightGroup;
            // }
        }

        private static string GetTextureBuildInPath(SceneAsset sceneAsset)
        {
            return $"Assets/Sprites/BuildInUI/{sceneAsset.name}";
        }

        private void MoveWrongAtlasTexture()
        {
            foreach (var (scene, textures) in this.notInRightAtlasTexture)
            {
                if (textures.Count == 0) continue;
                var folderPath = GetTextureBuildInPath(scene);
                if (AssetSearcher.CreateFolderIfNotExist(folderPath))
                {
                    AssetDatabase.Refresh();
                    CreateAtlasFromFolders.CreateAtlasForFolder(folderPath, AssetDatabase.LoadAssetAtPath<Object>(folderPath));
                    AssetDatabase.Refresh();
                }
                foreach (var texture in textures)
                {
                    AssetSearcher.MoveToNewFolder(texture, folderPath);
                }
            }
            AssetDatabase.Refresh();
        }
        
        private void ReGroup()
        {
            //if there is no asset in BuilbuildInSceneAssetsThatNotInRightGroupd and notBuildInSceneAssetsThatNotInRightGroup, return
            if (this.buildInSceneAssetsThatNotInRightGroup.Sum(kpv => kpv.Value.Count) == 0 && this.notBuildInSceneAssetsThatNotInRightGroup.Count == 0)
            {
                EditorUtility.DisplayDialog("Optimizing BuildIn Scene Assets!!", "There are no assets to reorganize.", "OK");
                return;
            }
            
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

            foreach (var (scene, values) in this.buildInSceneAssetsThatNotInRightGroup)
            {
                foreach (var asset in values)
                {
                    AssetSearcher.MoveAssetToGroup(asset, GetSceneAssetGroupName(scene));
                    processedAssets++;
                    movedAssets.Add(AssetDatabase.GetAssetPath(asset));
                    EditorUtility.DisplayProgressBar("ReGrouping Assets", $"Processing {AssetDatabase.GetAssetPath(asset)}", processedAssets / (float)totalAssets);
                }
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