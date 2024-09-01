using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillState : State<EnemyEntity>
{
    // 현재 Entity가 실행 중인 Skill
    public Skill RunningSkill { get; private set; }


}
