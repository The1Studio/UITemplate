namespace TheOneStudio.UITemplate.UITemplate.Utils
{
    using SystemInfo = UnityEngine.Device.SystemInfo;

    public enum QualityLevel
    {
        Low,
        Medium,
        High,
    }

    public class PerformanceUtil
    {
        public static QualityLevel CheckDevicePerformance()
        {
            #if UNITY_IOS
            if (IsiOSLowEnd())
                return Quality.Low;
            else if (IsiOSMediumEnd())
            {
                return Quality.Medium;
            }
            return Quality.High;
            #endif

            var ramMb    = SystemInfo.systemMemorySize;
            var cpuCores = SystemInfo.processorCount;

            if (ramMb >= 8192 || cpuCores >= 8) return QualityLevel.High;
            if (ramMb >= 4096 || cpuCores >= 6) return QualityLevel.Medium;
            return QualityLevel.Low;
        }

        public static bool IsiOSLowEnd()
        {
            var model = SystemInfo.deviceModel.ToLower();

            // iPhone 6/6S/7/8/X/SE1 và các bản Plus
            if (model.StartsWith("iphone5")     // iPhone 5/5C
                || model.StartsWith("iphone6")  // iPhone 5S
                || model.StartsWith("iphone7")  // iPhone 6/6+
                || model.StartsWith("iphone8")  // iPhone 6S, SE1
                || model.StartsWith("iphone9")  // iPhone 7/7+
                || model.StartsWith("iphone10") // iPhone 8/8+/X
            )
                return true;

            // iPad 2/3/4/5/6/7, Mini 1-4, Air 1-2
            if (model.StartsWith("ipad2")
                || model.StartsWith("ipad3")
                || model.StartsWith("ipad4")
                || model.StartsWith("ipad5")
                || model.StartsWith("ipad6")
                || model.StartsWith("ipad7")
            )
                return true;

            return false;
        }

        public static bool IsiOSMediumEnd()
        {
            var model = SystemInfo.deviceModel.ToLower();

            if (model.StartsWith("iphone11")
                || model.StartsWith("iphone12")
            )
                return true;

            if (model.StartsWith("ipad8")
                || model.StartsWith("ipad9")
            )
                return true;

            return false;
        }
    }
}