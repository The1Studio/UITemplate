namespace TheOneStudio.UITemplate.Quests.Data
{
    using System;
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.Quests.Data.Conditions;
    using TheOneStudio.UITemplate.Quests.Data.Rewards;
    using UnityEngine;

    [BlueprintReader("Quest")]
    public class QuestBlueprint : GenericBlueprintReaderByRow<string, QuestRecord>
    {
    }

    [CsvHeaderKey(nameof(Id))]
    public class QuestRecord
    {
        public string Id     { get; private set; }
        public Quest  Record { get; private set; }

        public QuestRecord()
        {
        }

        public QuestRecord(string id, Quest record)
        {
            this.Id     = id;
            this.Record = record;
        }

        [Serializable]
        public class Quest
        {
            [JsonProperty] [field: SerializeField] public string Id          { get; private set; } = "";
            [JsonProperty] [field: SerializeField] public string Name        { get; private set; } = "";
            [JsonProperty] [field: SerializeField] public string Description { get; private set; } = "";
            [JsonProperty] [field: SerializeField] public string Image       { get; private set; } = "";

            [JsonProperty] [field: SerializeReference] public IReadOnlyList<string>  Tags    { get; } = new List<string>();
            [JsonProperty] [field: SerializeReference] public IReadOnlyList<IReward> Rewards { get; } = new List<IReward>();

            [JsonProperty] [field: SerializeReference] public IReadOnlyList<ICondition> StartConditions    { get; } = new List<ICondition>();
            [JsonProperty] [field: SerializeReference] public IReadOnlyList<ICondition> ShowConditions     { get; } = new List<ICondition>();
            [JsonProperty] [field: SerializeReference] public IReadOnlyList<ICondition> CompleteConditions { get; } = new List<ICondition>();
            [JsonProperty] [field: SerializeReference] public IReadOnlyList<ICondition> ResetConditions    { get; } = new List<ICondition>();
        }
    }
}