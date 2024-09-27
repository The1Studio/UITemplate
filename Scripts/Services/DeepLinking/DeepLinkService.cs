namespace TheOneStudio.UITemplate.UITemplate.Services.DeepLinking
{
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class DeepLinkService : IInitializable
    {
        #region Inject

        private readonly SignalBus signalBus;

        [Preserve]
        public DeepLinkService(SignalBus signalBus) { this.signalBus = signalBus; }

        #endregion

        public void Initialize()
        {
            // if application installed and opened from deep link
            Application.deepLinkActivated += this.OnDeepLinkActivated;

            // if application not installs and opened from deep link
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                this.OnDeepLinkActivated(Application.absoluteURL);
            }
        }

        private void OnDeepLinkActivated(string url)
        {
            this.signalBus.Fire(new OnDeepLinkActiveSignal(url));
            Debug.Log($"onelog OnDeepLinkActivated: {url}");
        }
    }
}