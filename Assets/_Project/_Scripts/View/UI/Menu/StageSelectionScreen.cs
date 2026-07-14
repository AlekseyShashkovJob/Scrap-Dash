using UnityEngine;
using UnityEngine.UI;
using UI.Components.Buttons;
using Infrastructure.Navigation;
using ScrapDash.GameCore.Constants;
using TMPro;

namespace UI.Screens.MainMenu
{
    public class StageSelectionScreen : ScreenBase
    {
        [SerializeField] private TransitionController _navigator;
        [SerializeField] private StageCell[] _cells;
        [SerializeField] private TouchableElement _backBtn;

        private void OnEnable()
        {
            _backBtn.Subscribe(GoBack);
            PopulateCells();
        }

        private void OnDisable()
        {
            _backBtn.Unsubscribe(GoBack);
        }

        private void PopulateCells()
        {
            int progress = PlayerPrefs.GetInt(
                SessionRules.PROGRESS_STAGE_KEY, 0);

            for (int i = 0; i < _cells.Length; i++)
            {
                bool available = i <= progress;
                _cells[i].Configure(i, available, OnStageChosen);
            }
        }

        private void OnStageChosen(int index)
        {
            PlayerPrefs.SetInt(SessionRules.ACTIVE_STAGE_KEY, index);
            PlayerPrefs.Save();

            _navigator.Navigate(Infrastructure.Configuration.NavigationRoutes.SESSION_SCENE);
            Hide();
        }

        private void GoBack() => Hide();
    }

    [System.Serializable]
    public class StageCell
    {
        [SerializeField] private TouchableElement _button;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Image _background;

        [Space]
        [SerializeField] private Sprite _availableSprite;
        [SerializeField] private Sprite _lockedSprite;

        private static readonly Color AvailableColor = Color.white;
        private static readonly Color LockedColor = new Color32(180, 180, 180, 255);

        private int _stageIndex;
        private System.Action<int> _selectionHandler;

        public void Configure(int index, bool available, System.Action<int> onSelected)
        {
            _stageIndex = index;
            _selectionHandler = onSelected;

            _label.color = available ? AvailableColor : LockedColor;
            _background.sprite = available ? _availableSprite : _lockedSprite;

            _button.Unsubscribe(HandleTap);
            if (available)
                _button.Subscribe(HandleTap);
        }

        private void HandleTap()
        {
            _selectionHandler?.Invoke(_stageIndex);
        }
    }
}