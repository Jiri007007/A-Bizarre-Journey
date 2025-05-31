using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttackCollision : MonoBehaviour
{
    public Player player;

    public float doubleDmg = 0;


    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Ground") return;
        
     
        var dmg = col.gameObject.GetComponentInParent<IDamageable>();

        GameObject thisGameObj = gameObject;
        var player = thisGameObj.GetComponentInParent<Player>();
        /*if (player != null)
        {
            
        .Log(player);
        }*/
        float damageAmount = player != null ? player.BasicAttackDamage * doubleDmg : 20f; 

        if (doubleDmg != 1) 
        {
            Debug.Log("Double");
        }
        else
        {
            Debug.Log("Normal");
        }
        if (dmg != null)
        {
            dmg.Damage(damageAmount);
        }
        doubleDmg = 0;
    }
}
