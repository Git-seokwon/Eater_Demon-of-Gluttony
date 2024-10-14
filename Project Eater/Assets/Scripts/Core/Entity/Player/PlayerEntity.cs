using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : Entity
{
    public PlayerMovement PlayerMovement {  get; private set; }

    public MonoStateMachine<PlayerEntity> StateMachine { get; private set; }

    // �� �ع� ��ų 
    // 1. �⺻ ���� ��ų 
    // 2. �⺻ Ư�� ��ų 
    // 3. �ñر� ��ų 
    public Dictionary<string, List<Skill>> LatentSkill {  get; private set; }

    private List<Skill> currentLatentSkill;

    #region ���ں���
    [HideInInspector] public bool isRuthless;

    private float bonusDamagePercent;
    public float BonusDamagePercent
    {
        get => bonusDamagePercent;
        set => bonusDamagePercent = Mathf.Max(value, 0);
    }
    #endregion

    #region ����� ��
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

        // �⺻ ���� �׽�Ʈ �ڵ�
        // �� ����� ��ų �纻�� �����ؾ��� ������ �����ϸ� �ȵȴ�. 
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

    // IsInState �Լ� Wrapping
    // �� �ܺο��� StateMachine Property�� ��ġ�� �ʰ� Entity�� ���� �ٷ� ���� State��
    //    �Ǻ��� �� �ֵ��� �ߴ�.
    public bool IsInState<T>() where T : State<PlayerEntity>
        => StateMachine.IsInState<T>();

    public bool IsInState<T>(int layer) where T : State<PlayerEntity>
    => StateMachine.IsInState<T>(layer);

    public void AddLatentSkill(string skillName, List<Skill> skillList)
    {
        if (!LatentSkill.ContainsKey(skillName))
            LatentSkill[skillName] = skillList;
    }

    public void SetLatentSkill(string skillName)
    {
        currentLatentSkill = LatentSkill[skillName];
        
        if (currentLatentSkill != null)
            SkillSystem.SetupLatentSkills(currentLatentSkill);
    }

    private void BasicAttack(Vector2 mousePosition)
    {
        SkillSystem.Use(SkillSystem.defaultSkills[0]);
    }
}
