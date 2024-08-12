using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillData : MonoBehaviour
{
    // 스킬을 언제 끝낼 지를 나타내는 Option
    [UnderlineTitle("Setting")]
    public SkillRunningFinishOption runningFinishOption;

    // ※ Skill 지속 시간
    // → runningFinishOption이 FinishWhenDurationEnded이고, duration이 0이면 무한 지속 
    [Min(0f)]
    public float duration;

    // ※ Skill이 적용될 횟수 
    // → applyCount가 0이면 무한 적용 
    [Min(0)]
    public int applyCount;

    // ※ ApplyCount가 1보다 클 때, Apply를 실행할 주기
    // → 첫 한번은 효과가 바로 적용될 것이기 때문에, 한번 적용된 후부터 ApplyCycle 시간이 지나고 적용됨
    //    예를 들어서, ApplyCycle이 1초라면, 바로 한번 적용된 후 1초마다 적용되게 됨.
    [Min(0f)]
    public float applyCycle;

    // Skill의 적용 대상을 찾기 위한 Module
    [UnderlineTitle("Target Searcher")]
    public TargetSearcher targetSearcher;

    // Entity의 InSkillActionState를 언제 끝낼 지를 나타내는 Option
    [UnderlineTitle("Animation")]
    public InSkillActionFinishOption inSkillActionFinishOption;
}
