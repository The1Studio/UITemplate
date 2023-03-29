namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using System.Threading;
    using Core.AdsServices;
    using Cysharp.Threading.Tasks;
    using UnityEngine.UI;

    public class UITemplateAdsButton : Button
    {
        private IAdServices             adServices;
        private CancellationTokenSource cts;

        public void OnViewReady(IAdServices adService)
        {
            this.adServices = adService;
        }

        public void BindData()
        {
            this.cts = new CancellationTokenSource();
            UniTask.WaitUntil(() => this.adServices.IsRewardedAdReady(""), cancellationToken: this.cts.Token)
                   .ContinueWith(() => this.interactable = true);
        }

        public void Dispose()
        {
            this.interactable = false;
            this.cts.Cancel();
        }
    }
}