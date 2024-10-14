using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SkillPrecedingAction : ICloneable
{
    // Start와 Release 함수는 가상 함수이기 때문에 꼭 구현해 줄 필요가 없지만,
    // PrecedingAction의 Logic에 해당하는 Run 함수는 꼭 구현해야 하는 함수이므로 추상 함수로 만듦
    public virtual void Start(Skill skill) { }   // PrecedingAction 시작
    public abstract bool Run(Skill skill);       // PrecedingAction Update
                                                 // → Run 함수가 true를 return할 때까지 Update 함수를 매 Frame마다 실행
                                                 // → true를 return하면 PrecedingAction이 완료했다는 의미로 Release 함수 실행하고
                                                 //    Action 상태로 넘어간다.
    public virtual void Release(Skill skill) { } // PrecedingAction 종료

    // PrecedingAction 설명
    protected virtual IReadOnlyDictionary<string, string> GetStringsByKeyword() => null;

    public virtual string BuildDescription(string description, int skillIndex)
    {
        description = TextReplacer.Replace(description, "precedingAction", GetStringsByKeyword(), skillIndex.ToString());

        return description;
    }

    public abstract object Clone(); 
}
