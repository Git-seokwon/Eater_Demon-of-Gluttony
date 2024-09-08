using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillEffectListView : MonoBehaviour
{
    // Skill Effect View Prefab
    [SerializeField]
    private GameObject effectViewPrefab;
    // 버프 이펙트 표시 시작 지점 
    [SerializeField]
    private Transform buffEffectView; 
    // 디버프 이펙트 표시 시작 지점 
    [SerializeField]
    private Transform deBuffEffectView; 

    // UI에 표시할 Effect를 가진 SkillSystem 변수 
    private SkillSystem target;

    // 현재 생성된 SkillEffectView들을 저장하는 List 변수
    private List<SkillEffectView> effectViews = new();

    public SkillSystem Target
    {
        get => target;
        set
        {
            if (target == value) return;

            // 기존 target에 등록된 CallBack 함수들을 모두 해제 
            ReleaseEvents();
            DestroyEffectViews();

            target = value;
            if (target == null)
                return;

            target.onEffectStarted += OnBuffEffectStarted;
            target.onEffectStarted += OnDeBuffEffectStarted;
            target.onEffectReleased += OnEffectReleased;

            // Target에 적용 중인 Effect들을 foreach 문으로 돌면서 Release 상태가 아니라면 
            // OnEffectStarted 함수로 EffectView를 만든다. 
            foreach (var effect in target.RunningEffects)
            {
                if (!effect.IsReleased)
                {
                    OnBuffEffectStarted(target, effect);
                    OnDeBuffEffectStarted(target, effect);
                }
            }
        }
    }

    private void OnDestroy()
    {
        ReleaseEvents();
        DestroyEffectViews();
    }

    private void ReleaseEvents()
    {
        if (target == null)
            return;

        target.onEffectStarted -= OnBuffEffectStarted;
        target.onEffectStarted -= OnDeBuffEffectStarted;
        target.onEffectReleased -= OnEffectReleased;
    }

    private void DestroyEffectViews()
    {
        foreach (var effectView in effectViews)
            Destroy(effectView.gameObject);

        effectViews.Clear();
    }

    private void OnBuffEffectStarted(SkillSystem skillSystem, Effect effect)
    {
        if (!(effect.IsShowInUI && effect.Type == EffectType.Buff))
            return;

        // Effect의 IsShowInUI가 true이면 SkillEffectView를 하나 만들어서 해당 Effect로 Setup 
        var effectView = Instantiate(effectViewPrefab, buffEffectView).GetComponent<SkillEffectView>();
        effectView.Setup(effect);

        effectViews.Add(effectView);
    }

    private void OnDeBuffEffectStarted(SkillSystem skillSystem, Effect effect)
    {
        if (!(effect.IsShowInUI && effect.Type == EffectType.Debuff))
            return;

        // Effect의 IsShowInUI가 true이면 SkillEffectView를 하나 만들어서 해당 Effect로 Setup 
        var effectView = Instantiate(effectViewPrefab, deBuffEffectView).GetComponent<SkillEffectView>();
        effectView.Setup(effect);

        effectViews.Add(effectView);
    }

    private void OnEffectReleased(SkillSystem skillSystem, Effect effect)
    {
        int targetIndex = effectViews.FindIndex(x => x.Target == effect);
        if (targetIndex != -1) 
        {
            Destroy(effectViews[targetIndex].gameObject);
            effectViews.RemoveAt(targetIndex);
        }
    }
}
