using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatOverride
{
    [SerializeField]
    private Stat stat; // Target Stat
    [SerializeField]
    private bool isUseOverride; // defaultValue�� ������� ���� ���� 
    [SerializeField]
    private float overrideDefaultValue; // ��� ��
    // �� isUseOverride�� true�̸� Stat�� defaultValue�� overrideDefaultValue�� Setting

    // ���ڷ� stat�� �޴� ������ 
    public StatOverride(Stat stat)
        => this.stat = stat;

    // ���� ��� Stat�� ������ִ� �Լ� 
    public Stat CreateStat()
    {
        // stat �纻 ���� 
        // �� Instantiate�� �ν��Ͻ��� ����� ������ ID �� ���� ���� ����ȴ�. 
        var newStat = stat.Clone() as Stat;

        // ����� ���
        if (isUseOverride)
            newStat.DefaultValue = overrideDefaultValue; // �纻 Stat�� DefalutValue�� overrideDefaultValue�� ���� 

        return newStat;
    }
}
