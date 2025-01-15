using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public enum MonsterGrade
{
    Normal,
    Elite
}

public class EnemyEntity : Entity
{
    [SerializeField]
    private MonsterGrade monsterGrade;

    [SerializeField]
    private GameObject monsterDNA;
    [SerializeField]
    private GameObject meat;

    [Space(10)]
    [SerializeField]
    private float dnaProbability;

    public EnemyMovement EnemyMovement {  get; private set; }

    public MonoStateMachine<EnemyEntity> StateMachine { get; private set; }

    private Transform playerTransform;

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
        EnemyMovement.enabled = true;
        Animator.speed = 1f;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        onDead -= DropItem;
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

    protected override void FixedUpdate()
    {
        
    }

    protected override void SetUpMovement()
    {
        EnemyMovement = GetComponent<EnemyMovement>();
        EnemyMovement?.Setup(this);
    }

    protected override void StopMovement()
    {
        if (EnemyMovement)
            EnemyMovement.enabled = false;
    }

    protected override void SetUpStateMachine()
    {
        StateMachine = GetComponent<MonoStateMachine<EnemyEntity>>();
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
        EnemyMovement.enabled = false;
        rigidbody.velocity = Vector2.zero;

        rigidbody.AddForce(direction * strength, ForceMode2D.Impulse);

        if (!IsDead)
            StartCoroutine(EndKnockback(duration)); // ��: 0.5�� �� �˹� ����
    }

    private IEnumerator EndKnockback(float duration)
    {
        yield return new WaitForSeconds(duration);

        rigidbody.velocity = Vector2.zero;
        EnemyMovement.enabled = true;
    }

    #region ���� Item Drop
    private void DropItem(Entity entity)
    {
        PoolManager.Instance.ReuseGameObject(meat, transform.position, Quaternion.identity);

        if (ShouldDropDNA())
            DropMonsterDNA();
    }

    private bool ShouldDropDNA()
    {
        if (monsterDNA == null)
            return false;

        bool hasDNA = GameManager.Instance.isHasDNA(monsterDNA.GetComponent<MonsterDNA>().Id.ToString());

        switch (monsterGrade)
        {
            case MonsterGrade.Normal:
                return !hasDNA && Random.Range(0, 100) < Mathf.FloorToInt(dnaProbability * 100);

            case MonsterGrade.Elite:
                return !hasDNA;

            default:
                return false;
        }
    }

    private void DropMonsterDNA()
    {
        PoolManager.Instance.ReuseGameObject(monsterDNA, transform.position + new Vector3(0.1f, 0f, 0f),
                                             Quaternion.identity);

        GameManager.Instance.RecordDNADropped(monsterDNA.GetComponent<MonsterDNA>().Id.ToString());
    }
    #endregion

    // IsInState �Լ� Wrapping
    // �� �ܺο��� StateMachine Property�� ��ġ�� �ʰ� Entity�� ���� �ٷ� ���� State��
    //    �Ǻ��� �� �ֵ��� �ߴ�.
    public bool IsInState<T>() where T : State<EnemyEntity>
        => StateMachine.IsInState<T>();

    public bool IsInState<T>(int layer) where T : State<EnemyEntity>
    => StateMachine.IsInState<T>(layer);

    #region �浹 ������ 
    private float crashDamage;

    private Coroutine crashDamageRoutine;
    private bool isPlayerInRange;
    private WaitForSeconds crashSeconds;

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
        Sprite.flipX = playerTransform.position.x > transform.position.x;
    }

    // Dead Animation���� ȣ��
    private void DeActivate()
    {
        gameObject.SetActive(false);
    }
    
    public void GetAnger()
    {
        Debug.Log("ȭ�� ����");
    }

    protected override void OnDead()
    {
        base.OnDead();

        Debug.Log("�� �׾���");
    }
}
