namespace UITemplate.Editor.TheOneWindowTools.ListAndOptimize
{
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using TMPro;
    using UnityEditor;
    using UnityEditor.AddressableAssets.Settings;
    using UnityEngine;
    using UnityEngine.UI;

    public class TextFinderOdin : OdinEditorWindow
    {
        
        [ButtonGroup("Action")]
        [Button(ButtonSizes.Small)]
        private void FindAllText() {this.FindTMP();} 
        
        [MenuItem("TheOne/List And Optimize/Text List")]
        private static void OpenWindow() { GetWindow<TextFinderOdin>().Show(); }

        [ShowInInspector] [TableList(ShowPaging = true)] [Title("Texture", TitleAlignment = TitleAlignments.Centered)]
        private List<TextMeshProUGUI> textMeshProUguis = new();
        
        private void FindTMP()
        {
            this.textMeshProUguis.Clear();
            
            var TMP = AddressableSearcherTool.GetAllAssetInAddressable2<TextMeshProUGUI>();
            foreach (var textMP in TMP)
            {   
                var text         = textMP.Key;
                var assetPath    = AssetDatabase.GetAssetPath(text);
                var objectImport = AssetImporter.GetAtPath(assetPath);
                this.textMeshProUguis.Add(textMP.Key);
            }   
            Debug.Log(TMP.Count);   
        }
    }
}