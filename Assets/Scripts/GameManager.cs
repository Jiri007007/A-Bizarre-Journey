using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //[SerializeField]
    Character characterLeft;
    //[SerializeField]
    Character characterRight;
    [SerializeField]
    GameObject middle;


    [SerializeField]
    List<Character> allCharacters = new List<Character>();

    Character chL;
    Character chR;

    int chLWinScore;
    int chRWinScore;
    int drawCount;

    [SerializeField]
    int maxDrawCount = 3;
    [SerializeField]
    int pointsToWin = 2;

    int p1DeviceIndex;
    int p2DeviceIndex;

    string p1InputName;
    string p2InputName;

    [SerializeField]
    Button PauseButton;
    [SerializeField]
    GameObject PauseScreen;

    bool noPressing = true;
    public bool canPress => !noPressing;

    int q = 0;
    //g-gamepad j-joystick  count

    public Character check => chL;
    private PlayerInputManager playerInputManager;


    [SerializeField]
    GameObject deathPanel;

    Timer timer;

    // Start is called before the first frame update

    void Start()
    {
        SetupStage();
        SetStage();
        StartCoroutine(DelayStart(false));
        timer.enabled = true;
    }
    private void Awake()
    {
        LoadGamePrefabs();
    }

    private void SetupStage()
    {
        timer = FindObjectOfType<Timer>();
        timer.TimeOut += TimeOut;
        timer.enabled = false;
        CorrectKeyboardName(p1InputName);
        CorrectKeyboardName(p2InputName);
        chRWinScore = chLWinScore = drawCount = 0;
    }

    void SetStage()
    {
        ResetTimer();
        SpawnCharacters();
        setUpMiddle();
    }

    private void LoadGamePrefabs()
    {
        string p1CharName = PlayerPrefs.GetString("P1Character", "");
        string p2CharName = PlayerPrefs.GetString("P2Character", "");

        characterLeft = allCharacters.Find(c => c.chName == p1CharName);
        characterRight = allCharacters.Find(c => c.chName == p2CharName);

        p1InputName = PlayerPrefs.GetString("InputDevice1", "Keyboard1");
        p2InputName = PlayerPrefs.GetString("InputDevice2", "Keyboard2");

        p1DeviceIndex = PlayerPrefs.GetInt("p1DeviceIndex", 0);
        p2DeviceIndex = PlayerPrefs.GetInt("p2DeviceIndex", 0);


        Debug.Log("Ch:  " + p1CharName + " device: " + p1InputName + "index: " + p1DeviceIndex);
        Debug.Log(characterLeft);
        Debug.Log(characterLeft.chName);
    }

    private InputDevice GetInputDeviceType(string name, int deviceIndex)
    {

        InputDevice p;

        if (name == "Joystick")
        {
            p = Joystick.all[deviceIndex];
        }
        else if (name == "Gamepad")
        {
            p = Gamepad.all[deviceIndex];
        }
        else
        {
            p = Keyboard.current;
        }

        return p;
    }

    private void SpawnCharacters()
    {
        var player1 = PlayerInput.Instantiate(characterLeft.gameObject, controlScheme: p1InputName, pairWithDevice: GetInputDeviceType(p1InputName, p1DeviceIndex));
        var player2 = PlayerInput.Instantiate(characterRight.gameObject, controlScheme: p2InputName, pairWithDevice: GetInputDeviceType(p2InputName, p2DeviceIndex));


        player1.transform.position = new Vector3(-4, 0.1f, 0);
        player2.transform.position = new Vector3(4, 0.1f, 0);

        chL = player1.GetComponent<Character>();
        chR = player2.GetComponent<Character>();

        chL.PlayerDeath += HandlePlayerDeath;
        chR.PlayerDeath += HandlePlayerDeath;

            chL.opponent = chR;
            chR.opponent = chL;

    }

    private void CorrectKeyboardName(string inputName)
    {

        if (inputName == "Keyboard")
        {
            q++;
            inputName += q.ToString();
            Debug.Log(inputName + "     QWERTY");
        }
    }

    // Update is called once per frame
    void Update()
    {
        MiddlePos();
    }


    private void setUpMiddle()
    {
        middle.SetActive(true);
        MiddlePos();
    }
    private void MiddlePos()
    {
        if (chL == null || chR == null) return;
        Vector3 temp = new Vector3((chR.transform.position.x + chL.transform.position.x) / 2, middle.transform.position.y, middle.transform.position.z);
        middle.transform.position = temp;
    }

    private void HandlePlayerDeath(Character dChar)
    {

        Time.timeScale = 0;
        deathPanel.SetActive(true);
        TextMeshProUGUI p = deathPanel.GetComponentInChildren<TextMeshProUGUI>();
        Character ch;
        string playerNumber = "";
        if (dChar == chL)
        {
            ch = chR;
            playerNumber = "Player 2";
            chRWinScore++;
        }
        else
        {
            ch = chL;
            playerNumber = "Player 1";
            chLWinScore++;
        }
        p.text = ch.chName + "\n("+ playerNumber +")" +"\n W I N S";
        ResetTimer();
        EndRound(ch);
        //GameEnd / respawn (nextRound)

    }

    private void EndRound(Character cha)
    {

        Destroy(chL.gameObject);
        Destroy(chR.gameObject);
        PauseButton.gameObject.SetActive(false);
        if (cha != null && (chLWinScore >= pointsToWin || chRWinScore >= pointsToWin))
        {
            //WIN;
            Debug.Log("win");
            Debug.Log(chLWinScore + " " + chRWinScore + "" + pointsToWin);
            SceneManager.LoadScene("CharacterSelectorScreen");

        }
        else if (drawCount >= maxDrawCount)
        {
            if (chLWinScore > chRWinScore || chRWinScore > chLWinScore)
            {
                //DRAW - WIN;
                Debug.Log("drawwin");
                SceneManager.LoadScene("BattleScene");

            }
            else
            {
                //DRAW - END;
                Debug.Log("drawend");
                SceneManager.LoadScene("BattleScene");

            }
        }
        else
        {
            //Debug.Log("akaelhasadjhsfkdyhaskjafkhajk");
            StartCoroutine(DelayStart(true)); //START ROUND;
        }
    }

    private IEnumerator DelayStart(bool wait)
    {
        deathPanel.SetActive(true);
        if (wait)
        yield return new WaitForSecondsRealtime(1f);
        NextRoundText();
        yield return new WaitForSecondsRealtime(1.25f);
        NextRoundStartText();
        yield return new WaitForSecondsRealtime(0.2f);
        if (wait)
        SetStage();
        StartRound();
        deathPanel.SetActive(false);
    }





    private void NextRoundText()
    {
        var p = deathPanel.GetComponentInChildren<TextMeshProUGUI>();
        var roundNum = drawCount + chLWinScore + chRWinScore + 1;
        p.text = "Round: " + roundNum;
    }

    private void NextRoundStartText()
    {
        var p = deathPanel.GetComponentInChildren<TextMeshProUGUI>();
        p.text = "FIGHT";
    }


    private void ResetTimer()
    {
        Time.timeScale = 0;
        timer.gameStarted = false;
        timer.currentGameTime = 0;
        noPressing = true;
    }
    public void TimeOut()
    {
        var p = deathPanel.GetComponentInChildren<TextMeshProUGUI>();
        deathPanel.SetActive(true);
        p.text = "Time's up!" + "\nD R A W";
        drawCount++;
        ResetTimer();
        EndRound(null);
    }

    void StartRound()
    {
        noPressing = false;
        chL.canInput = chR.canInput = !noPressing;
        Time.timeScale = 1;
        timer.gameStarted = true;
        timer.currentGameTime = timer.gTime;
        PauseButton.gameObject.SetActive(true);
    }

}
