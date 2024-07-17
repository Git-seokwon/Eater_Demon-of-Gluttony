using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CustomAction : ICloneable
{
    // �� object data
    // �� CustomAction�� ��ӹ޴� �ڽ� Class���� data�� Type�� ���� ������ ó��
    public virtual void Start(object data) { }
    public virtual void Run(object data) { } // Update �Լ� 
    public virtual void Release(object data) { }

    public abstract object Clone(); 
}
