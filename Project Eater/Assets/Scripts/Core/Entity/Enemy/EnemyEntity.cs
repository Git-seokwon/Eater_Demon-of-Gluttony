using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public EnemyMovement EnemyMovement {  get; private set; }

    public MonoStateMachine<EnemyEntity> StateMachine { get; private set; }

    private bool isKnockbackActive;

    protected override void Awake()
    {
        base.Awake();

        crashSeconds = new WaitForSeconds(0.15f);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        onDead += DropItem;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        onDead -= DropItem;
    }

    protected override void Update()
    {
        base.Update();

        Debug.Log("ü�� : " + Stats.FullnessStat.Value);
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

    public void ApplyKnockback(Vector3 direction, float strength, float duration)
    {
        isKnockbackActive = true;
        this.EnemyMovement.enabled = false;
        rigidbody.velocity = Vector2.zero;

        rigidbody.AddForce(direction * strength, ForceMode2D.Impulse);

        if (!IsDead)
            StartCoroutine(EndKnockback(duration)); // ��: 0.5�� �� �˹� ����
    }

    private IEnumerator EndKnockback(float duration)
    {
        yield return new WaitForSeconds(duration);

        isKnockbackActive = false;
        rigidbody.velocity = Vector2.zero;
        this.EnemyMovement.enabled = true;
    }

    #region ���� Item Drop
    private void DropItem(Entity entity)
    {
        PoolManager.Instance.ReuseGameObject(meat, transform.position, Quaternion.identity);

        switch (monsterGrade)
        {
            case MonsterGrade.Normal:
                // TODO : �Ϲ� ���ʹ� Ȯ���� DNA�� ����
                // if (GameManager.Instance.isHasDNA(monsterDNA.name) && )
                //     break;
         
                DropMonsterDNA();
                break;

            case MonsterGrade.Elite:
                if (GameManager.Instance.isHasDNA(monsterDNA.name))
                    break;
                
                DropMonsterDNA();
                break;

            default:
                break;
        }
    }

    private void DropMonsterDNA()
    {
        PoolManager.Instance.ReuseGameObject(monsterDNA, transform.position + new Vector3(0.1f, 0f, 0f),
                                             Quaternion.identity);

        GameManager.Instance.RecordDNADropped(monsterDNA.name);
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

    private void Start()
    {
        // ���� �浹 �������� �⺻ ���������� ����ϱ� ������ ó�� Start �Լ����� 1ȸ ����Ѵ�. 
        crashDamage = Stats.GetValue(Stats.DamageStat) / 2;
    }

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
}
