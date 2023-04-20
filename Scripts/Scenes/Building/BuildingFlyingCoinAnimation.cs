namespace TheOneStudio.HyperCasual.DrawCarBase.Scripts.Runtime.Scenes.Building
{
    using System.Collections.Generic;
    using DG.Tweening;
    using UnityEngine;

    public class BuildingFlyingCoinAnimation : MonoBehaviour
    {
        public SpriteRenderer spCoin;

        private Vector3 originScale = Vector3.one * 3;

        private Vector3     originPosition;
        private List<Tween> listRunningTween = new();
        private float       timeAnim         = 5f;
        private void        Awake() { this.originPosition = this.spCoin.transform.position; }

        private void OnEnable()
        {
            if (this.listRunningTween.Count > 0)
            {
                foreach (var t in this.listRunningTween)
                {
                    DOTween.Kill(t.target);
                }

                this.listRunningTween.Clear();
            }

            this.spCoin.transform.position   = this.originPosition;
            this.spCoin.transform.localScale = this.originScale;
            var finalYpos = this.spCoin.transform.position.y + 50;
            this.listRunningTween.Add(this.spCoin.transform.DOScale(Vector3.zero, this.timeAnim).SetEase(Ease.OutBack).SetLoops(-1, LoopType.Restart));
            this.listRunningTween.Add(this.spCoin.transform.DOMoveY(finalYpos, this.timeAnim).SetEase(Ease.OutBack).SetLoops(-1, LoopType.Restart));
        }
    }
}