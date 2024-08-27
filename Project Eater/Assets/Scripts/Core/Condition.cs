using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� Condition : ���� ���� ���� Module�� ����� ���� �߻� Class
public abstract class Condition<T> : ICloneable
{
    // ���� ��� ���θ� ��ȯ�ϴ� �Լ� 
    public abstract bool IsPass(T data);

    public abstract object Clone();
}
