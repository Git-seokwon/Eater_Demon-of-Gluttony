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

}
