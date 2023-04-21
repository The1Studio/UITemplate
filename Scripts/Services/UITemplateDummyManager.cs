namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services
{
    using TheOneStudio.UITemplate.UITemplate.Interfaces;

    public class UITemplateDummyManager : IUITemplateRemoteConfig
    {

        public string GetRemoteConfigStringValue(string key,string defaultValue) { return ""; }

        public bool GetRemoteConfigBoolValue(string key, bool defaultValue) { return defaultValue; }

        public long GetRemoteConfigLongValue(string key) { return 0; }

        public double GetRemoteConfigDoubleValue(string key) { return 0; }

        public int GetRemoteConfigIntValue(string key, int defaultValue) { return defaultValue; }

        public float GetRemoteConfigFloatValue(string key) { return 0; }
    }
}