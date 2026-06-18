using UnityEngine;
using UnityEngine.EventSystems;

namespace GameCore.Gameplay
{
    public class TruckInput : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private Truck _truck;
        [SerializeField] private RectTransform _activeZone;

        private Camera _uiCamera;

        private void Awake()
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                _uiCamera = canvas.worldCamera;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            UpdateDirection(eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            UpdateDirection(eventData.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _truck.SetDirection(0);
        }

        private void UpdateDirection(Vector2 screenPosition)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _activeZone, screenPosition, _uiCamera, out Vector2 localPoint);

            int direction = localPoint.x >= 0 ? 1 : -1;
            _truck.SetDirection(direction);
        }
    }
}