namespace TheOneStudio.HyperCasual.Editor
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sylvan.Data.Csv;
    using TheOneStudio.UITemplate.Quests.Data;
    using UnityEditor;
    using UnityEngine;

    public class QuestEditorWindow : OdinEditorWindow
    {
        private const string BLUEPRINT_PATH = "Assets/Resources/BlueprintData/UITemplateQuest.csv";

        [MenuItem("TheOne/Quest Editor")]
        public static void Open()
        {
            GetWindow<QuestEditorWindow>("Quest Editor");
        }

        [SerializeField] private List<QuestRecord.Quest> quests;

        private void Awake()
        {
            this.Load();
        }

        private void Load()
        {
            if (!File.Exists(BLUEPRINT_PATH))
            {
                File.WriteAllText(BLUEPRINT_PATH, "Id,Record");
            }
            var       blueprint = new QuestBlueprint();
            var       text      = File.ReadAllText(BLUEPRINT_PATH);
            using var reader    = CsvDataReader.Create(new StringReader(text));
            while (reader.Read()) blueprint.Add(reader);
            this.quests = blueprint.Values.Select(row => row.Record).ToList();
        }

        [Button]
        private void Save()
        {
            var blueprint = new QuestBlueprint();
            this.quests.ForEach(quest => blueprint.Add(quest.Id, new QuestRecord(quest.Id, quest)));
            var text = blueprint.SerializeToRawData().Select(row => string.Join(",", row.Select(cell => $"\"{cell.Replace("\"", "\"\"")}\"")));
            File.WriteAllLines(BLUEPRINT_PATH, text);
        }
    }
}