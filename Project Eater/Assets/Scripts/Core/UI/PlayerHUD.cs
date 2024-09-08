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

    // ���� EntityHUD�� �����ְ� �ִ� ���
    [SerializeField]
    private PlayerEntity target;

    private void OnDisable()
    {
        ReleaseEvents();
        effectListView.Target = null;
    }

    public void Show()
    {
        // ���� Target�� ����� Event���� ����
        ReleaseEvents();

        var stats = target.Stats;

        // HPStat�� SkillCostStat�� onValueChanged Event�� ���� Callback �Լ��� �����Ѵ�. 
        // �� Event�� ���ؼ� Stat ���� ��ȭ�� �Ͼ�� UI�� Update �ȴ�. 
        stats.FullnessStat.onValueChanged += OnFullnessStatChanged;
        stats.ExpStat.onValueChanged += OnExpStatChanged;

        // ���� ��ġ��� UI�� Update ���ش�. 
        UpdateStatView(stats.FullnessStat, fullnessFillImage, fullnessValueText);
        UpdateStatView(stats.ExpStat, expFillImage, expValueText);

        // effectListView�� Target���� target�� SkillSystem�� �����´�. 
        // �� effectListView�� SkillSystem�� Effect���� �����ͼ� ǥ�����ش�.
        effectListView.Target = target.SkillSystem;
    }

    private void UpdateStatView(Stat stat, Image statFillAmount, TextMeshProUGUI statText)
    {
        statFillAmount.fillAmount = stat.Value / stat.MaxValue;
        statText.text = $"{Mathf.RoundToInt(stat.Value)} / {stat.MaxValue}";
    }

    // target Entity�� ����ߴ� CallBack �Լ����� �������ִ� �Լ� 
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
