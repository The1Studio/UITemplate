namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using UnityEngine.UI;

    public class UITemplateAdsButton : Button
    {
        private UITemplateAdServiceWrapper adServices;
        private CancellationTokenSource    cts;

        public void OnViewReady(UITemplateAdServiceWrapper adService) { this.adServices = adService; }

        public void BindData(string place)
        {
            this.cts          = new CancellationTokenSource();
            this.interactable = false;
            UniTask.WaitUntil(() => this.adServices.IsRewardedAdReady(place), cancellationToken: this.cts.Token).ContinueWith(() => this.interactable = true);
        }

        public void Dispose()
        {
            this.cts?.Dispose();
        }
    }
}