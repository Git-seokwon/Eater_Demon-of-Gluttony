using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyCCState : State<EnemyEntity>
{
    // ���� ������ ���� or �̸� 
    public abstract string Description { get; }
}
