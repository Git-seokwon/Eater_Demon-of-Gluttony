using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1BossPreSpawnEffect : BossPreSpawnEffect
{
    [SerializeField]
    private Animator animator;

    public override void PlayEffect()
    {
        animator.SetBool("IsActive", true);
    }

    // 애니메이션 이벤트에서 호출
    private void OnBossSpawnAnimationEnd()
    {
        RequestBossSpawn(); // 보스 소환 요청
    }
}
