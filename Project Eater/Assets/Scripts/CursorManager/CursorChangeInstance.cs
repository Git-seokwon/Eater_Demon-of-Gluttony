using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;

public class CursorChangeInstance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData) => CursorManager.Instance.ChangeCursor(CursorType.Default);

    public void OnPointerEnter(PointerEventData eventData) => CursorManager.Instance.ChangeCursor(CursorType.Select);

    public void OnPointerExit(PointerEventData eventData) => CursorManager.Instance.ChangeCursor(CursorType.Default);
}
