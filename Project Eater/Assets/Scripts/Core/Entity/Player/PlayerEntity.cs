using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[Serializable]
public struct LatentSkillData
{
    // 해방 스킬 고유 Index
    public int index;
    // 해방 스킬 Level Data
    public int level;
}

public class PlayerEntity : Entity
{
    public delegate void ChangeMeatStack(PlayerEntity owner);
    public delegate void ChangeDeathStack(PlayerEntity owner);

    public event ChangeMeatStack onChangeMeathStack;
    public event ChangeDeathStack onChangeDeathStack;

    #region 사냥 본능
    public delegate void GetMeatHandler();
    public event GetMeatHandler onGetMeat;

    private int meatStack = 0;
    public int MeatStack
    {
        get => meatStack;
        set
        {
            meatStack = Mathf.Max(value, 0);
            onChangeMeathStack?.Invoke(this);
        }
    }
    #endregion

    public PlayerMovement PlayerMovement {  get; private set; }

    public MonoStateMachine<PlayerEntity> StateMachine { get; private set; }

    public override int EntitytSight
        => PlayerMovement.playerLookDirection == AimDirection.Right ? 1 : -1;

    #region 해방 스킬  
    [HideInInspector]
    public List<LatentSkillData> savedLatentSkills = new List<LatentSkillData>();

    // Index 0 : 기본 지속 효과 스킬
    // Index 1 : 기본 공격 스킬 
    [SerializeField]
    private LatentSkill latentSkill; // LatentSkillNode 정보를 가지고 있는 DataBase
    // latentSkill에서 해방 스킬 정보들을 Dictionary 자료형으로 받는다.
    private Dictionary<int, LatentSkillSlotNode> latentSkills = new();

    // 플레이어가 현재 소유하고 있는 해방 스킬 List
    private List<LatentSkillSlotNode> ownLatentSkills = new();
    // 플레이어가 현재 장착하고 있는 해방 스킬 
    private LatentSkillSlotNode currentLatentSkill;

    public List<LatentSkillSlotNode> OwnLatentSkills => ownLatentSkills;
    public LatentSkillSlotNode CurrentLatentSkill => currentLatentSkill;
    public Dictionary<int, LatentSkillSlotNode> LatentSkills => latentSkills;
    #endregion

    #region 무자비함
    [HideInInspector] public bool isRuthless;

    private float bonusDamagePercent;
    public float BonusDamagePercent
    {
        get => bonusDamagePercent;
        set => bonusDamagePercent = Mathf.Max(value, 0);
    }
    #endregion

    #region 사신의 낫
    private int deathStack = 0;
    public int DeathStack
    {
        get => deathStack;
        set
        {
            deathStack = Mathf.Max(value, 0);
            onChangeDeathStack?.Invoke(this);
        }
    }
    #endregion

    #region Grit
    [HideInInspector] public bool isGrit;
    [SerializeField] private GameObject gritShield;
    public GameObject GritShield => gritShield;
    private Coroutine superArmorCoroutine;
    public Coroutine SuperArmorCoroutine => superArmorCoroutine;
    #endregion

    private EffectAnimation effectAnimation;
    public EffectAnimation EffectAnimation => effectAnimation;

    #region DamagedEffect
    private Vignette vignette;
    private const float maxVignetteIntensity = 0.5f;
    private const float maxVignetteSmoothness = 0.25f;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        effectAnimation = GetComponent<EffectAnimation>();

        GameManager.Instance.CinemachineVS?.m_Profile.TryGet(out vignette);

        vignette.smoothness.value = maxVignetteSmoothness;
        vignette.intensity.value = 0f;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        // 체력 정상화 
        // → 스킬로 인해 영향을 받은 Stat들은 OnDead 함수의 SkillSystem.RemoveEffectAll(); 로 인해 다 초기화 된다. 
        Stats.SetDefaultValue(Stats.FullnessStat, Stats.FullnessStat.MaxValue);

