namespace UITemplate.Editor.ProjectMigration
{
    using UITemplate.Editor.ProjectMigration.MigrationModules;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public class ProjectAutoMigrationScript
    {
        [InitializeOnLoadMethod]
        private static void OnProjectLoadedInEditor()
        {
            PackageMigration.CheckAndUpdatePackageManagerSettings();
            ProguardMigration.CheckAndUpdateProguardFile();
            ProjectSettingMigration.APICompatibilityLevel();
            FolderMigration.RemoveUselessFolder();
            
            if (EditorUtility.scriptCompilationFailed || EditorApplication.isCompiling)
            {
                Debug.LogWarning("Skipping migration due to compilation errors or isCompiling.");
                return;
            }
            
            // TODO: Temporary disable auto migration for Applovin, Update it later
            // ApplovinMigration.DoMigrate();
            LevelPlayMigration.DoMigration();
        }

        static ProjectAutoMigrationScript()
        {
            EditorApplication.focusChanged += (focus) =>
            {
                if (!focus) return;
                OnProjectLoadedInEditor();
            };
            OnProjectLoadedInEditor();
        }
    }
}