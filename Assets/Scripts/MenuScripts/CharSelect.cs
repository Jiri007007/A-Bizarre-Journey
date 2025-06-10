using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Reflection;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharSelect : MonoBehaviour
{


    public string inputDevice1 { get; set; } = "Keyboard1";
    public string inputDevice2 { get; set; } = "Keyboard2";

    public InputControl inpC1 { get; set; } = Keyboard.current;

    public InputControl inpC2 { get; set; } = Keyboard.current;

    public int p1ControlerJoystickIndex { get; set; }

    public int p2ControlerJoystickIndex { get; set; }

    Character p1Character;
    Character p2Character;

    bool gameStarted = false;

    [SerializeField]
    List<Character> characters = new List<Character>();

    List<CharBox> charBoxes = new List<CharBox>();

    [SerializeField]
    CharBox characterBoxPrefab;


    [SerializeField]
    GameObject characterBox;

    [SerializeField]
    TextMeshProUGUI text1, text2;

    [SerializeField]
    MainMenu mainMenuPanel;
    //public event Action<string> ChangedCharacter;
    //public event Action<string> SelectedCharacter;

    int player1Index;
    int player2Index;
    int p1LastIndex;
    int p2LastIndex;

    bool p1Selected = false;
    bool p2Selected = false;

    int charactersInRow;


    float joystickCooldown = 0.5f;
    float joystickTimeDelay = 0;

    public event Action GoToMenu;



    void Start()
    {
        BuildCharacterBox();
        if (charBoxes.Count < 1 || charBoxes is null)
            Debug.Log("Nedostatek postav");
        Invoke(nameof(SetCharacterIndexes), 0.01f);
        mainMenuPanel.ResetCharacterSelectionScreen += ResetSelectedCharacters;
    }

    void ResetSelectedCharacters()
    {
        p1Selected = p2Selected = false;
        charBoxes.ForEach(chB => chB.CharacterUnselected());
        SetCharacterIndexes();
    }
    private void SetCharacterIndexes()
    {
        player1Index = 0;
        if (charBoxes.Count > charactersInRow)
        {
            player2Index = charactersInRow - 1;
        }
        else
        {
            player2Index = charBoxes.Count - 1;
        }
        p1LastIndex = player1Index; p2LastIndex = player2Index;

        charBoxes[player1Index].CurrentlySelecting("Player 1");
        charBoxes[player2Index].CurrentlySelecting("Player 2");
    }

    private void BuildCharacterBox()
    {
        RectTransform charRT = characterBoxPrefab.GetComponent<RectTransform>();
        RectTransform boxRT = characterBox.GetComponent<RectTransform>();
        float charBoxPrefabWidth = charRT.rect.width;
        float charBoxPrefabHeight = charRT.rect.height;

        float yOverflow = 0;
        float xReset = 0;
        var j = 0;


        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i] != null)
            {
                var charBox = Instantiate(characterBoxPrefab, characterBox.transform);
                charBoxes.Add(charBox);
                charBoxes[i].character = characters[i];
                charBoxes[i].SettupCharacterBox(characters[i]);
                //charBox.transform.localScale = new Vector3(3/4, 3/4, 3/4);
                if (boxRT.localPosition.x + i * charBoxPrefabWidth * 4 / 3 - xReset > boxRT.rect.width)
                {
                    yOverflow += charBoxPrefabHeight * 4 / 3;
                    xReset = i * charBoxPrefabWidth * 4 / 3;
                    j++;
                    charactersInRow = i / j;
                }
                var borderV = new Vector3(boxRT.rect.width / 2 - charBoxPrefabWidth / 2, -boxRT.rect.height / 2, 0);
                var spacing = new Vector3(i * charBoxPrefabWidth * 4 / 3 - xReset, -yOverflow, 0);
                charBox.transform.localPosition = boxRT.localPosition - borderV + spacing;

            }
        }
    }

    public void MainMenuButtonPressed() 
    {
        GoToMenu?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        //možná zmìnit
        /*
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MainMenuButtonPressed();
        }
        */
        if (mainMenuPanel.inMainMenu)
            return;

        if (p1Selected && p2Selected && !gameStarted)
        {
            gameStarted = true;
            StartGame();
        }
        if (!gameStarted)
        {
            SetInputInfoText();
            UpdateCharacterIndexes();
        }
    }

    private void StartGame()
    {
        p1Character = charBoxes[player1Index].character;
        p2Character = charBoxes[player2Index].character;

        //Debug.Log(p1Character.chName + "  TOHLE SMRDÍ  " + p2Character.chName);
        SaveGamePrefs();
    }

    private void SaveGamePrefs()
    {
        PlayerPrefs.SetString("P1Character", p1Character.chName);
        PlayerPrefs.SetString("P2Character", p2Character.chName);

        PlayerPrefs.SetString("InputDevice1", inputDevice1);
        PlayerPrefs.SetString("InputDevice2", inputDevice2);

        PlayerPrefs.SetInt("p1DeviceIndex", p1ControlerJoystickIndex);
        PlayerPrefs.SetInt("p1DeviceIndex", p1ControlerJoystickIndex);

        //PlayerPrefs.Save();

        SceneManager.LoadScene("BattleScene");
    }

    private void UpdateCharacterIndexes()
    {
        var p1Res = CheckCharacterChangeInputs(inpC1, player1Index, p1Selected);
        var p2Res = CheckCharacterChangeInputs(inpC2, player2Index, p2Selected);
        player1Index = p1Res.Item1;
        player2Index = p2Res.Item1;
        p1Selected = p1Res.Item2;
        p2Selected = p2Res.Item2;
        

        p1LastIndex = UpdateCharacterBoxes(player1Index, p1LastIndex, p1Selected, "Player 1");
        p2LastIndex = UpdateCharacterBoxes(player2Index, p2LastIndex, p2Selected, "Player 2");
    }

    int UpdateCharacterBoxes(int playerIndex, int lastIndex, bool selected, string playerText)
    {
        if (selected)
        {
            charBoxes[playerIndex].CharacterSelected(playerText);
        }
        else
        {
            charBoxes[lastIndex].CharacterUnselected();
            charBoxes[playerIndex].CurrentlySelecting(playerText);
        }

        return playerIndex;
    }

    private (int, bool) CheckCharacterChangeInputs(InputControl ic, int playerIndex, bool selected)
    {
        var playerLastInd = playerIndex;

        if (ic is Keyboard keyboard)
        {

            if (inpC1 == inpC2 && inpC1 == Keyboard.current)
            {
                if (!selected)
                {
                    if (playerIndex == player1Index)
                    {
                        if (keyboard.aKey.wasPressedThisFrame)
                            playerIndex--;
                        if (keyboard.dKey.wasPressedThisFrame)
                            playerIndex++;
                        if (keyboard.wKey.wasPressedThisFrame)
                            playerIndex -= charactersInRow;
                        if (keyboard.sKey.wasPressedThisFrame)
                            playerIndex += charactersInRow;
                        if (keyboard.eKey.wasPressedThisFrame)
                            selected = true;
                    }
                    else
                    {
                        if (keyboard.leftArrowKey.wasPressedThisFrame)
                            playerIndex--;
                        if (keyboard.rightArrowKey.wasPressedThisFrame)
                            playerIndex++;
                        if (keyboard.upArrowKey.wasPressedThisFrame)
                            playerIndex -= charactersInRow;
                        if (keyboard.downArrowKey.wasPressedThisFrame)
                            playerIndex += charactersInRow;
                        if (keyboard.numpad0Key.wasPressedThisFrame)
                            selected = true;
                    }
                }
                else
                {
                    if (player1Index == playerIndex)
                    {
                        if (keyboard.eKey.wasPressedThisFrame)
                            selected = false;
                    }
                    else
                    {
                        if (keyboard.numpad0Key.wasPressedThisFrame)
                            selected = false;
                    }
                }
            }
            else
            {
                if (!selected)
                {
                    if (keyboard.leftArrowKey.wasPressedThisFrame || keyboard.aKey.wasPressedThisFrame)
                        playerIndex--;
                    if (keyboard.rightArrowKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame)
                        playerIndex++;
                    if (keyboard.upArrowKey.wasPressedThisFrame || keyboard.wKey.wasPressedThisFrame)
                        playerIndex -= charactersInRow;
                    if (keyboard.downArrowKey.wasPressedThisFrame || keyboard.sKey.wasPressedThisFrame)
                        playerIndex += charactersInRow;
                    if (keyboard.eKey.wasPressedThisFrame || keyboard.numpad0Key.wasPressedThisFrame)
                        selected = true;
                }
                else
                {
                    if (keyboard.eKey.wasPressedThisFrame || keyboard.numpad0Key.wasPressedThisFrame)
                        selected = false;
                }

            }

        }
        else if (ic is Gamepad gamepad)
        {
            if (!selected)
            {
                if (gamepad.dpad.left.wasPressedThisFrame || gamepad.leftStick.left.wasPressedThisFrame)
                    playerIndex--;
                if (gamepad.dpad.right.wasPressedThisFrame || gamepad.leftStick.right.wasPressedThisFrame)
                    playerIndex++;
                if (gamepad.dpad.up.wasPressedThisFrame || gamepad.leftStick.up.wasPressedThisFrame)
                    playerIndex -= charactersInRow;
                if (gamepad.dpad.down.wasPressedThisFrame || gamepad.leftStick.down.wasPressedThisFrame)
                    playerIndex += charactersInRow;
                if (gamepad.xButton.wasPressedThisFrame)
                    selected = true;
            }
            else
            {
                if (gamepad.xButton.wasPressedThisFrame)
                    selected = false;
            }
        }
        else if (ic is Joystick joystick)
        {
            var joystickThreshold = 0.5f;

            var joystickX = joystick.stick.x.ReadValue();
            var joystickY = joystick.stick.y.ReadValue();

            Vector2Control dpadControl;
            ButtonControl joystickConfirmButton = null;

            float joystickHatswitchX = 0;
            float joystickHatswitchY = 0;
            joystickTimeDelay += Time.fixedDeltaTime;
            if (joystickTimeDelay >= joystickCooldown)
            {
                foreach (var control in joystick.allControls)
                {
                    Debug.Log(control.name);
                    if (control is Vector2Control vecControl && vecControl != joystick.stick)
                    {
                        dpadControl = vecControl;
                        //Debug.Log(dpadControl.name);
                        Vector2 dpadValue = dpadControl.ReadValue();
                        joystickHatswitchX = dpadValue.x;
                        joystickHatswitchY = dpadValue.y;
                        break;
                    }
                }
                foreach (var control in joystick.allControls)
                {
                    if (control is ButtonControl buttonControl)
                    {
                        if (control.name == "buttonSouth" || control.name == "button1" || control.name == "button2")
                        {
                            joystickConfirmButton = buttonControl;
                            Debug.Log("X Button found: " + control.name);
                            break;
                        }
                        else if ((control.name == "trigger" || control.name.Contains("button")) && joystickConfirmButton is null)
                        {
                            joystickConfirmButton = buttonControl;
                        }
                    }
                }

                if (!selected)
                {


                    if (joystickX < -joystickThreshold || joystickHatswitchX < 0)
                    {
                        playerIndex--;
                        joystickTimeDelay = 0;
                    }
                    else if (joystickX > joystickThreshold || joystickHatswitchX > 0)
                    {
                        playerIndex++;
                        joystickTimeDelay = 0;
                    }
                    if (joystickY < -joystickThreshold || joystickHatswitchY < 0)
                    {
                        playerIndex += charactersInRow;
                        joystickTimeDelay = 0;
                    }
                    else if (joystickY > joystickThreshold || joystickHatswitchY > 0)
                    {
                        playerIndex -= charactersInRow;
                        joystickTimeDelay = 0;
                    }
                    if (joystickConfirmButton.wasPressedThisFrame)
                        selected = true;
                }
                else
                {
                    if (joystickConfirmButton.wasPressedThisFrame)
                        selected = false;
                }
                joystickHatswitchX = joystickHatswitchY = 0;
            }
            /*if (joystick.hatswitch is null)
            {
                if (joystickX < -joystickThreshold)
                {
                    playerIndex--;
                }
                else if (joystickX > joystickThreshold)
                {
                    playerIndex++;
                }
                if (joystickY < -joystickThreshold)
                {
                    playerIndex += charactersInRow;
                }
                else if (joystickY > joystickThreshold)
                {
                    playerIndex -= charactersInRow;
                }
            }
            else
            {
                

                var joystickHatswitchX = joystick.hatswitch.x.ReadValue();
                var joystickHatswitchY = joystick.hatswitch.y.ReadValue();

                if (joystickX < -joystickThreshold || joystickHatswitchX < 0)
                {
                    playerIndex--;
                }
                else if (joystickX > joystickThreshold || joystickHatswitchX > 0)
                {
                    playerIndex++;
                }
                if (joystickY < -joystickThreshold || joystickHatswitchY < 0)
                {
                    playerIndex += charactersInRow;
                }
                else if (joystickY > joystickThreshold || joystickHatswitchY > 0)
                {
                    playerIndex -= charactersInRow;
                }
            }
        }
            //Debug.Log(joystickHatswitchX + "   " + joystickHatswitchY);

            */
        }

        if (playerIndex > charBoxes.Count - 1)
        {
            playerIndex = 0;
        }
        else if (playerIndex < 0)
        {
            playerIndex = charBoxes.Count - 1;
        }
        if (playerIndex == player1Index || playerIndex == player2Index)
        {
            playerIndex = playerLastInd;
        }

        return (playerIndex, selected);
    }

    private void SetInputInfoText()
    {
        if (text1.text != "Current Device: " + inputDevice1)
            text1.text = "Current Device: " + inputDevice1;
        if (text2.text != "Current Device: " + inputDevice2)
            text2.text = "Current Device: " + inputDevice2;
    }
}
