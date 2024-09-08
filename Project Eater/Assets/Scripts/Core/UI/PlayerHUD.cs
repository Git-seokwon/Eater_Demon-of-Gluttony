using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : SingletonMonobehaviour<PlayerHUD>  
{
    [Header("Stat View")]
    [SerializeField]
    private Image fullnessFillImage;
    [SerializeField]
    private TextMeshProUGUI fullnessValueText;
    [SerializeField]
    private Image expFillImage;
    [SerializeField]
    private TextMeshProUGUI expValueText;

    [Header("Effect List View")]
    [SerializeField]
    private SkillEffectListView effectListView;

    // 현재 EntityHUD가 보여주고 있는 대상
    [SerializeField]
    private PlayerEntity target;

    private void OnDisable()
    {
        ReleaseEvents();
        effectListView.Target = null;
    }

    public void Show()
    {
        // 기존 Target에 등록한 Event들을 해제
        ReleaseEvents();

        var stats = target.Stats;

        // HPStat과 SkillCostStat의 onValueChanged Event에 각각 Callback 함수를 연결한다. 
        // → Event에 의해서 Stat 값의 변화가 일어나면 UI가 Update 된다. 
        stats.FullnessStat.onValueChanged += OnFullnessStatChanged;
        stats.ExpStat.onValueChanged += OnExpStatChanged;

        // 현재 수치들로 UI를 Update 해준다. 
        UpdateStatView(stats.FullnessStat, fullnessFillImage, fullnessValueText);
        UpdateStatView(stats.ExpStat, expFillImage, expValueText);

        // effectListView의 Target으로 target의 SkillSystem을 가져온다. 
        // → effectListView가 SkillSystem의 Effect들을 가져와서 표시해준다.
        effectListView.Target = target.SkillSystem;
    }

    private void UpdateStatView(Stat stat, Image statFillAmount, TextMeshProUGUI statText)
    {
        statFillAmount.fillAmount = stat.Value / stat.MaxValue;
        statText.text = $"{Mathf.RoundToInt(stat.Value)} / {stat.MaxValue}";
    }

    // target Entity에 등록했던 CallBack 함수들을 해제해주는 함수 
    private void ReleaseEvents()
    {
        if (!target)
            return; 

        target.Stats.FullnessStat.onValueChanged -= OnFullnessStatChanged;
        target.Stats.ExpStat.onValueChanged -= OnExpStatChanged;
    }


    private void OnFullnessStatChanged(Stat stat, float currentValue, float prevValue)
        => UpdateStatView(stat, fullnessFillImage, fullnessValueText);

    private void OnExpStatChanged(Stat stat, float currentValue, float prevValue)
        => UpdateStatView(stat, expFillImage, expValueText);
}
