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

    #region 축적
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
    // Index 0 : 기본 공격 스킬 
    // Index 1 : 기본 특성 스킬 
    [SerializeField]
    private LatentSkill latentSkill;
    private Dictionary<int, LatentSkillSlotNode> latentSkills = new();

    private List<LatentSkillSlotNode> ownLatentSkills = new();
    private LatentSkillSlotNode currentLatentSkill;

    public IReadOnlyList<LatentSkillSlotNode> OwnLatentSkills => ownLatentSkills;
    public LatentSkillSlotNode CurrentLatentSkill => currentLatentSkill;
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

    private void Start()
    {
        PlayerHUD.Instance.Show();

        
        /*var clone = SkillSystem.Register(SkillSystem.defaultSkills[0]);
        SkillSystem.Equip(clone, 1);*/
        

        var skills = SkillSystem.SkillSlot.Where(pair => pair.Key.Item1 == 0).Select(pair => pair.Value).ToList();
        foreach (var skill in skills)
            SkillSystem.AddAcquirableSkills(skill);
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.P))
        {
            deathStack += 100;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            // 해방 스킬 획득 테스트 
            AcquireLatentSkill(0);
            ChangeLatentSkill(0);
            SkillSystem.SetupLatentSkills();
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

    protected override void SetUpLatentSkill() => latentSkills = latentSkill.GetSlotNodes();

    // IsInState 함수 Wrapping
    // → 외부에서 StateMachine Property를 거치지 않고 Entity를 통해 바로 현재 State를
    //    판별할 수 있도록 했다.
    public bool IsInState<T>() where T : State<PlayerEntity>
        => StateMachine.IsInState<T>();

    public bool IsInState<T>(int layer) where T : State<PlayerEntity>
    => StateMachine.IsInState<T>(layer);

    public void AcquireLatentSkill(int index) => ownLatentSkills.Add(latentSkills[index]);
    public void ChangeLatentSkill(int number) => currentLatentSkill = ownLatentSkills[number];

    public void OnGetMeat() => onGetMeat?.Invoke();
}
