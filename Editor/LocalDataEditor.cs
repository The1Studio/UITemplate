namespace UITemplate.Editor
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Interfaces;
    using GameFoundation.Scripts.Utilities.Extension;
    using Newtonsoft.Json;
    using UnityEditor;
    using UnityEngine;

    public class LocalDataEditor : EditorWindow
    {
        private const string LocalDataPrefix = "LD-";

        private List<ILocalData> localData;

        private Vector2 scrollPosition;

        private bool toggleAll;

        private List<bool> toggleGroupStates = new();

        private void OnGUI()
        {
            this.titleContent = new GUIContent("Local Data Editor");

            if (GUILayout.Button("Clear Local Data"))
            {
                PlayerPrefs.DeleteAll();
                this.localData = null;
                this.toggleGroupStates.Clear();
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Load Local Data"))
            {
                this.localData         = this.LoadAllLocalData();
                this.toggleGroupStates = this.localData.Select(_ => false).ToList();
            }

            if (GUILayout.Button("Save Local Data"))
                if (this.localData is not null)
                    for (var i = 0; i < this.localData.Count; i++)
                    {
                        var data = this.localData[i];

                        if (this.toggleGroupStates[i])
                        {
                            var key  = $"{LocalDataPrefix}{data.GetType().Name}";
                            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                            PlayerPrefs.SetString(key, json);
                        }
                    }

            GUILayout.EndHorizontal();

            this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition);

            if (this.localData is not null)
            {
                GUILayout.Space(20);

                GUILayout.BeginHorizontal();

                GUILayout.Label("Local Data", EditorStyles.boldLabel);

                if (GUILayout.Button("Toggle/UnToggle All"))
                {
                    this.toggleAll = !this.toggleAll;
                    for (var i = 0; i < this.toggleGroupStates.Count; i++)
                        this.toggleGroupStates[i] = this.toggleAll;
                }

                GUILayout.EndHorizontal();

                for (var i = 0; i < this.localData.Count; i++)
                {
                    var data = this.localData[i];

                    this.toggleGroupStates[i] = EditorGUILayout.BeginToggleGroup($"{data.GetType().Name}", this.toggleGroupStates[i]);

                    if (this.toggleGroupStates[i])
                    {
                        var json = EditorGUILayout.TextArea(JsonConvert.SerializeObject(data, Formatting.Indented));
                        if (JsonConvert.DeserializeObject(json, data.GetType()) is ILocalData newData)
                            this.localData[i] = newData;
                    }

                    EditorGUILayout.EndToggleGroup();
                }
            }

            GUILayout.EndScrollView();
        }

        [MenuItem("Window/TheOneStudio/Local Data Editor")]
        public static void ShowWindow()
        {
            GetWindow(typeof(LocalDataEditor));
        }

        private List<ILocalData> LoadAllLocalData()
        {
            var result = new List<ILocalData>();

            var localDataTypes = ReflectionUtils.GetAllDerivedTypes<ILocalData>();

            foreach (var type in localDataTypes)
            {
                var key = $"{LocalDataPrefix}{type.Name}";
                if (PlayerPrefs.HasKey(key))
                {
                    var json = PlayerPrefs.GetString(key);
                    if (JsonConvert.DeserializeObject(json, type) is ILocalData data)
                        result.Add(data);
                }
            }

            return result;
        }
    }
}