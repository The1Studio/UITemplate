namespace TheOneStudio.UITemplate.UITemplate.Interfaces
{
    public interface IUITemplateRemoteConfig
    {
        string GetRemoteConfigStringValue(string key);
        bool   GetRemoteConfigBoolValue(string key);
        long   GetRemoteConfigLongValue(string key);
        double GetRemoteConfigDoubleValue(string key);
        int    GetRemoteConfigIntValue(string key);
        float  GetRemoteConfigFloatValue(string key);
    }
}