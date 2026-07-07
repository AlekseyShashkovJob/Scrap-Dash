using UnityEngine;
using UI.Components.Buttons;

namespace UI.Screens.MainMenu
{
    public class OfflineNotice : ScreenBase
    {
        [SerializeField] private Infrastructure.Navigation.TransitionController _navigator;
        [SerializeField] private TouchableElement _retryPortrait;
        [SerializeField] private TouchableElement _retryLandscape;

        [SerializeField] private GameObject _bgPortrait;
        [SerializeField] private GameObject _bgLandscape;

        private Vector2Int _cachedResolution;

        private void Update()
        {
            Vector2Int res = new Vector2Int(Screen.width, Screen.height);
            if (res != _cachedResolution)
            {
                _cachedResolution = res;
                AdaptLayout();
            }
        }

        public override void Show()
        {
            base.Show();
            _cachedResolution = new Vector2Int(Screen.width, Screen.height);
            AdaptLayout();
        }

        private void OnEnable()
        {
            _retryPortrait.Subscribe(Reload);
            _retryLandscape.Subscribe(Reload);
        }

        private void OnDisable()
        {
            _retryPortrait.Unsubscribe(Reload);
            _retryLandscape.Unsubscribe(Reload);
        }

        private void Reload()
        {
            string active = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            _navigator.Navigate(active);
            Hide();
        }

        private void AdaptLayout()
        {
            bool portrait = Screen.height >= Screen.width;
            _bgPortrait.SetActive(portrait);
            _bgLandscape.SetActive(!portrait);
        }
    }
}