using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CC�⸦ ������ ��� �ൿ�� ������ ����ٰ� CC�� ������ �ٽ� ������ ����������. 
public abstract class PlayerCCState : State<PlayerEntity>
{
    // ���� ������ ���� or �̸� 
    public abstract string Description { get; }
}
