namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using UnityEngine;

    public class UITemplateStarRateView : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> StarOnList;

        [SerializeField]
        private List<GameObject> StarOffList;

        [SerializeField]
        private float timeAnimStar = 1.5f;

        [SerializeField]
        private float timeDelay = 0.3f;

        public async UniTask SetStarRate(int rate)
        {
            foreach (var starOn in this.StarOnList)
            {
                starOn.SetActive(false);
                starOn.transform.localScale = Vector3.zero;
            }

            foreach (var starOff in this.StarOffList) starOff.SetActive(true);

            for (var i = 0; i < this.StarOnList.Count; i++)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(this.timeDelay));
                var isActive = i < rate;
                this.StarOnList[i].SetActive(isActive);
                this.StarOnList[i].transform.DORotate(new Vector3(0, 0, -360), this.timeAnimStar, RotateMode.WorldAxisAdd).SetEase(Ease.OutBack);
                this.StarOnList[i].transform.DOScale(Vector3.one, this.timeAnimStar).SetEase(Ease.OutBack);

                this.StarOffList[i].SetActive(!isActive);
            }
        }
    }
}