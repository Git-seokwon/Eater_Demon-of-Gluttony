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

    // Player Entity�� SkillSystem�� ������ ����
    [SerializeField]
    private SkillSystem skillSystem;

    // �Ŀ� �������� ������ ��, SetUp�ϴ� �Լ��� �����ϱ� 
    private void Start()
    {
        skillSystem.onSkillStateChanged += OnSkillStateChanged;
        gameObject.SetActive(false);
    }

    private void InfoUpdate(float currentTime, float maxTime)
    {
        // fillImage�� �������� ���ֱ� 
        fillImage.fillAmount = currentTime / maxTime;
        // TimeText�� (���� �ð� / �ִ� �ð�)���� ���� 
        timeText.text = $"{currentTime:F1} : {maxTime:F1}";
    }

    private void OnSkillStateChanged(SkillSystem skillSystem, Skill skill, 
        State<Skill> newState, State<Skill> prevState, int layer)
    {
        // ��ų CastingState�� �� 
        if (skill.IsInState<CastingState>())
        {
            gameObject.SetActive(true);
            StartCoroutine("CastingProgressUpdate", skill);
        }
        // ��ų ChargingState�� �� 
        else if (skill.IsInState<ChargingState>())
        {
            gameObject.SetActive(true);
            StartCoroutine("ChargingProgressUpdate", skill);
        }
    }

    private IEnumerator CastingProgressUpdate(Skill skill)
    {
        // whlile ������ ��ų�� Casting ���� Check
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

        // whlile ������ ��ų�� Charging ���� Check
        while (skill.IsInState<ChargingState>())
        {
            InfoUpdate(skill.CurrentChargeDuration, skill.ChargeDuration);
            // ��ų�� �ּ� �������� �����ߴٸ� fillImage�� Color�� ��������� �ٲ��ش�. 
            // �� ��ų�� ���� ������ ���¶�� �� �ð������� �����ش�. 
            if (skill.IsMinChargeCompleted)
                fillImage.color = Color.yellow;

            yield return null;
        }

        // Charging�� ������ fillImage�� Color�� ���� Color�� �����ְ� gameObject ���ֱ� 
        fillImage.color = defaultColor;
        gameObject.SetActive(false);
    }
}
