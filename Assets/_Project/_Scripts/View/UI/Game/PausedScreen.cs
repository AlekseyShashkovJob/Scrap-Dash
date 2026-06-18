using GameCore;
using Misc.SceneManagment;
using UnityEngine;
using View.Button;

namespace View.UI.Game
{
    public class PausedScreen : UIScreen
    {
        [SerializeField] private UIScreen _optionsScreen;
        [SerializeField] private CustomButton _continue;
        [SerializeField] private CustomButton _settings;
        [SerializeField] private CustomButton _restart;
        [SerializeField] private CustomButton _menu;

        [SerializeField] private SceneLoader _sceneLoader;

        private void OnEnable()
        {
            _continue.AddListener(ContinueGame);
            _settings.AddListener(OpenOptions);
            _restart.AddListener(Restart);
            _menu.AddListener(BackToMenu);
        }

        private void OnDisable()
        {
            _continue.RemoveListener(ContinueGame);
            _settings.RemoveListener(OpenOptions);
            _restart.RemoveListener(Restart);
            _menu.RemoveListener(BackToMenu);
        }

        public override void StartScreen()
        {
            base.StartScreen();
            Time.timeScale = 0.0f;
        }

        private void ContinueGame()
        {
            Time.timeScale = 1.0f;
            CloseScreen();
        }

        private void OpenOptions()
        {
            _optionsScreen.StartScreen();
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