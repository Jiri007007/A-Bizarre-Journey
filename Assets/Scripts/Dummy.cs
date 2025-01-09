using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Character
{
    [SerializeField]
    float attackInterval;
    float time;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= attackInterval) 
        {
            BasicAttack();
            time = 0;
        }
    }
}
