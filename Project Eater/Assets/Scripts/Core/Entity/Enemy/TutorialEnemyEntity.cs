using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemyEntity : Entity
{
    public TutorialEnemyMovement EnemyMovement { get; private set; }

    public MonoStateMachine<TutorialEnemyEntity> StateMachine { get; private set; }

    private Transform playerTransform;

    protected override void Awake()
    {
        base.Awake();

        crashSeconds = new WaitForSeconds(0.15f);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private void Start()
    {
        playerTransform = GameManager.Instance.player.transform;

        // ���� �浹 �������� �⺻ ���������� ����ϱ� ������ ó�� Start �Լ����� 1ȸ ����Ѵ�. 
        crashDamage = Stats.GetValue(Stats.AttackStat) / 2;
    }

    protected override void Update()
    {
        base.Update();

        UpdateDirection();
    }

    protected override void SetUpMovement()
    {
        EnemyMovement = GetComponent<TutorialEnemyMovement>();
        EnemyMovement?.Setup(this);
    }

    public override void StopMovement()
    {
        rigidbody.velocity = Vector2.zero;

        if (EnemyMovement)
            EnemyMovement.enabled = false;
    }

    protected override void SetUpStateMachine()
    {
        StateMachine = GetComponent<MonoStateMachine<TutorialEnemyEntity>>();
        StateMachine?.Setup(this);
    }

    public override void TakeDamage(Entity instigator, object causer, float damage, bool isCrit,
        bool isHitImpactOn = true, bool isTrueDamage = false, bool isRealDead = true)
    {
        base.TakeDamage(instigator, causer, damage, isCrit, isHitImpactOn, isTrueDamage, isRealDead);

        Debug.Log("������ ���� : " + damage);

        // �ǰ� ����Ʈ
        if (!IsDead)
            FlashEffect();
    }

    public void ApplyKnockback(Vector3 direction, float strength, float duration)
    {
        if (IsDead) return;

        EnemyMovement.enabled = false;
        rigidbody.velocity = Vector2.zero;

        rigidbody.AddForce(direction * strength, ForceMode2D.Impulse);

        if (!IsDead)
            StartCoroutine(EndKnockback(duration));
    }

    private IEnumerator EndKnockback(float duration)
    {
        yield return new WaitForSeconds(duration);

        rigidbody.velocity = Vector2.zero;
        EnemyMovement.enabled = true;
    }

    public bool IsInState<T>() where T : State<TutorialEnemyEntity>
        => StateMachine.IsInState<T>();

    public bool IsInState<T>(int layer) where T : State<TutorialEnemyEntity>
    => StateMachine.IsInState<T>(layer);

    #region �浹 ������
    private float crashDamage;

    private Coroutine crashDamageRoutine;
    private bool isPlayerInRange;
    private WaitForSeconds crashSeconds;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsDead)
            return;

        if (collision.tag == Settings.playerTag)
        {
            isPlayerInRange = true;
            crashDamageRoutine = StartCoroutine(DealDamageOverTime(collision.GetComponent<Entity>()));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag)
        {
            isPlayerInRange = false;

            if (crashDamageRoutine != null)
            {
                StopCoroutine(crashDamageRoutine);
                crashDamageRoutine = null;
            }
        }
    }

    private IEnumerator DealDamageOverTime(Entity player)
    {
        while (isPlayerInRange)
        {
            player.TakeDamage(this, null, crashDamage, false, false);

            yield return crashSeconds;
        }
    }
    #endregion

    #region FlashWhite
    private void FlashEffect()
    {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        Sprite.material.SetInt("_Flash", 1);
        yield return new WaitForSeconds(0.2f);
        Sprite.material.SetInt("_Flash", 0);

    }
    #endregion

    private void UpdateDirection()
    {
        if (playerTransform == null)
            return;

        // �÷��̾� ��ġ�� ���� ��ġ�� X �� ��
        transform.localScale = playerTransform.position.x > transform.position.x
            ? new Vector2(-1, 1) : new Vector2(1, 1);
    }

    // Dead Animation���� ȣ��
    private void DeActivate()
    {
        gameObject.SetActive(false);
    }

    public override void OnDead(bool isRealDead = true)
    {
        base.OnDead(isRealDead);

        isPlayerInRange = false;
    }
}
