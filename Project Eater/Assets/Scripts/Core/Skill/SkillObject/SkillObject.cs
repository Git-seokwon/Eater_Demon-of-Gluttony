using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObject : MonoBehaviour
{
    // Skill의 첫 적용에 ApplyCycle 시간만큼 Delay를 줄 것인가?
    // Ex) ApplyCycle = 0.5, 0.5초 뒤부터 Skill 적용 시작
    [SerializeField]
    private bool isDelayFirstApplyByCycle;
    // SkillObject가 Destroy되는 시간에 ApplyCycle만큼 지연을 줄 것인가?
    // Ex) ApplyCycle = 0.5, 원래라면 3초에 Skill을 다 적용하고 Destroy 해야하지만 3.5초에 Destroy함
    [SerializeField]
    private bool isDelayDestroyByCycle;

    [SerializeField]
    private bool isSearchOnApply;

    private float currentDuration;
    private float currentApplyCycle;
    private int currentApplyCount;

    private TargetSearcher targetSearcher;

    // Skill의 소유주
    public Entity Owner { get; private set; }
    // 해당 SkillObject를 Spawn한 Skill
    public Skill Spawner { get; private set; }
    // SkillObject가 Skill을 적용할 Target을 찾기위한 TargetSearcher
    public TargetSearcher TargetSearcher => targetSearcher;
    // SkillObject의 Transform Scale
    public Vector2 ObjectScale { get; private set; }

    public float Duration { get; private set; }
    public int ApplyCount { get; private set; }
    public float ApplyCycle { get; private set; }

    // SkillObject가 Destroy되는 시간
    public float DestroyTime { get; private set; }

    // Skill 적용이 가능한가? 
    // ※ ApplyCount가 0이면 Duration 동안 계속 반복한다. 
    private bool IsApplicable => (ApplyCount == 0 || currentApplyCount < ApplyCount) &&
        currentApplyCycle >= ApplyCycle;
    private bool isStart;

    public void SetUp(Skill spawner, TargetSearcher targetSearcher, float duration, int applyCount, Vector2 objectScale)
    {
        // spawner의 경우 인자로 받은 spawner의 Clone을 Setting
        // → SkillObject를 Spawn 시킨 Skill의 Level 정보를 그대로 저장하기 위함 (새로 생성되면 )
        Spawner = spawner.Clone() as Skill;
        Owner = spawner.Owner;
        // targetSearcher도 SkillObject에 종속시키기 위해서 복사 생성자로 Copy를 만들어 할당한다. 
        this.targetSearcher = new TargetSearcher(targetSearcher);
        Duration = duration;
        ApplyCount = applyCount;
        ObjectScale = objectScale;
        ApplyCycle = CalculateApplyCycle(duration, applyCount);
        // DestroyTime 기본은 지속 시간이 끝났을 때 파괴하도록 Duration 값으로 설정, isDelayDestroyByCycle Check 여부에 따라 
        // ApplyCycle를 더해준다. 
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
        // ApplyCount가 1이면, Cycle이 필요 없으니 0을 return 
        if (applyCount == 1)
            return 0f;
        // isDelayFirstApplyByCycle가 false이면 처음 한 번은 바로 적용되기 때문에 duration / (applyCount - 1) 값을 return 한다. 
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
