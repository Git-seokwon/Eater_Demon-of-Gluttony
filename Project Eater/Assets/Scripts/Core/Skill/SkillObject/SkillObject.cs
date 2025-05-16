using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObject : MonoBehaviour
{
    // Skill�� ù ���뿡 ApplyCycle �ð���ŭ Delay�� �� ���ΰ�?
    // Ex) ApplyCycle = 0.5, 0.5�� �ں��� Skill ���� ����
    [SerializeField]
    private bool isDelayFirstApplyByCycle;
    // SkillObject�� Destroy�Ǵ� �ð��� ApplyCycle��ŭ ������ �� ���ΰ�?
    // Ex) ApplyCycle = 0.5, ������� 3�ʿ� Skill�� �� �����ϰ� Destroy �ؾ������� 3.5�ʿ� Destroy��
    [SerializeField]
    private bool isDelayDestroyByCycle;

    [SerializeField]
    private bool isSearchOnApply;

    private float currentDuration;
    private float currentApplyCycle;
    private int currentApplyCount;

    private TargetSearcher targetSearcher;

    // Skill�� ������
    public Entity Owner { get; private set; }
    // �ش� SkillObject�� Spawn�� Skill
    public Skill Spawner { get; private set; }
    // SkillObject�� Skill�� ������ Target�� ã������ TargetSearcher
    public TargetSearcher TargetSearcher => targetSearcher;
    // SkillObject�� Transform Scale
    public Vector2 ObjectScale { get; private set; }

    public float Duration { get; private set; }
    public int ApplyCount { get; private set; }
    public float ApplyCycle { get; private set; }

    // SkillObject�� Destroy�Ǵ� �ð�
    public float DestroyTime { get; private set; }

    // Skill ������ �����Ѱ�? 
    // �� ApplyCount�� 0�̸� Duration ���� ��� �ݺ��Ѵ�. 
    private bool IsApplicable => (ApplyCount == 0 || currentApplyCount < ApplyCount) &&
        currentApplyCycle >= ApplyCycle;
    private bool isStart;

    public void SetUp(Skill spawner, TargetSearcher targetSearcher, float duration, int applyCount, Vector2 objectScale)
    {
        // spawner�� ��� ���ڷ� ���� spawner�� Clone�� Setting
        // �� SkillObject�� Spawn ��Ų Skill�� Level ������ �״�� �����ϱ� ���� (���� �����Ǹ� )
        Spawner = spawner.Clone() as Skill;
        Owner = spawner.Owner;
        // targetSearcher�� SkillObject�� ���ӽ�Ű�� ���ؼ� ���� �����ڷ� Copy�� ����� �Ҵ��Ѵ�. 
        this.targetSearcher = new TargetSearcher(targetSearcher);
        Duration = duration;
        ApplyCount = applyCount;
        ObjectScale = objectScale;
        ApplyCycle = CalculateApplyCycle(duration, applyCount);
        // DestroyTime �⺻�� ���� �ð��� ������ �� �ı��ϵ��� Duration ������ ����, isDelayDestroyByCycle Check ���ο� ���� 
        // ApplyCycle�� �����ش�. 
        DestroyTime = Duration + (isDelayDestroyByCycle ? ApplyCycle : 0f);
        currentApplyCount = 0;
        currentDuration = currentApplyCycle = 0f;
        
        if (!isDelayFirstApplyByCycle)
        {
            if (!isSearchOnApply)
                Apply();
            else
                StartCoroutine(ApplySingleEffect());
        }

        isStart = true;
    }

    private void Update()
    {
        if (!isStart) return;

        currentDuration += Time.deltaTime;
        currentApplyCycle += Time.deltaTime;

        if (IsApplicable)
        {
            if (!isSearchOnApply)
                Apply();
            else
               StartCoroutine(ApplySingleEffect());
        }

        if (currentDuration >= DestroyTime)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        Clear();
    }

    public float CalculateApplyCycle(float duration, int applyCount)
    {
        // ApplyCount�� 1�̸�, Cycle�� �ʿ� ������ 0�� return 
        if (applyCount == 1)
            return 0f;
        // isDelayFirstApplyByCycle�� false�̸� ó�� �� ���� �ٷ� ����Ǳ� ������ duration / (applyCount - 1) ���� return �Ѵ�. 
        else
            return isDelayFirstApplyByCycle ? (duration / applyCount) : (duration / (applyCount - 1));
    }

    private void Apply()
    {
        targetSearcher.SelectImmediate(Owner, gameObject, transform.position);
        var result = targetSearcher.SearchTargets(Owner, gameObject);

        foreach (var target in result.targets)
            target.GetComponent<SkillSystem>().Apply(Spawner);

        currentApplyCount++;
        currentApplyCycle %= ApplyCycle;
    }

    private IEnumerator ApplySingleEffect()
    {;
        currentApplyCount++;
        currentApplyCycle %= ApplyCycle;

        foreach (var effect in Spawner.currentEffects)
        {
            if (effect != null && effect.CurrentLevelData.isRenewSearchingInSkillObject)
                yield return new WaitForSeconds(effect.ApplyCycle);

            targetSearcher.SelectImmediate(Owner, gameObject, transform.position);
            var result = targetSearcher.SearchTargets(Owner, gameObject);

            foreach (var target in result.targets)
                target.GetComponent<SkillSystem>().Apply(effect);
        }
    }

    private void Clear()
    {
        StopAllCoroutines();

        Spawner = null;
        Owner = null;
        targetSearcher = null;
        isStart = false;
    }
}
