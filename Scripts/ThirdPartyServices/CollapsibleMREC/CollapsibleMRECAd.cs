namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.CollapsibleMREC
{
    using System.Threading;
    using Core.AdsServices;
    using Cysharp.Threading.Tasks;
    using GameFoundation.DI;
    using ServiceImplementation.Configs.Ads;
    using UnityEngine;
    using UnityEngine.UI;

    public class CollapsibleMRECAd : MonoBehaviour
    {
        [SerializeField] private RectTransform bgTransform;
        [SerializeField] private Button        btnClose;

        private const string MREC_PLACEMENT = "collapsible_mrec";

        private CancellationTokenSource refreshMrecCts;
        private CancellationTokenSource displayMrecCts;

        #region Inject

        [Inject] private UITemplateAdServiceWrapper adServiceWrapper;
        [Inject] private AdServicesConfig           adServicesConfig;

        private void Awake()
        {
            this.bgTransform.sizeDelta = new Vector2(0, 250f * (Screen.dpi / 160f) + 200f); // Calculate size
            this.bgTransform.gameObject.SetActive(false);
            this.btnClose.onClick.AddListener(this.OnClickClose);
        }

        #endregion

        #region Public Method

        public void Show()
        {
            if (!this.adServicesConfig.EnableCollapsibleMrec) return;
            Debug.Log("oneLog: SHOW collapsible mrec");
            this.bgTransform.gameObject.SetActive(true);
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
            this.adServiceWrapper.ShowBannerAd();
            this.adServiceWrapper.HideMREC(MREC_PLACEMENT, AdScreenPosition.BottomCenter);
            this.bgTransform.gameObject.SetActive(false);
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