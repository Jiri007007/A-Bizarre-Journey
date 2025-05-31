using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(CharacterController))]
public abstract class Character : MonoBehaviour, IDamageable
{
    [SerializeField]
    protected GameObject character;

    [SerializeField]
    protected Sprite sprite;
    public Sprite image => sprite;

    protected Rigidbody rb;
    protected Animator animator;

    [SerializeField]
    protected BoxCollider basicAttackPrefab;


    [SerializeField]
    protected string characterName;
    public string chName => characterName;


    protected float xDifference = 1.1f;
    [SerializeField]
    protected float turnedSide;

    protected float hp;
    protected float attackDuration = 0.5f;
    protected float stamina;

    [SerializeField]
    protected float staminaBeforeRegenDelay;
    [SerializeField]
    protected float staminaRegenDelay;

    protected float staminareFreshRate = 1f;
    protected bool staminaRecentUse = false;


    public Character opponent
    {
        get => opp; set => opp = value;
    }
    protected Character opp;

    public bool isBlocking
    {
        get => block; set => block = value;
    }
    protected bool block = false;

    protected bool notEnoughStamina;



    [field: SerializeField] public float maxHealth { get; set; }

    [field: SerializeField] public float maxStamina { get; set; }
    public float currentHealth { get { return hp; } set {hp = value;} }

    public float currentStamina { get { return stamina; } set { stamina = value; } }

    protected float attackHeight = 2.5f;

    GameObject panel;
    TextMeshProUGUI p;


    public event Action<Character> PlayerDeath;


    protected void Start()
    {
        hp = maxHealth;
        stamina = maxStamina;
        rb = character.GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        if (gameObject.GetComponent<Player>() != null)
        {
            panel.SetActive(false);
        }

    }

    protected void Awake()
    {
        panel = GameObject.FindGameObjectWithTag("D_panel");
        p = panel.GetComponentInChildren<TextMeshProUGUI>();
    }

    public virtual void Damage(float damageDealt)
    {
        if (this is Player player && player.isBlocking)
        {
            damageDealt = 0;
        }

        currentHealth -= damageDealt;
  
        if (currentHealth <= 0)

            //die animation - corutine prob
            Die();
    }


    protected void CheckOpponentPosition()
    {
        if (opponent == null || gameObject == null) return;
        if (gameObject.transform.position.x <= opponent.transform.position.x)
        {
            gameObject.transform.eulerAngles = new Vector3(
                gameObject.transform.eulerAngles.x,
                90,
                gameObject.transform.eulerAngles.z
                );
            turnedSide = 1;
        }
        else
        {
            gameObject.transform.eulerAngles = new Vector3(
                gameObject.transform.eulerAngles.x,
                270,
                gameObject.transform.eulerAngles.z
                );
            turnedSide = -1;
        }
        
    }



    public virtual void StaminaUse(float staminaSpent) 
    {
        if (staminaSpent > currentStamina)
        {
            notEnoughStamina = true;
        } else
        {
            notEnoughStamina = false;
        }
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
                stamina += 1 * staminareFreshRate;
                StartCoroutine(Delay(staminaRegenDelay, false));
            }
            else if (stamina >= maxStamina)
            {
                stamina = maxStamina;
                staminareFreshRate = 1f;
            }
        }
    

    }

    protected void Update()
    {
        CheckOpponentPosition();
    }

    private void Exhausted()
    {
        staminareFreshRate = 0.5f;
    }

    

    public virtual void Die() 
    {
        PlayerDeath.Invoke(this);
    }

    protected virtual void HealStamina() 
    {

    }

    public void BasicAttack() 
    {
        Vector3 pos = character.transform.position + new Vector3(xDifference * turnedSide, attackHeight, 0);
        var attack = Instantiate(basicAttackPrefab, pos, Quaternion.identity);
        attack.transform.SetParent(character.transform);

        Destroy(attack.gameObject,attackDuration);
    }


    // Start is called before the first frame update

}
