namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using GameFoundation.DI;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.CollapsibleMREC;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class CollapsibleMrecService : IInitializable
    {
        private readonly Transform                  parent;
        private readonly IGameAssets                gameAssets;
        private readonly SignalBus                  signalBus;
        private readonly UITemplateAdServiceWrapper adServiceWrapper;

        private CollapsibleMrecView View;
        private string              placement;

        [Preserve]
        public CollapsibleMrecService(
            Transform                  parent,
            IGameAssets                gameAssets,
            SignalBus                  signalBus,
            UITemplateAdServiceWrapper adServiceWrapper
        )
        {
            this.parent           = parent;
            this.gameAssets       = gameAssets;
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
            this.View.BgTransform.gameObject.SetActive(signal.IsActive);
        }

        private void OnClickClose()
        {
            this.View.BgTransform.gameObject.SetActive(false);
            #if THEONE_COLLAPSIBLE_MREC
            this.adServiceWrapper.InternalCloseCollapsibleMREC(this.placement);
            #endif
        }
    }
}