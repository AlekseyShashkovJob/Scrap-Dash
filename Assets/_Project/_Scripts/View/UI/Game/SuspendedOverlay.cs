using ScrapDash.GameCore;
using Infrastructure.Navigation;
using UI.Components.Buttons;
using UnityEngine;

namespace UI.Screens.Gameplay
{
    public class SuspendedOverlay : ScreenBase
    {
        [SerializeField] private ScreenBase _preferencesScreen;
        [SerializeField] private TouchableElement _resumeBtn;
        [SerializeField] private TouchableElement _prefsBtn;
        [SerializeField] private TouchableElement _retryBtn;
        [SerializeField] private TouchableElement _homeBtn;

        [SerializeField] private TransitionController _navigator;

        private void OnEnable()
        {
            _resumeBtn.Subscribe(Resume);
            _prefsBtn.Subscribe(OpenPreferences);
            _retryBtn.Subscribe(Retry);
            _homeBtn.Subscribe(GoHome);
        }

        private void OnDisable()
        {
            _resumeBtn.Unsubscribe(Resume);
            _prefsBtn.Unsubscribe(OpenPreferences);
            _retryBtn.Unsubscribe(Retry);
            _homeBtn.Unsubscribe(GoHome);
        }

        public override void Show()
        {
            base.Show();
            Time.timeScale = 0f;
        }

        private void Resume()
        {
            Time.timeScale = 1f;
            Hide();
        }

        private void OpenPreferences()
        {
            _preferencesScreen.Show();
        }

        private void Retry()
        {
            Time.timeScale = 1f;
            SessionDirector.Instance.Terminate();
            _navigator.Navigate(Infrastructure.Configuration.NavigationRoutes.SESSION_SCENE);
            Hide();
        }

        private void GoHome()
        {
            Time.timeScale = 1f;
            SessionDirector.Instance.Terminate();
            _navigator.Navigate(Infrastructure.Configuration.NavigationRoutes.HOME_SCENE);
            Hide();
        }
    }
}