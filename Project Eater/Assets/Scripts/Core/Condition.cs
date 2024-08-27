using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ※ Condition : 여러 가지 조건 Module을 만들기 위한 추상 Class
public abstract class Condition<T> : ICloneable
{
    // 조건 통과 여부를 반환하는 함수 
    public abstract bool IsPass(T data);

    public abstract object Clone();
}
