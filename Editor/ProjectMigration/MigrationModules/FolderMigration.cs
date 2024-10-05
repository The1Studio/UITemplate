namespace TheOne.Tool.Migration.ProjectMigration.MigrationModules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public static class FolderMigration
    {
        [NonSerialized]
        // Define a global list of folders
        private static readonly List<string> Folders = new()
        {
            "Assets/ExternalDependencyManager",
            "Assets/PlayServicesResolver"
            // Add more folders as needed
        };

        public static void RemoveUselessFolder()
        {
            var folderDeleted = false;

            // Iterate over the global list of folders
            foreach (var unused in Folders.Where(DeleteFolderIfExists))
            {
                folderDeleted = true;
            }

            if (folderDeleted)
            {
                AssetDatabase.Refresh();
            }
        }

        private static bool DeleteFolderIfExists(string folderPath)
        {
            // Check if the folder exists
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                // Delete the folder
                AssetDatabase.DeleteAsset(folderPath);

                Debug.Log($"Folder '{folderPath}' has been deleted.");

                return true;
            }

            return false;
        }
    }
}