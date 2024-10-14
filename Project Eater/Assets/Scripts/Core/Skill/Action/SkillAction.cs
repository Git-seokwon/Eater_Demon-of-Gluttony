using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SkillAction : ICloneable
{
    // ��ü������ SkillPrecedingAction ��ũ��Ʈ�� �����
    // �� Run�� �ƴ� Apply �Լ��� ���� ���� keyword�� skillAction�� ���� ���� �ٸ���. + Awake �Լ�
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
