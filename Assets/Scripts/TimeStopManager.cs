using System.Collections;
using UnityEngine;

public class TimeStopManager : MonoBehaviour
{
    public bool isTimeStopped => timeStopped;
    bool timeStopped = false;
    float originalTimeScale = 1f;
    Coroutine resumeCoroutine;
    
    

    public void StopTime(float duration)
    {
        if (!timeStopped)
        {
            timeStopped = true;
            originalTimeScale = Time.timeScale;
            Time.timeScale = 0f; // Fully stop time

            resumeCoroutine = StartCoroutine(ResumeTimeAfter(duration));
        }
    }

    public void ResumeTime()
    {
        if (timeStopped)
        {
            if (resumeCoroutine != null)
            {
                StopCoroutine(resumeCoroutine);
            }

            StartCoroutine(SmoothResumeTime()); // Use a smooth effect
        }
    }

    private IEnumerator ResumeTimeAfter(float duration)
    {
        yield return new WaitForSecondsRealtime(duration); // Ignores Time.timeScale
        ResumeTime();
    }

    private IEnumerator SmoothResumeTime()
    {
        float elapsedTime = 0f;
        float transitionTime = 0.5f; // 0.5s smooth transition
        Time.timeScale = 0.1f; // Start with slow-motion

        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.unscaledDeltaTime; // Use unscaled time
            Time.timeScale = Mathf.Lerp(0.1f, originalTimeScale, elapsedTime / transitionTime);
            yield return null;
        }

        Time.timeScale = originalTimeScale;
        timeStopped = false;
    }

}
