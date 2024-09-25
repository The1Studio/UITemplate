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
            if (EditorUtility.scriptCompilationFailed || EditorApplication.isCompiling)
            {
                Debug.LogWarning("Skipping migration due to compilation errors or isCompiling.");
                return;
            }
            
            ProguardMigration.CheckAndUpdateProguardFile();
            PackageMigration.CheckAndUpdatePackageManagerSettings();
            FolderMigration.RemoveUselessFolder();
            // TODO: Temporary disable auto migration for Applovin, Update it later
            // ApplovinMigration.DoMigrate();
            LevelPlayMigration.DoMigration();
            ProjectSettingMigration.APICompatibilityLevel();
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