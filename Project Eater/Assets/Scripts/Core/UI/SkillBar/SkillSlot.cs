using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// ��ų�� ���콺�� ������ ���� ��, SkillTooltip�� ������ �� �ֵ��� IPointerEnterHandler�� IPointerExitHandler�� ��� �޴´�. 
// �� IPointer Interface : https://code-piggy.tistory.com/entry/Unity-IPointer-Interface
public class SkillSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    protected Image iconImage;
    [SerializeField]
    protected Image borderImage;

    // Slot�� ������ ��ų 
    protected Skill skill;

    // Icon�� ���콺 �����͸� �÷��� �� ����Ǵ� �Լ� 
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Slot�� ��ų�� �Ҵ�Ǿ� �ִٸ� SkillTooltip Class�� Show �Լ� ���� 
        if (skill)
            SkillTooltip.Instance.Show(skill);
    }

    // Icon�� ���콺 �����Ͱ� ������ ������ �� ����Ǵ� �Լ� 
    public void OnPointerExit(PointerEventData eventData)
    {
        // Slot�� ��ų�� �Ҵ�Ǿ� �ִٸ� SkillTooltip Class�� Hide �Լ� ���� 
        if (skill)
            SkillTooltip.Instance.Hide();
    }
}
