using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorChangeInstance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData) => CursorManager.Instance.ChangeCursor(CursorType.Select);

    public void OnPointerExit(PointerEventData eventData) => CursorManager.Instance.ChangeCursor(CursorType.Default);
}
