using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[System.Serializable]
public struct SkillInfo
{
    [SerializeField]
    private int tier;
    [SerializeField]
    private int index;

    public int Tier => tier;
    public int Index => index;  
}

public class MonsterDNA : MonoBehaviour
{
    [SerializeField]
    private SkillInfo[] skillInfos;
    [SerializeField]
    private int id;

    public int Id => id;

    private Transform playerTransform;
    private Vector2 bounceTarget;
    private Coroutine currentCoroutine;

    private void OnEnable()
    {
        // �÷��̾��� �ڼ� �������� �ݶ��̴� ������ �����ϱ� 
        GetComponent<CircleCollider2D>().radius = GameManager.Instance.player.Stats.AbsorptionStat.Value;

        StageManager.Instance.onDeActivateItem += Deactivate;
    }

    private void OnDisable()
    {
        StageManager.Instance.onDeActivateItem -= Deactivate;
    }

    private IEnumerator BounceAndMoveToPlayer(PlayerEntity player)
    {
        var playerRenderer = player.GetComponent<SpriteRenderer>();

        // �� Ƣ�� ���� 
        // �÷��̾� ���� ��ġ ���ϱ� 
        // �� player.GetComponent<SpriteRenderer>().bounds.size.y * 0.5f : �÷��̾��� SpriteRenderer ������ ����
        // �� bounds.size.y�� �̿��� �÷��̾� ���� ���̸� ����ϰ� �̸� �������� Vector3.up �������� 0.5�踦 �Ѵ�. 
        Vector3 playerChestPosition = playerTransform.position + Vector3.up * (playerRenderer.bounds.size.y * 0.5f);

        // �� transform.position - playerTransform.position : �÷��̾� �� ��� ����
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
        // �÷��̾� ������ �̵� 
        while ((transform.position - playerChestPosition).sqrMagnitude > closeEnoughSqrDistance)
        {
            if (player.IsDead)
                yield break;

            Vector2 direction = (playerChestPosition - transform.position).normalized;
            transform.position += (Vector3)(direction * Settings.itemMoveSpeed * Time.deltaTime);

            playerChestPosition = UpdatePlayerPosition(playerChestPosition, playerRenderer);
            yield return null;
        }

        foreach (var skillInfo in skillInfos)
        {
            player.SkillSystem.AddAcquirableSkills(skillInfo.Tier, skillInfo.Index);
            EventNotice.Instance.OnRegisterDNA(skillInfo.Tier, skillInfo.Index);
        }

        gameObject.SetActive(false);

        currentCoroutine = null;
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
            playerTransform = player.transform;
            currentCoroutine = StartCoroutine(BounceAndMoveToPlayer(player));
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
