using System.Collections;
using UnityEngine;

public class Meet : MonoBehaviour
{
    // 체력 회복 수치
    [SerializeField]
    private float healFullness;
    // 엘리트 몬스터의 고기 Flag
    [SerializeField]
    private bool isElite;

    private Transform playerTransform;
    private Vector2 bounceTarget;
    private Coroutine currentCoroutine;

    private void OnEnable()
    {
        // 플레이어의 자석 스텟으로 콜라이더 반지름 설정하기 
        GetComponent<CircleCollider2D>().radius = GameManager.Instance.player.Stats.AbsorptionStat.Value;

        StageManager.Instance.onDeActivateItem += Deactivate;
    }

    private void OnDisable()
    {
        if (currentCoroutine != null)
            StopAllCoroutines();
        currentCoroutine = null;

        StageManager.Instance.onDeActivateItem -= Deactivate;
    }

    private IEnumerator BounceAndMoveToPlayer(PlayerEntity player, Stats stats)
    {
        var playerRenderer = player.GetComponent<SpriteRenderer>();

        // ※ 튀는 연출 
        // 플레이어 가슴 위치 구하기 
        // ★ player.GetComponent<SpriteRenderer>().bounds.size.y * 0.5f : 플레이어의 SpriteRenderer 높이의 절반
        // → bounds.size.y를 이용해 플레이어 모델의 높이를 계산하고 이를 기준으로 Vector3.up 방향으로 0.5배를 한다. 
        Vector3 playerChestPosition = playerTransform.position + Vector3.up * (playerRenderer.bounds.size.y * 0.5f);

        // ※ transform.position - playerTransform.position : 플레이어 → 고기 방향
        bounceTarget = transform.position + (transform.position - playerChestPosition).normalized * Settings.bounceDistance;
        float elapsedTime = 0f;

        while (elapsedTime < Settings.bounceDuration)
        {
            if (player.IsDead)
                yield break;

            transform.position = Vector2.Lerp(transform.position, bounceTarget, elapsedTime / Settings.bounceDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        var closeEnoughSqrDistance = Settings.itemCloseEnoughDistance * Settings.itemCloseEnoughDistance;
        playerChestPosition = UpdatePlayerPosition(playerChestPosition, playerRenderer);
        // 플레이어 쪽으로 이동 
        while ((transform.position - playerChestPosition).sqrMagnitude > closeEnoughSqrDistance)
        {
            if (player.IsDead)
                yield break;

            Vector2 direction = (playerChestPosition - transform.position).normalized;
            transform.position += (Vector3)(direction * Settings.itemMoveSpeed * Time.deltaTime);

            playerChestPosition = UpdatePlayerPosition(playerChestPosition, playerRenderer);
            yield return null;
        }

        // 고기를 섭취하면 플레이어의 체력(FullnessStat)이 상승
        stats.IncreaseDefaultValue(stats.FullnessStat, healFullness);

        // onGetMeat Event 발생
        player.OnGetMeat();

        // 플레이어 경험치 상승
        GameManager.Instance.GetExp(isElite);

        gameObject.SetActive(false);
    }
   
    private Vector3 UpdatePlayerPosition(Vector3 playerChestPosition, SpriteRenderer playerRenderer)
    {
        playerTransform = GameManager.Instance.player.transform;
        playerChestPosition = playerTransform.position +
            Vector3.up * (playerRenderer.bounds.size.y * 0.5f);

        return playerChestPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag)
        {
            if (currentCoroutine != null)
                return;

            var player = collision.GetComponent<PlayerEntity>();
            var stats = player.Stats;

            playerTransform = player.transform;

            currentCoroutine = StartCoroutine(BounceAndMoveToPlayer(player, stats));
        }
    }

    private void Deactivate()
    {
        Debug.Log("Deactivate 실행");
        gameObject.SetActive(false);
    }
}
