using System;
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

    [Space(10)]
    [SerializeField]
    private int meatCount = 3;
    [SerializeField]
    private float meatRadius = 4f;
    [SerializeField]
    private GameObject bloodEffectPrefab; // 피 애니메이션 프리팹
    [SerializeField]
    private Transform bloodEffectPosition; // 피 애니메이션이 생성될 위치

    public BossMovement BossMovement { get; private set; }
    public MonoStateMachine<BossEntity> StateMachine { get; private set; }

    private Transform playerTransform;

    #region 충돌 데미지
    private float crashDamage;

    private Coroutine crashDamageRoutine;
    private bool isPlayerInRange;
    private WaitForSeconds crashSeconds;

    // 돌진 스킬 발동 on/off
    private bool isRushAssault = false;
    private Skill skill;

    public bool IsRushAssault
    {
        get => isRushAssault;
        set => isRushAssault = value;
    }
    #endregion

    #region 매 체력 8%마다 고기 드랍
    private float previousThresholdHP;  // 8% 감소할 때 갱신되는 값
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
        // 망멸의 낫으로 인해 차단된 경우 다시 해당 기능을 켜준다. 
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

        // 몬스터 충돌 데미지는 기본 데미지에서 계산하기 때문에 처음 Start 함수에서 1회 계산한다. 
        crashDamage = Stats.GetValue(Stats.AttackStat) / 2;
        // 첫 번째 8% 체크 기준
        previousThresholdHP = Stats.FullnessStat.MaxValue * 0.92f;
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

        // 피격 이펙트
        if (!IsDead)
            FlashEffect();

        // 매 체력 8%마다 고기 드랍
        while (Stats.FullnessStat.DefaultValue <= previousThresholdHP) // 8% 이하로 내려갈 때마다 반복
        {
            if (IsDead)
                return;

            SpawnMeatItems();
            PlayBloodEffect();
            previousThresholdHP -= Stats.FullnessStat.MaxValue * 0.08f; // 다음 8% 체크 기준 갱신
        }
    }

    private void SpawnMeatItems()
    {
        for (int i = 0; i < meatCount; i++)
        {
            Vector2 spawnPosition = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * meatRadius;
            PoolManager.Instance.ReuseGameObject(meat, spawnPosition, Quaternion.identity);
        }
    }

    private void PlayBloodEffect()
    {
        if (bloodEffectPrefab != null && bloodEffectPosition != null)
        {
            PoolManager.Instance.ReuseGameObject(bloodEffectPrefab, bloodEffectPosition.position, Quaternion.identity);
        }
    }

    public void ApplyKnockback(Vector3 direction, float strength, float duration)
    {
        BossMovement.enabled = false;
        rigidbody.velocity = Vector2.zero;

        rigidbody.AddForce(direction * strength, ForceMode2D.Impulse);

        if (!IsDead)
            StartCoroutine(EndKnockback(duration)); // 예: 0.5초 후 넉백 종료
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

        // 플레이어 위치와 몬스터 위치의 X 값 비교
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

    // IsInState 함수 Wrapping
    // → 외부에서 StateMachine Property를 거치지 않고 Entity를 통해 바로 현재 State를
    //    판별할 수 있도록 했다.
    public bool IsInState<T>() where T : State<BossEntity>
        => StateMachine.IsInState<T>();

    public bool IsInState<T>(int layer) where T : State<BossEntity>
    => StateMachine.IsInState<T>(layer);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag)
        {
            isPlayerInRange = true;

            // 돌진 스킬 사용 시, isRushAssault가 true가 되어 플레이어에게 돌진 스킬 효과를 Apply 한다. 
            if (isRushAssault)
                crashDamageRoutine = StartCoroutine(RushAssault(collision.GetComponent<Entity>()));
            else
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

    // 돌진 스킬 적용
    private IEnumerator RushAssault(Entity player)
    {
        while (isPlayerInRange)
        {
            player.SkillSystem.Apply(skill);

            yield return crashSeconds;
        }
    }

    public void SetUpRushAssault(Skill skill) => this.skill = skill;
    public void SetOffRushAssault() => skill = null;

    // Dead Animation에서 호출
    private void DeActivate()
    {
        gameObject.SetActive(false);
    }
}
