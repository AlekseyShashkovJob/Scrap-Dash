using System;
using UnityEngine;
using UI.Components.Buttons;
using UnityEngine.UI;

namespace UI.Screens.MainMenu
{
    public class PreferencesScreen : ScreenBase
    {
        [SerializeField] private TouchableElement _backBtn;
        [SerializeField] private TouchableElement _audioBtn;
        [SerializeField] private TouchableElement _hapticsBtn;
        [SerializeField] private Image _audioIcon;
        [SerializeField] private Image _hapticsIcon;

        [SerializeField] private Sprite _enabledSprite;
        [SerializeField] private Sprite _disabledSprite;

        private bool _audioOn;
        private bool _hapticsOn;

        public static Action OnAudioToggled;
        public static Action OnHapticsToggled;

        private void OnEnable()
        {
            _backBtn.Subscribe(GoBack);
            _audioBtn.Subscribe(ToggleAudio);
            _hapticsBtn.Subscribe(ToggleHaptics);

            _audioOn = PlayerPrefs.GetInt(Infrastructure.Platform.StorageKeys.AudioEnabled, 1) == 1;
            _hapticsOn = PlayerPrefs.GetInt(Infrastructure.Platform.StorageKeys.HapticsEnabled, 0) == 1;

            RefreshAudioVisual();
            RefreshHapticsVisual();
        }

        private void OnDisable()
        {
            _backBtn.Unsubscribe(GoBack);
            _audioBtn.Unsubscribe(ToggleAudio);
            _hapticsBtn.Unsubscribe(ToggleHaptics);
        }

        private void GoBack() => Hide();

        private void ToggleAudio()
        {
            _audioOn = !_audioOn;
            PlayerPrefs.SetInt(Infrastructure.Platform.StorageKeys.AudioEnabled, _audioOn ? 1 : 0);
            RefreshAudioVisual();
            OnAudioToggled?.Invoke();
        }

        private void ToggleHaptics()
        {
            _hapticsOn = !_hapticsOn;
            PlayerPrefs.SetInt(Infrastructure.Platform.StorageKeys.HapticsEnabled, _hapticsOn ? 1 : 0);
            RefreshHapticsVisual();
            OnHapticsToggled?.Invoke();
        }

        private void RefreshAudioVisual()
        {
            _audioIcon.sprite = _audioOn ? _enabledSprite : _disabledSprite;
        }

        private void RefreshHapticsVisual()
        {
            _hapticsIcon.sprite = _hapticsOn ? _enabledSprite : _disabledSprite;
        }
    }
}