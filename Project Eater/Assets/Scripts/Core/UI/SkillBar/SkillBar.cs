using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SkillBar : MonoBehaviour
{
    // 각 Skill Slot 최대 칸 수 
    [SerializeField]
    private int slotCount;

    // 스킬 목록을 보여줄 Target SkillSystem
    [SerializeField]
    private SkillSystem skillSystem;
    // 생성된 Slot을 저장 
    // 1) 0 ~ 3 : Active Skill
    // 2) 4 ~ 7 : Passive Skill
    [SerializeField]
    private List<ActiveSkillSlot> activeSlots = new();
    [SerializeField]
    private List<PassiveSkillSlot> passiveSlots = new();

    private bool IsActiveSkillSlotFull => activeSlots.Count >= slotCount;
    private bool IsPassiveSkillSlotFull => passiveSlots.Count >= slotCount;
    private bool IsActiveSkillSlotEmpty => activeSlots.Count <= 0;
    private bool IsPassiveSkillSlotEmpty => passiveSlots.Count <= 0;

    private void Start()
    {
        // 스킬 시스템의 onSkillEquipped Event에 OnSkillEquipped 메서드 등록 
        skillSystem.onSkillEquipped += OnSkillEquipped;
        skillSystem.onSkillDisarm += OnSkillDisarmed;

        // Active 스킬의 경우, 스킬 사용키를 보여줘야 함 
        for (int i = 0; i < slotCount; i++)
            activeSlots[i].SetupActive((KeyCode)49 + i);
    }

    private void OnDestroy()
    {
        skillSystem.onSkillEquipped -= OnSkillEquipped;
        skillSystem.onSkillDisarm -= OnSkillDisarmed;
    }

    // 스킬을 Slot에 추가하는 함수 
    private void TryEquipSlot(Skill skill, int keyNumber)
    {
        // keyNumbder가 -1이면 해방 스킬이므로 slot 등록 X
        if (keyNumber == -1) return;

        if (skill.Type == SkillType.Active && !IsActiveSkillSlotFull)
        {
            activeSlots[keyNumber].Skill = skill;
        }
        else if (skill.Type == SkillType.Passive && !IsPassiveSkillSlotFull)
        {
            passiveSlots[keyNumber].Skill = skill;
        }
    }

    // 스킬을 Slot에서 해제하는 함수 
    private void TryDisarmSlot(Skill skill, int keyNumber)
    {
        // keyNumbder가 -1이면 해방 스킬이므로 slot 해제 X
        if (keyNumber == -1) return;

        if (skill.Type == SkillType.Active && !IsActiveSkillSlotEmpty)
        {
            activeSlots[keyNumber].Skill = null;
        }
        else if (skill.Type == SkillType.Passive && !IsPassiveSkillSlotEmpty)
        {
            passiveSlots[keyNumber].Skill = null;
        }
    }

    private void OnSkillEquipped(SkillSystem skillSystem, Skill skill, int keyNumbder)
        => TryEquipSlot(skill, keyNumbder);

    private void OnSkillDisarmed(SkillSystem skillSystem, Skill skill, int keyNumbder)
        => TryDisarmSlot(skill, keyNumbder);
}
