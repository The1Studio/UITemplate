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
            if (value <= 1)
            {
                this.RewardImage.rectTransform.anchoredPosition = new Vector2(0, 0);
                this.RewardImage.rectTransform.sizeDelta = new Vector2(220, 220);
                this.RewardValue.gameObject.SetActive(false);
            }
            else
            {
                this.RewardImage.rectTransform.anchoredPosition = new Vector2(0, 15);
                this.RewardImage.rectTransform.sizeDelta = new Vector2(150, 150);
                this.RewardValue.gameObject.SetActive(true);
            }
        }

        private void SetChestActive(bool isActive)
        {
            this.ChestButton.gameObject.SetActive(isActive);
            this.RewardImage.gameObject.SetActive(!isActive);
            this.RewardValue.gameObject.SetActive(!isActive);
        }
    }
}