using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    float maxHealth { get; set; }
    float currentHealth { get; set; }


    void Damage(float damageDealt);

    void Die();
}
