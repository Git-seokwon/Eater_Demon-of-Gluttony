using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.CullingGroup;

public class ActiveSkillSlot : SkillSlot
{
    [SerializeField]
    private Image blindImage;
    [SerializeField]
    private Image usingBorderImage;
    [SerializeField]
    private TextMeshProUGUI cooldownText;
    [SerializeField]
    private TextMeshProUGUI remainInputCountText;
    [SerializeField]
    private TextMeshProUGUI keyCodeText;
    [SerializeField]
    private TextMeshProUGUI stackCountText;

    // Slot�� �Ҵ�� KeyCode�� ������ ���� 
    private KeyCode useKeyCode;

    public Skill Skill
    {
        get => skill;
        set
        {
            // ���� ��ų ������ ���� �Ҵ�Ǿ� �ִٸ� ��ų�� CallBack �Լ��� �����Ѵ�. 
            if (skill)
            {
                skill.onStateChanged -= OnStateChanged;
                if (skill.ExecutionType == SkillExecutionType.Input)
                    skill.onCurrentApplyCountChanged -= OnSkillCurrentApplyCountChanged;
            }
            skill = value;

            if (skill != null)
            {
                skill.onStateChanged += OnStateChanged;
                if (skill.ExecutionType == SkillExecutionType.Input)
                    skill.onCurrentApplyCountChanged += OnSkillCurrentApplyCountChanged;

                iconImage.gameObject.SetActive(true);
                iconImage.sprite = skill.Icon;

                if (skill.StackCountDisplay)
                {
                    stackCountText.gameObject.SetActive(true);
                    SetEventStackCount(skill.CodeName, (skill.Owner as PlayerEntity));
                }
                else
                    stackCountText.gameObject.SetActive(false);
            }
            else
                SetSkillUIAction(false);
        }
    }

    private void OnEnable()
    {
        SetSkillUIAction(false);
    }

    private void OnDisable()
    {
        if (skill)
        {
            skill.onStateChanged -= OnStateChanged;
            UnSetEventStackCount(skill.CodeName, (skill.Owner as PlayerEntity));
        }
    }

    private void Update()
    {
        if (!skill)
            return;

        UpdateBlindImage();
        UpdateInput();
    }

    public void SetupActive(KeyCode useKeyCode)
    {
        this.useKeyCode = useKeyCode;
        // keyCodeText�� �Ҵ�� KeyCode�� Text ���·� ����Ѵ�. 
        keyCodeText.text = ((int)useKeyCode - 48).ToString();
    }

    // ��ų�� ��� ���� ���θ� ��Ÿ���� BlindImage Update
    private void UpdateBlindImage()
    {
        if (skill.IsInState<ReadyState>())
        {
            // ��ų�� Ready ������ ��, ��ų�� ����� �� ���� ���¶�� blindImage�� ���ְ� 
            // ��� ������ ���¶�� blindImage�� ���ش�. 
            if (!skill.IsUseable)
                blindImage.gameObject.SetActive(true);
            else
                blindImage.gameObject.SetActive(false);
        }
    }

    // Key Input ó���� Update�ϴ� �Լ� 
    private void UpdateInput()
    {
        if ((skill.Owner as PlayerEntity).isLevelUp)
            return;

        // ��ų�� ��� ������ ������ ��, Input�� ������ 
        if (skill.IsUseable && Input.GetKeyDown(useKeyCode))
        {
            // ��ų�� Owner SkillSystem�� CancelTargetSearching �Լ��� �����ؼ� 
            // Target Search ���� ��ų�� �ִٸ� ����ϰ� Skill�� ����Ѵ�. 
            skill.Owner.SkillSystem.CancelTargetSearchingInActive();
            skill.Use();
        }
    }

    // ��ų�� ���¿� ���� UI�� ��ȭ�� �ִ� �Լ� 
    private void OnStateChanged(Skill skill, State<Skill> currentState, State<Skill> prevState, int layer)
    {
        var stateType = currentState.GetType();

        if (layer == 0)
        {
            // State�� Ready State�� �ƴ϶�� ��ų ��� ���� �ǹ��ϴ� usingBorderImage�� ���ش�. 
            if (stateType != typeof(ReadyState))
                usingBorderImage.gameObject.SetActive(true);
            // Ready State��� usingBorderImage�� ���ش�.
            else
                usingBorderImage.gameObject.SetActive(false);
        }

        if (stateType == typeof(CooldownState))
            StartCoroutine("ShowCooldown");
        else if (stateType == typeof(InActionState))
            StartCoroutine("ShowActionInfo");
    }

