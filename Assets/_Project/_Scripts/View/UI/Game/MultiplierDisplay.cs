using UnityEngine;
using TMPro;
using System.Collections;
using GameCore;

namespace View.UI.Game
{
    public class MultiplierDisplay : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private TMP_Text _multiplierText;

        [Header("Animation")]
        [SerializeField] private float _scaleMultiplier = 1.05f;
        [SerializeField] private float _animationDuration = 0.15f;

        private Vector3 _baseScale;
        private Coroutine _animCoroutine;

        private void Awake()
        {
            _baseScale = _container.localScale;
        }

        private void Start()
        {
            GameManager.Instance.OnMultiplierChanged += UpdateMultiplier;
            UpdateMultiplier(GameManager.Instance.Multiplier);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.OnMultiplierChanged -= UpdateMultiplier;
        }

        private void UpdateMultiplier(int multiplier)
        {
            _multiplierText.text = $"X{multiplier}";

            if (multiplier > 1)
                PlayPunchAnimation();
        }

        private void PlayPunchAnimation()
        {
            if (_animCoroutine != null)
                StopCoroutine(_animCoroutine);

            _animCoroutine = StartCoroutine(PunchRoutine());
        }

        private IEnumerator PunchRoutine()
        {
            float halfDuration = _animationDuration * 0.5f;
            Vector3 targetScale = _baseScale * _scaleMultiplier;

            // Scale up
            float elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                _container.localScale = Vector3.Lerp(_baseScale, targetScale, t);
                yield return null;
            }

            // Scale down
            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                _container.localScale = Vector3.Lerp(targetScale, _baseScale, t);
                yield return null;
            }

            _container.localScale = _baseScale;
            _animCoroutine = null;
        }
    }
}