using UnityEngine;
using View.Button;

namespace View.UI.Menu
{
    public class MainMenuScreen : UIScreen
    {
        [SerializeField] private UIScreen _optionsScreen;
        [SerializeField] private UIScreen _privacyScreen;
        [SerializeField] private UIScreen _levelsScreen;
        [SerializeField] private UIScreen _leaderboardScreen;

        [SerializeField] private Misc.SceneManagment.SceneLoader _sceneLoader;

        [Space, Header("Buttons")]
        [SerializeField] private CustomButton _startGame;
        [SerializeField] private CustomButton _settings;
        [SerializeField] private CustomButton _privacy;
        [SerializeField] private CustomButton _privacySmall;
        [SerializeField] private CustomButton _levels;
        [SerializeField] private CustomButton _leaderboard;

        private void OnEnable()
        {
            _startGame.AddListener(OpenGame);
            _settings.AddListener(OpenOptions);
            _privacy.AddListener(OpenPrivacy);
            _privacySmall.AddListener(OpenPrivacy);
            _levels.AddListener(OpenLevels);
            _leaderboard.AddListener(OpenLeaderboard);
        }

        private void OnDisable()
        {
            _startGame.RemoveListener(OpenGame);
            _settings.RemoveListener(OpenOptions);
            _privacy.RemoveListener(OpenPrivacy);
            _privacySmall.RemoveListener(OpenPrivacy);
            _levels.RemoveListener(OpenLevels);
            _leaderboard.RemoveListener(OpenLeaderboard);
        }

        public override void StartScreen()
        {
            base.StartScreen();
        }

        private void OpenGame()
        {
            int lastUnlocked = PlayerPrefs.GetInt(
                GameCore.Constants.GameConstants.LAST_UNLOCKED_LEVEL_KEY, 0);

            PlayerPrefs.SetInt(
                GameCore.Constants.GameConstants.LAST_SELECTED_LEVEL_KEY, lastUnlocked);
            PlayerPrefs.Save();

            _sceneLoader.ChangeScene(Misc.Data.SceneConstants.GAME_SCENE);
            CloseScreen();
        }

        private void OpenOptions() => _optionsScreen.StartScreen();
        private void OpenPrivacy() => _privacyScreen.StartScreen();
        private void OpenLevels() => _levelsScreen.StartScreen();
        private void OpenLeaderboard() => _leaderboardScreen.StartScreen();
    }
}