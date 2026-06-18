using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameCore.Gameplay
{
    public class Truck : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _speed = 600f;
        [SerializeField] private float _slowMultiplier = 0.3f;

        [Header("Animation")]
        [SerializeField] private Image _truckImage;
        [SerializeField] private Sprite[] _frames;
        [SerializeField] private float _frameInterval = 0.12f;

        private RectTransform _rectTransform;
        private RectTransform _parentRect;
        private int _moveDirection;
        private int _currentFrame;
        private float _frameTimer;

        private float _minX;
        private float _maxX;
        private float _currentSpeedMultiplier = 1f;
        private Coroutine _slowCoroutine;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _parentRect = transform.parent.GetComponent<RectTransform>();
        }

        private void Start()
        {
            CalculateBoundaries();
        }

        private void Update()
        {
            Move();
            Animate();
        }

        public void SetDirection(int direction)
        {
            _moveDirection = direction;
            UpdateFlip();
        }

        public void ApplySlow(float duration)
        {
            if (_slowCoroutine != null)
                StopCoroutine(_slowCoroutine);

            _slowCoroutine = StartCoroutine(SlowRoutine(duration));
        }

        private IEnumerator SlowRoutine(float duration)
        {
            _currentSpeedMultiplier = _slowMultiplier;
            yield return new WaitForSeconds(duration);
            _currentSpeedMultiplier = 1f;
            _slowCoroutine = null;
        }

        private void Move()
        {
            if (_moveDirection == 0) return;

            Vector2 pos = _rectTransform.anchoredPosition;
            pos.x += _moveDirection * _speed * _currentSpeedMultiplier * Time.deltaTime;
            pos.x = Mathf.Clamp(pos.x, _minX, _maxX);
            _rectTransform.anchoredPosition = pos;
        }

        private void CalculateBoundaries()
        {
            float parentHalfWidth = _parentRect.rect.width * 0.5f;
            float truckHalfWidth = _rectTransform.rect.width * 0.5f;

            _minX = -parentHalfWidth + truckHalfWidth;
            _maxX = parentHalfWidth - truckHalfWidth;
        }

        private void UpdateFlip()
        {
            if (_moveDirection == 0) return;

            Vector3 scale = _truckImage.rectTransform.localScale;
            scale.x = _moveDirection > 0 ? 1f : -1f;
            _truckImage.rectTransform.localScale = scale;
        }

        private void Animate()
        {
            if (_frames == null || _frames.Length == 0) return;

            _frameTimer += Time.deltaTime;
            if (_frameTimer >= _frameInterval)
            {
                _frameTimer = 0f;
                _currentFrame = (_currentFrame + 1) % _frames.Length;
                _truckImage.sprite = _frames[_currentFrame];
            }
        }
    }
}