using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D;
using UnityEngine;

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

    #region 해방 스킬  
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

    [SerializeField]
    private GameObject testUI;

    protected override void Awake()
    {
        base.Awake();

        SetUpLatentSkill();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (PlayerMovement.enabled == false)
            PlayerMovement.enabled = true;
    }

    private void Start()
    {
        PlayerHUD.Instance.Show();
        
        if (SkillSystem.defaultSkills.Length > 0)
        {
            var clone = SkillSystem.Register(SkillSystem.defaultSkills[0]);
            SkillSystem.Equip(clone, 1);
        }

        // 해방 스킬 획득 테스트 
        AcquireLatentSkill(0);
        ChangeLatentSkill(0);

        foreach (var latentSkill in ownLatentSkills)
        {
            latentSkill.SetLatentSkillLevel(1);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.P))
        {
            deathStack += 100;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayerController.Instance.enabled = false;
            GameManager.Instance.CinemachineTarget.enabled = false;
            testUI.GetComponent<LatentSkillUpgrade>().SetUp(ownLatentSkills, currentLatentSkill);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            TakeDamage(this, null, Stats.FullnessStat.MaxValue, true);
        }
    }

    protected override void SetUpMovement()
    {
        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerMovement?.Setup(this);
    }

    protected override void StopMovement()
    {
        if (PlayerMovement)
            PlayerMovement.enabled = false;
    }

    protected override void SetUpStateMachine()
    {
        StateMachine = GetComponent<MonoStateMachine<PlayerEntity>>();
        StateMachine?.Setup(this);
    }

    public override void OnDead()
    {
        base.OnDead();
        
        effectAnimation?.EndEffect();
        StageManager.Instance.LoseStage();
    }

    private void SetUpLatentSkill() => latentSkills = latentSkill.GetSlotNodes();

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
}
