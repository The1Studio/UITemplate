namespace TheOneStudio.UITemplate.UITemplate.Scenes.ChestRoom
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateChestItemView : MonoBehaviour
    {
        public Button   ChestButton;
        public Image    RewardImage;
        public TMP_Text RewardValue;

        public void Init() { this.SetChestActive(true); }

        public void OpenChest(Sprite rewardSprite, int value)
        {
            this.SetChestActive(false);
            this.RewardImage.sprite = rewardSprite;
            this.RewardValue.text   = value.ToString();
        }

        private void SetChestActive(bool isActive)
        {
            this.ChestButton.gameObject.SetActive(isActive);
            this.RewardImage.gameObject.SetActive(!isActive);
            this.RewardValue.gameObject.SetActive(!isActive);
        }
    }
}