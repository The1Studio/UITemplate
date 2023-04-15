namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services
{
    using TheOneStudio.UITemplate.UITemplate.Interfaces;

    public class UITemplateDummyManager : IUITemplateRemoteConfig
    {
        public string GetRemoteConfigStringValue(string key) { return ""; }

        public bool GetRemoteConfigBoolValue(string key) { return true; }

        public long GetRemoteConfigLongValue(string key) { return 0; }

        public double GetRemoteConfigDoubleValue(string key) { return 0; }

        public int GetRemoteConfigIntValue(string key, int defaultValue) { return defaultValue; }

        public float GetRemoteConfigFloatValue(string key) { return 0; }
    }
}