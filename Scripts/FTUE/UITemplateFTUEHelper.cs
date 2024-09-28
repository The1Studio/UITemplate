namespace TheOneStudio.UITemplate.UITemplate.FTUE
{
    using System;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateFTUEHelper
    {
        private class FTUECondition
        {
            public const string Equal     = "Equal";
            public const string NotEqual  = "NotEqual";
            public const string Higher    = "Higher";
            public const string Lower     = "Lower";
            public const string HighEqual = "HighEqual";
            public const string LowEqual  = "LowEqual";
        }

        public bool CompareIntWithCondition(int leftValue, int rightValue, string condition)
        {
            return condition switch
            {
                FTUECondition.Equal => leftValue == rightValue,
                FTUECondition.NotEqual => leftValue != rightValue,
                FTUECondition.Higher => leftValue > rightValue,
                FTUECondition.Lower => leftValue < rightValue,
                FTUECondition.HighEqual => leftValue >= rightValue,
                FTUECondition.LowEqual => leftValue <= rightValue,
                _ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
            };
        }

        public bool CompareFloatWithCondition(float leftValue, float rightValue, string condition)
        {
            return condition switch
            {
                FTUECondition.Equal => Math.Abs(leftValue - rightValue) < float.Epsilon,
                FTUECondition.NotEqual => Math.Abs(leftValue - rightValue) > float.Epsilon,
                FTUECondition.Higher => leftValue > rightValue,
                FTUECondition.Lower => leftValue < rightValue,
                FTUECondition.HighEqual => leftValue >= rightValue,
                FTUECondition.LowEqual => leftValue <= rightValue,
                _ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
            };
        }
    }
}