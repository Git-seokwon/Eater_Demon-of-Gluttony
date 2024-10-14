using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.CullingGroup;

public class LatentSkillSlot : SkillSlot
{
    [SerializeField]
    private Image blindImage;
    [SerializeField]
    private TextMeshProUGUI cooldownText;

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

    private void OnEnable()
    {
        SetSkillUIAction(false);
    }

    private void OnDisable()
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

    public void SetupActive(KeyCode useKeyCode) => this.useKeyCode = useKeyCode;


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
            skill.Owner.SkillSystem.CancelTargetSearchingInActive();
            skill.Use();
        }
    }

    // ��ų�� ���¿� ���� UI�� ��ȭ�� �ִ� �Լ� 
    private void OnStateChanged(Skill skill, State<Skill> currentState, State<Skill> prevState, int layer)
    {
        var stateType = currentState.GetType();

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
    }
}
