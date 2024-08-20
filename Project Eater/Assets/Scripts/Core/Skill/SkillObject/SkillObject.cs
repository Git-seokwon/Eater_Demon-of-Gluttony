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

}
