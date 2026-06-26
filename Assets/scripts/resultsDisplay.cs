using UnityEngine;
using TMPro;

public class ResultsDisplay : MonoBehaviour
{
    public TextMeshProUGUI resultsText;

    void Start()
    {
        DisplayFinalScores();
    }

    void DisplayFinalScores()
    {
        string finalDisplay = "";
        float totalTime = 0;

        // 1. Loop through the list to get individual levels
        for (int i = 0; i < GameData.levelTimes.Count; i++)
        {
            float time = GameData.levelTimes[i];
            totalTime += time; // Add to the running total

            // Format as Level 1: 00:00
            finalDisplay += "Level " + (i + 1) + ": " + FormatTime(time) + "<br>";
        }

        // 2. Add the Total at the bottom
        finalDisplay += "<br><b>TOTAL: " + FormatTime(totalTime) + "</b>";

        // 3. Update the UI
        resultsText.text = finalDisplay;
    }

    // Helper function to turn seconds into MM:SS format
    string FormatTime(float time)
    {
        int seconds = (int)time;
        int milliseconds = (int)((time - seconds) * 100);
        return string.Format("{0:00}:{1:00}", seconds, milliseconds);
    }
}