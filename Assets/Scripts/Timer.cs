using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    float gameTime;
    float currentGameTime;

    [SerializeField]
    TextMeshProUGUI timeOutput;
    float timeRound;


    void Start()
    {
        currentGameTime = gameTime;
    }

    // Update is called once per frame
    void Update()
    {

        currentGameTime -= Time.deltaTime;
        timeRound = currentGameTime;
        timeOutput.text = (Math.Round(timeRound,0)).ToString();
        if (currentGameTime <= 0)
        {

            var panel = GameObject.FindGameObjectWithTag("D_panel");
            var p = panel.GetComponentInChildren<TextMeshProUGUI>();
            panel.SetActive(true);
            p.text = "Time's up";

            Time.timeScale = 0;
        }
    }
}
