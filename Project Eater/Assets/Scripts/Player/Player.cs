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
    #endregion

    #region Player Movement
    public Vector2 inputVec { get; private set; }
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

    // Animation Finish through StateMachine
    public void AnimationTrigger() => stateMachine.currentState.AnimationFinish();

    // Player Input - Move
    void OnMove(InputValue inputValue)
    {
        inputVec = inputValue.Get<Vector2>();
    }
}
