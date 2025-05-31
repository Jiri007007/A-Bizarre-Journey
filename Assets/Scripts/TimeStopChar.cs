using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimeStopChar : Player
{


    TimeStopManager timeStopManager; 
    public float timeStopDuration = 5f;

    float recoveryTime = 2f; //Dodìlat
    float shortestTimeStopTime = 1f;

    new void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
        timeStopManager = FindObjectOfType<TimeStopManager>();
    }

    protected override void SpecialAttack()
    {
        if (!canSuper) return;
        if (timeStopManager.isTimeStopped)
        {
            timeStopManager.ResumeTime();
            Debug.Log("RESUME");
        }
        else
        {
            Debug.Log("STOP");
            timeStopManager.StopTime(timeStopDuration);
        }


    }


}
