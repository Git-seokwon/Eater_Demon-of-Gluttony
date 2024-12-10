using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IconDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // 드래그 할 때 이동되는 아이콘 
    public static Image draggedIcon;
    public static Skill skill;

    [SerializeField]
    private InventorySlot slot;

    private CanvasGroup activeSkills;
    private CanvasGroup passiveSkills;

    // 시작 알파 값
    private float startAlpha = 1f;

    public InventorySlot Slot => slot;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작 시 대상 Icon의 부모가 가진 스킬 정보를 할당 
        skill = slot.SlotSkill;

        // 스킬 아이콘 이미지 가져오기 
        Sprite iconImage = GetComponent<Image>().sprite;

        // CanvasGroup 정보 할당 
        activeSkills = slot.GetActiveSkills();
        passiveSkills = slot.GetPassiveSkills();    

        CreateDraggedIcon(iconImage);

        if (skill.Type == SkillType.Active)
            passiveSkills.alpha = 0.5f;
        else
            activeSkills.alpha = 0.5f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedIcon != null)
            draggedIcon.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedIcon != null)
        {
            Destroy(draggedIcon.gameObject);
            draggedIcon = null;
        }

        activeSkills.alpha = startAlpha;
        passiveSkills.alpha = startAlpha;

        skill = null;
    }

    private void CreateDraggedIcon(Sprite iconImage)
    {
        // Canvas 상에 드래그용 Image 생성
        GameObject iconObject = new GameObject("DraggedIcon");
        iconObject.transform.SetParent(slot.GetTransform(), false);

        // Image 컴포넌트 추가 및 설정
        draggedIcon = iconObject.AddComponent<Image>();
        draggedIcon.sprite = iconImage;
        // 드래그 중 raycast 방지 
        draggedIcon.raycastTarget = false;

        // 원래 아이콘 크기 유지 
        RectTransform rectTransform = iconObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = GetComponent<RectTransform>().sizeDelta;
    }
}
