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
        currentApplyCycle < ApplyCycle;

    public void SetUp(Skill spawner, TargetSearcher targetSearcher, float duration, int applyCount, Vector2 objectScale)
    {
        Spawner = spawner;
        Owner = spawner.Owner;
        this.targetSearcher = new TargetSearcher(targetSearcher);
        ApplyCount = applyCount;
        ObjectScale = objectScale;
        ApplyCycle = CalculateApplyCycle(duration, applyCount);
        DestroyTime = Duration + (isDelayDestroyByCycle ? ApplyCycle : 0f);


    }

    public float CalculateApplyCycle(float duration, int applyCount)
    {
        if (applyCount == 1)
            return 0f;
        else
            return isDelayFirstApplyByCycle ? (duration / applyCount) : (duration / (applyCount - 1));
    }
}
