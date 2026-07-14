using UnityEngine;

namespace UI.Screens
{
    public class AppBootstrapper : MonoBehaviour
    {
        [SerializeField] private ScreenBase _homeScreen;

        private void Start()
        {
            LockPortrait();

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