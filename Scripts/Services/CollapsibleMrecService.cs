namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System.Threading;
    using Core.AdsServices;
    using Cysharp.Threading.Tasks;
    using GameFoundation.DI;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Signals;
    using ServiceImplementation.Configs.Ads;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.CollapsibleMREC;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class CollapsibleMrecService : IInitializable
    {
        private readonly Transform                  parent;
        private readonly IGameAssets                gameAssets;
        private readonly AdServicesConfig           adServicesConfig;
        private readonly SignalBus                  signalBus;
        private readonly UITemplateAdServiceWrapper adServiceWrapper;

        private CollapsibleMrecView     View;
        private CancellationTokenSource refreshMrecCts;
        private CancellationTokenSource displayMrecCts;
        private string                  placement;

        [Preserve]
        public CollapsibleMrecService(
            Transform                  parent,
            IGameAssets                gameAssets,
            AdServicesConfig           adServicesConfig,
            SignalBus                  signalBus,
            UITemplateAdServiceWrapper adServiceWrapper
        )
        {
            this.parent           = parent;
            this.gameAssets       = gameAssets;
            this.adServicesConfig = adServicesConfig;
            this.signalBus        = signalBus;
            this.adServiceWrapper = adServiceWrapper;
        }

        public async void Initialize()
        {
            var collapsibleMrecObj = await this.gameAssets.InstantiateAsync(nameof(CollapsibleMrecView), Vector3.zero, Quaternion.identity, this.parent);
            this.View = collapsibleMrecObj.GetComponent<CollapsibleMrecView>();
            this.View.BtnClose.onClick.AddListener(this.OnClickClose);
            this.View.BgTransform.gameObject.SetActive(false);
            this.signalBus.Subscribe<UITemplateOnUpdateCollapMrecStateSignal>(this.UpdateView);
        }

        private void UpdateView(UITemplateOnUpdateCollapMrecStateSignal signal)
        {
            this.placement = signal.Placement;
            if (signal.IsActive)
            {
                this.Show();
            }
            else
            {
                this.Hide();
            }
        }

        private void Show()
        {
            Debug.Log("oneLog: SHOW collapsible mrec");
            this.View.BgTransform.gameObject.SetActive(true);
            UniTask.WaitForSeconds(this.adServicesConfig.CollapsibleMrecDisplayTime, true, cancellationToken: (this.displayMrecCts = new()).Token)
                .ContinueWith(
                    () =>
                    {
                        this.HideMREC();
                        this.InternalRefreshMrec();
                    }).Forget();
        }

        private void Hide()
        {
            if (!this.adServicesConfig.EnableCollapsibleMrec) return;
            this.ResetMrecDisplayCts();
            this.ResetMrecRefreshCts();
            this.HideMREC();
        }

        private void OnClickClose()
        {
            this.ResetMrecDisplayCts();
            this.HideMREC();
            this.InternalRefreshMrec();
        }

        private void InternalRefreshMrec()
        {
            UniTask.WaitForSeconds(this.adServicesConfig.CollapsibleMrecInterval, true, cancellationToken: (this.refreshMrecCts = new()).Token)
                .ContinueWith(this.Show).Forget();
        }

        private void HideMREC()
        {
            Debug.Log("oneLog: HIDE collapsible mrec");
            this.View.BgTransform.gameObject.SetActive(false);
            this.adServiceWrapper.HideMREC(this.placement, AdScreenPosition.BottomCenter);
            this.adServiceWrapper.ShowBannerAd();
        }

        private void ResetMrecRefreshCts()
        {
            this.refreshMrecCts?.Cancel();
            this.refreshMrecCts?.Dispose();
            this.refreshMrecCts = null;
        }

        private void ResetMrecDisplayCts()
        {
            this.displayMrecCts?.Cancel();
            this.displayMrecCts?.Dispose();
            this.displayMrecCts = null;
        }
    }
}