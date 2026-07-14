using ScrapDash.GameCore;
using Infrastructure.Navigation;
using TMPro;
using UnityEngine;
using UI.Components.Buttons;

namespace UI.Screens.Gameplay
{
    public class CompletionOverlay : ScreenBase
    {
        [SerializeField] private TouchableElement _homeBtn;
        [SerializeField] private TouchableElement _retryBtn;
        [SerializeField] private TouchableElement _advanceBtn;

        [SerializeField] private TransitionController _navigator;

        [SerializeField] private TMP_Text _sessionPointsLabel;
        [SerializeField] private TMP_Text _recordLabel;

        private System.Action _retryCallback;
        private System.Action _advanceCallback;

        private void OnEnable()
        {
            _homeBtn.Subscribe(GoHome);
            _retryBtn.Subscribe(HandleRetry);
            _advanceBtn.Subscribe(HandleAdvance);
        }

        private void OnDisable()
        {
            _homeBtn.Unsubscribe(GoHome);
            _retryBtn.Unsubscribe(HandleRetry);
            _advanceBtn.Unsubscribe(HandleAdvance);
        }

        public void Present(int sessionPoints, int record, bool isFinalStage,
            System.Action onRetry, System.Action onAdvance)
        {
            _retryCallback = onRetry;
            _advanceCallback = onAdvance;

            base.Show();
            Time.timeScale = 0f;

            _sessionPointsLabel.text = $"SCORE {sessionPoints}";
            _recordLabel.text = $"BEST {record}";

            _advanceBtn.gameObject.SetActive(!isFinalStage);
        }

        private void GoHome()
        {
            Time.timeScale = 1f;
            SessionDirector.Instance.Terminate();
            _navigator.Navigate(Infrastructure.Configuration.NavigationRoutes.HOME_SCENE);
            Hide();
        }

        private void HandleRetry()
        {
            Time.timeScale = 1f;
            SessionDirector.Instance.Terminate();
            _retryCallback?.Invoke();
            Hide();
        }

        private void HandleAdvance()
        {
            Time.timeScale = 1f;
            SessionDirector.Instance.Terminate();
            _advanceCallback?.Invoke();
            Hide();
        }
    }
}