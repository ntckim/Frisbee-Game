using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class gameManager : MonoBehaviour
{
    public static gameManager instance; // Allows other scripts to find this easily
    public TextMeshProUGUI targetText;
    [SerializeField] private int targetCount;

    private void Awake()
    {
        // Singleton pattern: ensures only one Manager exists
        if (instance == null) instance = this;
    }

    void Start()
    {
        // Automatically find every object with the "Target" script
        targetCount = GameObject.FindGameObjectsWithTag("target").Length;
        Debug.Log("Targets to break: " + targetCount);
        UpdateTargetUI();
    }

    public void RemoveTarget(){
        targetCount--;
        UpdateTargetUI();
        if (targetCount <= 0)
        {
            WinLevel();
        }
    }

    void WinLevel(){
        StartCoroutine(WinLevelCoroutine());
    }
    IEnumerator WinLevelCoroutine(){
         Debug.Log("Level Complete!");
        float levelTime = countdownTimer.getTime();
        GameData.levelTimes.Add(levelTime);
        int current = SceneManager.GetActiveScene().buildIndex;
        Time.timeScale = 0f; 

        yield return new WaitForSecondsRealtime(0.6f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(current+1);
        Debug.Log("Level Time: " + levelTime + " seconds");
        countdownTimer.ResetTimer();
        // Load the next level or show a UI screen
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    void UpdateTargetUI()
    {
        if (targetText != null)
        {
            targetText.text = "Targets <br>left: <color=red><size=110%>" + targetCount + "</size></color>";
        }
    }
}
