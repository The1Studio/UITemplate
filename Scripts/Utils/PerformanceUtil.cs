﻿namespace TheOneStudio.UITemplate.UITemplate.Utils
{
    using System.Linq;
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
            #if UNITY_ANDROID
            return CheckDevicePerformanceAndroid();
            #else
            return CheckDevicePerformanceIOS();
            #endif
        }

        #region Android

        private static QualityLevel CheckDevicePerformanceAndroid()
        {
            var ramMb     = SystemInfo.systemMemorySize;
            var cpuCores  = SystemInfo.processorCount;
            var gpuModel  = SystemInfo.graphicsDeviceName.ToLower();
            var gpuMemory = SystemInfo.graphicsMemorySize; // in MB

            var isWeakGpu = WeakAndroidGpus.Any(weak => gpuModel.Contains(weak));

            if (ramMb >= 8192 && cpuCores >= 8 && gpuMemory >= 2000 && !isWeakGpu) return QualityLevel.High;
            if (ramMb >= 4096 && cpuCores >= 4 && gpuMemory >= 1000 && !isWeakGpu) return QualityLevel.Medium;

            return QualityLevel.Low;
        }

        private static readonly string[] WeakAndroidGpus = new string[]
        {
            // Adreno
            "adreno 3", "adreno 4", "adreno 505", "adreno 506", "adreno 508", "adreno 509", "adreno 510", "adreno 512", "adreno 516", "adreno 610", "adreno 612", "adreno 615", "adreno 618", "adreno 620",
            // Mali
            "mali-400", "mali-450", "mali-t720", "mali-t760", "mali-t820", "mali-t830", "mali-t860", "mali-t880", "mali-g31", "mali-g51", "mali-g52", "mali-g57 mc1", "mali-g57 mc2",
            // PowerVR
            "powervr ge8100", "powervr ge8300", "powervr sgx544", "powervr sgx543", "powervr gx6250", "powervr gx6450", "powervr ge8320", "powervr ge8322",
            // Tegra
            "tegra 3", "tegra 4", "tegra k1", "tegra x1",
            // Vivante, ARM khác
            "vivante", "mali-470",
        };

        #endregion

        #region iOS

        private static QualityLevel CheckDevicePerformanceIOS()
        {
            if (IsiOSLowEnd())
                return QualityLevel.Low;
            else if (IsiOSMediumEnd())
            {
                return QualityLevel.Medium;
            }
            return QualityLevel.High;
        }

        private static bool IsiOSLowEnd()
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

        private static bool IsiOSMediumEnd()
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

        #endregion
    }
}