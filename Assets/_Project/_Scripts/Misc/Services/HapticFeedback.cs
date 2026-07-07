using UnityEngine;

namespace Infrastructure.Platform
{
    public static class HapticFeedback
    {
        public static bool Enabled => PlayerPrefs.GetInt(StorageKeys.HapticsEnabled, 0) == 1;

        public static void Trigger()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (Enabled)
                Handheld.Vibrate();
#endif
        }
    }
}