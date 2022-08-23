using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropHandler : MonoBehaviour, IDropHandler
{
    public Action<GameObject, GameObject> OnItemDroppedEvent;
    public void OnDrop(PointerEventData eventData)
    {
        OnItemDroppedEvent?.Invoke(this.gameObject, eventData.selectedObject);
    }
}
