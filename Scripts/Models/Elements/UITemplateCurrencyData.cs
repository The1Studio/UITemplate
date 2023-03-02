namespace TheOneStudio.UITemplate.UITemplate.Models
{
    public class UITemplateCurrencyData
    {
        internal string Id    { get; private set; }
        public   int    Value { get; internal set; }

        public UITemplateCurrencyData(string id, int value)
        {
            this.Id    = id;
            this.Value = value;
        }
    }
}