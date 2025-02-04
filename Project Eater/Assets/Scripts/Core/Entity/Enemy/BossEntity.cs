using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEntity : Entity
{
    [SerializeField]
    private GameObject baal_GreatShard;
    [SerializeField]
    private GameObject meat;
    [SerializeField]
    private GameObject bossDNA;

    public BossMovement BossMovement { get; private set; }
    public MonoStateMachine<BossEntity> StateMachine { get; private set; }

    private Transform playerTransform;

    #region �浹 ������
    private float crashDamage;

    private Coroutine crashDamageRoutine;
    private bool isPlayerInRange;
    private WaitForSeconds crashSeconds;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        crashSeconds = new WaitForSeconds(0.15f);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        onDead += DropItem;
        // ������ ������ ���� ���ܵ� ��� �ٽ� �ش� ����� ���ش�. 
        BossMovement.enabled = true;
        Animator.speed = 1f;
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
        BossMovement = GetComponent<BossMovement>();
        BossMovement?.Setup(this);
    }

    protected override void StopMovement()
    {
        if (BossMovement)
            BossMovement.enabled = false;
    }

    protected override void SetUpStateMachine()
    {
        StateMachine = GetComponent<MonoStateMachine<BossEntity>>();
        StateMachine?.Setup(this);
    }

    public override void TakeDamage(Entity instigator, object causer, float damage, bool isTrueDamage = false, bool isTakeDamageEffect = true)
    {
        base.TakeDamage(instigator, causer, damage, isTrueDamage, isTakeDamageEffect);

        // �ǰ� ����Ʈ
        if (!IsDead)
            FlashEffect();
    }

    public void ApplyKnockback(Vector3 direction, float strength, float duration)
    {
        BossMovement.enabled = false;
        rigidbody.velocity = Vector2.zero;

        rigidbody.AddForce(direction * strength, ForceMode2D.Impulse);

        if (!IsDead)
            StartCoroutine(EndKnockback(duration)); // ��: 0.5�� �� �˹� ����
    }

    private IEnumerator EndKnockback(float duration)
    {
        yield return new WaitForSeconds(duration);

        rigidbody.velocity = Vector2.zero;
        BossMovement.enabled = true;
    }

    private void DropItem(Entity entity)
    {
        PoolManager.Instance.ReuseGameObject(baal_GreatShard, transform.position, Quaternion.identity);

        if (ShouldDropDNA())
            DropMonsterDNA();
    }

    private bool ShouldDropDNA()
    {
        if (bossDNA == null)
            return false;

        return GameManager.Instance.isHasLatentSkill(bossDNA.GetComponent<MonsterDNA>().Id);
    }

    private void DropMonsterDNA()
    {
        PoolManager.Instance.ReuseGameObject(bossDNA, transform.position + new Vector3(0.1f, 0f, 0f),
                                             Quaternion.identity);

        GameManager.Instance.RecordLatentSkillDropped(bossDNA.GetComponent<MonsterDNA>().Id);
    }

    private void UpdateDirection()
    {
        if (playerTransform == null)
            return;

        // �÷��̾� ��ġ�� ���� ��ġ�� X �� ��
        Sprite.flipX = playerTransform.position.x > transform.position.x;
    }

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

    // IsInState �Լ� Wrapping
    // �� �ܺο��� StateMachine Property�� ��ġ�� �ʰ� Entity�� ���� �ٷ� ���� State��
    //    �Ǻ��� �� �ֵ��� �ߴ�.
    public bool IsInState<T>() where T : State<BossEntity>
        => StateMachine.IsInState<T>();

    public bool IsInState<T>(int layer) where T : State<BossEntity>
    => StateMachine.IsInState<T>(layer);

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
            player.TakeDamage(this, null, crashDamage);

            yield return crashSeconds;
        }
    }

    // Dead Animation���� ȣ��
    private void DeActivate()
    {
        gameObject.SetActive(false);
    }
}
