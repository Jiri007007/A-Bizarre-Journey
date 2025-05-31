using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    //public event Action PauseGame;

    [SerializeField]
    Timer timer;

    [SerializeField]
    Button pauseButton;
    [SerializeField]
    GameObject pauseScreen;

    GameManager gameManager;
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        pauseButton.enabled = true;
        pauseButton.gameObject.SetActive(true);
        pauseScreen.SetActive(false);
        timer.gamePaused = false;
    }

    
    public void PauseButtonPressed()
    {
        pauseButton.enabled = false;
        pauseButton.gameObject.SetActive(false);
        pauseScreen.SetActive(true);
        timer.gamePaused = true;
    }

    public void ResumeButtonPressed()
    {
        pauseButton.enabled = true;
        pauseButton.gameObject.SetActive(true);
        pauseScreen.SetActive(false);
        timer.gamePaused = false;
    }

    public void MenuButtonPressed() 
    {
        //Dodìlat
        SceneManager.LoadScene("CharacterSelectorScreen");
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame && gameManager.canPress)
        {
            if (timer.gamePaused) ResumeButtonPressed();
            else PauseButtonPressed();
        }
    }

}
