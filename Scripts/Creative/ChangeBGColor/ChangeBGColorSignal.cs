namespace TheOneStudio.UITemplate.UITemplate.Creative.ChangeBGColor
{
    public class ChangeBGColorSignal
    {
        public ChangeBGColorSignal(string hexCode, float r, float g, float b, float a)
        {
            this.hexCode = hexCode;
            this.R       = r;
            this.G       = g;
            this.B       = b;
            this.A       = a;
        }

        public string hexCode;
        public float    R;
        public float    G;
        public float    B;
        public float    A;
    }
}