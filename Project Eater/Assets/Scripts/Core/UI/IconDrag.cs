using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IconDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // �巡�� �� �� �̵��Ǵ� ������ 
    public static Image draggedIcon;
    public static Skill skill;

    [SerializeField]
    private InventorySlot slot;

    private CanvasGroup activeSkills;
    private CanvasGroup passiveSkills;

    // ���� ���� ��
    private float startAlpha = 1f;

    public InventorySlot Slot => slot;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // �巡�� ���� �� ��� Icon�� �θ� ���� ��ų ������ �Ҵ� 
        skill = slot.SlotSkill;

        // ��ų ������ �̹��� �������� 
        Sprite iconImage = GetComponent<Image>().sprite;

        // CanvasGroup ���� �Ҵ� 
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
        // Canvas �� �巡�׿� Image ����
        GameObject iconObject = new GameObject("DraggedIcon");
        iconObject.transform.SetParent(slot.GetTransform(), false);

        // Image ������Ʈ �߰� �� ����
        draggedIcon = iconObject.AddComponent<Image>();
        draggedIcon.sprite = iconImage;
        // �巡�� �� raycast ���� 
        draggedIcon.raycastTarget = false;

        // ���� ������ ũ�� ���� 
        RectTransform rectTransform = iconObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = GetComponent<RectTransform>().sizeDelta;
    }
}
