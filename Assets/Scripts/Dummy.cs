using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Character
{
    [SerializeField]
    float attackInterval;
    float time;
    Animator animator;

    // Update is called once per frame
    new void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        time += Time.deltaTime;
        if (time >= attackInterval) 
        {
            animator.SetBool("IsAttacking", true);
            BasicAttack();
            time = 0;
        }
        else
        {
            animator.SetBool("IsAttacking", false);
        }
    }
}
