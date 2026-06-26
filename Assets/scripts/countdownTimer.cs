using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Required for TextMeshPro

public class countdownTimer : MonoBehaviour
{
   //public float timeRemaining = 60; // Set your starting time in seconds
    public static float timeSpent= 0f;
    public TextMeshProUGUI timerText; // Drag your TimerText object here
    public bool timerIsRunning = false;
    public delegate void TimerExpired();
    public static event TimerExpired OnTimerEnd;
    //private float timeRemaining;
    private void Start()
    {
        // Starts the timer automatically
        timerIsRunning = true;
        timerText.color = Color.white;
        //float timeRemaining = timeTotal;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            
            timeSpent += Time.deltaTime;
            DisplayTime(timeSpent);
        }
    }

    void DisplayTime(float timeToDisplay){
        float seconds = Mathf.FloorToInt(timeToDisplay);
        float milliseconds = (timeToDisplay % 1) * 100;
        timerText.text = string.Format("{0:00}:{1:00}", seconds, milliseconds);
    }

    public void EndTimer()
    {
        timerIsRunning = false;
        OnTimerEnd?.Invoke();
    }
    public static float getTime()
    {
        return timeSpent;
    }
    public static void ResetTimer()
    {
        timeSpent = 0f;
    }
}