namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("UITemplateMiscParam", true)]
    public class UITemplateMiscParamBlueprint : GenericBlueprintReaderByCol
    {
        public string PolicyAddress { get; set; }
        public string TermsAddress  { get; set; }
    }
}