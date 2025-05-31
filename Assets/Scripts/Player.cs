using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEngine.InputSystem.XR;


public class Player : Character
{

    protected enum PlayerState { Idle, Walking, Jumping, JumpingAttack, Crouching, CrouchingAttack, Attacking, Blocking, Hurt, Dead, Exhausted, Super }
    protected PlayerState currentState = PlayerState.Idle;
    protected PlayerState previousState;


    public float BasicAttackDamage { get; set; } = 20f;


    //InputActionMap inputActionMap;

    [SerializeField]
    Collider attackCollider;

    [SerializeField]
    protected float walkSpeed;
    [SerializeField]
    protected float jumpForce;

    protected bool canInput;
    protected bool isJumping;
    protected bool isSuper1;
    protected bool isAttacking;
    protected bool gettingHit;
    protected bool doubleJump;
    protected bool canJump = true;

    protected int walkingDirection;
    protected int pointDirection;

    protected Vector2 movementInput = Vector2.zero;



    protected bool canAttack = true;
    protected bool canSuper = true;
    [SerializeField]
    protected float attackCooldown = 0.25f;
    [SerializeField]
    protected float superCooldown = 0.25f;


    new void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
    }

    new void Awake() 
    {
        base.Awake();
 
    }



    // Update is called once per frame
    protected new void Update()
    {
        base.Update();
        //CheckOpponentPosition();
        PointDirection();
        HandleStateTransitions();
        HandleStateActions();

        //Debug.Log(currentState);
        //Movement();
        //HealStamina();
    }

    private void PointDirection()
    {
        if (movementInput.y > 0) 
        {
            pointDirection = 1;
        }
        else if (movementInput.y < 0)
        {
            pointDirection = -1;
        }
        else
        {
            pointDirection = 0;
        }
    }

    protected void HandleStateActions()
    {
        // Idle, Walking, Jumping, JumpingAttack, Crouching, CrouchingAttack, Attacking, Blocking, Hurt, Dead, Exhausted, Super 
        //Debug.Log(currentState);
        switch (currentState)
        {

            case PlayerState.Idle:
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsAttacking", false);
                animator.SetBool("IsCrouching", false);
                Idle();
                break;

            case PlayerState.Walking:
                animator.SetBool("IsWalking", true);
                Move();
                break;

            case PlayerState.Jumping:
                if (previousState == PlayerState.Walking)
                {
                    animator.SetBool("IsWalking", false);
                }
                animator.SetBool("IsJumping", true);
                Jump();
                break;
            case PlayerState.Crouching:
                animator.SetBool("IsCrouching", true);
                animator.SetBool("IsAttacking", false);
                Crouch();
                break;

            case PlayerState.Attacking:
                if (previousState == PlayerState.Walking)
                {
                    animator.SetBool("IsWalking", true);
                }
                animator.SetBool("IsAttacking", true);
                Attack();
                break;

            case PlayerState.JumpingAttack:
                animator.SetBool("IsAttacking", true);
                JumpAttack();
                break;

            case PlayerState.CrouchingAttack:
                animator.SetBool("IsAttacking", true);
                CrouchAttack();
                break;

            case PlayerState.Blocking:
                Debug.Log("HUH");
                Block();
                break;

            case PlayerState.Hurt:
                Hurt();
                break;

            case PlayerState.Super:
                animator.SetBool("IsAttacking", true);
                Super();
                break;

        }

    }

    protected void Move()
    {
        Vector3 position = character.transform.position;

        if (movementInput.x > 0)
        {
            position.x += walkSpeed * Time.deltaTime;
        }
        else if (movementInput.x < 0)
        {
            position.x -= walkSpeed * Time.deltaTime;
        }
        character.transform.position = position;
    }

    protected void Idle()
    {

    }

    protected void HandleStateTransitions()
    {
        // Idle, Walking, Jumping, JumpingAttack, Crouching, CrouchingAttack, Attacking, Blocking, Hurt, Dead, Exhausted, Super 







        switch (currentState)
        {
            case PlayerState.Idle:
                if (movementInput.x > 0 || movementInput.x < 0)
                    TransitionToState(PlayerState.Walking);
                if (movementInput.y < 0)
                    TransitionToState(PlayerState.Crouching);
                if (isJumping && currentStamina >= 0)
                    TransitionToState(PlayerState.Jumping);
                if (isBlocking)
                    TransitionToState(PlayerState.Blocking);
                if (isAttacking)
                    TransitionToState(PlayerState.Attacking);
                if (isSuper1)
                    TransitionToState(PlayerState.Super);
                //if (Input.GetKeyDown(KeyCode.G))
                //TransitionToState(PlayerState.Super);
                //
                //TransitionToState(PlayerState.Hurt);
                if (currentHealth <= 0)
                    TransitionToState(PlayerState.Dead);

                break;

            case PlayerState.Walking:

                if (!(movementInput.x > 0 || movementInput.x < 0))
                    TransitionToState(PlayerState.Idle);
                if (isJumping && currentStamina >= 0)
                    TransitionToState(PlayerState.Jumping);
                //
                //TransitionToState(PlayerState.Hurt);
                if (currentHealth <= 0)
                    TransitionToState(PlayerState.Dead);
                break;

            case PlayerState.Jumping:
                if (canJump && !isJumping)
                    TransitionToState(PlayerState.Idle);
                if (isAttacking)
                    TransitionToState(PlayerState.JumpingAttack);
                //
                //TransitionToState(PlayerState.Hurt);
                if (currentHealth <= 0)
                    TransitionToState(PlayerState.Dead);
                break;

            case PlayerState.JumpingAttack:
                if (!isJumping && canJump)
                    TransitionToState(PlayerState.Idle);
                //
                //TransitionToState(PlayerState.Hurt);
                if (currentHealth <= 0)
                    TransitionToState(PlayerState.Dead);
                break;

            case PlayerState.Crouching:
                if (!(movementInput.y < 0))
                    TransitionToState(PlayerState.Idle);
                if (isAttacking)
                    TransitionToState(PlayerState.CrouchingAttack);
                //
                //TransitionToState(PlayerState.Hurt);
                if (currentHealth <= 0)
                    TransitionToState(PlayerState.Dead);
                break;

            case PlayerState.CrouchingAttack:
                if (!isAttacking)
                    TransitionToState(PlayerState.Crouching);
                //
                //TransitionToState(PlayerState.Hurt);
                if (currentHealth <= 0)
                    TransitionToState(PlayerState.Dead);
                break;

            case PlayerState.Attacking:
                if (!isAttacking)
                    TransitionToState(PlayerState.Idle);
                if (isBlocking)
                    TransitionToState(PlayerState.Blocking);
                //
                //TransitionToState(PlayerState.Hurt);
                if (currentHealth <= 0)
                    TransitionToState(PlayerState.Dead);
                break;

            case PlayerState.Blocking:
                if (!isBlocking)
                {
                    TransitionToState(PlayerState.Idle);
                }
                if (isAttacking)
                    TransitionToState(PlayerState.Attacking);
                //
                //TransitionToState(PlayerState.Hurt);
                if (currentHealth <= 0)
                    TransitionToState(PlayerState.Dead);
                break;

            case PlayerState.Hurt:
                TransitionToState(PlayerState.Idle);
                //Dodelat
                break;
            case PlayerState.Exhausted:
                break;

            case PlayerState.Super:
                TransitionToState(PlayerState.Idle);
                break;
        }


    }

    protected void TransitionToState(PlayerState newState)
    {
        previousState = currentState;
        currentState = newState;
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


    protected void Super()
    {
        SpecialAttack();
    }

    protected void Hurt()
    {
    }

    protected void Block()
    {
        isBlocking = true;
    }

    protected void CrouchAttack()
    {
        if (isAttacking)
        Attack();
    }

    protected void JumpAttack()
    {
        if (isAttacking)
        Attack();

    }
    string jumpString;
    //public void OnJump(InputAction.CallbackContext ctx) => jumpString = ctx.ReadValue<string>();

    protected void Attack()
    {
        if (previousState == PlayerState.Jumping || previousState == PlayerState.Crouching)
        {
            attackHeight = 0;
        }
        else
        {
            attackHeight = 2.5f;
        }
        BasicAttack();
        isAttacking = false;
        Move(); //mozna zmenit potom
    }

    protected void Punch(Collider col)
    {
        
        //var dmg = col.gameObject.GetComponentInParent<IDamageable>();

    }

    protected void Crouch()
    {
    }

    protected void Jump()
    {
        Move(); //zmìnit až budou animace

        if (!canJump) return;
        StaminaUse(20);
        if (notEnoughStamina) return;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        canJump = false;

    }
    
    protected virtual void SpecialAttack()
    {
        float specialAttackDamage = BasicAttackDamage * 2;

        Vector3 pos = character.transform.position + new Vector3(xDifference * turnedSide, 0, 0);
        var attack = Instantiate(basicAttackPrefab, pos, Quaternion.identity);
        attack.transform.SetParent(character.transform);

        var collision = attack.GetComponent<BasicAttackCollision>();
        if (collision != null)
        {
            collision.doubleDmg = 2;
        }

        Destroy(attack.gameObject, attackDuration);
    }
    

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            canJump = true;
            doubleJump = true;
        }
    }


    protected IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        canAttack = true;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext ctx)
    {
        
        isJumping = ctx.action.triggered;        
    }
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (canAttack && ctx.action.triggered)
        {
            isAttacking = true;
            StartCoroutine(AttackCooldown());
        }
    }

    public void OnBlock(InputAction.CallbackContext ctx)
    {
        isBlocking = ctx.action.triggered;
    }
    public void OnSuper1(InputAction.CallbackContext ctx)
    {
        if (canSuper && ctx.action.triggered)
        {
            SpecialAttack();
            StartCoroutine(AttackCooldown());
        }
    }

    protected IEnumerator SuperCooldown()
    {
        canSuper = false;
        yield return new WaitForSeconds(attackCooldown);
        isSuper1 = false;
        canSuper = true;
    }

}



