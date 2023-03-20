#if FIREBASE_REMOTE_CONFIG
namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Firebase;
    using Firebase.Extensions;
    using Firebase.RemoteConfig;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Interfaces;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Signals;
    using Zenject;

    public class FirebaseRemoteConfig : IFirebaseRemoteConfig
    {
        private readonly ILogService logger;
        private readonly SignalBus   signalBus;
        public           bool        IsFirebaseReady { get; private set; }

        public FirebaseRemoteConfig(ILogService logger, SignalBus signalBus)
        {
            this.logger = logger;
            this.InitFirebase();
            this.signalBus = signalBus;
        }

        private void InitFirebase()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;

                if (dependencyStatus == DependencyStatus.Available)
                {
                    this.IsFirebaseReady = true;

                    this.FetchDataAsync();
                }
                else
                {
                    this.logger.Error($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            });
        }

        private Task FetchDataAsync()
        {
            var fetchTask =
                Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                                                                                      TimeSpan.Zero);

            return fetchTask.ContinueWithOnMainThread(this.FetchComplete);
        }

        private async void FetchComplete(Task fetchTask)
        {
            if (fetchTask.IsCanceled)
            {
                this.logger.Log("Fetch canceled.");
            }
            else if (fetchTask.IsFaulted)
            {
                this.logger.Log("Fetch encountered an error.");
            }
            else if (fetchTask.IsCompleted)
            {
                this.logger.Log("Fetch completed successfully!");
                this.signalBus.Fire<FirebaseInitializeSucceededSignal>();
            }

            var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;

            switch (info.LastFetchStatus)
            {
                case LastFetchStatus.Success:
                    await Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync();

                    break;
                case LastFetchStatus.Failure:
                    switch (info.LastFetchFailureReason)
                    {
                        case FetchFailureReason.Error:
                            this.logger.Log("Fetch failed for unknown reason");

                            break;
                        case FetchFailureReason.Throttled:
                            this.logger.Log("Fetch throttled until " + info.ThrottledEndTime);

                            break;
                        case FetchFailureReason.Invalid:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case LastFetchStatus.Pending:
                    this.logger.Log("Latest Fetch call still pending.");

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Get Data Remote Config

        public string GetRemoteConfigStringValue(string key) { return !this.HasKey(key) ? "" : Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue; }

        public bool GetRemoteConfigBoolValue(string key)
        {
            if (!this.HasKey(key) || !this.IsFirebaseReady)
            {
                return false;
            }

            var value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;

            return bool.TryParse(value, out var result) && result;
        }

        public long GetRemoteConfigLongValue(string key)
        {
            if (!this.HasKey(key) || !this.IsFirebaseReady)
            {
                return 0;
            }

            var value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;

            return long.TryParse(value, out var result) ? result : 0;
        }

        public double GetRemoteConfigDoubleValue(string key)
        {
            if (!this.HasKey(key) || !this.IsFirebaseReady)
            {
                return 0d;
            }

            var value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;

            return double.TryParse(value, out var result) ? result : 0;
        }

        public int GetRemoteConfigIntValue(string key)
        {
            if (!this.HasKey(key) || !this.IsFirebaseReady)
            {
                return 0;
            }

            var value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;

            return int.TryParse(value, out var result) ? result : 0;
        }

        public float GetRemoteConfigFloatValue(string key)
        {
            if (!this.HasKey(key) || !this.IsFirebaseReady)
            {
                return 0f;
            }

            var value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;

            return float.TryParse(value, out var result) ? result : 0f;
        }

        private bool HasKey(string key) { return Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Keys != null && Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Keys.Contains(key); }

        #endregion
    }
}
#endif