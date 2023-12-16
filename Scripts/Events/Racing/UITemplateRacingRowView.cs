namespace TheOneStudio.UITemplate.UITemplate.Events.Racing
{
    using TMPro;
    using Zenject;
    using UnityEngine;
    using UnityEngine.UI;
    using TheOneStudio.HyperCasual.GamePlay.Models;
    using TheOneStudio.UITemplate.UITemplate.Signals;

    public class UITemplateRacingRowView : MonoBehaviour
    {
        private UITemplateEventRacingDataController uiTemplateEventRacingDataController;
        private SignalBus                           signalBus;

        public TMP_Text nameText;
        public TMP_Text scoreText;
        public Slider   progressSlider;
        public Image    yourIcon;
        public Image    flagImage;
        public Button   buttonChest;

        [Inject]
        public void Constructor(UITemplateEventRacingDataController uiTemplateEventRacingDataController, SignalBus newSignalBus)
        {
            this.uiTemplateEventRacingDataController = uiTemplateEventRacingDataController;
            this.signalBus                           = newSignalBus;
        }

        public virtual void InitView(UITemplateRacingPlayerData playerData)
        {
            this.nameText.text       = playerData.Name;
            this.scoreText.text      = playerData.Score.ToString();
            this.flagImage.sprite    = this.uiTemplateEventRacingDataController.GetCountryFlagSprite(playerData.CountryCode);
            this.buttonChest.enabled = playerData.IsPlayer && this.uiTemplateEventRacingDataController.RacingEventComplete();
            this.buttonChest.onClick.AddListener(this.OnOpenChest);
        }

        private void OnOpenChest() { this.signalBus.Fire<RacingEventCompleteSignal>(); }
    }
}