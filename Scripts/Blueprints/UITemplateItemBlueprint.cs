namespace TheOneStudio.UITemplate.UITemplate.Blueprints
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;
    using TheOneStudio.UITemplate.UITemplate.Models;

    [BlueprintReader("UITemplateItem", true)]
    public class UITemplateItemBlueprint : GenericBlueprintReaderByRow<string, UITemplateItemRecord>
    {
    }

    [CsvHeaderKey("Id")]
    public class UITemplateItemRecord
    {
        public string       Id            { get; set; }
        public string       Name          { get; set; }
        public string       Description   { get; set; }
        public string       ImageAddress  { get; set; }
        public string       Category      { get; set; }
        public bool         IsDefaultItem { get; set; }
        public List<string> ListItemId    { get; set; }
    }
}