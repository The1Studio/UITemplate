namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System.Threading;
    using Core.AdsServices;
    using Cysharp.Threading.Tasks;
    using GameFoundation.DI;
    using GameFoundation.Scripts.AssetLibrary;
    using ServiceImplementation.Configs.Ads;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.CollapsibleMREC;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class CollapsibleMrecService : IInitializable
    {
        private readonly Transform                  parent;
        private readonly IGameAssets                gameAssets;
        private readonly UITemplateAdServiceWrapper adServiceWrapper;
        private readonly AdServicesConfig           adServicesConfig;

        private       CollapsibleMrecView     View;
        private const string                  MREC_PLACEMENT = "collapsible_mrec";
        private       CancellationTokenSource refreshMrecCts;
        private       CancellationTokenSource displayMrecCts;

        public bool IsShowCollapsibleMrec { get; private set; }

        [Preserve]
        public CollapsibleMrecService(
            Transform                  parent,
            IGameAssets                gameAssets,
            UITemplateAdServiceWrapper adServiceWrapper,
            AdServicesConfig           adServicesConfig
        )
        {
            this.parent           = parent;
            this.gameAssets       = gameAssets;
            this.adServiceWrapper = adServiceWrapper;
            this.adServicesConfig = adServicesConfig;
        }

        public async void Initialize()
        {
            var collapsibleMrecObj = await this.gameAssets.InstantiateAsync(nameof(CollapsibleMrecView), Vector3.zero, Quaternion.identity, this.parent);
            this.View = collapsibleMrecObj.GetComponent<CollapsibleMrecView>();
            this.View.BtnClose.onClick.AddListener(this.OnClickClose);
        }

        #region Public Method

        public void Show()
        {
            if (this.View == null) return;
            if (!this.adServicesConfig.EnableCollapsibleMrec) return;
            this.IsShowCollapsibleMrec = true;
            Debug.Log("oneLog: SHOW collapsible mrec");
            this.View.BgTransform.gameObject.SetActive(true);
            this.adServiceWrapper.HideBannerAd();
            this.adServiceWrapper.ShowMREC(MREC_PLACEMENT, AdScreenPosition.BottomCenter);
            UniTask.WaitForSeconds(this.adServicesConfig.CollapsibleMrecDisplayTime, true, cancellationToken: (this.displayMrecCts = new()).Token)
                .ContinueWith(
                    () =>
                    {
                        this.HideMREC();
                        this.InternalRefreshMrec();
                    }).Forget();
        }

        public void Hide()
        {
            if (!this.IsShowCollapsibleMrec) return;
            if (!this.adServicesConfig.EnableCollapsibleMrec) return;
            this.ResetMrecDisplayCts();
            this.ResetMrecRefreshCts();
            this.HideMREC();
        }

        #endregion

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
            this.IsShowCollapsibleMrec = false;
            this.adServiceWrapper.ShowBannerAd();
            this.adServiceWrapper.HideMREC(MREC_PLACEMENT, AdScreenPosition.BottomCenter);
            this.View.BgTransform.gameObject.SetActive(false);
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