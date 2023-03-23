namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.AnalyticEvents.AdOne
{
    using Core.AnalyticServices.Data;

    public class UIAppear : IEvent
    {
        public string screen_name;
        public string name;
        
        public UIAppear(string screenName, string name)
        {
            this.screen_name = screenName;
            this.name        = name;
        }
    }
}