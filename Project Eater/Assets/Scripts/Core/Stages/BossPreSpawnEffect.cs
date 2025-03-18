using UnityEngine;

public abstract  class BossPreSpawnEffect : MonoBehaviour
{
    public event System.Action OnBossSpawnRequested; // 보스 소환 요청 이벤트

    public abstract void PlayEffect();

    protected void RequestBossSpawn()
    {
        OnBossSpawnRequested?.Invoke(); // 이벤트 호출 (구독자가 있으면 실행)
    }
}
