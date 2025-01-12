using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatOverride
{
    [SerializeField]
    private Stat stat; // Target Stat
    [SerializeField]
    private bool isUseOverride; // defaultValue를 덮어쓸지에 대한 여부 
    [SerializeField]
    private float overrideDefaultValue; // 덮어쓸 값
    // → isUseOverride가 true이면 Stat의 defaultValue를 overrideDefaultValue로 Setting

    // 인자로 stat을 받는 생성자 
    public StatOverride(Stat stat)
        => this.stat = stat;

    // 값을 덮어쓴 Stat을 만들어주는 함수 
    public Stat CreateStat()
    {
        // stat 사본 생성 
        // → Instantiate로 인스턴스를 만들기 때문에 ID 등 변수 값이 복사된다. 
        var newStat = stat.Clone() as Stat;

        // 덮어쓰기 사용
        if (isUseOverride)
            newStat.DefaultValue = overrideDefaultValue; // 사본 Stat의 DefalutValue를 overrideDefaultValue로 설정 

        return newStat;
    }
}
