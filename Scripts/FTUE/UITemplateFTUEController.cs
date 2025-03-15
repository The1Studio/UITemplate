namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.FTUE.RemoteConfig;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Signal;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services.Highlight;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class UITemplateFTUEController
    {
        private readonly HighlightController                highlightController;
        private readonly UITemplateFTUEBlueprintDataHandler uiTemplateFtueBlueprint;
        private readonly SignalBus                          signalBus;
        private          string                             currentActiveStepId;

        [Preserve]
        public UITemplateFTUEController(
            HighlightController                highlightController,
            UITemplateFTUEBlueprintDataHandler uiTemplateFtueBlueprint,
            SignalBus                          signalBus
        )
        {
            this.highlightController     = highlightController;
            this.uiTemplateFtueBlueprint = uiTemplateFtueBlueprint;
            this.signalBus               = signalBus;
        }

        private GameObject highlightedObject;

        public bool ThereIsFTUEActive()
        {
            return !string.IsNullOrEmpty(this.currentActiveStepId);
        }

        public void DoDeactiveFTUE(string stepId)
        {
            if (stepId.IsNullOrEmpty() || !stepId.Equals(this.currentActiveStepId)) return;
            this.currentActiveStepId = null;

            var record = this.uiTemplateFtueBlueprint.GetDataById(stepId);

            if (string.IsNullOrEmpty(record.HighLightPath)) return;
            if (this.uiTemplateFtueBlueprint[stepId].HideOnComplete)
            {
                this.highlightedObject.SetActive(false);
            }
            this.highlightController.TurnOffHighlight();
        }

        public void DoActiveFTUE(string stepId)
        {
            this.currentActiveStepId = stepId;
            this.SetHighlight(stepId).Forget();
        }

        private async UniTask SetHighlight(string stepId)
        {
            var record = this.uiTemplateFtueBlueprint.GetDataById(stepId);
            await this.highlightController.SetHighlight(record.HighLightPath,
                record.ButtonCanClick,
                () =>
                {
                    this.signalBus.Fire(new FTUEButtonClickSignal(stepId));
                });
            this.highlightController.ConfigHand(TypeConfigHand.AllAppear, record.HandSizeDelta, record.Radius, record.HandAnchor, record.HandRotation);
            this.highlightedObject = this.highlightController.GetHighlightedObject();
            if (this.highlightedObject != null)
            {
                this.signalBus.Fire(new FTUEShowTooltipSignal(this.uiTemplateFtueBlueprint.GetDataById(stepId).TooltipText, this.highlightedObject, record.TooltipDuration));
            }
        }
    }
}