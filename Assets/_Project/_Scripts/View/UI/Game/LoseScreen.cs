using GameCore;
using Misc.SceneManagment;
using TMPro;
using UnityEngine;
using View.Button;

namespace View.UI.Game
{
    public class LoseScreen : UIScreen
    {
        [SerializeField] private CustomButton _back;
        [SerializeField] private CustomButton _restart;

        [SerializeField] private SceneLoader _sceneLoader;

        [SerializeField] private TMP_Text _currentScoreText;
        [SerializeField] private TMP_Text _bestScoreText;

        private void OnEnable()
        {
            _back.AddListener(BackToMenu);
            _restart.AddListener(Restart);
        }

        private void OnDisable()
        {
            _back.RemoveListener(BackToMenu);
            _restart.RemoveListener(Restart);
        }

        public void Show(int currentScore, int bestScore)
        {
            base.StartScreen();
            Time.timeScale = 0.0f;

            _currentScoreText.text = $"SCORE {currentScore}";
            _bestScoreText.text = $"BEST {bestScore}";
        }

        private void Restart()
        {
            Time.timeScale = 1.0f;
            GameManager.Instance.StopGame();
            _sceneLoader.ChangeScene(Misc.Data.SceneConstants.GAME_SCENE);
            CloseScreen();
        }

        private void BackToMenu()
        {
            Time.timeScale = 1.0f;
            GameManager.Instance.StopGame();
            _sceneLoader.ChangeScene(Misc.Data.SceneConstants.MENU_SCENE);
            CloseScreen();
        }
    }
}