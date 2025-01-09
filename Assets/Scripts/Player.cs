using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using OpenCover.Framework.Model;
using System.Runtime.CompilerServices;



public class Player : Character
{

    private enum PlayerState { Idle, Walking, Jumping, Crouching, Attacking, Blocking, Hurt, Dead, Exhausted }
    private PlayerState currentState = PlayerState.Idle;

    public float BasicAttackDamage { get; set; } = 20f;





    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float jumpForce;

    protected bool canInput;
    protected bool isJumping;
    protected bool isAttacking;
    protected bool gettingHit;






  
    // Update is called once per frame



    //public event Action CallDamage;

    private void Awake() //pøed startem
    {

        //CallDamage += DoDamage;
    }

    private void DoDamage(GameObject obj)
    {
        //obj.Damage(50f);
    }

    void Update()
    {


        CheckOpponentPosition();


        //HandleStateTransitions();
        //HandleStateActions();



        Movement();
        //HealStamina();
    }


    private void HandleStateActions()
    {
        throw new NotImplementedException();
    }

    private void HandleStateTransitions()
    {
        switch (currentState)
        {
            case PlayerState.Idle:  
                break;
            case PlayerState.Walking:
                break;
            case PlayerState.Jumping:
                break;
            case PlayerState.Crouching:
                break;
            case PlayerState.Attacking:
                break;
            case PlayerState.Blocking:
                break;
            case PlayerState.Hurt:
                break;
            case PlayerState.Dead:
                break;
            case PlayerState.Exhausted:
                break;
            default:
                break;
        }



        /*
        switch (currentState)
        {
            case PlayerState.Idle:
                if (movementInput.x != 0)
                    TransitionToState(PlayerState.Walking);
                if (isJumping)
                    TransitionToState(PlayerState.Jumping);
                if (isAttacking)
                    TransitionToState(PlayerState.Attacking);
                break;

            case PlayerState.Walking:
                if (movementInput.x == 0)
                    TransitionToState(PlayerState.Idle);
                if (isJumping)
                    TransitionToState(PlayerState.Jumping);
                if (isAttacking)
                    TransitionToState(PlayerState.Attacking);
                break;

            case PlayerState.Jumping:
                if (rb.velocity.y == 0) // Grounded check
                    TransitionToState(PlayerState.Idle);
                break;

            case PlayerState.Attacking:
                // Transition back to Idle or Walking once the attack animation ends
                if (!isAttacking)
                    TransitionToState(movementInput.x == 0 ? PlayerState.Idle : PlayerState.Walking);
                break;
        
        }
        */
    }

    private void CheckOpponentPosition()
    {

    }

    private void TransitionToState(PlayerState newState)
    {
        currentState = newState;
    }


    private void ResetAttack()
    {
        isAttacking = false;
    }

    

    protected override void HealStamina()
    {
        /*staminaRegenTimer = staminaMaxRegenTimer;
        staminaTime = staminaRegenDelay;
        staminaTime -= Time.deltaTime;*/
        if (stamina < maxStamina && !staminaRecentUse)
        {

            StartCoroutine(Delay(staminaRegenDelay, false));
        }
        else if (staminaRecentUse)
        {
            StartCoroutine(Delay(0, true));

        }
    }

    private void Movement()
    {
        Vector3 position = character.transform.position;



        //if (!canInput) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            BasicAttack();
            //StartCoroutine(Delay(staminaBeforeRegenDelay, true));
        }
        else
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpecialAttack();
        }
        else
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {

            if (position.y > 0.6) return;
            if (stamina <= 0) return;
            StaminaUse(20);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
        {

            if (Input.GetKey(KeyCode.RightArrow))
            {
                position.x += walkSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                position.x -= walkSpeed * Time.deltaTime;
            }


            character.transform.position = position;
        }
        return;
    }

    public void SpecialAttack()
    {
        float specialAttackDamage = BasicAttackDamage * 2;

        Vector3 pos = character.transform.position + new Vector3(xDifference * turnedSide, 0, 0);
        var attack = Instantiate(basicAttackPrefab, pos, Quaternion.identity);
        attack.transform.SetParent(character.transform);

        var collision = attack.GetComponent<BasicAttackCollision>();
        if (collision != null)
        {
            // Pass the special attack damage value to the collision script.
            collision.doubleDmg = 2;
        }

        Destroy(attack.gameObject, attackDuration);
    }
}











/*

public class PlayerController : MonoBehaviour
{

    private enum PlayerState { Idle, Walking, Jumping, Crouching, Attacking, CrouchAttacking, Blocking, Hurt, Dead }
    private PlayerState currentState = PlayerState.Idle;

    private PlayerInputActions inputActions;
    private Rigidbody2D rb;

    // Movement properties
    public float speed = 5f;
    public float jumpForce = 10f;

    private Vector2 movementInput;
    private bool isJumping;
    private bool isAttacking;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        // Register input actions
        inputActions.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => movementInput = Vector2.zero;
        
        inputActions.Player.Jump.performed += ctx => Jump();
        inputActions.Player.Attack.performed += ctx => Attack();

        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        HandleStateTransitions();
        HandleStateActions();
    }


    private void HandleStateTransitions()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                if (movementInput.x != 0)
                    TransitionToState(PlayerState.Walking);
                if (isJumping)
                    TransitionToState(PlayerState.Jumping);
                if (isAttacking)
                    TransitionToState(PlayerState.Attacking);
                break;

            case PlayerState.Walking:
                if (movementInput.x == 0)
                    TransitionToState(PlayerState.Idle);
                if (isJumping)
                    TransitionToState(PlayerState.Jumping);
                if (isAttacking)
                    TransitionToState(PlayerState.Attacking);
                break;

            case PlayerState.Jumping:
                if (rb.velocity.y == 0) // Grounded check
                    TransitionToState(PlayerState.Idle);
                break;

            case PlayerState.Attacking:
                // Transition back to Idle or Walking once the attack animation ends
                if (!isAttacking)
                    TransitionToState(movementInput.x == 0 ? PlayerState.Idle : PlayerState.Walking);
                break;

            // Add other state transitions as needed (Hurt, Dead, etc.)
        }
    }

    // Perform actions based on the current state
    private void HandleStateActions()
    {
        switch (currentState)
        {
            case PlayerState.Walking:
                Move();
                break;
            case PlayerState.Jumping:
                Jump();
                break;
            case PlayerState.Attacking:
                Attack();
                break;
            case PlayerState.Idle:
                // Idle state logic (e.g., play idle animation)
                break;
            // Add other actions here
        }
    }

    // Transition to a new state
    private void TransitionToState(PlayerState newState)
    {
        currentState = newState;
        // Handle any setup required for new state, e.g., playing animations
    }

    // Movement logic
    private void Move()
    {
        Vector2 movement = new Vector2(movementInput.x * speed, rb.velocity.y);
        rb.velocity = movement;
    }

    // Jump logic
    private void Jump()
    {
        if (currentState != PlayerState.Jumping)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumping = true;
        }
    }

    // Attack logic
    private void Attack()
    {
        if (currentState != PlayerState.Attacking)
        {
            isAttacking = true;
            // Play attack animation, handle attack damage, etc.
            Invoke(nameof(ResetAttack), 0.5f); // Example attack duration
        }
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }
}

*/