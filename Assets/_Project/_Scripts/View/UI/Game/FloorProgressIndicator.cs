using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace UI.Screens.Gameplay
{
    public class FloorProgressIndicator : MonoBehaviour
    {
        [SerializeField] private RectTransform _barRoot;
        [SerializeField] private Image _fill;
        [SerializeField] private TMP_Text _percentLabel;
        [SerializeField] private TMP_Text _floorLabel;

        [Header("Completion FX")]
        [SerializeField] private float _celebrationScale = 1.05f;
        [SerializeField] private float _celebrationDuration = 0.15f;
        [SerializeField] private float _resetPause = 0.1f;

        private Vector3 _normalScale;
        private Coroutine _fxRoutine;

        private void Awake()
        {
            _normalScale = _barRoot.localScale;
        }

        public void SetProgress(float ratio)
        {
            _fill.fillAmount = ratio;
            _percentLabel.text = $"{Mathf.RoundToInt(ratio * 100f)}%";

            if (ratio >= 1f)
                PlayCelebration();
        }

        public void SetFloorLabel(int current, int total)
        {
            _floorLabel.text = $"Floor {current}/{total}";
        }

        private void PlayCelebration()
        {
            if (_fxRoutine != null)
                StopCoroutine(_fxRoutine);

            _fxRoutine = StartCoroutine(CelebrationSequence());
        }

        private IEnumerator CelebrationSequence()
        {
            Vector3 target = _normalScale * _celebrationScale;
            float half = _celebrationDuration * 0.5f;

            float t = 0f;
            while (t < half)
            {
                t += Time.unscaledDeltaTime;
                _barRoot.localScale = Vector3.Lerp(_normalScale, target, t / half);
                yield return null;
            }

            t = 0f;
            while (t < half)
            {
                t += Time.unscaledDeltaTime;
                _barRoot.localScale = Vector3.Lerp(target, _normalScale, t / half);
                yield return null;
            }

            _barRoot.localScale = _normalScale;

            yield return new WaitForSecondsRealtime(_resetPause);

            _fill.fillAmount = 0f;
            _percentLabel.text = "0%";
            _fxRoutine = null;
        }
    }
}