using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public GameObject pauseMenu; // Assign your Panel here
    public GameObject deathMenu; 
    public static bool isPaused = false;
    void Start(){
        pauseMenu.SetActive(false);
        deathMenu.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !deathMenu.activeSelf && 
        !UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("Title") && 
        !UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("EndScreen"))
        {
            Debug.Log("Escape key pressed");
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f; // Resumes game time
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked; // Optional: hides cursor
    }
    void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f; // Freezes the game world
        isPaused = true;
        Cursor.lockState = CursorLockMode.None; // Shows cursor
    }
    public void deathScreen(){
        deathMenu.SetActive(true);
        Time.timeScale = 0f; // Freezes the game world
        isPaused = true;
        Cursor.lockState = CursorLockMode.None; // Shows cursor
    }
    public void restartLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        isPaused = false;
        Time.timeScale = 1f; // Ensure time is normal
             
        pauseMenu.SetActive(false);
        deathMenu.SetActive(false);
    }
    public void restartGame(){
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        isPaused = false;
        Time.timeScale = 1f; // Ensure time is normal
        pauseMenu.SetActive(false);
        deathMenu.SetActive(false);
    }
    public void levelSelect(){
        ///
    }
    public void quitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
    public void Play(){
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
        GameData.levelTimes.Clear();

        // 2. Reset the timer if it's static
        countdownTimer.ResetTimer(); 

        // 3. Ensure time is moving (in case you were paused)
        Time.timeScale = 1f;
    }
}