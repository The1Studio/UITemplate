namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateAnimationHelper
    {
        private List<Tween>             tweens = new();
        private CancellationTokenSource cancellationTokenSource;

        public async void SetActiveFreeObject(GameObject watchAdButton, GameObject noThankButton, GameObject freeGameObject, bool isActive, bool force = false)
        {
            var noThankDelay  = 2;
            var scaleDuration = 0.5f;
            foreach (var tween in this.tweens)
            {
                tween.Kill();
            }

            this.tweens.Clear();
            this.cancellationTokenSource?.Cancel();
            this.cancellationTokenSource?.Dispose();
            this.cancellationTokenSource = new();

            try
            {
                if (force)
                {
                    watchAdButton.transform.localScale  = !isActive ? Vector3.one : Vector3.zero;
                    freeGameObject.transform.localScale = isActive ? Vector3.one : Vector3.zero;
                    if (!isActive)
                    {
                        noThankButton.transform.localScale = Vector3.zero;
                        await UniTask.Delay(TimeSpan.FromSeconds(noThankDelay), cancellationToken: this.cancellationTokenSource.Token);
                        this.tweens.Add(noThankButton.transform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutBack));
                    }
                    else
                    {
                        noThankButton.transform.localScale = Vector3.zero;
                    }
                }
                else
                {
                    if (isActive)
                    {
                        this.tweens.Add(watchAdButton.transform.DOScale(Vector3.zero, scaleDuration).SetEase(Ease.InBack));
                        this.tweens.Add(noThankButton.transform.DOScale(Vector3.zero, scaleDuration).SetEase(Ease.InBack));
                        await UniTask.Delay(TimeSpan.FromSeconds(scaleDuration), cancellationToken: this.cancellationTokenSource.Token);
                        this.tweens.Add(freeGameObject.transform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutBack));
                    }
                    else
                    {
                        this.tweens.Add(freeGameObject.transform.DOScale(Vector3.zero, scaleDuration).SetEase(Ease.InBack));
                        await UniTask.Delay(TimeSpan.FromSeconds(scaleDuration), cancellationToken: this.cancellationTokenSource.Token);
                        this.tweens.Add(watchAdButton.transform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutBack));
                        await UniTask.Delay(TimeSpan.FromSeconds(noThankDelay), cancellationToken: this.cancellationTokenSource.Token);
                        this.tweens.Add(noThankButton.transform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutBack));
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                // ignore
            }
        }
    }
}