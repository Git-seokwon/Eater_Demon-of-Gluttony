using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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
    private bool isBig;

    [SerializeField]
    private GameObject monsterDNA;
    [SerializeField]
    private GameObject meat;

    [Space(10)]
    [SerializeField]
    private float dnaProbability;

    [Space(10)]
    [SerializeField]
    private GameObject bleedingEfffect;
    [SerializeField]
    private Transform bleedingFXPos;
    private GameObject bleedingEffectObject;

    public EnemyMovement EnemyMovement {  get; private set; }

    public MonoStateMachine<EnemyEntity> StateMachine { get; private set; }

    private Transform playerTransform;

    [HideInInspector] public bool IsHorizontalFlip = false;

    #region 스텟 보정 
    public float defaultHp { get; private set; }        // HP 디폴트 값 
    public float defaultAttack { get; private set; }    // Attack 디폴트 값 
    public float defaultDefence { get; private set; }   // Defence 디폴트 값 
    public float defaultMoveSpeed { get; private set; } // MoveSpeed 디폴트 값 
    #endregion

    protected override void Awake()
    {
        base.Awake();

        crashSeconds = new WaitForSeconds(0.15f);

        // 몬스터 스텟 디폴트 값 Setting
        defaultHp = Stats.GetValue(Stats.FullnessStat);
        defaultAttack = Stats.GetValue(Stats.AttackStat);
        defaultDefence = Stats.GetValue(Stats.DefenceStat);
        defaultMoveSpeed = Stats.GetValue(Stats.MoveSpeedStat);        
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        // 체력 정상화 
        // → OnEnable에 즉시 체력을 정상화 시키고, MonsterAI에서 체력 재조정을 한다. 
        // → 안하면 체력이 0인 상태로 부활하기에 태어나자마자 사망판정받음
        Stats.SetDefaultValue(Stats.FullnessStat, Stats.FullnessStat.MaxValue);
        StopMovement();

        // Animator 초기화 해준다. 
        if (Animator != null)
        {
            Animator.Rebind();  // 모든 상태 초기화 (변수, 레이어 등)
            Animator.Update(0); // 즉시 상태 반영
        }

        onDead += DropItem;
        Sprite.material.SetInt("_Flash", 0);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private void Start()
    {
        playerTransform = GameManager.Instance.player.transform;

        // 몬스터 충돌 데미지는 기본 데미지에서 계산하기 때문에 처음 Start 함수에서 1회 계산한다. 
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

    public override void StopMovement()
    {
        rigidbody.velocity = Vector2.zero;

        if (EnemyMovement)
            EnemyMovement.enabled = false;
    }

    protected override void SetUpStateMachine()
    {
        StateMachine = GetComponent<MonoStateMachine<EnemyEntity>>();
        StateMachine?.Setup(this);
    }

    public override void TakeDamage(Entity instigator, object causer, float damage, bool isCrit,
        bool isHitImpactOn = true, bool isTrueDamage = false, bool isReadDead = true)
    {
        base.TakeDamage(instigator, causer, damage, isCrit, isHitImpactOn, isTrueDamage, isReadDead);

        // 피격 이펙트
        if (!IsDead && isHitImpactOn)
        {
            FlashEffect();
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.hit);
        }
    }

    public void ApplyKnockback(Vector3 direction, float strength, float duration)
    {
        if (IsDead) return;

        StopMovement();

        rigidbody.AddForce(direction * strength, ForceMode2D.Impulse);

        if (!IsDead)
            StartCoroutine(EndKnockback(duration));

        /* 벽 뚫리는 버그 있으면 해당 코드 사용하기 
        // 벽이 있는지 체크
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, strength, LayerMask.GetMask("Wall"));

        if (hit.collider != null)
        {
            float wallOffset = 0.2f; // 벽과의 거리 유지
            // 벽이 가까우면 넉백 거리를 조정
            strength = Mathf.Max(hit.distance - wallOffset, 0f); // 최소 거리 유지
        }

        rigidbody.AddForce(direction.normalized * strength, ForceMode2D.Impulse);

        if (!IsDead)
            StartCoroutine(EndKnockback(duration));
        */
    }

    private IEnumerator EndKnockback(float duration)
    {
        yield return new WaitForSeconds(duration);

        rigidbody.velocity = Vector2.zero;
        if (IsInState<EnemyDefaultState>())
        {
            EnemyMovement.enabled = true;
        }
    }

    #region 몬스터 Item Drop
    private void DropItem(Entity entity, bool isRealDead)
    {
        if (!isRealDead) return;

        PoolManager.Instance.ReuseGameObject(meat, transform.position, Quaternion.identity);

        if (ShouldDropDNA())
            DropMonsterDNA();
    }

    private bool ShouldDropDNA()
    {
        if (monsterDNA == null)
            return false;

        bool hasDNA = GameManager.Instance.isHasDNA(monsterDNA.GetComponent<MonsterDNA>().Id);

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
        if (playerTransform == null || !IsHorizontalFlip)
            return;

        // 플레이어 위치와 몬스터 위치의 X 값 비교
        if (!isBig)
        {
            transform.localScale = playerTransform.position.x > transform.position.x 
                ? new Vector2(-1, 1) : new Vector2(1, 1); 
        }
        else
        {
            transform.localScale = playerTransform.position.x > transform.position.x
                ? new Vector2(-1.5f, 1.5f) : new Vector2(1.5f, 1.5f);
        }
    }

    // Dead Animation에서 호출
    private void DeActivate()
    {
        gameObject.SetActive(false);
    }
    
    // 몬스터 광폭화 함수 
    public void GetAnger()
    {
        // 공격력 1.5배 증가 
        // → 증가된 공격력은 Monster AI의 SetEnemy 함수에서 다시 초기화 된다. 
        Stats.IncreaseDefaultValue(Stats.AttackStat, Stats.AttackStat.DefaultValue * 0.5f);

        // 이동 속도 1.2배 증가 
        // → 증가된 이동 속도는 Monster AI의 SetEnemy 함수에서 다시 초기화 된다. 
        Stats.IncreaseDefaultValue(Stats.MoveSpeedStat, Stats.MoveSpeedStat.DefaultValue * 0.5f);
    }

    public override void OnDead(bool isRealDead = true)
    {
        base.OnDead(isRealDead);
        // 자폭 몬스터는 OnDead의 마지막 부분에서 사망 처리를 한다.  
        if (isSelfDestructive)
        {
            StateMachine.ExecuteCommand(EntityStateCommand.ToDeadState);
        }

        isPlayerInRange = false;
        if (crashDamageRoutine != null)
        {
            StopCoroutine(crashDamageRoutine);
            crashDamageRoutine = null;
        }
        StopCoroutine(FlashRoutine());

        if (isRealDead)
            GetComponent<QuestReporter>().Report();
    }

    public override void PlayBleedingEffect()
    {
        // BloodFX 재생
        bleedingEffectObject = PoolManager.Instance.ReuseGameObject(bleedingEfffect, Vector3.zero, Quaternion.identity);
        // bloodFXPos의 자식으로 만들어서 위치 따라가게 하기 
        bleedingEffectObject.transform.SetParent(bleedingFXPos, false);
    }

    public override void StopBleedingEffect()
    {
        if (bleedingEffectObject == null)
            return;

        // 자식 해제 
        bleedingEffectObject.transform.SetParent(null);
        bleedingEffectObject.SetActive(false);
    }
}
