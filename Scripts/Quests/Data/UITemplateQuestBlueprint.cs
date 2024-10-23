namespace TheOneStudio.UITemplate.Quests.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BlueprintFlow.BlueprintReader;
    using Newtonsoft.Json;
    using TheOne.Data.Conversion;
    using TheOne.Extensions;
    using TheOneStudio.UITemplate.Quests.Conditions;
    using TheOneStudio.UITemplate.Quests.Rewards;
    using TheOneStudio.UITemplate.Quests.TargetHandler;
    using UnityEngine.Scripting;

    [Preserve]
    [BlueprintReader("UITemplateQuest")]
    public sealed class UITemplateQuestBlueprint : GenericBlueprintReaderByRow<string, QuestRecord>
    {
        public sealed class JsonConverter : Converter
        {
            private static readonly HashSet<Type> SupportedTypes = new() { typeof(IReward), typeof(ICondition), typeof(IRedirectTarget) };

            private readonly Dictionary<Type, IReadOnlyDictionary<string, Type>> typeMap = new();

            protected override bool CanConvert(Type type) => SupportedTypes.Contains(type);

            protected override object ConvertFromString(string str, Type baseType)
            {
                var index = str.IndexOf(":", StringComparison.Ordinal);
                var type  = this.GetTypeMap(baseType)[str[..index]];
                return JsonConvert.DeserializeObject(str[(index + 1)..], type)!;
            }

            protected override string ConvertToString(object obj, Type baseType)
            {
                var typeStr = this.GetTypeMap(baseType).First(kv => kv.Value == obj.GetType()).Key;
                return $"{typeStr}:{JsonConvert.SerializeObject(obj)}";
            }

            private IReadOnlyDictionary<string, Type> GetTypeMap(Type baseType)
            {
                return this.typeMap.GetOrAdd(
                    baseType,
                    () =>
                    {
                        var postFix = baseType.Name[1..];
                        return baseType.GetDerivedTypes().ToDictionary(type =>
                            type.Name.EndsWith(postFix)
                                ? type.Name[..^postFix.Length]
                                : type.Name
                        );
                    }
                );
            }
        }
    }

    [Preserve]
    [CsvHeaderKey(nameof(Id))]
    public sealed class QuestRecord
    {
        public string          Id          { get; [Preserve] private set; }
        public string          Name        { get; [Preserve] private set; }
        public string          Description { get; [Preserve] private set; }
        public string          Image       { get; [Preserve] private set; }
        public HashSet<string> Tags        { get; [Preserve] private set; }

        public List<IReward>         Rewards            { get; [Preserve] private set; }
        public List<ICondition>      StartConditions    { get; [Preserve] private set; }
        public List<ICondition>      ShowConditions     { get; [Preserve] private set; }
        public List<ICondition>      CompleteConditions { get; [Preserve] private set; }
        public List<ICondition>      ResetConditions    { get; [Preserve] private set; }
        public List<IRedirectTarget> Target             { get; [Preserve] private set; }

        public bool HasTag(string tag)
        {
            return this.Tags.Contains(tag);
        }
    }
}