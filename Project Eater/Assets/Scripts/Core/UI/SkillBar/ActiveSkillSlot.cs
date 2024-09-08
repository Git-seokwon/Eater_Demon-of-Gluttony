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

    // Slot에 할당된 KeyCode를 저장할 변수 
    private KeyCode useKeyCode;

    public Skill Skill
    {
        get => skill;
        set
        {
            // 현재 스킬 변수에 값이 할당되어 있다면 스킬의 onStateChanged Event에 OnSkillStateChanged CallBack 함수를 해제한다. 
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
        // keyCodeText로 할당된 KeyCode를 Text 형태로 출력한다. 
        keyCodeText.text = ((int)useKeyCode - 48).ToString();
    }

    // 스킬의 사용 가능 여부를 나타내는 BlindImage Update
    private void UpdateBlindImage()
    {
        if (skill.IsInState<ReadyState>())
        {
            // 스킬이 Ready 상태일 때, 스킬을 사용할 수 없는 상태라면 blindImage를 켜주고 
            // 사용 가능한 상태라면 blindImage를 꺼준다. 
            if (!skill.IsUseable)
                blindImage.gameObject.SetActive(true);
            else
                blindImage.gameObject.SetActive(false);
        }
    }

    // Key Input 처리를 Update하는 함수 
    private void UpdateInput()
    {
        // 스킬이 사용 가능한 상태일 때, Input이 들어오면 
        if (skill.IsUseable && Input.GetKeyDown(useKeyCode))
        {
            // 스킬의 Owner SkillSystem의 CancelTargetSearching 함수를 실행해서 
            // Target Search 중인 스킬이 있다면 취소하고 Skill을 사용한다. 
            skill.Owner.SkillSystem.CancelTargetSearching();
            skill.Use();
        }
    }

    // 스킬의 상태에 따라 UI에 변화를 주는 함수 
    private void OnStateChanged(Skill skill, State<Skill> currentState, State<Skill> prevState, int layer)
    {
        var stateType = currentState.GetType();

        if (layer == 0)
        {
            // State가 Ready State가 아니라면 스킬 사용 중을 의미하는 usingBorderImage를 켜준다. 
            if (stateType != typeof(ReadyState))
                usingBorderImage.gameObject.SetActive(true);
            // Ready State라면 usingBorderImage를 꺼준다.
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
        // ApplyCycle을 Image로 표현해 주기 위해 blindImage를 킨다. 
        if (skill.ApplyCycle > 0f)
            blindImage.gameObject.SetActive(true);

        // 스킬이 Input Type이면 남은 ApplyCount를 의미하는 remainInputCountText를 켜주고 
        // 스킬의 onCurrentApplyCountChanged Evet에 OnSkillCurrentApplyCountChanged 함수를 등록하고 바로 Text Update
        if (skill.ExecutionType == SkillExecutionType.Input)
        {
            remainInputCountText.gameObject.SetActive(true);
            skill.onCurrentApplyCountChanged += OnSkillCurrentApplyCountChanged;
            OnSkillCurrentApplyCountChanged(skill, skill.CurrentApplyCount, 0);
        }

        // 스킬이 InActionState가 아닐 때까지 반복 
        while (skill.IsInState<InActionState>())
        {
            // blindImage가 켜져 있다면 현재 Apply Cycle에 따라 fillAmount가 줄어들게 한다. 
            if (blindImage.gameObject.activeSelf)
                blindImage.fillAmount = 1f - (skill.CurrentApplyCycle / skill.ApplyCycle);

            yield return null;
        }

        // 스킬이 CooldownState가 아니라면 blindImage를 꺼준다. 
        if (!skill.IsInState<CooldownState>())
            blindImage.gameObject.SetActive(false);

        // 위에서 등록한 CallBack 함수 해제 
        skill.onCurrentApplyCountChanged -= OnSkillCurrentApplyCountChanged;

        // 값 다시 초기화 
        remainInputCountText.gameObject.SetActive(false);
        usingBorderImage.gameObject.SetActive(false);
        usingBorderImage.fillAmount = 1f;
    }

    private IEnumerator ShowCooldown()
    {
        blindImage.gameObject.SetActive(true);
        cooldownText.gameObject.SetActive(true);

        // 스킬이 CooldownState일 동안 while문으로 UI들을 Update 한다. 
        while (skill.IsInState<CooldownState>())
        {
            // ※ F1 : 소숫점 한 자리까지만 표현 
            cooldownText.text = skill.CurrentCooldown.ToString("F1");
            // blindImage를 점점 사라지게 한다. 
            blindImage.fillAmount = skill.CurrentCooldown / skill.Cooldown;
            yield return null;
        }

        blindImage.gameObject.SetActive(false);
        blindImage.fillAmount = 1f;

        cooldownText.gameObject.SetActive(false);
    }

    // SetActive 함수로 자식 UI들을 키거나 꺼주는 함수  
    // → KeyCode Text는 꺼주지 않는다. 왜냐하면, KeyCode는 Skill UI가 아닌 Slot에 할당된 Key를 보여주는 
    //    Slot 자체의 UI라 항상 보여줘야 하기 때문이다. 
    // → 마찬가지로, borderImage도 항상 보여준다. 
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
