using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Character : MonoBehaviour, IDamageable
{

    [SerializeField]
    protected GameObject character;

    protected Rigidbody rb;

    [SerializeField]
    protected BoxCollider basicAttackPrefab;


    protected float xDifference = 1.1f;
    [SerializeField]
    protected float turnedSide = 1;

    protected float hp;
    protected float attackDuration = 0.5f;
    protected float stamina;

    [SerializeField]
    protected float staminaBeforeRegenDelay;
    [SerializeField]
    protected float staminaRegenDelay;

    protected bool staminaRecentUse = false;




    protected GameObject opponent;


    [field: SerializeField] public float maxHealth { get; set; }

    [field: SerializeField] public float maxStamina { get; set; }
    public float currentHealth { get { return hp; } set {hp = value;} }

    public float currentStamina { get { return stamina; } set { stamina = value; } }

    


    void Start()
    {
        hp = maxHealth;
        stamina = maxStamina;
        rb = character.GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    public virtual void Damage(float damageDealt)
    { 
        currentHealth -= damageDealt;
  
      //  Debug.Log(currentHealth);
        if (currentHealth <= 0)
            Die();
    }


    public virtual void StaminaUse(float staminaSpent) 
    {
        currentStamina -= staminaSpent;
        staminaRecentUse = true;
        if (currentStamina <= 0)
            Exhausted();

        StartCoroutine(Delay(staminaBeforeRegenDelay, true));
    }
    protected IEnumerator Delay(float delayTime, bool heal)
    {
        yield return new WaitForSeconds(delayTime);
        if (heal)
        {
            
            HealStamina();
            staminaRecentUse = false;
        }
        else if(!heal)
        {
            if (stamina < maxStamina && !staminaRecentUse)
            {
                stamina++;
                StartCoroutine(Delay(staminaRegenDelay, false));
            }
        }
    

    }

    private void Exhausted()
    {
    }

    

    public virtual void Die() 
    {
        Destroy(gameObject);
        //GameEnd / respawn (nextRound)
    }

    protected virtual void HealStamina() 
    {

    }

    public void BasicAttack() 
    {
        Vector3 pos = character.transform.position + new Vector3(xDifference * turnedSide, 0, 0);
        var attack = Instantiate(basicAttackPrefab, pos, Quaternion.identity);
        attack.transform.SetParent(character.transform);

        Destroy(attack.gameObject,attackDuration);
    }


    // Start is called before the first frame update

}
