namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.AdOne
{
    using Core.AnalyticServices.Data;

    public class ButtonClick : IEvent
    {
        public string screen_name;
        public string name;

        public ButtonClick(string screenName, string name)
        {
            this.screen_name = screenName;
            this.name        = name;
        }
    }
}