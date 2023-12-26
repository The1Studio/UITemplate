namespace TheOneStudio.UITemplate.UITemplate.Scenes.BottomBarNavigator
{
    using DG.Tweening;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class BottomBarNavigatorTabButtonView : MonoBehaviour
    {
        public Button   Button;
        public TMP_Text TabName;
        public Image    TabIcon;

        private bool isActive = false;
        
        public void Init()
        {
            this.SetActive(false);
            this.TabName.transform.localScale = Vector3.zero;
        }
        
        public void SetActive(bool isActive)
        {
            if (isActive == this.isActive) return;

            this.isActive = isActive;
            var duration = 0.3f;

            if (isActive)
            {
                this.TabIcon.rectTransform.DOAnchorPosY(-30, duration).SetEase(Ease.OutBounce);
                this.TabIcon.transform.DOScale(1.3f, duration).SetEase(Ease.OutBounce);
                this.TabName.transform.DOScale(1, duration).SetEase(Ease.OutBack);
            }
            else
            {
                this.TabIcon.rectTransform.DOAnchorPosY(-100, duration).SetEase(Ease.InBack);
                this.TabIcon.transform.DOScale(1f, duration).SetEase(Ease.InBack);   
                this.TabName.transform.DOScale(0, duration).SetEase(Ease.InBack);
            }
        }
    }
}