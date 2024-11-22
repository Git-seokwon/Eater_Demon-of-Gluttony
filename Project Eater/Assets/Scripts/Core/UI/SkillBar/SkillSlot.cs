using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// ��ų�� ���콺�� ������ ���� ��, SkillTooltip�� ������ �� �ֵ��� IPointerEnterHandler�� IPointerExitHandler�� ��� �޴´�. 
// �� IPointer Interface : https://code-piggy.tistory.com/entry/Unity-IPointer-Interface
public class SkillSlot : MonoBehaviour
{
    [SerializeField]
    protected Image iconImage;
    [SerializeField]
    protected Image borderImage;

    // Slot�� ������ ��ų 
    protected Skill skill;
}
