namespace UITemplate.Editor.Optimization
{
    using UITemplate.Editor.Optimization.Addressable;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public static class ProjectAutoOptimizationScript
    {
        static ProjectAutoOptimizationScript()
        {
            if (!SessionState.GetBool("AutoOptimizeRan", false))
            {
                SessionState.SetBool("AutoOptimizeRan", true);
                AutoOptimize();
            }
        }

        private static void AutoOptimize()
        {
            Debug.Log("Auto optimize");
            BuildInScreenFinderOdin.AutoOptimize();
        }
    }
}