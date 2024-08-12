using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillData : MonoBehaviour
{
    // ��ų�� ���� ���� ���� ��Ÿ���� Option
    [UnderlineTitle("Setting")]
    public SkillRunningFinishOption runningFinishOption;

    // �� Skill ���� �ð�
    // �� runningFinishOption�� FinishWhenDurationEnded�̰�, duration�� 0�̸� ���� ���� 
    [Min(0f)]
    public float duration;

    // �� Skill�� ����� Ƚ�� 
    // �� applyCount�� 0�̸� ���� ���� 
    [Min(0)]
    public int applyCount;

    // �� ApplyCount�� 1���� Ŭ ��, Apply�� ������ �ֱ�
    // �� ù �ѹ��� ȿ���� �ٷ� ����� ���̱� ������, �ѹ� ����� �ĺ��� ApplyCycle �ð��� ������ �����
    //    ���� ��, ApplyCycle�� 1�ʶ��, �ٷ� �ѹ� ����� �� 1�ʸ��� ����ǰ� ��.
    [Min(0f)]
    public float applyCycle;

    // Skill�� ���� ����� ã�� ���� Module
    [UnderlineTitle("Target Searcher")]
    public TargetSearcher targetSearcher;

    // Entity�� InSkillActionState�� ���� ���� ���� ��Ÿ���� Option
    [UnderlineTitle("Animation")]
    public InSkillActionFinishOption inSkillActionFinishOption;
}
