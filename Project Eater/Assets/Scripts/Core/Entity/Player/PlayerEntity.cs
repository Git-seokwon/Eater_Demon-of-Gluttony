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

    #region ��� ����
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

    #region �ع� ��ų  
    // Index 0 : �⺻ Ư�� ��ų
    // Index 1 : �⺻ ���� ��ų 
    [SerializeField]
    private LatentSkill latentSkill; // LatentSkillNode ������ ������ �ִ� DataBase
    // latentSkill���� �ع� ��ų �������� Dictionary �ڷ������� �޴´�.
    private Dictionary<int, LatentSkillSlotNode> latentSkills = new();

    // �÷��̾ ���� �����ϰ� �ִ� �ع� ��ų List
    private List<LatentSkillSlotNode> ownLatentSkills = new();
    // �÷��̾ ���� �����ϰ� �ִ� �ع� ��ų 
    private LatentSkillSlotNode currentLatentSkill;

    public List<LatentSkillSlotNode> OwnLatentSkills => ownLatentSkills;
    public LatentSkillSlotNode CurrentLatentSkill => currentLatentSkill;
    public Dictionary<int, LatentSkillSlotNode> LatentSkills => latentSkills;
    #endregion

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

    private void Start()
    {
        PlayerHUD.Instance.Show();
        
        var clone = SkillSystem.Register(SkillSystem.defaultSkills[0]);
        SkillSystem.Equip(clone, 1);

        // �پ� ���� �߰��� 
        GameManager.Instance.BaalFlesh = 90000;
        GameManager.Instance.Baal_GreatShard = 100;

        // �ع� ��ų ȹ�� �׽�Ʈ 
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

    protected override void OnDead()
    {
        base.OnDead();
        
        Debug.Log("�� �÷��̾� �׾���");
        
        Debug.Log("�������� ���� ó���ض�");
        
        effectAnimation?.EndEffect();
        StageManager.Instance.LoseStage();
    }

    private void SetUpLatentSkill() => latentSkills = latentSkill.GetSlotNodes();

    // IsInState �Լ� Wrapping
    // �� �ܺο��� StateMachine Property�� ��ġ�� �ʰ� Entity�� ���� �ٷ� ���� State��
    //    �Ǻ��� �� �ֵ��� �ߴ�.
    public bool IsInState<T>() where T : State<PlayerEntity>
        => StateMachine.IsInState<T>();

    public bool IsInState<T>(int layer) where T : State<PlayerEntity>
    => StateMachine.IsInState<T>(layer);

    #region Latent Skill
    public void AcquireLatentSkill(int index) => ownLatentSkills.Add(latentSkills[index]);
    public void ChangeLatentSkill(int number) => currentLatentSkill = ownLatentSkills[number];
    #endregion

    public void OnGetMeat() => onGetMeat?.Invoke();

    // Dead Animation���� ȣ��
    private void DeActivate() => gameObject.SetActive(false);
}
