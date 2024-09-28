namespace TheOneStudio.UITemplate.UITemplate.Events.Racing
{
    using GameFoundation.DI;
    using TheOneStudio.HyperCasual.GamePlay.Models;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class UITemplateRacingRowView : MonoBehaviour
    {
        private UITemplateEventRacingDataController uiTemplateEventRacingDataController;

        public TMP_Text nameText;
        public TMP_Text scoreText;
        public Slider   progressSlider;
        public Image    yourIcon;
        public Image    flagImage;
        public Button   buttonChest;

        [Header("Animation")] public Animator animatorButtonChest;

        protected bool IsPlayer;

        protected virtual void Awake()
        {
            var container = this.GetCurrentContainer();
            this.uiTemplateEventRacingDataController = container.Resolve<UITemplateEventRacingDataController>();
        }

        public virtual void InitView(UITemplateRacingPlayerData playerData, int indexPlayer, UnityAction onOpenChest = null)
        {
            this.IsPlayer            = this.uiTemplateEventRacingDataController.IsPlayer(indexPlayer);
            this.nameText.text       = playerData.Name;
            this.scoreText.text      = playerData.Score.ToString();
            this.flagImage.sprite    = this.uiTemplateEventRacingDataController.GetCountryFlagSprite(playerData.CountryCode);
            this.buttonChest.enabled = this.IsPlayer;
            this.buttonChest.onClick.AddListener(onOpenChest);
        }

        public virtual void CheckStatus()
        {
            var isWin = this.uiTemplateEventRacingDataController.RacingEventComplete();
            this.animatorButtonChest.enabled = isWin && this.IsPlayer;
        }
    }
}