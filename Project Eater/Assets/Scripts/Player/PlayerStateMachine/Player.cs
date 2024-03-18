using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region Component
    public Animator animator { get; private set; }
    new public Rigidbody2D rigidbody2D { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    #endregion

    #region State
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    #endregion

    #region Player Movement
    public Vector2 inputVec { get; private set; }
    #endregion

    #region Player Aim
    [HideInInspector] public Vector3 startSkillDirection;     // 스킬 방향
    [HideInInspector] public float startSkillAngleDegrees;    // 스킬 발사 각도
    [HideInInspector] public float playerAngleDegrees;        // 플레이어 각도
    [HideInInspector] public AimDirection playerAimDirection; // 플레이어 Aim 방향

    [HideInInspector] public Vector3 startSkillShootPosition; // Latent Skill Projectile Firing Position
                                            // 캐릭터 프리팹에서 자식 오브젝트로 만들기 
                                            // 애니메이션 창에서 Transform 조절하기 
    #endregion

    #region Latent Skill
    public List<LatentSkill> latentSkills = new List<LatentSkill>();
    // 초기 해방 스킬 세팅
    [HideInInspector] LatentSkillEnum latentSkillNum = LatentSkillEnum.SpearOfGluttony;
    #endregion

    #region Player Stat
    [HideInInspector] public PlayerStatSO playerStat;
    #endregion



    private void Awake()
    {
        // Load Component
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize Finite State Mahcine
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
    }

    private void Start()
    {
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        stateMachine.currentState.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.currentState.FixedUpdate();
    }

    private void LateUpdate()
    {
        stateMachine.currentState.LateUpdate();
    }

    // Animation Finish through StateMachine
    public void AnimationTrigger() => stateMachine.currentState.AnimationFinish();

    // Initialize the player
    public void Initialize(PlayerStatSO playerStat, LatentSkillSO latentSkill)
    {
        this.playerStat = playerStat;

        CreatingPlayerStartingLatentSkill(latentSkill);
    }

    private void CreatingPlayerStartingLatentSkill(LatentSkillSO latentSkill)
    {
        // Clear list
        latentSkills.Clear();

        // Populate latentSkill list from starting latentSkill
        AddLatentSkillToPlayer(latentSkill);
    }

    // Add a Latent Skill to the player 
    public void AddLatentSkillToPlayer(LatentSkillSO latentSkill)
    {
        LatentSkill newLatentSkill = new LatentSkill()
        {

        };
    }

    // Player Input - Move
    void OnMove(InputValue inputValue)
    {
        inputVec = inputValue.Get<Vector2>();
    }

    // TODO : PlayerInput 시스템 사용해서 기본 공격 실행 메서드 작성
    void OnStartSkill()
    {
        // 게임 모드 체크 : 전투 상황이 아니라면 기본 공격 X
        if (GameManager.Instance.gameState != GameState.normalRound && GameManager.Instance.gameState != GameState.bossRound)
            return;

        
    }

    // TODO : PlayerInput 시스템 사용해서 스킬 공격 실행 메서드 작성
    void OnFirstSkill()
    {

    }

    void OnSecondSkill()
    {

    }

    void OnThirdSkill()
    {

    }

    void OnFourthSkill()
    {

    }

    // TODO : PlayerInput 시스템 사용해서 궁극 공격 실행 메서드 작성
}
