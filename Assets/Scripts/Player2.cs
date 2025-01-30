using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : Player
{
    [SerializeField]
    protected BoxCollider longRangeAttack;
    [SerializeField]
    protected float projectileSpeed = 10f;
    protected override void SpecialAttack()
    {

        Vector3 pos = character.transform.position + new Vector3(xDifference * turnedSide, attackHeight, 0);
        var attack = Instantiate(longRangeAttack, pos, Quaternion.identity);

        // Ensure the attack has a Rigidbody2D (for 2D) or Rigidbody (for 3D)
        Rigidbody rb = attack.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = new Vector3(projectileSpeed * turnedSide, 0, 0); // Moves in the character's facing direction
        }

        Destroy(attack.gameObject, attackDuration);
    }
}
