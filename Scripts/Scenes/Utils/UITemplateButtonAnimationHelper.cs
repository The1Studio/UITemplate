namespace UITemplate.Scripts.Scenes.Popups
{
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using UnityEngine;

    public class UITemplateButtonAnimationHelper
    {
        private const float   timeBtnAnim        = 0.5f;
        private const int     timeDelayUnitask   = 500;

        public async UniTask AnimationButton(Transform transform)
        {
            transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), timeBtnAnim).SetEase(Ease.OutBounce)
                .OnComplete(() => transform.localScale = Vector3.one);
            await UniTask.Delay(timeDelayUnitask);
        }
    }
}