using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StatScaleFloat // float값 1개와 Stat값 1개를 받아서 둘의 값을 곱해서 return 해주는 구조체 
{
    public float defaultValue;
    public Stat scaleStat; // defaultValue에 적용할 percent stat

    public float GetValue(Stats stats)
    {
        // defaultValue에 scaleStat을 곱한 값을 return 
        // ex) stat.Value가 0.5라면 defaultValue는 1.5배가 되서 return한다. 
        if (scaleStat && stats.TryGetStat(scaleStat, out var stat))
            return defaultValue * (1 + stat.Value);
        // 그냥 defaultValue 값을 return     
        else
            return defaultValue;
    }
}
