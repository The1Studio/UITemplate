namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;
    using UnityEngine.Scripting;

    [Preserve]
    [BlueprintReader("UITemplateMiscParam", true)]
    public class UITemplateMiscParamBlueprint : GenericBlueprintReaderByCol
    {
        public string PolicyAddress { get; set; }
        public string TermsAddress  { get; set; }
    }
}