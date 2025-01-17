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
using Unity.VisualScripting;
using UnityEngine.UIElements;



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
    protected bool isAttacking;
    protected bool gettingHit;
    protected bool doubleJump;

    protected int walkingDirection;

    Animator animator;
    

    new void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
    }



    // Update is called once per frame
    void Update()
    {
        //CheckOpponentPosition();


        HandleStateTransitions();
        HandleStateActions();
        
        
        //Movement();
        //HealStamina();
    }


    protected void HandleStateActions()
    {
        // Idle, Walking, Jumping, JumpingAttack, Crouching, CrouchingAttack, Attacking, Blocking, Hurt, Dead, Exhausted, Super 
        Debug.Log(currentState);
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
                Block();
                break;

            case PlayerState.Hurt:
                Hurt();
                break;

            case PlayerState.Super:
                Super();
                break;

        }

    }

    protected void Move()
    {
        Vector3 position = character.transform.position;

        if (Input.GetKey(KeyCode.D))
        {
            position.x += walkSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.A))
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
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
                    TransitionToState(PlayerState.Walking);
                if (Input.GetKey(KeyCode.S))
                    TransitionToState(PlayerState.Crouching);
                if (Input.GetKeyDown(KeyCode.W) && currentStamina >= 0 && !isJumping)
                    TransitionToState(PlayerState.Jumping);
                if (Input.GetKey(KeyCode.F))
                    TransitionToState(PlayerState.Blocking);
                if (Input.GetKeyDown(KeyCode.E))
                    TransitionToState(PlayerState.Attacking);
                //if (Input.GetKeyDown(KeyCode.G))
                //TransitionToState(PlayerState.Super);
                //
                //TransitionToState(PlayerState.Hurt);
                if (currentHealth <= 0)
                    TransitionToState(PlayerState.Dead);

                break;

            case PlayerState.Walking:

                if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
                    TransitionToState(PlayerState.Idle);
                if (Input.GetKeyDown(KeyCode.W) && currentStamina >= 0 && !isJumping)
                    TransitionToState(PlayerState.Jumping);
                //
                //TransitionToState(PlayerState.Hurt);
                if (currentHealth <= 0)
                    TransitionToState(PlayerState.Dead);
                break;

            case PlayerState.Jumping:
                if (!isJumping)
                    TransitionToState(PlayerState.Idle);
                if (Input.GetKeyDown(KeyCode.E))
                    TransitionToState(PlayerState.JumpingAttack);
                //
                //TransitionToState(PlayerState.Hurt);
                if (currentHealth <= 0)
                    TransitionToState(PlayerState.Dead);
                break;

            case PlayerState.JumpingAttack:
                if (!isJumping)
                    TransitionToState(PlayerState.Idle);
                //
                //TransitionToState(PlayerState.Hurt);
                if (currentHealth <= 0)
                    TransitionToState(PlayerState.Dead);
                break;

            case PlayerState.Crouching:
                if (!Input.GetKey(KeyCode.S))
                    TransitionToState(PlayerState.Idle);
                if (Input.GetKeyDown(KeyCode.E))
                    TransitionToState(PlayerState.CrouchingAttack);
                //
                //TransitionToState(PlayerState.Hurt);
                if (currentHealth <= 0)
                    TransitionToState(PlayerState.Dead);
                break;

            case PlayerState.CrouchingAttack:
                if (!Input.GetKeyDown(KeyCode.E))
                    TransitionToState(PlayerState.Crouching);
                //
                //TransitionToState(PlayerState.Hurt);
                if (currentHealth <= 0)
                    TransitionToState(PlayerState.Dead);
                break;

            case PlayerState.Attacking:
                if (!Input.GetKeyDown(KeyCode.E))
                    TransitionToState(PlayerState.Idle);
                if (Input.GetKeyDown(KeyCode.F))
                    TransitionToState(PlayerState.Blocking);
                //
                //TransitionToState(PlayerState.Hurt);
                if (currentHealth <= 0)
                    TransitionToState(PlayerState.Dead);
                break;

            case PlayerState.Blocking:
                if (!Input.GetKey(KeyCode.F))
                    TransitionToState(PlayerState.Idle);

                if (Input.GetKeyDown(KeyCode.E))
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
                //TransitionToState(PlayerState.Idle);
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
        //SpecialAttack();
    }

    protected void Hurt()
    {
    }

    protected void Block()
    {
    }

    protected void CrouchAttack()
    {
        Attack();
    }

    protected void JumpAttack()
    {
        Attack();
    }

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

        if (isJumping) return;
        StaminaUse(20);
        if (notEnoughStamina) return;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isJumping = true;

    }
    /*
    protected void SpecialAttack()
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
    */

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            doubleJump = true;
        }
    }
}



