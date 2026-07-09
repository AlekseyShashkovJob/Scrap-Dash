using UnityEngine;

namespace Infrastructure.Platform
{
    public static class HapticFeedback
    {
        public static void Trigger()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (PlayerPrefs.GetInt(StorageKeys.HapticsEnabled, 0) == 1)
                Handheld.Vibrate();
#endif
        }
    }
}