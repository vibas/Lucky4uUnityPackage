using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lucky4u.Utility
{
    public class DropHandler : MonoBehaviour, IDropHandler
    {
        public Action<GameObject, GameObject> OnItemDroppedEvent;
        public void OnDrop(PointerEventData eventData)
        {
            OnItemDroppedEvent?.Invoke(this.gameObject, eventData.selectedObject);
        }
    }
}