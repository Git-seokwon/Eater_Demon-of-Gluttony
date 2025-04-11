using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class EffectAction : ICloneable // Clone 함수로 Module을 복제하기 위해서 ICloneable 인터페이스를 상속 
{
    // Effect가 시작될 때, 호출되는 시작 함수 
    // → 필요할 때만 구현하면 되기 때문에 가상 함수로 만듬
    // ※ effect : Action을 소유하고 있는 Effect 
    // ※ user : Effect를 사용한 Entity
    // ※ target : Effect가 적용될 Entity
    // ※ level : Effect의 level
    // ※ scale : Effect의 위력을 조절하는 용도 → 주로, 일정 시간 충전해서 쓰는 Charge Skill에 사용
    public virtual void Start(Effect effect, Entity user, Entity target, int level, float scale) { }

    // 실제 Effect의 효과를 구현하는 함수 
    // ※ stack : Effect의 현재 Stack 수 
    public abstract bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale);

    // Effect가 종료될 때, 호출되는 종료 함수 
    // → 필요할 때만 구현하면 되기 때문에 가상 함수로 만듬
    public virtual void Release(Effect effect, Entity user, Entity target, int level, float scale) { }

    // Effect의 Stack이 바뀌었을 때, 호출되는 함수 
    // → Stack이 바뀌었을 때, 할 작업을 작성하면 된다. 
    // ex) Stack마다 힘 10을 증가시켜주는 효과라면, 새로운 Stack 값에 맞춰 증가시킨 힘을 수정해주면 된다. 
    // Stack마다 Bonus 값을 주는 Action일 경우, 이 함수를 통해서 Bonus 값을 새로 갱신할 수 있다. 
    public virtual void OnEffectStackChanged(Effect effect, Entity user, Entity target, int level, int stack, float scale) { }

    protected virtual IReadOnlyDictionary<string, string> GetStringByKeyword(Effect effect) => null;

    // Effect의 설명인 Description Text를 GetStringByKeyword 함수를 통해 만든 Dictionary로 Replace 작업을 하는 함수 
    // ※ effect : Action을 소유한 Effect
    // ※ description : Replace를 실행할 Text
    // ※ stackActionIndex : Stack마다 적용할 효과가 여러 개인 경우, 배열처럼 각 효과마다 Index 번호(stackActionIndex)를 부여
    // ※ stack : Action이 몇 Stack의 Action인지 나타내는 변수 
    // → Stack형 스킬이 아닌 경우, stack 값은 0이 된다. 
    // ※ effectIndex : Skill이 가진 여러 Effect 중에 이 Action을 소유한 Effect가 몇 번째 Effect 인지를 나타낸다.
    public string BuildDescription(Effect effect, string description, int skillIndex, int stackActionIndex, int stack, int effectIndex)
    {
        // Replace Data가 든 Dictionary를 가져오기 
        var stringByKeyword = GetStringByKeyword(effect);
        // Replace Data가 없으면 description을 그대로 출력 
        if (stringByKeyword == null)
            return description;

        // Stack형 스킬이 아닌 경우 
        if (stack == 0)
            // ※ prefix : "effectAction"
            // ※ suffix : Effect의 Index 값 (Skill에서 몇 번째 Index인가?)
            // Ex) description = "적에게 $[EffectAction.defaultDamage.0] 피해를 줍니다."
            // defaultDamage = 300, effectIndex = 0 → stringsByKeyword = new() { { "defaultDamage", defaultDamage.ToString() } };
            //                                                                            KEY                   VALUE
            // description.Replace("$[EffectAction.defaultDamage.0]", "300") => "적에게 300 피해를 줍니다."
            description = TextReplacer.Replace(description, skillIndex + ".effectAction", stringByKeyword, effectIndex.ToString());
        else
            // Ex) Mark = $[0.effectAction.defaultDamage.StackActionIndex.Stack.EffectIndex]
            description = TextReplacer.Replace(description, skillIndex + ".effectAction", stringByKeyword, $"{stackActionIndex}.{stack}.{effectIndex}");

        return description;
    }

    // ※ Prototype Pattern : 자식 객체가 부모 Class Type으로 Upcasting한 상태에서 원래 자료형을 몰라도 복사본을 생성할 수 있다.
    // → EffecAction을 상속받는 자식 Class 객체의 복사본을 만들어 return 해주는 함수 
    public abstract object Clone(); 
}
