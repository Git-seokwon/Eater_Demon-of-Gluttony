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
