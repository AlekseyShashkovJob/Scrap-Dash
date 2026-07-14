using UnityEngine;
using TMPro;
using System.Collections;
using ScrapDash.GameCore;

namespace UI.Screens.Gameplay
{
    public class ComboCounter : MonoBehaviour
    {
        [SerializeField] private RectTransform _wrapper;
        [SerializeField] private TMP_Text _label;

        [Header("Punch Effect")]
        [SerializeField] private float _punchScale = 1.05f;
        [SerializeField] private float _punchDuration = 0.15f;

        private Vector3 _restScale;
        private Coroutine _punchRoutine;

        private void Awake()
        {
            _restScale = _wrapper.localScale;
        }

        private void Start()
        {
            SessionDirector.Instance.OnComboUpdated += Refresh;
            Refresh(SessionDirector.Instance.ComboMultiplier);
        }

        private void OnDestroy()
        {
            if (SessionDirector.Instance == null) return;
            SessionDirector.Instance.OnComboUpdated -= Refresh;
        }

        private void Refresh(int combo)
        {
            _label.text = $"X{combo}";

            if (combo > 1)
                TriggerPunch();
        }

        private void TriggerPunch()
        {
            if (_punchRoutine != null)
                StopCoroutine(_punchRoutine);

            _punchRoutine = StartCoroutine(PunchSequence());
        }

        private IEnumerator PunchSequence()
        {
            float half = _punchDuration * 0.5f;
            Vector3 peak = _restScale * _punchScale;

            float t = 0f;
            while (t < half)
            {
                t += Time.deltaTime;
                _wrapper.localScale = Vector3.Lerp(_restScale, peak, t / half);
                yield return null;
            }

            t = 0f;
            while (t < half)
            {
                t += Time.deltaTime;
                _wrapper.localScale = Vector3.Lerp(peak, _restScale, t / half);
                yield return null;
            }

            _wrapper.localScale = _restScale;
            _punchRoutine = null;
        }
    }
}