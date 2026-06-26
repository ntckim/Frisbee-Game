using System.Collections.Generic;

public static class GameData
{
    // This list will hold the completion times for every level
    public static List<float> levelTimes = new List<float>();
    public static int Deaths = 0;

    // Helper to format time as 00:00
    public static string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}