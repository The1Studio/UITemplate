namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using BlueprintFlow.BlueprintReader;
    using UnityEngine.Scripting;

    [Preserve]
    [BlueprintReader("UITemplateMiscParam", true)]
    public class UITemplateMiscParamBlueprint : GenericBlueprintReaderByCol
    {
        public string PolicyAddress { get; [Preserve] private set; }
        public string TermsAddress  { get; [Preserve] private set; }
    }
}