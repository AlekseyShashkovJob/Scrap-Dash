using GameCore;
using Misc.SceneManagment;
using TMPro;
using UnityEngine;
using View.Button;

namespace View.UI.Game
{
    public class VictoryScreen : UIScreen
    {
        [SerializeField] private CustomButton _back;
        [SerializeField] private CustomButton _restart;
        [SerializeField] private CustomButton _next;

        [SerializeField] private SceneLoader _sceneLoader;

        [SerializeField] private TMP_Text _currentScoreText;
        [SerializeField] private TMP_Text _bestScoreText;

        private System.Action _onRestart;
        private System.Action _onNext;

        private void OnEnable()
        {
            _back.AddListener(BackToMenu);
            _restart.AddListener(HandleRestart);
            _next.AddListener(HandleNext);
        }

        private void OnDisable()
        {
            _back.RemoveListener(BackToMenu);
            _restart.RemoveListener(HandleRestart);
            _next.RemoveListener(HandleNext);
        }

        public void Show(int currentScore, int bestScore, bool isLastLevel,
            System.Action onRestart, System.Action onNext)
        {
            _onRestart = onRestart;
            _onNext = onNext;

            base.StartScreen();
            Time.timeScale = 0.0f;

            _currentScoreText.text = $"SCORE {currentScore}";
            _bestScoreText.text = $"BEST {bestScore}";

            _next.gameObject.SetActive(!isLastLevel);
        }

        private void BackToMenu()
        {
            Time.timeScale = 1.0f;
            GameManager.Instance.StopGame();
            _sceneLoader.ChangeScene(Misc.Data.SceneConstants.MENU_SCENE);
            CloseScreen();
        }

        private void HandleRestart()
        {
            Time.timeScale = 1.0f;
            GameManager.Instance.StopGame();
            _onRestart?.Invoke();
            CloseScreen();
        }

        private void HandleNext()
        {
            Time.timeScale = 1.0f;
            GameManager.Instance.StopGame();
            _onNext?.Invoke();
            CloseScreen();
        }
    }
}