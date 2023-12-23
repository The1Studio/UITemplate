namespace TheOneStudio.UITemplate.Quests.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BlueprintFlow.BlueprintReader;
    using BlueprintFlow.BlueprintReader.Converter;
    using BlueprintFlow.BlueprintReader.Converter.TypeConversion;
    using GameFoundation.Scripts.Utilities.Extension;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.Quests.Conditions;
    using TheOneStudio.UITemplate.Quests.Rewards;

    [BlueprintReader("UITemplateQuest")]
    public sealed class UITemplateQuestBlueprint : GenericBlueprintReaderByRow<string, QuestRecord>
    {
        static UITemplateQuestBlueprint()
        {
            CsvHelper.RegisterTypeConverter(typeof(IReward), new Converter<IReward>());
            CsvHelper.RegisterTypeConverter(typeof(ICondition), new Converter<ICondition>());
            CsvHelper.RegisterTypeConverter(typeof(List<IReward>), new ListGenericConverter(';'));
            CsvHelper.RegisterTypeConverter(typeof(List<ICondition>), new ListGenericConverter(';'));
        }

        private class Converter<T> : ITypeConverter
        {
            private readonly Dictionary<string, Type> typeMap;

            public Converter()
            {
                var postFix = typeof(T).Name[1..];
                this.typeMap = ReflectionUtils.GetAllDerivedTypes<T>()
                    .ToDictionary(type =>
                        type.Name.EndsWith(postFix)
                            ? type.Name[..^postFix.Length]
                            : type.Name
                    );
            }

            object ITypeConverter.ConvertFromString(string str, Type _)
            {
                var index = str.IndexOf(":", StringComparison.Ordinal);
                var type  = this.typeMap[str[..index]];
                return JsonConvert.DeserializeObject(str[(index + 1)..], type);
            }

            string ITypeConverter.ConvertToString(object obj, Type type)
            {
                var typeStr = this.typeMap.First(kv => kv.Value == type).Key;
                return $"{typeStr}:{JsonConvert.SerializeObject(obj)}";
            }
        }
    }

    [CsvHeaderKey(nameof(Id))]
    public sealed class QuestRecord
    {
        public string       Id          { get; private set; }
        public string       Name        { get; private set; }
        public string       Description { get; private set; }
        public string       Image       { get; private set; }
        public List<string> Tags        { get; private set; }

        public List<IReward>    Rewards            { get; private set; }
        public List<ICondition> StartConditions    { get; private set; }
        public List<ICondition> ShowConditions     { get; private set; }
        public List<ICondition> CompleteConditions { get; private set; }
        public List<ICondition> ResetConditions    { get; private set; }
    }
}