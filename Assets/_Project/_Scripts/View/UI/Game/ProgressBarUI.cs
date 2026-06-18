using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace View.UI.Game
{
    public class ProgressBarUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _barContainer;
        [SerializeField] private Image _fillImage;
        [SerializeField] private TMP_Text _percentText;
        [SerializeField] private TMP_Text _floorText;

        [Header("Animation")]
        [SerializeField] private float _scaleMultiplier = 1.05f;
        [SerializeField] private float _animationDuration = 0.15f;
        [SerializeField] private float _resetDelay = 0.1f;

        private Vector3 _baseScale;
        private Coroutine _animCoroutine;

        private void Awake()
        {
            _baseScale = _barContainer.localScale;
        }

        public void UpdateProgress(float progress)
        {
            _fillImage.fillAmount = progress;

            int percent = Mathf.RoundToInt(progress * 100f);
            _percentText.text = $"{percent}%";

            if (progress >= 1f)
            {
                PlayCompleteAnimation();
            }
        }

        public void UpdateFloor(int current, int total)
        {
            _floorText.text = $"Floor {current}/{total}";
        }

        private void PlayCompleteAnimation()
        {
            if (_animCoroutine != null)
                StopCoroutine(_animCoroutine);

            _animCoroutine = StartCoroutine(CompleteRoutine());
        }

        private IEnumerator CompleteRoutine()
        {
            Vector3 targetScale = _baseScale * _scaleMultiplier;
            float halfDuration = _animationDuration * 0.5f;

            // Scale up
            float elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / halfDuration;
                _barContainer.localScale = Vector3.Lerp(_baseScale, targetScale, t);
                yield return null;
            }

            // Scale down
            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / halfDuration;
                _barContainer.localScale = Vector3.Lerp(targetScale, _baseScale, t);
                yield return null;
            }

            _barContainer.localScale = _baseScale;

            // Небольшая пауза перед сбросом
            yield return new WaitForSecondsRealtime(_resetDelay);

            // Сброс
            _fillImage.fillAmount = 0f;
            _percentText.text = "0%";

            _animCoroutine = null;
        }
    }
}