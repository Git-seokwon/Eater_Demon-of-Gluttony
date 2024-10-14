using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyCCState : State<EnemyEntity>
{
    // 현재 상태의 설명 or 이름 
    public abstract string Description { get; }
}
