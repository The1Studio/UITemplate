namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("UITemplateJackpotReward", true)]
    public class UITemplateJackpotRewardBlueprint: GenericBlueprintReaderByRow<string, UITemplateJackpotRewardRecord>
    {
        
    }
    
    [CsvHeaderKey("StepId")]
    public class UITemplateJackpotRewardRecord
    {
        public string StepId      { get; set; }
        public string JackpotItem { get; set; }
    }
}