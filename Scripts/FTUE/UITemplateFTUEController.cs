namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Signal;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services.Highlight;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class UITemplateFTUEController
    {
        private readonly HighlightController     highlightController;
        private readonly UITemplateFTUEBlueprint uiTemplateFtueBlueprint;
        private readonly SignalBus               signalBus;
        private          string                  currentActiveStepId;

        [Preserve]
        public UITemplateFTUEController(HighlightController highlightController, UITemplateFTUEBlueprint uiTemplateFtueBlueprint, SignalBus signalBus)
        {
            this.highlightController     = highlightController;
            this.uiTemplateFtueBlueprint = uiTemplateFtueBlueprint;
            this.signalBus               = signalBus;
        }

        public bool ThereIsFTUEActive() => !string.IsNullOrEmpty(this.currentActiveStepId);

        public void DoDeactiveFTUE(string stepId)
        {
            if (stepId.IsNullOrEmpty() || !stepId.Equals(this.currentActiveStepId)) return;
            this.currentActiveStepId = null;

            var record = this.uiTemplateFtueBlueprint.GetDataById(stepId);

            if (string.IsNullOrEmpty(record.HighLightPath)) return;
            this.highlightController.TurnOffHighlight();
        }

        public void DoActiveFTUE(string stepId, HashSet<GameObject> disableObjectSet)
        {
            this.currentActiveStepId = stepId;
            foreach (var disableObject in disableObjectSet)
            {
                disableObject.SetActive(true);
            }
            this.SetHighlight(stepId).Forget();
        }

        private async UniTask SetHighlight(string stepId)
        {
            var record = this.uiTemplateFtueBlueprint.GetDataById(stepId);
            await this.highlightController.SetHighlight(record.HighLightPath, record.ButtonCanClick, onButtonDown: () =>
            {
                this.signalBus.Fire(new FTUEButtonClickSignal(stepId));
            });
            this.highlightController.ConfigHand(TypeConfigHand.AllAppear, record.HandSizeDelta, record.Radius, record.HandAnchor, record.HandRotation);
        }
    }
}