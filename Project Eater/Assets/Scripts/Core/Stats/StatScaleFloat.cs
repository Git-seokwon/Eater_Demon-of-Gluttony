using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StatScaleFloat // float�� 1���� Stat�� 1���� �޾Ƽ� ���� ���� ���ؼ� return ���ִ� ����ü 
{
    public float defaultValue;
    public Stat scaleStat; // defaultValue�� ������ percent stat

    public float GetValue(Stats stats)
    {
        // defaultValue�� scaleStat�� ���� ���� return 
        // ex) stat.Value�� 0.5��� defaultValue�� 1.5�谡 �Ǽ� return�Ѵ�. 
        if (scaleStat && stats.TryGetStat(scaleStat, out var stat))
            return defaultValue * (1 + stat.Value);
        // �׳� defaultValue ���� return     
        else
            return defaultValue;
    }
}
