namespace TheOne.Tool.Migration.ProjectMigration.MigrationModules
{
    using UnityEditor;
    using UnityEditor.Build;

    public static class ProjectSettingMigration
    {
        public static void APICompatibilityLevel()
        {
            if (PlayerSettings.GetApiCompatibilityLevel(NamedBuildTarget.Android) != ApiCompatibilityLevel.NET_Unity_4_8)
            {
                PlayerSettings.SetApiCompatibilityLevel(NamedBuildTarget.Android, ApiCompatibilityLevel.NET_Unity_4_8);
                AssetDatabase.SaveAssets();
            }

            if (PlayerSettings.GetApiCompatibilityLevel(NamedBuildTarget.iOS) != ApiCompatibilityLevel.NET_Unity_4_8)
            {
                PlayerSettings.SetApiCompatibilityLevel(NamedBuildTarget.iOS, ApiCompatibilityLevel.NET_Unity_4_8);
                AssetDatabase.SaveAssets();
            }

            if (PlayerSettings.GetApiCompatibilityLevel(NamedBuildTarget.WebGL) != ApiCompatibilityLevel.NET_Unity_4_8)
            {
                PlayerSettings.SetApiCompatibilityLevel(NamedBuildTarget.WebGL, ApiCompatibilityLevel.NET_Unity_4_8);
                AssetDatabase.SaveAssets();
            }
        }
    }
}