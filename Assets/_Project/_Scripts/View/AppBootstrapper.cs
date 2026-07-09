using UnityEngine;

namespace UI.Screens
{
    public class AppBootstrapper : MonoBehaviour
    {
        [SerializeField] private ScreenBase _homeScreen;
        [SerializeField] private GameObject _splash;

        private void Start()
        {
            LockPortrait();

            if (_splash != null)
                _splash.SetActive(false);

            _homeScreen.Show();
        }

        private void LockPortrait()
        {
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToPortrait = true;
            Screen.orientation = ScreenOrientation.Portrait;
        }
    }
}