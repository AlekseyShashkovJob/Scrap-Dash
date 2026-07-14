using UnityEngine;
using UI.Components.Buttons;
using Infrastructure.Navigation;
using ScrapDash.GameCore.Constants;

namespace UI.Screens.MainMenu
{
    public class HomeScreen : ScreenBase
    {
        [SerializeField] private ScreenBase _preferencesScreen;
        [SerializeField] private ScreenBase _policyScreen;
        [SerializeField] private ScreenBase _stageSelectionScreen;
        [SerializeField] private ScreenBase _rankingScreen;

        [SerializeField] private TransitionController _navigator;

        [Space, Header("Buttons")]
        [SerializeField] private TouchableElement _playBtn;
        [SerializeField] private TouchableElement _prefsBtn;
        [SerializeField] private TouchableElement _policyBtn;
        [SerializeField] private TouchableElement _policyAltBtn;
        [SerializeField] private TouchableElement _stagesBtn;
        [SerializeField] private TouchableElement _rankingBtn;

        private void OnEnable()
        {
            _playBtn.Subscribe(LaunchSession);
            _prefsBtn.Subscribe(OpenPreferences);
            _policyBtn.Subscribe(OpenPolicy);
            _policyAltBtn.Subscribe(OpenPolicy);
            _stagesBtn.Subscribe(OpenStages);
            _rankingBtn.Subscribe(OpenRanking);
        }

        private void OnDisable()
        {
            _playBtn.Unsubscribe(LaunchSession);
            _prefsBtn.Unsubscribe(OpenPreferences);
            _policyBtn.Unsubscribe(OpenPolicy);
            _policyAltBtn.Unsubscribe(OpenPolicy);
            _stagesBtn.Unsubscribe(OpenStages);
            _rankingBtn.Unsubscribe(OpenRanking);
        }

        public override void Show()
        {
            base.Show();
        }

        private void LaunchSession()
        {
            int progress = PlayerPrefs.GetInt(
                SessionRules.PROGRESS_STAGE_KEY, 0);

            PlayerPrefs.SetInt(
                SessionRules.ACTIVE_STAGE_KEY, progress);
            PlayerPrefs.Save();

            _navigator.Navigate(Infrastructure.Configuration.NavigationRoutes.SESSION_SCENE);
            Hide();
        }

        private void OpenPreferences() => _preferencesScreen.Show();
        private void OpenPolicy() => _policyScreen.Show();
        private void OpenStages() => _stageSelectionScreen.Show();
        private void OpenRanking() => _rankingScreen.Show();
    }
}