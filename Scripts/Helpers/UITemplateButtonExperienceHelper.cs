namespace TheOneStudio.UITemplate.UITemplate.Helpers
{
    using System.Collections.Generic;
    using GameFoundation.DI;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using GameFoundation.Scripts.Utilities;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.Interfaces;
    using UIModule.Utilities.UIStuff;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class UITemplateButtonExperienceHelper : IInitializable
    {
        #region inject

        private readonly SignalBus           signalBus;
        private readonly IVibrationService   vibrationService;
        private readonly IAudioService       soundServices;
        private readonly GameFeaturesSetting gameFeaturesSetting;

        #endregion

        private HashSet<IScreenPresenter> openedScreenList = new();

        [Preserve]
        public UITemplateButtonExperienceHelper(SignalBus signalBus, IVibrationService vibrationService, IAudioService soundServices, GameFeaturesSetting gameFeaturesSetting)
        {
            this.signalBus           = signalBus;
            this.vibrationService    = vibrationService;
            this.soundServices       = soundServices;
            this.gameFeaturesSetting = gameFeaturesSetting;
        }

        public void Initialize() { this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShowHandler); }

        private void OnScreenShowHandler(ScreenShowSignal obj)
        {
            if (!this.openedScreenList.Add(obj.ScreenPresenter)) return;

            var newButtons = obj.ScreenPresenter.CurrentTransform.GetComponentsInChildren<Button>(true);
            foreach (var newButton in newButtons)
            {
                newButton.onClick.AddListener(() =>
                {
                    this.vibrationService.PlayPresetType(this.gameFeaturesSetting.vibrationPresetType);
                    if (!this.gameFeaturesSetting.clickButtonSound.IsNullOrEmpty())
                    {
                        this.soundServices.PlaySound(this.gameFeaturesSetting.clickButtonSound);
                    }
                });
                if (this.gameFeaturesSetting.enableScaleAnimationOnCLicked && newButton.gameObject.GetComponent<AnimationButton>() == null)
                {
                    newButton.gameObject.AddComponent<AnimationButton>();
                }
            }
        }
    }
}