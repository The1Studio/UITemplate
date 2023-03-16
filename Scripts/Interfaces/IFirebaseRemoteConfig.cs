namespace TheOneStudio.UITemplate.UITemplate.Scripts.Interfaces
{
    public interface IFirebaseRemoteConfig
    {
        string GetRemoteConfigStringValue(string key);
        bool   GetRemoteConfigBoolValue(string key);
        long   GetRemoteConfigLongValue(string key);
        double GetRemoteConfigDoubleValue(string key);
        int    GetRemoteConfigIntValue(string key);
        float  GetRemoteConfigFloatValue(string key);
    }
}