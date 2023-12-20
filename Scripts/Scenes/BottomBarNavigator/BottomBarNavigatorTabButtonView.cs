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

        public void Init()
        {
            this.SetActive(false);
        }
        
        public void SetActive(bool isActive)
        {
            var duration = 0.3f;

            if (isActive)
            {
                this.TabIcon.rectTransform.DOMoveY(-30, duration).SetEase(Ease.InOutBack);
                this.TabName.transform.DOScale(1.3f, duration).SetEase(Ease.OutBounce);   
            }
            else
            {
                this.TabIcon.rectTransform.DOMoveY(-100, duration).SetEase(Ease.InBack);
                this.TabName.transform.DOScale(1f, duration).SetEase(Ease.InBack);   
            }
        }
    }
}