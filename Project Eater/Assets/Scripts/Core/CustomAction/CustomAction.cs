using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CustomAction : ICloneable
{
    // ※ object data
    // → CustomAction을 상속받는 자식 Class에서 data의 Type에 따라 적절히 처리
    public virtual void Start(object data) { }
    public virtual void Run(object data) { } // Update 함수 
    public virtual void Release(object data) { }

    public abstract object Clone(); 
}
