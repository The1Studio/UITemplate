namespace TheOneStudio.UITemplate.UITemplate.Events.Racing
{
    using TMPro;
    using Zenject;
    using UnityEngine;
    using UnityEngine.UI;
    using TheOneStudio.HyperCasual.GamePlay.Models;
    using Action = UnityEngine.Events.UnityAction;

    public class UITemplateRacingRowView : MonoBehaviour
    {
        private UITemplateEventRacingDataController uiTemplateEventRacingDataController;

        public TMP_Text nameText;
        public TMP_Text scoreText;
        public Slider   progressSlider;
        public Image    yourIcon;
        public Image    flagImage;
        public Button   buttonChest;

        [Inject]
        public void Constructor(UITemplateEventRacingDataController uiTemplateEventRacingDataController)
        {
            this.uiTemplateEventRacingDataController = uiTemplateEventRacingDataController;
        }

        public virtual void InitView(UITemplateRacingPlayerData playerData, Action onOpenChest = null)
        {
            this.nameText.text       = playerData.Name;
            this.scoreText.text      = playerData.Score.ToString();
            this.flagImage.sprite    = this.uiTemplateEventRacingDataController.GetCountryFlagSprite(playerData.CountryCode);
            this.buttonChest.enabled = playerData.IsPlayer;
            this.buttonChest.onClick.AddListener(onOpenChest);
        }
    }
}