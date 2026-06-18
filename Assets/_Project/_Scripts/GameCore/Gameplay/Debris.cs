using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Gameplay
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Debris : MonoBehaviour
    {
        public DebrisType Type { get; private set; }

        private RectTransform _rectTransform;
        private float _fallSpeed;

        private Action<Debris> _onCaught;
        private Action<Debris> _onMissed;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            GetComponent<Image>().raycastTarget = false;
        }

        public void Init(DebrisType type, float fallSpeed,
            Action<Debris> onCaught, Action<Debris> onMissed)
        {
            Type = type;
            _fallSpeed = fallSpeed;
            _onCaught = onCaught;
            _onMissed = onMissed;
        }

        private void Update()
        {
            Vector2 pos = _rectTransform.anchoredPosition;
            pos.y -= _fallSpeed * Time.deltaTime;
            _rectTransform.anchoredPosition = pos;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Truck"))
            {
                _onCaught?.Invoke(this);
                Destroy(gameObject);
            }
            else if (other.CompareTag("Ground"))
            {
                _onMissed?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }
}