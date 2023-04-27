using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] secondBoxes;
    [SerializeField] private TextMeshProUGUI[] minuteBoxes;
    public UnityEvent OnEnd;
    
    private float totalTimeInSeconds;


    private float audioStartTime;
    private bool isActive;

    public void SetupTimer(float totalTimeInSeconds)
    {
        UpdateTextBox(totalTimeInSeconds);
        this.totalTimeInSeconds = totalTimeInSeconds;
    }

    public void ActivateTimer()
    {
        isActive = true;
        audioStartTime = (float) AudioSettings.dspTime;
    }

    private bool hasEnded;
    // Update is called once per frame
    void Update() {
        if (hasEnded) return;
        if (!isActive) return;
        float currentTime = (float) AudioSettings.dspTime;
        float totalTimeProgressed = currentTime - audioStartTime;
        float timeRemaining = totalTimeInSeconds - totalTimeProgressed;
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            OnEnd.Invoke();
            hasEnded = true;
        }
        UpdateTextBox(timeRemaining);

    }

    public void UpdateTextBox(float timeInSeconds)
    {
        string minutes = Mathf.Floor(timeInSeconds / 60).ToString("00");
        string seconds = (timeInSeconds % 60).ToString("00");
        foreach (TextMeshProUGUI minuteBox in minuteBoxes) {
            minuteBox.text = minutes;
        }
        
        foreach (TextMeshProUGUI secondBox in secondBoxes) {
            secondBox.text = seconds;
        }
    }
}
