namespace TheOneStudio.UITemplate.UITemplate.Extension
{
    using System;
    using System.Collections.Generic;
    using UniRx;
    using Random = UnityEngine.Random;

    public static class UITemplateExtension
    {
        public static void GachaItemWithTimer<T>(this List<T> items, IDisposable randomTimerDispose, Action<T> onComplete, Action<T> everyCycle, float currentCooldownTime = 1f,
            float currentCycle = 0.5f, int finalItemIndex = -1)
        {
            randomTimerDispose = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(currentCycle)).Subscribe(_ =>
            {
                everyCycle?.Invoke(items[Random.Range(0, items.Count)]);

                if (currentCooldownTime <= 0)
                {
                    onComplete?.Invoke(finalItemIndex != -1 ? items[finalItemIndex] : items[Random.Range(0, items.Count)]);

                    randomTimerDispose.Dispose();

                    return;
                }

                currentCooldownTime -= currentCycle;
            });
        }
    }
}