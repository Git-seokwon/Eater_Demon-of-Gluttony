using UnityEngine;

public abstract  class BossPreSpawnEffect : MonoBehaviour
{
    public event System.Action OnBossSpawnRequested; // ���� ��ȯ ��û �̺�Ʈ

    public abstract void PlayEffect();

    protected void RequestBossSpawn()
    {
        OnBossSpawnRequested?.Invoke(); // �̺�Ʈ ȣ�� (�����ڰ� ������ ����)
    }
}
