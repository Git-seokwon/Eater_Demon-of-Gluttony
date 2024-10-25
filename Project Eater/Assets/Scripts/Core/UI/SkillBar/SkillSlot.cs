using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 스킬에 마우스를 가져다 댔을 때, SkillTooltip을 보여줄 수 있도록 IPointerEnterHandler와 IPointerExitHandler를 상속 받는다. 
// ※ IPointer Interface : https://code-piggy.tistory.com/entry/Unity-IPointer-Interface
public class SkillSlot : MonoBehaviour
{
    [SerializeField]
    protected Image iconImage;
    [SerializeField]
    protected Image borderImage;

    // Slot이 소유한 스킬 
    protected Skill skill;
}
