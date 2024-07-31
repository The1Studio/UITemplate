namespace UITemplate.Editor.Optimization.Addressable
{
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;

    public class AddressableGroupOdin : OdinEditorWindow
    {
        [MenuItem("TheOne/List And Optimize/Addressable Group")]
        private static void OpenWindow() { GetWindow<BuildInScreenFinderOdin>().Show(); }
    }
}