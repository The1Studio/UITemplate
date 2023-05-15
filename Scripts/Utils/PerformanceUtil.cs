namespace TheOneStudio.UITemplate.UITemplate.Utils
{
    using UnityEngine;
    using SystemInfo = UnityEngine.Device.SystemInfo;

    public enum Quality
    {
        Low,
        Medium,
        High
    }

    public class PerformanceUtil
    {
        public static Quality CheckDevicePerformance()
        {
            Quality quality;

            // Check device specifications
            if (SystemInfo.processorCount <= 2 && SystemInfo.systemMemorySize <= 2048)
            {
                quality = Quality.Low; // Considered low-end if CPU cores <= 2 and memory <= 2GB
            }
            else if (SystemInfo.processorCount >= 4 && SystemInfo.systemMemorySize >= 4096)
            {
                quality = Quality.High; // Considered high-end if CPU cores >= 4 and memory >= 4GB
            }
            else
            {
                quality = Quality.Medium; // In between low-end and high-end
            }

            return quality;
        }
        
        public static Quality CheckDevicePerformanceByVram()
        {
            Quality deviceQuality;

            var vramSize = SystemInfo.graphicsMemorySize;
            Debug.Log("Device VRAM: " + vramSize + " MB");

            // Adjust the thresholds based on your desired categorization
            var lowEndThreshold  = 512;  // In megabytes
            var highEndThreshold = 2048; // In megabytes

            if (vramSize < lowEndThreshold)
            {
                deviceQuality = Quality.Low; // VRAM below the low-end threshold
            }
            else if (vramSize >= highEndThreshold)
            {
                deviceQuality = Quality.High; // VRAM equal to or above the high-end threshold
            }
            else
            {
                deviceQuality = Quality.Medium; // VRAM in between low-end and high-end
            }

            return deviceQuality;
        }
    }
}