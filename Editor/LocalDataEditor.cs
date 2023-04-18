namespace UITemplate.Editor
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Interfaces;
    using GameFoundation.Scripts.Utilities.Extension;
    using Newtonsoft.Json;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Serialization;
    using UnityEditor;
    using UnityEngine;

    public class LocalDataEditor : OdinEditorWindow
    {
        private const string LocalDataPrefix = "LD-";

        [OdinSerialize, HideLabel] [ListDrawerSettings(Expanded = true, ShowPaging = true, ShowItemCount = true, IsReadOnly = true, DraggableItems = false, NumberOfItemsPerPage = 5)]
        private List<ILocalData> localData;

        private Vector2 scrollPosition;

        [HorizontalGroup]
        [Button(ButtonSizes.Large)]
        private void LoadLocalData()
        {
            this.localData = LoadAllLocalData();
            Debug.LogError($"Load Complete");
        }

        [HorizontalGroup]
        [Button(ButtonSizes.Large)]
        private void SaveLocalData()
        {
            if (this.localData is { Count: 0 }) return;

            foreach (var data in this.localData)
            {
                var key  = $"{LocalDataPrefix}{data.GetType().Name}";
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                PlayerPrefs.SetString(key, json);
            }

            Debug.LogError($"Save Complete");
        }

        [Button(ButtonSizes.Large)]
        [GUIColor(0.5f, 0, 0)]
        private void ClearLocalData()
        {
            PlayerPrefs.DeleteAll();
            this.localData = null;
            Debug.LogError($"Clear Complete");
        }

        [MenuItem("Tools/TheOneStudio/Local Data Editor")]
        public static void ShowWindow() { GetWindow(typeof(LocalDataEditor)); }

        private static List<ILocalData> LoadAllLocalData()
        {
            var result = new List<ILocalData>();

            var localDataTypes = ReflectionUtils.GetAllDerivedTypes<ILocalData>();

            foreach (var type in localDataTypes)
            {
                var key = $"{LocalDataPrefix}{type.Name}";

                if (!PlayerPrefs.HasKey(key)) continue;
                var json = PlayerPrefs.GetString(key);

                if (JsonConvert.DeserializeObject(json, type) is ILocalData data)
                    result.Add(data);
            }

            return result;
        }
    }
}