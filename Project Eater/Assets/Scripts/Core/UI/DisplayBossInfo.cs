using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayBossInfo : MonoBehaviour
{
    [SerializeField]
    private Image fullnessFillImage;
    [SerializeField]
    private TextMeshProUGUI bossNameText;

    // ���� DisplayBossInfo�� �����ְ� �ִ� ���
    private BossEntity target;

    private void OnDisable()
    {
        ReleaseEvents();
    }

    public void Show(BossEntity boss)
    {
        // ���� Target�� ����� Event���� ����
        ReleaseEvents();

        target = boss;
        var stats = target.Stats;

        // HPStat�� SkillCostStat�� onValueChanged Event�� ���� Callback �Լ��� �����Ѵ�. 
        // �� Event�� ���ؼ� Stat ���� ��ȭ�� �Ͼ�� UI�� Update �ȴ�. 
        stats.FullnessStat.onValueChanged += OnFullnessStatChanged;
        // ���� �̸� ǥ�� 
        bossNameText.text = target.BossName;

        // ���� ��ġ��� UI�� Update ���ش�. 
        UpdateStatView(stats.FullnessStat, fullnessFillImage);

        gameObject.SetActive(true);
    }

    private void UpdateStatView(Stat stat, Image statFillAmount)
        => statFillAmount.fillAmount = stat.Value / stat.MaxValue;

    // target Entity�� ����ߴ� CallBack �Լ����� �������ִ� �Լ� 
    private void ReleaseEvents()
    {
        if (!target)
            return;

        target.Stats.FullnessStat.onValueChanged -= OnFullnessStatChanged;
        target = null;
    }

    private void OnFullnessStatChanged(Stat stat, float currentValue, float prevValue)
    => UpdateStatView(stat, fullnessFillImage);
}
