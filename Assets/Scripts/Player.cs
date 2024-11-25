using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
   [SerializeField]
   GameObject player;
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float jumpForce;
    

    Rigidbody rb;


    [field:SerializeField]public float maxHealth { get; set; }
    public float currentHealth { get; set; }

    public void Damage(float damageDealt)
    {
    }

    public void Die()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = player.GetComponent<Rigidbody>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = player.transform.position;

        if (Input.GetKeyDown(KeyCode.UpArrow)) 
        {
            if (position.y > 0.6) return;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
        { 

            if (Input.GetKey(KeyCode.RightArrow))
            {
                position.x += walkSpeed * Time.deltaTime;
            }
            else if(Input.GetKey(KeyCode.LeftArrow))
            {
                position.x -= walkSpeed * Time.deltaTime;
            }

            
            player.transform.position = position;
        }
        return;
    }
}
