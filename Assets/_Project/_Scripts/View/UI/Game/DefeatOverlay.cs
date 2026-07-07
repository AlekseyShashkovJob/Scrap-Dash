using GameCore;
using Infrastructure.Navigation;
using TMPro;
using UI.Components.Buttons;
using UnityEngine;

namespace UI.Screens.Gameplay
{
    public class DefeatOverlay : ScreenBase
    {
        [SerializeField] private TouchableElement _menuBtn;
        [SerializeField] private TouchableElement _retryBtn;

        [SerializeField] private TransitionController _navigator;

        [SerializeField] private TMP_Text _sessionPointsLabel;
        [SerializeField] private TMP_Text _recordLabel;

        private void OnEnable()
        {
            _menuBtn.Subscribe(GoHome);
            _retryBtn.Subscribe(Retry);
        }

        private void OnDisable()
        {
            _menuBtn.Unsubscribe(GoHome);
            _retryBtn.Unsubscribe(Retry);
        }

        public void Present(int sessionPoints, int record)
        {
            base.Show();
            Time.timeScale = 0f;

            _sessionPointsLabel.text = $"SCORE {sessionPoints}";
            _recordLabel.text = $"BEST {record}";
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