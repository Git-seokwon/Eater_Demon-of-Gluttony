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

    // 현재 DisplayBossInfo가 보여주고 있는 대상
    private BossEntity target;

    private void OnDisable()
    {
        ReleaseEvents();
    }

    public void Show(BossEntity boss)
    {
        // 기존 Target에 등록한 Event들을 해제
        ReleaseEvents();

        target = boss;
        var stats = target.Stats;

        // HPStat과 SkillCostStat의 onValueChanged Event에 각각 Callback 함수를 연결한다. 
        // → Event에 의해서 Stat 값의 변화가 일어나면 UI가 Update 된다. 
        stats.FullnessStat.onValueChanged += OnFullnessStatChanged;
        // 보스 이름 표시 
        bossNameText.text = target.BossName;

        // 현재 수치들로 UI를 Update 해준다. 
        UpdateStatView(stats.FullnessStat, fullnessFillImage);

        gameObject.SetActive(true);
    }

    private void UpdateStatView(Stat stat, Image statFillAmount)
        => statFillAmount.fillAmount = stat.Value / stat.MaxValue;

    // target Entity에 등록했던 CallBack 함수들을 해제해주는 함수 
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
