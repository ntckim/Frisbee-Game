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
    private bool frisbeeWon;

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

    public void FrisbeeCaught()
    {
        if (frisbeeWon) return;
        frisbeeWon = true;
        StartCoroutine(FrisbeeWinCoroutine());
    }

    IEnumerator FrisbeeWinCoroutine()
    {
        playermovement player = FindObjectOfType<playermovement>();
        if (player != null) player.DisableMovement();

        ShowWinMessage("You Win!");
        Debug.Log("You caught the frisbee!");

        float levelTime = countdownTimer.getTime();
        GameData.levelTimes.Add(levelTime);
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1f;
    }

    void ShowWinMessage(string message)
    {
        if (targetText != null)
        {
            targetText.text = "<color=green><size=150%>" + message + "</size></color>";
            return;
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        GameObject textObj = new GameObject("WinMessage");
        textObj.transform.SetParent(canvas.transform, false);

        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(600f, 120f);

        TextMeshProUGUI winText = textObj.AddComponent<TextMeshProUGUI>();
        TextMeshProUGUI existingText = FindObjectOfType<TextMeshProUGUI>();
        if (existingText != null) winText.font = existingText.font;
        winText.text = message;
        winText.fontSize = 72f;
        winText.alignment = TextAlignmentOptions.Center;
        winText.color = Color.green;
    }
}
