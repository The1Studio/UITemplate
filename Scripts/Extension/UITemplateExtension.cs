namespace TheOneStudio.UITemplate.UITemplate.Extension
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        public static T RandomGachaWithWeight<T>(this IList<T> elements, IList<float> weights)
        {
            // Validate input
            if (elements == null || weights == null || elements.Count != weights.Count || elements.Count == 0)
            {
                throw new ArgumentException("Invalid input");
            }

            // Normalize weights
            var sum               = weights.Sum();
            var normalizedWeights = weights.Select(w => w / sum).ToList();

            // Generate random number between 0 and 1
            var rnd          = new System.Random();
            var randomNumber = rnd.NextDouble();

            // Select element based on weights
            for (var i = 0; i < elements.Count; i++)
            {
                if (randomNumber < normalizedWeights[i])
                {
                    return elements[i];
                }

                randomNumber -= normalizedWeights[i];
            }

            return elements[^1];
        }

        public static bool IsNullOrEmpty(this string str) { return string.IsNullOrEmpty(str); }
    }
}