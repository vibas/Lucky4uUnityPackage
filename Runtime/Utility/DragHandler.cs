using UnityEngine;
using UnityEngine.EventSystems;

namespace Lucky4u.Utility
{
    public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        CanvasGroup canvasGroup => GetComponent<CanvasGroup>();
        Vector3 startPosition;
        public bool canDrag;
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (canDrag)
            {
                canvasGroup.blocksRaycasts = false;
                startPosition = transform.position;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.position = startPosition;
            canvasGroup.blocksRaycasts = true;
        }
    }
}