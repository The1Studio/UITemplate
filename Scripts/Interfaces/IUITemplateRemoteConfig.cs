namespace TheOneStudio.UITemplate.UITemplate.Interfaces
{
    public interface IUITemplateRemoteConfig
    {
        string GetRemoteConfigStringValue(string key);
        bool   GetRemoteConfigBoolValue(string   key, bool defaultValue);
        long   GetRemoteConfigLongValue(string   key);
        double GetRemoteConfigDoubleValue(string key);
        int    GetRemoteConfigIntValue(string    key, int defaultValue);
        float  GetRemoteConfigFloatValue(string  key);
    }
}