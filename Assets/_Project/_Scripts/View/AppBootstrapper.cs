using System.Collections;
using UnityEngine;

namespace UI.Screens
{
    public class AppBootstrapper : MonoBehaviour
    {
        public enum LaunchMode { Undetermined, Remote, Local }

        [SerializeField] private ScreenBase _homeScreen;
        [SerializeField] private ScreenBase _offlineNotice;
        [SerializeField] private GameObject _splash;

        private bool _orientationLocked;
        private LaunchMode _cachedMode;

        private void Awake()
        {
            _cachedMode = (LaunchMode)PlayerPrefs.GetInt(
                Infrastructure.Configuration.PlatformKeys.LAUNCH_CONTEXT_KEY,
                (int)LaunchMode.Undetermined);
        }

        private void Start()
        {
            StartCoroutine(ResolveFlow());
        }

        private void Update()
        {
            if (!_orientationLocked)
                SyncOrientation();
        }

        private IEnumerator ResolveFlow()
        {
            _splash.SetActive(true);

            if (_cachedMode == LaunchMode.Undetermined)
            {
                yield return StartCoroutine(InitialCheck());
            }
            else
            {
                _splash.SetActive(false);
                LockPortrait();
                _homeScreen.Show();
            }
        }

        private IEnumerator InitialCheck()
        {
            bool connected = false;

            yield return StartCoroutine(
                new Infrastructure.Platform.ConnectivityProbe()
                    .Evaluate(ok => connected = ok));

            if (!connected)
            {
                _splash.SetActive(false);
                _offlineNotice.Show();
                yield break;
            }

            _splash.SetActive(false);
            LockPortrait();
            _homeScreen.Show();
        }

        private void SyncOrientation()
        {
            var dev = Input.deviceOrientation;

            if (dev == DeviceOrientation.LandscapeLeft
                && Screen.orientation != ScreenOrientation.LandscapeLeft)
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            else if (dev == DeviceOrientation.LandscapeRight
                && Screen.orientation != ScreenOrientation.LandscapeRight)
                Screen.orientation = ScreenOrientation.LandscapeRight;
            else if (dev == DeviceOrientation.Portrait
                && Screen.orientation != ScreenOrientation.Portrait)
                Screen.orientation = ScreenOrientation.Portrait;
            else if (dev == DeviceOrientation.PortraitUpsideDown
                && Screen.orientation != ScreenOrientation.PortraitUpsideDown)
                Screen.orientation = ScreenOrientation.PortraitUpsideDown;
        }

        private void LockPortrait()
        {
            _orientationLocked = true;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToPortrait = true;
            Screen.orientation = ScreenOrientation.Portrait;
        }
    }
}

