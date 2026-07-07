using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Components.Buttons
{
    [RequireComponent(typeof(Image))]
    public abstract class TouchableElement : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler
    {
        public event Action OnActivated;

        private bool _pressed;

        public void Subscribe(Action handler) => OnActivated += handler;
        public void Unsubscribe(Action handler) => OnActivated -= handler;

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            _pressed = true;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (_pressed)
            {
                _pressed = false;
                OnActivated?.Invoke();
            }
        }
    }
}