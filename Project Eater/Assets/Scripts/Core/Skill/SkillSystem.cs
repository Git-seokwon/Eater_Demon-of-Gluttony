using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class SkillSystem : MonoBehaviour
{
    // 초기화 함수 : SkillSystem의 소유자를 Setting 
    public void Setup(Entity entity)
    {

    }

    // Effect를 Owner에게 적용시키는 함수 
    // → 이미 적용된 Effect가 있다면, Effect Option에 따라 다른 작업 혹은 추가적인 작업을 한다. 
    // → ApplyNewEffect 함수는 private 함수이고 Apply 함수는 public 함수이기 때문에 기본적으로 Effect를 적용시킬 때는 이 함수를 쓴다. 
    public void Apply(Effect effect)
    {

    }

    // Effect List를 인자로 받는 Apply Overloading 함수 
    public void Apply(IReadOnlyList<Effect> effects)
    {
        foreach (var effect in effects)
            Apply(effect);
    }

    // Skill을 인자로 받는 Apply Overloading 함수 
    public void Apply(Skill skill)
    {
        Apply(skill.Effects);
    }

    // 실행 중인 모든 Skill을 취소해주는 함수 
    // 1) isForce : 강제로 스킬을 취소할 지 여부 
    public void CancleAll(bool isForce = false)
    {

    }
}
