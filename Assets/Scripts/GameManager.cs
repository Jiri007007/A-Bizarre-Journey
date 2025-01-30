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
    [SerializeField]
    GameObject middle;

    Character chL;
    Character chR;


    // Start is called before the first frame update
    void Start()
    {
        SpawnCharacters();
        setUpMiddle();
    }

    private void setUpMiddle()
    {
        middle.SetActive(true);
        MiddlePos();
    }

    private void SpawnCharacters()
    {

            Vector3 pos = new Vector3(-4, 1, 0);
            chL = Instantiate(characterLeft, pos, Quaternion.identity);
            Vector3 pos2 = new Vector3(4, 1, 0);
            chR = Instantiate(characterRight, pos2, Quaternion.identity);
        //attack.transform.SetParent(character.transform);

            chL.opponent = chR;
            chR.opponent = chL;
    }

    // Update is called once per frame
    void Update()
    {
        MiddlePos();
    }

    private void MiddlePos()
    {
        if (chL == null || chR == null) return;  
        Vector3 temp = new Vector3((chR.transform.position.x + chL.transform.position.x)/2, middle.transform.position.y, middle.transform.position.z);
        middle.transform.position = temp;
    }
}