    private IEnumerator ShowActionInfo()
    {
        if (skill == null) yield break;

        // ApplyCycle�� Image�� ǥ���� �ֱ� ���� blindImage�� Ų��. 
        if (skill.ApplyCycle > 0f)
            blindImage.gameObject.SetActive(true);

        // ��ų�� Input Type�̸� ���� ApplyCount�� �ǹ��ϴ� remainInputCountText�� ���ְ� 
        // ��ų�� onCurrentApplyCountChanged Evet�� OnSkillCurrentApplyCountChanged �Լ��� ����ϰ� �ٷ� Text Update
        if (skill.ExecutionType == SkillExecutionType.Input)
        {
            remainInputCountText.gameObject.SetActive(true);
            OnSkillCurrentApplyCountChanged(skill, skill.CurrentApplyCount, 0);
        }

        // ��ų�� InActionState�� �ƴ� ������ �ݺ� 
        while (skill != null && skill.IsInState<InActionState>())
        {
            // blindImage�� ���� �ִٸ� ���� Apply Cycle�� ���� fillAmount�� �پ��� �Ѵ�. 
            if (blindImage.gameObject.activeSelf)
                blindImage.fillAmount = 1f - (skill.CurrentApplyCycle / skill.ApplyCycle);

            yield return null;
        }

        // ��ų�� CooldownState�� �ƴ϶�� blindImage�� ���ش�. 
        if (skill != null && !skill.IsInState<CooldownState>())
            blindImage.gameObject.SetActive(false);

        // �� �ٽ� �ʱ�ȭ 
        remainInputCountText.gameObject.SetActive(false);
        usingBorderImage.gameObject.SetActive(false);
        usingBorderImage.fillAmount = 1f;
    }

    private IEnumerator ShowCooldown()
    {
        blindImage.gameObject.SetActive(true);
        cooldownText.gameObject.SetActive(true);

        // ��ų�� CooldownState�� ���� while������ UI���� Update �Ѵ�. 
        while (skill != null && skill.IsInState<CooldownState>())
        {
            // �� F1 : �Ҽ��� �� �ڸ������� ǥ�� 
            cooldownText.text = skill.CurrentCooldown.ToString("F1");
            // blindImage�� ���� ������� �Ѵ�. 
            blindImage.fillAmount = skill.CurrentCooldown / skill.Cooldown;
            yield return null;
        }

        blindImage.gameObject.SetActive(false);
        blindImage.fillAmount = 1f;

        cooldownText.gameObject.SetActive(false);
    }

    // SetActive �Լ��� �ڽ� UI���� Ű�ų� ���ִ� �Լ�  
    // �� KeyCode Text�� ������ �ʴ´�. �ֳ��ϸ�, KeyCode�� Skill UI�� �ƴ� Slot�� �Ҵ�� Key�� �����ִ� 
    //    Slot ��ü�� UI�� �׻� ������� �ϱ� �����̴�. 
    // �� ����������, borderImage�� �׻� �����ش�. 
    private void SetSkillUIAction(bool isOn)
    {
        cooldownText.gameObject.SetActive(isOn);
        blindImage.gameObject.SetActive(isOn);
        iconImage.gameObject.SetActive(isOn);
        remainInputCountText.gameObject.SetActive(isOn);
        usingBorderImage.gameObject.SetActive(isOn);
        stackCountText.gameObject.SetActive(isOn);
    }

    private void OnSkillCurrentApplyCountChanged(Skill skill, int currentApplyCount, int prevApplyCount)
    => remainInputCountText.text = (skill.ApplyCount - currentApplyCount).ToString();

    private void SetEventStackCount(string codeName, PlayerEntity owner)
    {
        switch (codeName)
        {
            case "DEATHSCYTHE":
                owner.onChangeDeathStack += UpdateDeathCount;
                UpdateDeathCount(owner);
                break;

            case "PREDATORY_INSTINCT":
                owner.onChangeMeathStack += UpdateMeatCount;
                UpdateMeatCount(owner);
                break;

            default:
                break;
        }
    }

    private void UnSetEventStackCount(string codeName, PlayerEntity owner)
    {
        switch (codeName)
        {
            case "DEATHSCYTHE":
                owner.onChangeDeathStack -= UpdateDeathCount;
                break;

            case "PREDATORY_INSTINCT":
                owner.onChangeMeathStack -= UpdateMeatCount;
                break;

            default:
                break;
        }
    }

    private void UpdateMeatCount(PlayerEntity owner)
        => stackCountText.text = owner.MeatStack.ToString();

    private void UpdateDeathCount(PlayerEntity owner)
        => stackCountText.text = owner.DeathStack.ToString();
}
