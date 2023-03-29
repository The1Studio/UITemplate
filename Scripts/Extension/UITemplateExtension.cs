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

        public static T GetRandomElement<T>(this IList<T> elements, IList<float> weights)
        {
            //Validate the weights
            if (elements == null || weights == null || elements.Count != weights.Count || elements.Count == 0)
            {
                throw new ArgumentException("The elements and weights must be non-null and of equal length.");
            }

            // Normalize the weights
            var totalWeight       = weights.Sum();
            var normalizedWeights = weights.Select(w => w / totalWeight).ToList();

            // Generate a random number between 0 and 1
            var randomValue = Random.value;

            // Select element based on the weights
            for (var i = 0; i < elements.Count; i++)
            {
                if (randomValue < normalizedWeights[i])
                {
                    return elements[i];
                }

                randomValue -= normalizedWeights[i];
            }

            return elements[^1];
        }
    }
}