using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
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

    // Slot�� �Ҵ�� KeyCode�� ������ ���� 
    private KeyCode useKeyCode;

    public Skill Skill
    {
        get => skill;
        set
        {
            // ���� ��ų ������ ���� �Ҵ�Ǿ� �ִٸ� ��ų�� onStateChanged Event�� OnSkillStateChanged CallBack �Լ��� �����Ѵ�. 
            if (skill)
                skill.onStateChanged -= OnStateChanged;
            skill = value;

            if (skill != null)
            {
                skill.onStateChanged += OnStateChanged;

                iconImage.gameObject.SetActive(true);
                iconImage.sprite = skill.Icon;
            }
            else
                SetSkillUIAction(false);
        }
    }

    private void OnDestroy()
    {
        if (skill)
            skill.onStateChanged -= OnStateChanged;
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
        // ��ų�� ��� ������ ������ ��, Input�� ������ 
        if (skill.IsUseable && Input.GetKeyDown(useKeyCode))
        {
            // ��ų�� Owner SkillSystem�� CancelTargetSearching �Լ��� �����ؼ� 
            // Target Search ���� ��ų�� �ִٸ� ����ϰ� Skill�� ����Ѵ�. 
            skill.Owner.SkillSystem.CancelTargetSearching();
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
        // ApplyCycle�� Image�� ǥ���� �ֱ� ���� blindImage�� Ų��. 
        if (skill.ApplyCycle > 0f)
            blindImage.gameObject.SetActive(true);

        // ��ų�� Input Type�̸� ���� ApplyCount�� �ǹ��ϴ� remainInputCountText�� ���ְ� 
        // ��ų�� onCurrentApplyCountChanged Evet�� OnSkillCurrentApplyCountChanged �Լ��� ����ϰ� �ٷ� Text Update
        if (skill.ExecutionType == SkillExecutionType.Input)
        {
            remainInputCountText.gameObject.SetActive(true);
            skill.onCurrentApplyCountChanged += OnSkillCurrentApplyCountChanged;
            OnSkillCurrentApplyCountChanged(skill, skill.CurrentApplyCount, 0);
        }

        // ��ų�� InActionState�� �ƴ� ������ �ݺ� 
        while (skill.IsInState<InActionState>())
        {
            // blindImage�� ���� �ִٸ� ���� Apply Cycle�� ���� fillAmount�� �پ��� �Ѵ�. 
            if (blindImage.gameObject.activeSelf)
                blindImage.fillAmount = 1f - (skill.CurrentApplyCycle / skill.ApplyCycle);

            yield return null;
        }

        // ��ų�� CooldownState�� �ƴ϶�� blindImage�� ���ش�. 
        if (!skill.IsInState<CooldownState>())
            blindImage.gameObject.SetActive(false);

        // ������ ����� CallBack �Լ� ���� 
        skill.onCurrentApplyCountChanged -= OnSkillCurrentApplyCountChanged;

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
        while (skill.IsInState<CooldownState>())
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
    }

    private void OnSkillCurrentApplyCountChanged(Skill skill, int currentApplyCount, int prevApplyCount)
    => remainInputCountText.text = (skill.ApplyCount - currentApplyCount).ToString();
}