        if (PlayerMovement.enabled == false)
            PlayerMovement.enabled = true;
    }

    private void Start()
    {
        if (MusicManager.Instance != null && SceneManager.GetActiveScene().name == "MainScene")
            MusicManager.Instance.PlayMusic(GameResources.Instance.LobbyMenuMusic);
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void SetUpMovement()
    {
        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerMovement?.Setup(this);
    }

    public override void StopMovement()
    {
        if (PlayerMovement)
        {
            PlayerMovement.Stop();

            // PlayerMovement를 비활성화 하기 전에 대쉬를 종료하고 스페이스 키도 초기화 한다. 
            PlayerMovement.StopPlayerDashRoutine(null);

            PlayerMovement.enabled = false;
        }
    }

    protected override void SetUpStateMachine()
    {
        StateMachine = GetComponent<MonoStateMachine<PlayerEntity>>();
        StateMachine?.Setup(this);
    }

    public override void OnDead(bool isRealDead = true)
    {
        base.OnDead(isRealDead);

        effectAnimation?.EndEffect();
        StageManager.Instance.LoseStage();

        // Player Death 효과음 재생
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.playerDeath);
    }

    public void SetUpLatentSkill() => latentSkills = latentSkill.GetSlotNodes();

    // IsInState 함수 Wrapping
    // → 외부에서 StateMachine Property를 거치지 않고 Entity를 통해 바로 현재 State를
    //    판별할 수 있도록 했다.
    public bool IsInState<T>() where T : State<PlayerEntity>
        => StateMachine.IsInState<T>();

    public bool IsInState<T>(int layer) where T : State<PlayerEntity>
    => StateMachine.IsInState<T>(layer);

    #region Latent Skill
    public void AcquireLatentSkill(int index) => ownLatentSkills.Add(latentSkills[index]);
    public void ChangeLatentSkill(int number) => currentLatentSkill = ownLatentSkills[number];
    public void LevelUpLatentSkill(LatentSkillSlotNode latentSkill) => latentSkill.LatentSkillLevelUp();
    public void LoadLatentSkill(int index, int level)
    {
        // ※ ListExtensions 
        // → 확장 메서드(Extension Method)를 활용하여 List<T>의 기능을 확장하는 방식
        // → 확장 메서드를 사용하면 기존 List<T> 클래스에 새로운 기능을 추가할 수 있지만, 원본 클래스를 수정하지 않아도 된다. 
        // ※ 확장 메서드 만드는 방식 
        // 1. static 클래스를 만든다.
        // 2. static 메서드를 정의한다.
        // 3. 첫 번째 매개변수 앞에 this 키워드를 붙여 확장할 클래스를 지정한다.
        // → public static class ListExtensions 참고
        var latentSkill = ownLatentSkills.AddAndReturn(latentSkills[index]);
        latentSkill.SetLatentSkillLevel(level);
    }
    public void SetLatentSkills() => ChangeLatentSkill(0);
    #endregion

    public void OnGetMeat() => onGetMeat?.Invoke();

    // Dead Animation에서 호출
    private void DeActivate() => gameObject.SetActive(false);

    public void DecreaseFullness(float amount)
    {
        if (IsDead)
            return;

        Stats.FullnessStat.DefaultValue -= amount;

        if (Mathf.Approximately(Stats.FullnessStat.DefaultValue, 0f))
        {
            OnDead();
        }
    }

    protected override void ExecutionGrit(ref float damage)
    {
        if (!isGrit) return;

        // 피격 데미지 무시
        damage = 0;
        // 무적 상태
        if (superArmorCoroutine == null)
            superArmorCoroutine = StartCoroutine(SuperArmor());
        // 투지 소모
        isGrit = false;
    }

    private IEnumerator SuperArmor()
    {
        GritShield.SetActive(true);
        Collider.enabled = false;

        yield return new WaitForSeconds(Settings.superArmorDuration);

        GritShield.SetActive(false);
        Collider.enabled = true;
        superArmorCoroutine = null;
    }

    // 스테이지 클리어 시, 플레이어에게 적용할 코드
    public void StageClear()
    {
        StopMovement();
        SkillSystem.CancelAll(true);

        effectAnimation?.EndEffect();
    }

    public override void TakeDamage(Entity instigator, object causer, float damage, bool isCrit, bool isHitImpactOn = true, bool isTrueDamage = false, bool isRealDead = true)
    {
        base.TakeDamage(instigator, causer, damage, isCrit, isHitImpactOn, isTrueDamage, isRealDead);

        if (isRealDead && !IsDead)
        {
            StartCoroutine(PlayEffectCoroutine(0.5f));
        }
    }

    IEnumerator PlayEffectCoroutine(float timeForEffect)
    {
        vignette.intensity.value = maxVignetteIntensity;

        for (float time = 0.0f; time < timeForEffect; time += Time.deltaTime)
        {
            vignette.intensity.value = Mathf.Lerp(maxVignetteIntensity, 0f, time / timeForEffect);
            yield return null;
        }
    }
}
