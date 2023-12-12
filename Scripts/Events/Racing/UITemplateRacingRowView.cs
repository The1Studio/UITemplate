namespace TheOneStudio.UITemplate.UITemplate.Events.Racing
{
    using TheOneStudio.HyperCasual.GamePlay.Models;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateRacingRowView : MonoBehaviour
    {
        private UITemplateEventRacingDataController uiTemplateEventRacingDataController;
        
        public TMP_Text nameText;
        public TMP_Text scoreText;
        public Slider   progressSlider;
        public Image    yourIcon;
        public Image    flagImage;

        [Inject]
        public void Constructor(UITemplateEventRacingDataController uiTemplateEventRacingDataController)
        {
            this.uiTemplateEventRacingDataController = uiTemplateEventRacingDataController;
        }

        public virtual void InitView(UITemplateRacingPlayerData playerData)
        {
            this.nameText.text = playerData.Name;
            this.scoreText.text   = playerData.Score.ToString();
            this.flagImage.sprite = this.uiTemplateEventRacingDataController.GetCountryFlagSprite(playerData.CountryCode);
        }
    }
}