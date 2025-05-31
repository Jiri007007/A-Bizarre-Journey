using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using System.Collections;
public class InputDetector : MonoBehaviour
{
    CharSelect cS;

    [SerializeField]
    int playerInput = 1;

    [SerializeField]
    TextMeshProUGUI errorMessage;

    InputDetector anotherPlayer;

    public string Device => detectedDevice;
    string detectedDevice = "None";

    public InputControl inp => input_Device;
    InputControl input_Device = null;

    public int deviceIndex => controllerJoystickIndex;
    int controllerJoystickIndex = 0;

    bool waitingForInput = false;
    bool error = false;


    void Start()
    {
        var parentTransform = transform.parent;
        foreach (Transform s in parentTransform)
        {
            if (s != transform)
            {
                anotherPlayer = s.GetComponentInChildren<InputDetector>();

                if (anotherPlayer != null) break;
            }
        }
        //Debug.Log(anotherPlayer);

        cS = GetComponentInParent<CharSelect>();
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(StartListening);
    }
    void Update()
    {
        if (waitingForInput)
        {
            DetectInputDevice();
        }
    }

    public void StartListening()
    {
        waitingForInput = !waitingForInput;
        detectedDevice = "Waiting for input...";
    }

    void StopListening()
    {
        waitingForInput = false;
    }

    private void DetectInputDevice()
    {
        var gamp = false;
        var joy = false;
        if (Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed)
        {
            StopListening();
            return;
        }

        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            detectedDevice = "Keyboard" + playerInput.ToString();
            input_Device = Keyboard.current;
            waitingForInput = false;
        }
        if (Gamepad.all.Count > 0)
        {
            Debug.Log(anotherPlayer);
            Debug.Log(anotherPlayer.detectedDevice);

            int gamepadCount = 0;
            foreach (var button in Gamepad.current.allControls)
            {
                
                if (button is ButtonControl buttonControl && buttonControl.wasPressedThisFrame && !gamp)
                {
                    if (anotherPlayer.detectedDevice != "Gamepad" || Gamepad.all.Count > 1)
                    {
                        detectedDevice = "Gamepad";
                        input_Device = Gamepad.all[gamepadCount];
                        controllerJoystickIndex = gamepadCount;
                        waitingForInput = false;
                        gamp = true;
                        gamepadCount++;
                    }
                    else
                    {
                        error = true;
                    }
                }
                
            }

        }
        if (Joystick.all.Count > 0)
        {
            int joystickCount = 0;
            foreach (var button in Joystick.current.allControls)
            {
                if (button is ButtonControl buttonControl && buttonControl.wasPressedThisFrame && !joy)
                {
                    if (anotherPlayer.detectedDevice != "Joystick" || Joystick.all.Count > 1)
                    {
                        detectedDevice = "Joystick";
                        input_Device = Joystick.all[joystickCount];
                        controllerJoystickIndex = joystickCount;
                        waitingForInput = false;
                        joy = true;
                        joystickCount++;
                    }
                    else
                    {
                        error = true;
                    }
                }
            }
        }

        if (error)
        {
            errorMessage.text = "You can't use the same device for both players!";
            error = false;
            StartCoroutine(ClearErrorMessage());
        }

        if (!waitingForInput)
        {
            Debug.Log("Detected Device: " + detectedDevice);

            if (playerInput == 1)
            {
                cS.inputDevice1 = detectedDevice;
                cS.inpC1 = input_Device;
                cS.p1ControlerJoystickIndex = controllerJoystickIndex;
            }
            else
            {
                cS.inputDevice2 = detectedDevice;
                cS.inpC2 = input_Device;
                cS.p2ControlerJoystickIndex = controllerJoystickIndex;
            }
        }
    }

    private IEnumerator ClearErrorMessage()
    {
        yield return new WaitForSeconds(3f);
        errorMessage.text = "";
    }
}
