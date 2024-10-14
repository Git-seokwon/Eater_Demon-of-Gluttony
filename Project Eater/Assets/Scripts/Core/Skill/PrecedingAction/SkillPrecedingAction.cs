using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SkillPrecedingAction : ICloneable
{
    // Start�� Release �Լ��� ���� �Լ��̱� ������ �� ������ �� �ʿ䰡 ������,
    // PrecedingAction�� Logic�� �ش��ϴ� Run �Լ��� �� �����ؾ� �ϴ� �Լ��̹Ƿ� �߻� �Լ��� ����
    public virtual void Start(Skill skill) { }   // PrecedingAction ����
    public abstract bool Run(Skill skill);       // PrecedingAction Update
                                                 // �� Run �Լ��� true�� return�� ������ Update �Լ��� �� Frame���� ����
                                                 // �� true�� return�ϸ� PrecedingAction�� �Ϸ��ߴٴ� �ǹ̷� Release �Լ� �����ϰ�
                                                 //    Action ���·� �Ѿ��.
    public virtual void Release(Skill skill) { } // PrecedingAction ����

    // PrecedingAction ����
    protected virtual IReadOnlyDictionary<string, string> GetStringsByKeyword() => null;

    public virtual string BuildDescription(string description, int skillIndex)
    {
        description = TextReplacer.Replace(description, "precedingAction", GetStringsByKeyword(), skillIndex.ToString());

        return description;
    }

    public abstract object Clone(); 
}
