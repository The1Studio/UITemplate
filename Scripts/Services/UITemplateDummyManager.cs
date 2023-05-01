namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services
{
    using TheOneStudio.UITemplate.UITemplate.Interfaces;

    public class UITemplateDummyManager : IUITemplateRemoteConfig
    {
        public bool   IsConfigFetchedSucceed                                     => true;
        public string GetRemoteConfigStringValue(string key,string defaultValue) { return ""; }

        public bool GetRemoteConfigBoolValue(string key, bool defaultValue) { return defaultValue; }

        public long GetRemoteConfigLongValue(string key, long defaultValue) { return defaultValue; }

        public double GetRemoteConfigDoubleValue(string key, double defaultValue) { return defaultValue; }

        public int GetRemoteConfigIntValue(string key, int defaultValue) { return defaultValue; }

        public float GetRemoteConfigFloatValue(string key, float defaultValue) { return defaultValue; }
    }
}