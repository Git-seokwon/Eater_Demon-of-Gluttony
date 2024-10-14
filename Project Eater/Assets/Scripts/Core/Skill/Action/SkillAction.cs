using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SkillAction : ICloneable
{
    // 전체적으로 SkillPrecedingAction 스크립트와 비슷함
    // → Run이 아닌 Apply 함수를 쓰는 점과 keyword로 skillAction을 쓰는 점만 다르다. + Awake 함수
    public virtual void Start(Skill skill) { }
    public abstract void Apply(Skill skill);
    public virtual void Release(Skill skill) { }

    protected virtual IReadOnlyDictionary<string, string> GetStringByKeyword() => null;
    public virtual string BuildDescription(string description, int skillIndex)
    {
        description = TextReplacer.Replace(description, "skillAction", GetStringByKeyword(), skillIndex.ToString());

        return description;
    }

    public abstract object Clone();
}
