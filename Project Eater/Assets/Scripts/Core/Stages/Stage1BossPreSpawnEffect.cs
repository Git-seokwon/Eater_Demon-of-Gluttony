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

    // �ִϸ��̼� �̺�Ʈ���� ȣ��
    private void OnBossSpawnAnimationEnd()
    {
        RequestBossSpawn(); // ���� ��ȯ ��û
    }
}
