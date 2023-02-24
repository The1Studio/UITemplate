using UnityEngine;

namespace ThreeDISevenZeroR.UnityGifDecoder.Model
{
    public class GifImage
    {
        public bool userInput;
        public Color32[] colors;
        public int delay;

        public int DelayMs {get {return delay * 10;}}
        public float SafeDelayMs {get {return delay > 1 ? DelayMs : 100;}}
        
        public float DelaySeconds {get {return delay / 100f;}}
        public float SafeDelaySeconds {get {return SafeDelayMs / 1000f;}}
    }
}