namespace TheOneStudio.UITemplate.UITemplate.Models
{
    public class UITemplateCurrencyData
    {
        public string Id    { get; set; }
        public int    Value { get; set; }

        public UITemplateCurrencyData(string id, int value)
        {
            this.Id    = id;
            this.Value = value;
        }
    }
}