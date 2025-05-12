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
    private GameObject bloodEffectPrefab; // �� �ִϸ��̼� ������
    [SerializeField]
    private Transform bloodEffectPosition; // �� �ִϸ��̼��� ������ ��ġ

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

    #region �浹 ������
    private float crashDamage;

    private Coroutine crashDamageRoutine;
    private bool isPlayerInRange;
    private WaitForSeconds crashSeconds;

    // ���� ��ų �ߵ� on/off
    private bool isRushAssault = false;
    private Skill skill;

    public bool IsRushAssault
    {
        get => isRushAssault;
        set => isRushAssault = value;
    }
    #endregion

    #region �� ü�� 8%���� ��� ���
    private float previousThresholdHP;  // 8% ������ �� ���ŵǴ� ��
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

        // ü�� ����ȭ 
        // �� OnEnable�� ��� ü���� ����ȭ ��Ű��, MonsterAI���� ü�� �������� �Ѵ�. 
        // �� ���ϸ� ü���� 0�� ���·� ��Ȱ�ϱ⿡ �¾�ڸ��� �����������
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

        // ���� �浹 �������� �⺻ ���������� ����ϱ� ������ ó�� Start �Լ����� 1ȸ ����Ѵ�. 
        crashDamage = Stats.GetValue(Stats.AttackStat) / 2;
        // ù ��° 8% üũ ����
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

        // �ǰ� ����Ʈ
        if (!IsDead && isHitImpactOn)
        {
            FlashEffect();
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.hit);
        }

        // �� ü�� 8%���� ��� ���
        while (Stats.FullnessStat.DefaultValue <= previousThresholdHP) // 8% ���Ϸ� ������ ������ �ݺ�
        {
            if (IsDead)
                return;

            SpawnMeatItems();
            PlayBloodEffect();
            previousThresholdHP -= Stats.FullnessStat.MaxValue * 0.08f; // ���� 8% üũ ���� ����
        }
    }

    public override void OnDead(bool isReadDead = true)
    {
        base.OnDead(isReadDead);

        if (isReadDead)
            // Boss Death ȿ���� ���
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.bossDeath);

        StopAllCoroutines();
    }

    private void TakeDamageByCounterAttack(Entity entity, Entity instigator, object causer, float damage, 
        bool isCrit, bool isHitImpactOn)
    {
        if (!IsCounter) return;

        IsCounterApply = true;
        // ������ �������� �ٶ󺸴��� ����
        bool isBossFacingRight = Mathf.Approximately(transform.localScale.x, -1);
        // �÷��̾ �����ʿ� �ִ��� ����
        bool isPlayerOnRight = transform.position.x < GameManager.Instance.player.transform.position.x;

        if ((isBossFacingRight && isPlayerOnRight) || (!isBossFacingRight && !isPlayerOnRight))
        {
            Animator.speed = 1f; // ���� ���� �� �ִϸ��̼� ���� ���
        }
        else
        {
            StartCoroutine(ApplyStunEffect()); // �Ĺ� ���� �� ���� ȿ�� ����
        }
    }

    private IEnumerator ApplyStunEffect()
    {
        // ���� ȿ�� 
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

    // ī���� ��ų ��� �Լ� 
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
            StartCoroutine(EndKnockback(duration)); // ��: 0.5�� �� �˹� ����
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

        int temp = UnityEngine.Random.Range(0, 100); // 0 ~ 99 ���� ����
        if (temp < dropRate_GreatShard)
        {
            PoolManager.Instance.ReuseGameObject(baal_GreatShard, transform.position, Quaternion.identity); // ���� ��� 
            StageManager.Instance.CurrentStage.ItemDropRate = 20;                                           // Ȯ�� �ʱ�ȭ
        }
        else // ���� �̵��
            StageManager.Instance.CurrentStage.ItemDropRate = (dropRate_GreatShard + 20); // ���� ��� Ȯ�� 20% ���� 
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

        // �÷��̾� ��ġ�� ���� ��ġ�� X �� ��
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

            // ���� ��ų ��� ��, isRushAssault�� true�� �Ǿ� �÷��̾�� ���� ��ų ȿ���� Apply �Ѵ�. 
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

    // ���� ��ų ����
    private IEnumerator RushAssault(Entity player)
    {
        player.SkillSystem.Apply(skill);
        isRushAssault = false;

        yield return crashSeconds;

        crashDamageRoutine = StartCoroutine(DealDamageOverTime(player));
    }

    public void SetUpRushAssault(Skill skill) => this.skill = skill;
    public void SetOffRushAssault() => skill = null;

    // Dead Animation���� ȣ��
    private void DeActivate()
    {
        gameObject.SetActive(false);
    }

    private void OnFlipped() => IsFlipped = true;
    private void OffFlipped() => IsFlipped = false;

    public override void PlayBleedingEffect()
    {
        // BloodFX ���
        bleedingEffectObject = PoolManager.Instance.ReuseGameObject(bleedingEfffect, Vector3.zero, Quaternion.identity);
        // bloodFXPos�� �ڽ����� ���� ��ġ ���󰡰� �ϱ� 
        bleedingEffectObject.transform.SetParent(bleedingFXPos, false);
    }

    public override void StopBleedingEffect()
    {
        if (bleedingEffectObject == null)
            return;

        // �ڽ� ���� 
        bleedingEffectObject.transform.SetParent(null);
        bleedingEffectObject.SetActive(false);
    }
}
