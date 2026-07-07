using UnityEngine;

namespace UI.Screens.Common
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SplashOverlay : MonoBehaviour
    {
        private readonly float _bobSpeed = 5f;
        private readonly float _bobAmplitude = 34f;

        [SerializeField] private RectTransform _floatingElement;
        [SerializeField] private GameObject _portraitVariant;
        [SerializeField] private GameObject _landscapeVariant;

        private Vector2 _anchorOrigin;
        private Vector2 _previousScreenSize;

        private void Start()
        {
            _anchorOrigin = _floatingElement.anchoredPosition;
            _previousScreenSize = new Vector2(Screen.width, Screen.height);
            RefreshLayout();
        }

        private void Update()
        {
            if (DetectResize())
            {
                RecalculateOrigin();
                RefreshLayout();
            }

            float wave = Mathf.Sin(Time.time * _bobSpeed) * _bobAmplitude;
            _floatingElement.anchoredPosition = _anchorOrigin + new Vector2(0f, wave);
        }

        private bool DetectResize()
        {
            Vector2 current = new Vector2(Screen.width, Screen.height);
            if (current != _previousScreenSize)
            {
                _previousScreenSize = current;
                return true;
            }
            return false;
        }

        private void RecalculateOrigin()
        {
            float currentWave = Mathf.Sin(Time.time * _bobSpeed) * _bobAmplitude;
            _anchorOrigin = _floatingElement.anchoredPosition - new Vector2(0f, currentWave);
        }

        private void RefreshLayout()
        {
            bool portrait = Screen.height >= Screen.width;
            _portraitVariant.SetActive(portrait);
            _landscapeVariant.SetActive(!portrait);
        }
    }
}