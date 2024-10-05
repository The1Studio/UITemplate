namespace TheOne.Tool.Optimization
{
    using TheOne.Tool.Optimization.Addressable;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public static class ProjectAutoOptimizationScript
    {
        static ProjectAutoOptimizationScript()
        {
            if (EditorUtility.scriptCompilationFailed || EditorApplication.isCompiling)
            {
                Debug.LogWarning("Skipping migration due to compilation errors or isCompiling.");
                return;
            }
            
            if (!SessionState.GetBool("AutoOptimizeRan", false))
            {
                SessionState.SetBool("AutoOptimizeRan", true);            
                BuildInScreenFinderOdin.AutoOptimize();

            }
        }
    }
}