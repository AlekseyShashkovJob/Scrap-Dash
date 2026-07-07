using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Components.Buttons
{
    public class AnimatedTouchable : TouchableElement
    {
        private const float PressScale = 1.09f;

        [SerializeField] private AudioClip _tapSound;

        private Vector3 _originalScale;

        private void OnEnable()
        {
            _originalScale = transform.localScale;
        }

        private void OnDisable()
        {
            transform.localScale = _originalScale;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            Infrastructure.Platform.SFXPlayer.Instance.PlayOneShot(_tapSound);
            transform.localScale *= PressScale;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            transform.localScale = _originalScale;
            base.OnPointerUp(eventData);
        }
    }
}