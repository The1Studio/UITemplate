namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using Sirenix.Serialization;

    public class UITemplateCurrencyData
    {

                        public string Id          { get; private set; }
        [OdinSerialize] public int    Value       { get; internal set; }
        [OdinSerialize] public int    TotalEarned { get; internal set; }
        [OdinSerialize] public int    MaxValue    { get; set; }

        public UITemplateCurrencyData(string id, int value, int maxValue, int totalEarned = 0)
        {
            this.Id          = id;
            this.Value       = value;
            this.TotalEarned = totalEarned;
            this.MaxValue    = maxValue;

        }
    }
}