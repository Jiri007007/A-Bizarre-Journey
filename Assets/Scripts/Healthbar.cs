using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{

    [SerializeField]
    GameObject character;

    [SerializeField]
    private Slider healthBar;
    private float maxHealth;
    private float health;

    [SerializeField]
    private Slider staminaBar;
    private float maxStamina;
    private float stamina;


    private Character characterScript;

    // Start is called before the first frame update
    void Start()
    {
        
        characterScript = character.GetComponent<Character>();
        if (characterScript != null)
        {
            maxHealth = characterScript.maxHealth;
            healthBar.maxValue = maxHealth;

            maxStamina = characterScript.maxStamina;
            staminaBar.maxValue = maxStamina;
        }

        health = maxHealth;
        stamina = maxStamina;

    }

    // Update is called once per frame
    void Update()
    {
        health = characterScript.currentHealth;
        stamina = characterScript.currentStamina;
        //Debug.Log(health);
        //Debug.Log(healthBar.value);

        if (healthBar.value != health) 
        {
            healthBar.value = health;
        }
        if (staminaBar.value != stamina)
        {
            staminaBar.value = stamina;
        }
    }
}
