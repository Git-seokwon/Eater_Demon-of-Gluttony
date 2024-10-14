using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillProgressBar : MonoBehaviour
{
    [SerializeField]
    private Image fillImage;
    [SerializeField]
    private TextMeshProUGUI timeText;

    // Player Entity의 SkillSystem을 저장할 변수
    [SerializeField]
    private SkillSystem skillSystem;

    // 후에 스테이지 입장할 때, SetUp하는 함수로 변경하기 
    private void Start()
    {
        skillSystem.onSkillStateChanged += OnSkillStateChanged;
        gameObject.SetActive(false);
    }

    private void InfoUpdate(float currentTime, float maxTime)
    {
        // fillImage가 차오르는 해주기 
        fillImage.fillAmount = currentTime / maxTime;
        // TimeText는 (현재 시간 / 최대 시간)으로 설정 
        timeText.text = $"{currentTime:F1} : {maxTime:F1}";
    }

    private void OnSkillStateChanged(SkillSystem skillSystem, Skill skill, 
        State<Skill> newState, State<Skill> prevState, int layer)
    {
        // 스킬 CastingState일 때 
        if (skill.IsInState<CastingState>())
        {
            gameObject.SetActive(true);
            StartCoroutine("CastingProgressUpdate", skill);
        }
        // 스킬 ChargingState일 때 
        else if (skill.IsInState<ChargingState>())
        {
            gameObject.SetActive(true);
            StartCoroutine("ChargingProgressUpdate", skill);
        }
    }

    private IEnumerator CastingProgressUpdate(Skill skill)
    {
        // whlile 문으로 스킬이 Casting 상태 Check
        while (skill.IsInState<CastingState>())
        {
            InfoUpdate(skill.CurrentCastTime, skill.CastTime);
            yield return null;  
        }

        gameObject.SetActive(false);
    }

    private IEnumerator ChargingProgressUpdate(Skill skill) 
    {
        var defaultColor = fillImage.color;

        // whlile 문으로 스킬이 Charging 상태 Check
        while (skill.IsInState<ChargingState>())
        {
            InfoUpdate(skill.CurrentChargeDuration, skill.ChargeDuration);
            // 스킬이 최소 충전량에 도달했다면 fillImage의 Color를 노란색으로 바꿔준다. 
            // → 스킬이 가용 가능한 상태라는 걸 시각적으로 보여준다. 
            if (skill.IsMinChargeCompleted)
                fillImage.color = Color.yellow;

            yield return null;
        }

        // Charging이 끝나면 fillImage의 Color를 원래 Color로 돌려주고 gameObject 꺼주기 
        fillImage.color = defaultColor;
        gameObject.SetActive(false);
    }
}
