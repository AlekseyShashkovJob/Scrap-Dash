using UnityEngine;
using UI.Components.Buttons;

namespace UI.Screens.Gameplay
{
    public class ActiveSessionScreen : ScreenBase
    {
        [SerializeField] private TouchableElement _suspendBtn;
        [SerializeField] private ScreenBase _suspendedOverlay;

        private void OnEnable()
        {
            _suspendBtn.Subscribe(OnSuspend);
        }

        private void OnDisable()
        {
            _suspendBtn.Unsubscribe(OnSuspend);
        }

        private void OnSuspend()
        {
            _suspendedOverlay.Show();
        }
    }
}