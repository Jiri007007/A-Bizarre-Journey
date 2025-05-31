using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{

    Character character;

    GameManager gameManager;

    [SerializeField]
    private Slider healthBar;
    private float maxHealth;
    private float health;

    [SerializeField]
    private Slider staminaBar;
    private float maxStamina;
    private float stamina;

    [SerializeField]
    public RectTransform bar;
    bool alignRight = true;
    private RectTransform canvas;
    private float canvasWidth;
    private float healthBarWidth;

    [SerializeField]
    private Image image;

    private float setResolution = 1920f;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        character = GetComponentInParent<Character>();
        if (character != null)
        {
            maxHealth = character.maxHealth;
            healthBar.maxValue = maxHealth;

            maxStamina = character.maxStamina;
            staminaBar.maxValue = maxStamina;
            //image.sprite = character.image;
        }

        health = maxHealth;
        stamina = maxStamina;

        if (character == gameManager.check)
        {
            alignRight = false;
        }
        else
        {
            alignRight = true;
        }
        AdjustPosition();
    }

    private void AdjustPosition()
    {
        canvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
        healthBarWidth = bar.rect.width;
        bar.ForceUpdateRectTransforms();


        if (alignRight)
        {
            bar.anchorMin = new Vector2(1, 1);
            bar.anchorMax = new Vector2(1, 1);
            bar.pivot = new Vector2(0, 0.5f);

            // Correct positioning after rotation
            bar.anchoredPosition = new Vector2(-6, -2);
            bar.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            bar.anchorMin = new Vector2(0, 1);
            bar.anchorMax = new Vector2(0, 1);
            bar.pivot = new Vector2(0, 0.5f);

            // Normal positioning
            bar.anchoredPosition = new Vector2(15, -2);
            bar.localRotation = Quaternion.identity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        health = character.currentHealth;
        stamina = character.currentStamina;

        if (healthBar.value != health)
        {
            healthBar.value = health;
        }
        if (staminaBar.value != stamina)
        {
            staminaBar.value = stamina;
        }

        
        //Debug.Log(character.image);
        //AdjustSize();
    }

    private void AdjustSize()
    {
        float scaleFactor = canvasWidth / setResolution * 50;
        bar.localScale = new Vector3(scaleFactor, scaleFactor, 1);
        float offsetX = (bar.rect.width / 2) * scaleFactor; // Dynamic positioning

        if (alignRight)
        {
            bar.anchoredPosition = new Vector2(-offsetX, -(bar.rect.height / 2) * scaleFactor);
        }
        else
        {
            bar.anchoredPosition = new Vector2(offsetX, -(bar.rect.height / 2) * scaleFactor);
        }
    }
}
