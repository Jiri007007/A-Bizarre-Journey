using UnityEngine;
using UnityEngine.InputSystem;

public class TimeStopChar : Player
{
    [SerializeField] float timeStopCostPerSecond = 10f;
    [SerializeField] float timeStopCooldown = 3f;
    [SerializeField] float maxTimeStopDuration = 5f;

    bool isTimeStopped = false;
    bool canTimeStop = true;
    float currentTimeStopDuration = 0f;
    float timeStopCooldownTimer = 0f;
    bool hasCounteredTimeStop = false;
    bool opponentStoppedTime = false;

    Player[] allPlayers;
    TimeStopChar[] allTimeStopPlayers;
    public static bool BothPlayersTimeStopped { get; private set; }

    protected override void Start()
    {
        base.Start();
        allPlayers = FindObjectsOfType<Player>();
        allTimeStopPlayers = FindObjectsOfType<TimeStopChar>();
    }

    protected override void Update()
    {
        base.Update();

        if (isTimeStopped)
        {
            HandleActiveTimeStop();
        }
        else
        {

            HandleStaminaRegeneration();
        }

        if (timeStopCooldownTimer > 0)
        {
            timeStopCooldownTimer -= Time.deltaTime;
        }
    }

    private void HandleStaminaRegeneration()
    {
        if (!staminaRecentUse && currentStamina < maxStamina)
        {
            base.HealStamina();
        }
    }

    public void ManualResumeTime()
    {
        if (isTimeStopped)
        {
            EndTimeStop();
        }
    }

    protected override void SpecialAttack()
    {
        Debug.Log(stamina);
        if (!canTimeStop || timeStopCooldownTimer > 0)
            return;

        if (isTimeStopped)
        {
            ManualResumeTime();
            return;
        }

        opponentStoppedTime = false;
        foreach (var tsp in allTimeStopPlayers)
        {
            if (tsp != this && tsp.isTimeStopped)
            {
                opponentStoppedTime = true;
                break;
            }
        }

        if (opponentStoppedTime && !hasCounteredTimeStop)
        {
            StartTimeStop(true);
            hasCounteredTimeStop = true;
        }
        else if (!opponentStoppedTime) 
        {
            StartTimeStop(false);
        }
    }

    private void StartTimeStop(bool isCounter)
    {
        isTimeStopped = true;
        canTimeStop = false;
        currentTimeStopDuration = 0f;
        staminaRecentUse = true;

        foreach (var player in allPlayers)
        {
            if (player != this && (!isCounter || player is TimeStopChar))
            {
                player.SetTimeStopped(true);
            }
        }
    }

    private void EndTimeStop(bool wasCanceled = false)
    {
        isTimeStopped = false;
        staminaRecentUse = false; 

        if (!wasCanceled)
        {
            timeStopCooldownTimer = timeStopCooldown;
        }

        foreach (var player in allPlayers)
        {
            player.SetTimeStopped(false);
        }

        if (hasCounteredTimeStop)
        {
            Invoke(nameof(ResetCounterFlag), timeStopCooldown);
        }
    }

    private void CancelTimeStop()
    {
        if (isTimeStopped)
        {
            EndTimeStop(true);
        }
    }

    private void ResetCounterFlag()
    {
        hasCounteredTimeStop = false;
    }

    private void HandleActiveTimeStop()
    {
        currentStamina -= timeStopCostPerSecond * Time.deltaTime;
        currentTimeStopDuration += Time.deltaTime;

        if (currentStamina <= 0 || currentTimeStopDuration >= maxTimeStopDuration)
        {
            EndTimeStop();
        }
    }

    public void OnCancelTimeStop(InputAction.CallbackContext ctx)
    {
        if (ctx.action.triggered)
        {
            CancelTimeStop();
        }
    }

    public override void OnSuper1(InputAction.CallbackContext ctx)
    {
        if (!canInput && hasCounteredTimeStop) return;
        if (minimalStaminaForSuper > stamina) return;

        if (canSuper && ctx.action.triggered)
        {
            if (isTimeStopped)
            {
                CancelTimeStop();
            }
            else
            {
                SpecialAttack();
                StartCoroutine(AttackCooldown());
                StartCoroutine(SuperCooldown());
            }
        }
    }
}