namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    using BlueprintFlow.Signals;

    public class ScaleDecoration2DItem : ISignal
    {
        public float WidthScale;
        public float HeightScale;

        public ScaleDecoration2DItem(float widthScale, float heightScale)
        {
            this.WidthScale  = widthScale;
            this.HeightScale = heightScale;
        }
    }
}