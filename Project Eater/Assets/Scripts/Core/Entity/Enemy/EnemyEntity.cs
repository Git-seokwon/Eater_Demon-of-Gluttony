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

    [Space(10)]
    [SerializeField]
    private float dnaProbability;

    public EnemyMovement EnemyMovement {  get; private set; }

    public MonoStateMachine<EnemyEntity> StateMachine { get; private set; }

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
        this.EnemyMovement.enabled = false;
        rigidbody.velocity = Vector2.zero;

        rigidbody.AddForce(direction * strength, ForceMode2D.Impulse);

        if (!IsDead)
            StartCoroutine(EndKnockback(duration)); // 예: 0.5초 후 넉백 종료
    }

    private IEnumerator EndKnockback(float duration)
    {
        yield return new WaitForSeconds(duration);

        rigidbody.velocity = Vector2.zero;
        this.EnemyMovement.enabled = true;
    }

    #region 몬스터 Item Drop
    private void DropItem(Entity entity)
    {
        PoolManager.Instance.ReuseGameObject(meat, transform.position, Quaternion.identity);

        if (ShouldDropDNA())
            DropMonsterDNA();
    }

    private bool ShouldDropDNA()
    {
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

    // IsInState 함수 Wrapping
    // → 외부에서 StateMachine Property를 거치지 않고 Entity를 통해 바로 현재 State를
    //    판별할 수 있도록 했다.
    public bool IsInState<T>() where T : State<EnemyEntity>
        => StateMachine.IsInState<T>();

    public bool IsInState<T>(int layer) where T : State<EnemyEntity>
    => StateMachine.IsInState<T>(layer);

    #region 충돌 데미지 
    private float crashDamage;

    private Coroutine crashDamageRoutine;
    private bool isPlayerInRange;
    private WaitForSeconds crashSeconds;

    private void Start()
    {
        // 몬스터 충돌 데미지는 기본 데미지에서 계산하기 때문에 처음 Start 함수에서 1회 계산한다. 
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

    private void GetAnger()
    {

    }
}
