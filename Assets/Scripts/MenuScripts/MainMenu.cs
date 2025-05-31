using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    //        chL.PlayerDeath += HandlePlayerDeath;

    public bool inMainMenu { get; set; } = true;
    [SerializeField]
    CharSelect characterSelectorPanel;

    [SerializeField]
    TextMeshProUGUI gameNameText;

    public event Action ResetCharacterSelectionScreen;

    float pulseSpeed = 2f;
    float minScale = 1f;
    float maxScale = 1.2f;

    void Start()
    {
        Time.timeScale = 1;
        gameObject.SetActive(inMainMenu);
        characterSelectorPanel.gameObject.SetActive(!inMainMenu);
        characterSelectorPanel.GoToMenu += ToMainMenu;
    }

    private void Update()
    {
        if (!inMainMenu) return;
        MenuTextPulse();
    }

    private void MenuTextPulse()
    {
        var scale = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * pulseSpeed) + 1) / 2);
        gameNameText.transform.localScale = new Vector3(scale, scale, 1);
    }

    void ToMainMenu()
    {
        inMainMenu = true;
        characterSelectorPanel.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
    public void ToCharSelect()
    {
        //Debug.Log("Wahu?");
        inMainMenu = false;
        characterSelectorPanel.gameObject.SetActive(true);
        ResetCharacterSelectionScreen?.Invoke();
        StartCoroutine(SetSelf());
    }

    public void ExitGame()
    {
        //SaveGameStat();
    #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
    #else
    Application.Quit();
    #endif
    }

    private IEnumerator SetSelf()
    {
        yield return null;
        gameObject.SetActive(false);
    }
}
