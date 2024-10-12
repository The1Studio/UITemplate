namespace TheOne.Tool.Optimization.Texture.MenuItems
{
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.U2D;
    using UnityEngine;

    public static class CreateAtlasFromFolders
    {
        [MenuItem("Assets/TheOne/Create Atlas For Selected Folders", true)]
        private static bool ValidateCreateAtlasForSelectedFolders()
        {
            // Validate that the selection contains at least one folder.
            return Selection.objects.Any(obj => AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(obj)));
        }

        [MenuItem("Assets/TheOne/Create Atlas For Selected Folders")]
        private static void CreateAtlasForSelectedFolders()
        {
            EditorSettings.spritePackerMode = SpritePackerMode.SpriteAtlasV2;
            foreach (var selectedObject in Selection.objects)
            {
                var path = AssetDatabase.GetAssetPath(selectedObject);
                if (AssetDatabase.IsValidFolder(path)) CreateAtlasForFolder(path, selectedObject);
            }
        }

        public static void CreateAtlasForFolder(string folderPath, Object selectedObject)
        {
            var atlas = new SpriteAtlasAsset();
            atlas.Add(new[] { selectedObject });

            // Use string manipulation to get the parent folder's path
            var pathSegments     = folderPath.Split('/');
            var parentFolderPath = string.Join("/", pathSegments.Take(pathSegments.Length - 1));
            var atlasName        = Path.GetFileName(folderPath) + "Atlas";
            var atlasPath        = Path.Combine(parentFolderPath, atlasName + ".spriteatlasv2");

            UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] { atlas }, atlasPath, true);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            var importer = AssetImporter.GetAtPath(atlasPath) as SpriteAtlasImporter;

            // Configure the atlas settings to not allow rotation and disable crunch compression
            var packingSettings = new SpriteAtlasPackingSettings()
            {
                enableRotation     = false, // Prevent rotation
                enableTightPacking = false, // Example setting, adjust as needed
                padding            = 4,     // Example setting, adjust as needed
            };
            importer.packingSettings = packingSettings;

            var textureImporterPlatformSettings = new TextureImporterPlatformSettings()
            {
                maxTextureSize      = 2048,
                format              = TextureImporterFormat.Automatic,
                crunchedCompression = true,
            };
            importer.SetPlatformSettings(textureImporterPlatformSettings);
            importer.SaveAndReimport();
            AssetDatabase.Refresh();

            Debug.Log($"Sprite Atlas created: {atlasPath}");
        }
    }
}