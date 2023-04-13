namespace TheOneStudio.UITemplate.UITemplate.Models
{
    public class UITemplateCurrencyData
    {
        public string Id       { get; }
        public int    Value    { get; set; }
        public int    MaxValue { get; set; }

        public UITemplateCurrencyData(string id, int value, int maxValue)
        {
            this.Id       = id;
            this.Value    = value;
            this.MaxValue = maxValue;
        }
    }
}