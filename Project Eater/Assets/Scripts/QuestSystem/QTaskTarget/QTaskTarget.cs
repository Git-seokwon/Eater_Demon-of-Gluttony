using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QTaskTarget : ScriptableObject
{
    public abstract object Value { get; }

    public abstract bool IsEqual(object target); 
    // 원하는 target과 같은지 확인하는 메서드 -> target의 자료형에 따라 override 해줘야 하기 때문에 abstract.
}
