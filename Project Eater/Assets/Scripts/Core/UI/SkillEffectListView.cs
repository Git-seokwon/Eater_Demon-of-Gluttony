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
    // ���� ����Ʈ ǥ�� ���� ���� 
    [SerializeField]
    private Transform buffEffectView; 
    // ����� ����Ʈ ǥ�� ���� ���� 
    [SerializeField]
    private Transform deBuffEffectView; 

    // UI�� ǥ���� Effect�� ���� SkillSystem ���� 
    private SkillSystem target;

    // ���� ������ SkillEffectView���� �����ϴ� List ����
    private List<SkillEffectView> effectViews = new();

    public SkillSystem Target
    {
        get => target;
        set
        {
            if (target == value) return;

            // ���� target�� ��ϵ� CallBack �Լ����� ��� ���� 
            ReleaseEvents();
            DestroyEffectViews();

            target = value;
            if (target == null)
                return;

            target.onEffectStarted += OnBuffEffectStarted;
            target.onEffectStarted += OnDeBuffEffectStarted;
            target.onEffectReleased += OnEffectReleased;

            // Target�� ���� ���� Effect���� foreach ������ ���鼭 Release ���°� �ƴ϶�� 
            // OnEffectStarted �Լ��� EffectView�� �����. 
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

        // Effect�� IsShowInUI�� true�̸� SkillEffectView�� �ϳ� ���� �ش� Effect�� Setup 
        var effectView = Instantiate(effectViewPrefab, buffEffectView).GetComponent<SkillEffectView>();
        effectView.Setup(effect);

        effectViews.Add(effectView);
    }

    private void OnDeBuffEffectStarted(SkillSystem skillSystem, Effect effect)
    {
        if (!(effect.IsShowInUI && effect.Type == EffectType.Debuff))
            return;

        // Effect�� IsShowInUI�� true�̸� SkillEffectView�� �ϳ� ���� �ش� Effect�� Setup 
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
