using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : Player
{
    [SerializeField]
    protected BoxCollider longRangeAttack;
    [SerializeField]
    protected float projectileSpeed = 10f;

    private float nextActionTime = 0f;
    private float interval = 5f;


    bool canSuper = true;

     private new void Update()
    {
        base.Update();
        if (Time.time >= nextActionTime && !canSuper)
        {
            nextActionTime = Time.time + interval;
            canSuper = true;
        }


    }
    protected override void SpecialAttack()
    {
        if (!canSuper) return;

        Vector3 pos = character.transform.position + new Vector3(xDifference * turnedSide, attackHeight, 0);
        var attack = Instantiate(longRangeAttack, pos, Quaternion.identity);

        // Ensure the attack has a Rigidbody2D (for 2D) or Rigidbody (for 3D)
        Rigidbody rb = attack.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = new Vector3(projectileSpeed * turnedSide, 0, 0); // Moves in the character's facing direction
        }

        Destroy(attack.gameObject, attackDuration);
        canSuper = false;
    }
}
