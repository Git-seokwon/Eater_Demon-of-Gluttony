using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : Entity
{
    public PlayerMovement PlayerMovement {  get; private set; }

    public MonoStateMachine<PlayerEntity> StateMachine { get; private set; }

    #region 해방 스킬  
    // Index 0 : 기본 공격 스킬 
    // Index 1 : 기본 특성 스킬 
    // Index 2 : 궁극기  
    private List<Skill[]> latentSkills = new();
    // [HideInInspector]
    public Skill[] currentLatentSkill = Array.Empty<Skill>();
    public IReadOnlyList<Skill[]> LatentSkills => latentSkills;
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
    private int currentStackCount = 0;
    public int CurrentStackCount
    {
        get => currentStackCount;
        set => currentStackCount = Mathf.Max(value, 0);
    }
    #endregion

    private void Start()
    {
        PlayerHUD.Instance.Show();

        // 기본 공격 테스트 코드
        // → 등록한 스킬 사본을 장착해야지 원본을 장착하면 안된다. 
        var clone = SkillSystem.Register(SkillSystem.defaultSkills[0]);
        SkillSystem.Equip(clone, 1);
        var clone2 = SkillSystem.Register(SkillSystem.defaultSkills[1]);
        SkillSystem.Equip(clone2, 2);
        SkillSystem.Register(SkillSystem.defaultSkills[2]);
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.L))
        {
            SkillSystem.SkillLevelUp(SkillSystem.defaultSkills[0]);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            SkillSystem.SkillLevelUp(SkillSystem.defaultSkills[1]);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            SkillSystem.SetupLatentSkills();
        }

        Debug.Log("CurrentStackCount : " + CurrentStackCount);
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

    // IsInState 함수 Wrapping
    // → 외부에서 StateMachine Property를 거치지 않고 Entity를 통해 바로 현재 State를
    //    판별할 수 있도록 했다.
    public bool IsInState<T>() where T : State<PlayerEntity>
        => StateMachine.IsInState<T>();

    public bool IsInState<T>(int layer) where T : State<PlayerEntity>
    => StateMachine.IsInState<T>(layer);

    public void AcquireLatentSkill(Skill[] latentSkill) => latentSkills.Add(latentSkill);
    public void ChangeLatentSkill(int number) => currentLatentSkill = latentSkills[number];
}
