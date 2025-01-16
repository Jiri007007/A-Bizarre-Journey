using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    Character characterLeft;
    [SerializeField]
    Character characterRight;


    // Start is called before the first frame update
    void Start()
    {
        SpawnCharacters();

    }

    private void SpawnCharacters()
    {

            Vector3 pos = new Vector3(-4, 1, 0);
            Instantiate(characterLeft, pos, Quaternion.identity);
            Vector3 pos2 = new Vector3(4, 1, 0);
            Instantiate(characterRight, pos2, Quaternion.identity);
        //attack.transform.SetParent(character.transform);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
