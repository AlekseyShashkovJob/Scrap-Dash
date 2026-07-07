using UnityEngine;
using UI.Components.Buttons;

namespace UI.Screens.MainMenu
{
    public class PolicyScreen : ScreenBase
    {
        [SerializeField] private TouchableElement _backBtn;

        private void OnEnable() => _backBtn.Subscribe(GoBack);
        private void OnDisable() => _backBtn.Unsubscribe(GoBack);

        private void GoBack() => Hide();
    }
}