using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cooldown시 전이되는 State
public class CooldownState : State<Skill>
{
    public override void Enter()
    {
        // ※ CoolDown일 때, 스킬 비활성화 
        // CooldownState가 속한 Layer가 0번이고, Activate 상태라면 Deactivate 함수를 실행 
        // → 0번 Layer가 Mian Layer고 0번 Layer의 CurrentState가 CooldownState가 되면, Skill의 Deactiverk 실행된다는 뜻 
        // → SkillState의 경우(Instant, Toggle ...), Skill 사용과 관련된 State는 모두 0번 Layer에서 실행 
        //    (ToggleSkillState의 경우에는 1번 Layer도 사용한다)
        if (Layer == 0 && Entity.IsActivated)
            Entity.Deactivate();

        // ※ CoolDown 설정 
        // 현재의 Skill의 Cooldown이 0초라면, 현재 Cooldown을 SKill의 기본 Cooldown 값으로 설정 
        if (Entity.IsCooldownCompleted)
            Entity.CurrentCooldown = Entity.Cooldown;
    }

    // ※ Cooldown
    // → 전이 조건에 의해서 CurrentCooldown이 0초가 되면, ReadyStaet로 전이 된다. 
    public override void Update() => Entity.CurrentCooldown -= Time.deltaTime;
}
