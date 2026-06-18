using UnityEngine;
using UnityEngine.UI;
using View.Button;
using Misc.SceneManagment;
using TMPro;

namespace View.UI.Menu
{
    public class LevelsScreen : UIScreen
    {
        [SerializeField] private SceneLoader _sceneLoader;
        [SerializeField] private LevelButton[] _levelButtons;
        [SerializeField] private CustomButton _back;

        private void OnEnable()
        {
            _back.AddListener(BackToMenu);
            InitLevels();
        }

        private void OnDisable()
        {
            _back.RemoveListener(BackToMenu);
        }

        private void InitLevels()
        {
            int unlockedLevel = PlayerPrefs.GetInt(
                GameCore.Constants.GameConstants.LAST_UNLOCKED_LEVEL_KEY, 0);

            for (int i = 0; i < _levelButtons.Length; i++)
            {
                bool isUnlocked = i <= unlockedLevel;
                _levelButtons[i].Init(i, isUnlocked, OnLevelSelected);
            }
        }

        private void OnLevelSelected(int levelIndex)
        {
            PlayerPrefs.SetInt(
                GameCore.Constants.GameConstants.LAST_SELECTED_LEVEL_KEY, levelIndex);
            PlayerPrefs.Save();

            _sceneLoader.ChangeScene(Misc.Data.SceneConstants.GAME_SCENE);
            CloseScreen();
        }

        private void BackToMenu()
        {
            CloseScreen();
        }
    }

    [System.Serializable]
    public class LevelButton
    {
        [SerializeField] private CustomButton _button;
        [SerializeField] private TMP_Text _buttonText;
        [SerializeField] private Image _buttonImage;

        [Space]
        [SerializeField] private Sprite _unlockedSprite;
        [SerializeField] private Sprite _lockedSprite;

        private static readonly Color UnlockedColor = Color.white;
        private static readonly Color LockedColor = new Color32(180, 180, 180, 255);

        private int _index;
        private System.Action<int> _onClick;

        public void Init(int index, bool isUnlocked, System.Action<int> onClick)
        {
            _index = index;
            _onClick = onClick;

            _buttonText.color = isUnlocked ? UnlockedColor : LockedColor;
            _buttonImage.sprite = isUnlocked ? _unlockedSprite : _lockedSprite;

            _button.RemoveListener(HandleClick);
            if (isUnlocked)
                _button.AddListener(HandleClick);
        }

        private void HandleClick()
        {
            _onClick?.Invoke(_index);
        }
    }
}