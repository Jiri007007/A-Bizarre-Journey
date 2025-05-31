using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class CharBox : MonoBehaviour
{
    [SerializeField]
    Image image;
    [SerializeField]
    GameObject playerSelecting;
    [SerializeField]
    GameObject playerSelected;
    [SerializeField]
    TextMeshProUGUI playerText;

    CharSelect charactorSelector;

    public Character character { get; set; }




    // Start is called before the first frame update
    void Start()
    {
        if (character != null && character.image != null) image.sprite = character.image;
        playerSelected.SetActive(false);
        playerSelecting.SetActive(false);
        playerText.text = "";

        //Debug.Log(playerSelected + " " + playerSelecting + " " + playerText);

        charactorSelector = GetComponentInParent<CharSelect>();
        if (charactorSelector != null)
        {
            //Debug.Log("CharSelector není null");
            //chL.PlayerDeath += HandlePlayerDeath;
            //charactorSelector.ChangedCharacter += CurrentlySelecting;
            //charactorSelector.SelectedCharacter += CharacterSelected;
        }
    }
    public void SettupCharacterBox(Character ch)
    {
        if (ch.image is null) return;
        image.sprite = ch.image;
    }

    public void CurrentlySelecting(string player)
    {
        if (playerSelecting.activeSelf) return;
        playerSelecting.SetActive(true);
        playerSelected.SetActive(false);
        playerText.text = player;
    }
    public void CharacterSelected(string player)
    {
        playerSelecting.SetActive(false);
        playerSelected.SetActive(true);
        playerText.text = player + "*";
    }
    public void CharacterUnselected()
    {
        if (!playerSelecting.activeSelf && !playerSelected.activeSelf) return;
        playerSelecting.SetActive(false);
        playerSelected.SetActive(false);
        playerText.text = "";
    }


}
