using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipSlot : MonoBehaviour, IDropHandler
{
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private int slotIndex;
    [SerializeField]
    private bool isActive;
    [SerializeField]
    private GameObject skillKeyChangeWarningUI;

    private Skill slotSkill;

    public Skill SlotSkill
    {
        get => slotSkill;
        set
        {
            if (value == null)
            {
                slotSkill = null;
                iconImage.sprite = null;
                SetIconAlpha(0f);
                return;
            }
            if (slotSkill == value)
                return;

            // 사용 중인 액티브 스킬인 경우, 키 변경이 불가함 (Ready, Cooldown 상태일 때만 가능)
            if (value.StateMachine != null && value.Type == SkillType.Active 
                && (!value.IsInState<ReadyState>() && !value.IsInState<CooldownState>()))
            {
                // 안내 문구 출력 
                StartCoroutine(ShowSkillKeyChangeWarningUI());
                return; 
            }

            // 중복 등록 방지 
            HandleDuplicateSkill(value);

            // 위 조건문 때문에 prevSlotSkill과 slotSkill은 다르다.
            var prevSlotSkill = slotSkill;
            slotSkill = value;

            iconImage.sprite = slotSkill.Icon;
            SetIconAlpha(1f);

            var skillSystem = GameManager.Instance.player.SkillSystem;
            // 스킬 교체 : 전에 장착된 스킬 해제 
            if (prevSlotSkill != null)
                skillSystem.Disarm(prevSlotSkill, prevSlotSkill.skillKeyNumber);
            // 망멸의 낫 스킬의 경우, 스킬 Action에서 자동으로 장착하기 때문에 중복 장착이 되지 않도록 한다. 
            if (!skillSystem.FindEquippedSkill(slotSkill))
                skillSystem.Equip(slotSkill, slotIndex);
        }
    }

    private void OnEnable()
    {
        CheckAndEquipEvolvedSkill("DEATHSCYTHE", "ANNIHILATION_SCYTHE");

        // 스킬 조합으로 인해 장착된 스킬이 소모된 경우, Slot에서 지우기 
        if (SlotSkill != null && !GameManager.Instance.player.SkillSystem.ContainsInequippedskills(SlotSkill))
            SlotSkill = null;
    }

    // 해당 스크립트가 컴포넌트로 추가 된 게임 오브젝트 RactTransform 내에 포인터 드랍이 발생하면 실행되는 콜백 함수
    public void OnDrop(PointerEventData eventData)
    {
        if (IconDrag.skill == null)
            return;

        if ((isActive && IconDrag.skill.Type == SkillType.Active) ||
           (!isActive && IconDrag.skill.Type == SkillType.Passive))
        {
            SlotSkill = IconDrag.skill;
            // 스킬 장착 효과음 재생
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.equipSkill);
        }
    }

    private void SetIconAlpha(float alpha)
    {
        alpha = Mathf.Clamp01(alpha);
        Color color = iconImage.color;
        color.a = alpha;
        iconImage.color = color;
    }

    private void HandleDuplicateSkill(Skill value)
    {
        var equipSlots = isActive ? GameManager.Instance.EquipActiveSlots : GameManager.Instance.EquipPassiveSlots;
        foreach (var equipSlot in equipSlots)
        {
            if (equipSlot != this && equipSlot.slotSkill?.ID == value.ID)
            {
                equipSlot.ClearSlot();
                break;
            }
        }
    }

    private void ClearSlot()
    {
        GameManager.Instance.player.SkillSystem.Disarm(SlotSkill, SlotSkill.skillKeyNumber);
        SlotSkill = null;
    }

    private void CheckAndEquipEvolvedSkill(string baseSkillCodeName, string evolvedSkillCodeName)
    {
        var skillSystem = GameManager.Instance.player.SkillSystem;
        if (SlotSkill?.CodeName == baseSkillCodeName && skillSystem.ContainsInequippedskills(evolvedSkillCodeName))
            SlotSkill = skillSystem.FindEquippedSkill(evolvedSkillCodeName);
    }

    private IEnumerator ShowSkillKeyChangeWarningUI()
    {
        skillKeyChangeWarningUI.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        skillKeyChangeWarningUI.SetActive(false);
    }

    public void StageEnd() => SlotSkill = null;
}
