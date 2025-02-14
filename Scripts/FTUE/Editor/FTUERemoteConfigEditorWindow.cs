namespace UITemplate.Scripts.FTUE.Editor
{
    using System.IO;
    using BlueprintFlow.BlueprintReader;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEngine;
    using Sylvan.Data.Csv;
    using BlueprintFlow.BlueprintReader.Converter;
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json;
    using Sirenix.OdinInspector;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.FTUE.RemoteConfig;

    public class FTUERemoteConfigEditorWindow : OdinEditorWindow
    {
        private BlueprintRecordReader<UITemplateFTUERecord> blueprint;

        [MenuItem("TheOne/Editor/FTUE Remote Config")]
        public static void Open()
        {
            var window = GetWindow<FTUERemoteConfigEditorWindow>();
            window.Show();
        }

        #region Blueprint

        [TabGroup("General", "Blueprint")]
        [ButtonGroup("General/Blueprint/Button"), Button(ButtonSizes.Large)]
        private async UniTask ReadCsv()
        {
            this.blueprint ??= new(typeof(UITemplateFTUEBlueprint));
            var rawCsv = Resources.Load<TextAsset>("BlueprintData/UITemplateFTUE").ToString();
            await using var csv =
                await CsvDataReader.CreateAsync(new StringReader(rawCsv), CsvHelper.CsvDataReaderOptions);
            while (await csv.ReadAsync())
            {
                var (hasValue, record) = this.blueprint.GetRecord(csv);
                if (hasValue)
                {
                    this.ftueConfig[record.Id] = record;
                }
            }
        }

        [TabGroup("General", "Blueprint")]
        [ButtonGroup("General/Blueprint/Button"), Button(ButtonSizes.Large)]
        private void ToJson()
        {
            this.json = JsonConvert.SerializeObject(this.ftueConfig,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                });
        }

        [TabGroup("General", "Blueprint")] [SerializeField] private FTUEConfig ftueConfig;

        #endregion

        #region Json

        [TabGroup("General", "Json")] [TextArea(10, 30)] [ShowInInspector] private string json;

        [TabGroup("General", "Json")]
        [ButtonGroup("General/Json/Button"), Button(ButtonSizes.Large)]
        private void CopyJson()
        {
            EditorGUIUtility.systemCopyBuffer = this.json;
        }

        [TabGroup("General", "Json")]
        [ButtonGroup("General/Json/Button"), Button(ButtonSizes.Large)]
        private void ToDict()
        {
            this.ftueConfig = JsonConvert.DeserializeObject<FTUEConfig>(this.json);
        }

        #endregion
    }
}