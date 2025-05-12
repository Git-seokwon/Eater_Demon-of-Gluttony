using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class BossEntity : Entity
{
    [SerializeField]
    private string bossName;

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

    [Space(10)]
    [SerializeField]
    private GameObject bleedingEfffect;
    [SerializeField]
    private Transform bleedingFXPos;
    private GameObject bleedingEffectObject;

    private Transform playerTransform;
    public BossMovement BossMovement { get; private set; }
    public MonoStateMachine<BossEntity> StateMachine { get; private set; }
    public string BossName => bossName;

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

    #region CounterAttack
    [SerializeField]
    private Category removeTargetCategory;

    private bool isFlipped = true;
    public bool IsFlipped
    {
        get => isFlipped;
        set => isFlipped = value;
    }

    private bool isCounter = false;
    public bool IsCounter
    {
        get => isCounter;
        set => isCounter = value;
    }

    private bool isCounterApply = false;
    public bool IsCounterApply
    {
        get => isCounterApply;
        set => isCounterApply = value;
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        crashSeconds = new WaitForSeconds(0.15f);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        // 체력 정상화 
        // → OnEnable에 즉시 체력을 정상화 시키고, MonsterAI에서 체력 재조정을 한다. 
        // → 안하면 체력이 0인 상태로 부활하기에 태어나자마자 사망판정받음
        Stats.SetDefaultValue(Stats.FullnessStat, Stats.FullnessStat.MaxValue);

        onDead += DropItem;

        if (GameManager.Instance != null)
            playerTransform = GameManager.Instance.player.transform;

        isFlipped = true;
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

    public override void StopMovement()
    {
        if (BossMovement)
        {
            BossMovement.Stop();
            BossMovement.enabled = false;
        }
    }

    protected override void SetUpStateMachine()
    {
        StateMachine = GetComponent<MonoStateMachine<BossEntity>>();
        StateMachine?.Setup(this);
    }

    public override void TakeDamage(Entity instigator, object causer, float damage, bool isCrit,
       bool isHitImpactOn = true, bool isTrueDamage = false, bool isRealDead = true)
    {
        base.TakeDamage(instigator, causer, damage, isCrit, isHitImpactOn, isTrueDamage, isRealDead);

        // 피격 이펙트
        if (!IsDead && isHitImpactOn)
        {
            FlashEffect();
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.hit);
        }

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

    public override void OnDead(bool isReadDead = true)
    {
        base.OnDead(isReadDead);

        if (isReadDead)
            // Boss Death 효과음 재생
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.bossDeath);

        StopAllCoroutines();
    }

    private void TakeDamageByCounterAttack(Entity entity, Entity instigator, object causer, float damage, 
        bool isCrit, bool isHitImpactOn)
    {
        if (!IsCounter) return;

        IsCounterApply = true;
        // 보스가 오른쪽을 바라보는지 여부
        bool isBossFacingRight = Mathf.Approximately(transform.localScale.x, -1);
        // 플레이어가 오른쪽에 있는지 여부
        bool isPlayerOnRight = transform.position.x < GameManager.Instance.player.transform.position.x;

        if ((isBossFacingRight && isPlayerOnRight) || (!isBossFacingRight && !isPlayerOnRight))
        {
            Animator.speed = 1f; // 전방 공격 → 애니메이션 정상 재생
        }
        else
        {
            StartCoroutine(ApplyStunEffect()); // 후방 공격 → 기절 효과 적용
        }
    }

    private IEnumerator ApplyStunEffect()
    {
        // 기절 효과 
        var ccIcon = GetComponent<FloatingIcon>();
        SkillSystem.RemoveEffectAll(removeTargetCategory);
        StateMachine.ExecuteCommand(EntityStateCommand.ToStunningState);
        if (ccIcon != null)
            ccIcon.SetActiveCCSprite(1);

        yield return new WaitForSeconds(4f);

        StateMachine.ExecuteCommand(EntityStateCommand.ToDefaultState);
        if (ccIcon != null)
            ccIcon.SetDeActiveCCSprite(1);
    }

    public void SetCounterAttackEvent() => onTakeDamage += TakeDamageByCounterAttack;
    public void UnSetCounterAttackEvent() => onTakeDamage -= TakeDamageByCounterAttack;

    // 카운터 스킬 취소 함수 
    public IEnumerator CancelCounterAttack(BossEntity boss, Skill skill)
    {
        yield return new WaitForSeconds(3f);

        if (boss != null && boss.IsCounterApply) yield break;

        boss.SkillSystem.Cancel(skill, true);
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
            var go = PoolManager.Instance.ReuseGameObject(bloodEffectPrefab, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(bloodEffectPosition, false);
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

        if (IsInState<BossDefaultState>())
            BossMovement.enabled = true;
    }

    private void DropItem(Entity entity, bool isRealDead)
    {
        if (!isRealDead) return;

        DropGreatShard();

        if (ShouldDropDNA())
            DropBossDNA();
    }

    private void DropGreatShard()
    {
        int dropRate_GreatShard = StageManager.Instance.CurrentStage.ItemDropRate;

        int temp = UnityEngine.Random.Range(0, 100); // 0 ~ 99 난수 생성
        if (temp < dropRate_GreatShard)
        {
            PoolManager.Instance.ReuseGameObject(baal_GreatShard, transform.position, Quaternion.identity); // 파편 드랍 
            StageManager.Instance.CurrentStage.ItemDropRate = 20;                                           // 확률 초기화
        }
        else // 파편 미드랍
            StageManager.Instance.CurrentStage.ItemDropRate = (dropRate_GreatShard + 20); // 파편 드랍 확률 20% 증가 
    }

    private bool ShouldDropDNA()
    {
        if (bossDNA == null)
            return false;

        return !GameManager.Instance.isHasLatentSkill(bossDNA.GetComponent<BossDNA>().Id);
    }

    private void DropBossDNA()
    {
        PoolManager.Instance.ReuseGameObject(bossDNA, transform.position + new Vector3(0.1f, 0f, 0f),
                                             Quaternion.identity);

        GameManager.Instance.RecordLatentSkillDropped(bossDNA.GetComponent<BossDNA>().Id);
    }

    private void UpdateDirection()
    {
        if (playerTransform == null || !isFlipped)
            return;

        // 플레이어 위치와 몬스터 위치의 X 값 비교
        transform.localScale = playerTransform.position.x > transform.position.x
                ? new Vector2(-1, 1) : new Vector2(1, 1);
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
                StartCoroutine(RushAssault(collision.GetComponent<Entity>()));
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
            player.TakeDamage(this, null, crashDamage, false, false);

            yield return crashSeconds;
        }
    }

    // 돌진 스킬 적용
    private IEnumerator RushAssault(Entity player)
    {
        player.SkillSystem.Apply(skill);
        isRushAssault = false;

        yield return crashSeconds;

        crashDamageRoutine = StartCoroutine(DealDamageOverTime(player));
    }

    public void SetUpRushAssault(Skill skill) => this.skill = skill;
    public void SetOffRushAssault() => skill = null;

    // Dead Animation에서 호출
    private void DeActivate()
    {
        gameObject.SetActive(false);
    }

    private void OnFlipped() => IsFlipped = true;
    private void OffFlipped() => IsFlipped = false;

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
