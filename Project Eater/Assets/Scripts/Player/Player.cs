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
    #endregion

    #region Player Movement
    protected Vector2 inputVec;
    #endregion

    private void Awake()
    {
        // Load Component
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize Finite State Mahcine
        stateMachine = new PlayerStateMachine();
    }

    private void Start()
    {
        
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
