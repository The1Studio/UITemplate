namespace TheOneStudio.UITemplate.UITemplate.Interfaces
{
    using System;
    using Cysharp.Threading.Tasks;

    public interface IUITemplateRemoteConfig
    {
        bool   IsConfigFetchedSucceed { get; }
        string GetRemoteConfigStringValue(string key, string defaultValue = "");
        async void GetRemoteConfigStringValueAsync(string key, Action<string> setter, string defaultValue = "")
        {
            await UniTask.WaitUntil(() => this.IsConfigFetchedSucceed);
            setter.Invoke(this.GetRemoteConfigStringValue(key, defaultValue));
        }

        bool GetRemoteConfigBoolValue(string key, bool defaultValue);
        async void GetRemoteConfigBoolValueAsync(string key, Action<bool> setter, bool defaultValue)
        {
            await UniTask.WaitUntil(() => this.IsConfigFetchedSucceed);
            setter.Invoke(this.GetRemoteConfigBoolValue(key, defaultValue));
        }

        long GetRemoteConfigLongValue(string key, long defaultValue);
        async void GetRemoteConfigLongValueAsync(string key, Action<long> setter, long defaultValue)
        {
            await UniTask.WaitUntil(() => this.IsConfigFetchedSucceed);
            setter.Invoke(this.GetRemoteConfigLongValue(key, defaultValue));
        }
        double GetRemoteConfigDoubleValue(string key, double defaultValue);
        async void GetRemoteConfigDoubleValueAsync(string key, Action<double> setter, double defaultValue)
        {
            await UniTask.WaitUntil(() => this.IsConfigFetchedSucceed);
            setter.Invoke(this.GetRemoteConfigDoubleValue(key, defaultValue));
        }
        int GetRemoteConfigIntValue(string key, int defaultValue);
        async void GetRemoteConfigIntValueAsync(string key, Action<int> setter, int defaultValue)
        {
            await UniTask.WaitUntil(() => this.IsConfigFetchedSucceed);
            setter.Invoke(this.GetRemoteConfigIntValue(key, defaultValue));
        }
        float GetRemoteConfigFloatValue(string key, float defaultValue);
        async void GetRemoteConfigFloatValueAsync(string key, Action<float> setter, float defaultValue)
        {
            await UniTask.WaitUntil(() => this.IsConfigFetchedSucceed);
            setter.Invoke(this.GetRemoteConfigFloatValue(key, defaultValue));
        }
    }
}