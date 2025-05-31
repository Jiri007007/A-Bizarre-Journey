using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    float gameTime = 120;
    public float gTime => gameTime;
    public float currentGameTime { get; set; }

    [SerializeField]
    TextMeshProUGUI timeOutput;
    float timeRound;

    public bool gamePaused { get; set; } = false;

    public bool gameStarted { get; set; } = false;

    public event Action TimeOut;

    void Start()
    {
        currentGameTime = gameTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (gamePaused || !gameStarted)
        {
            Time.timeScale = 0;
            return;
        }
        if (Time.timeScale != 1)
            Time.timeScale = 1;

        currentGameTime -= Time.deltaTime;
        timeRound = currentGameTime;
        timeOutput.text = (Math.Round(timeRound,0)).ToString();
        if (currentGameTime <= 0)
        {
            TimeOut.Invoke();
        }
    }
}
