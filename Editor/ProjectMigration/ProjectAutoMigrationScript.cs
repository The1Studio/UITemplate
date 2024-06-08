namespace UITemplate.Editor.ProjectMigration
{
    using UITemplate.Editor.ProjectMigration.MigrationModules;
    using UnityEditor;

    [InitializeOnLoad]
    public class ProjectAutoMigrationScript
    {
        [InitializeOnLoadMethod]
        private static void OnProjectLoadedInEditor()
        {
            ProguardMigration.CheckAndUpdateProguardFile();
            PackageMigration.CheckAndUpdatePackageManagerSettings();
            FolderMigration.RemoveUselessFolder();
            ApplovinMigration.DoMigrate();
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