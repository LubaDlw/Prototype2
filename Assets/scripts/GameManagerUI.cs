using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerUI : MonoBehaviour
{
    public GameObject panel1; // Start panel
    public GameObject panel2; // How to play panel
    public GameObject panel3; // Final panel

    public GameObject LevelPanel;
    public GameObject missionCompletePanel; // Reference to mission complete panel
    
    [Header("Wave System")]
    public PlayerStats playerStats; // Reference to PlayerStats for wave info
    
    public bool gameOver = false;
    private bool gameStarted = false;

    void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player")
                                 .GetComponent<PlayerStats>();
        Time.timeScale = 1;
        gameOver = false;
        gameStarted = false;
        
        // Hide all panels initially
        panel1.SetActive(false);
        panel2.SetActive(false);
        panel3.SetActive(false);
        
        if (missionCompletePanel != null)
            missionCompletePanel.SetActive(false);
    }

    public void StartGame()
    {
        gameStarted = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Hide all UI panels when game starts
        if (LevelPanel != null)
        {
            LevelPanel.SetActive(false);
        }
        panel1.SetActive(false);
        panel2.SetActive(false);
        panel3.SetActive(false);
        
        // Resume time and wave system
        Time.timeScale = 1;
        
        Debug.Log("Survival game started! Waves beginning...");
    }

    public void level()
    {
        Debug.Log("Level button pressed - starting next wave");
        
        if (playerStats != null)
        {
            // Call the new StartNextWave method instead of generic StartGame
            playerStats.StartNextWave();
        }
        else
        {
            Debug.LogError("PlayerStats reference is missing in GameManagerUI!");
            // Fallback to generic start if PlayerStats is missing
            StartGame();
        }
    }

    public void ShowPanel1()
    {
        panel1.SetActive(true);
        panel2.SetActive(false);
        panel3.SetActive(false);
    }

    public void ShowPanel2()
    {
        panel1.SetActive(false);
        panel2.SetActive(true);
        panel3.SetActive(false);
    }

    public void ShowPanel3()
    {
        panel1.SetActive(false);
        panel2.SetActive(false);
        panel3.SetActive(true);
    }

    // Method to handle wave completion - called from PlayerStats
    public void OnWaveComplete(int waveNumber)
    {
        Debug.Log("GameManager: Wave " + waveNumber + " completed!");
        
        // You can add additional wave completion logic here
        // Such as showing rewards, updating UI, etc.
    }

    // Method to continue to next wave - called by mission complete panel button
    public void ContinueToNextWave()
    {
        Debug.Log("Continue to next wave button pressed");
        
        if (missionCompletePanel != null)
            missionCompletePanel.SetActive(false);
            
        if (playerStats != null)
        {
            playerStats.StartNextWave();
            Debug.Log("Continuing to next wave...");
        }
        else
        {
            Debug.LogError("PlayerStats reference is missing!");
        }
    }

    // Method to handle game over in survival mode
    public void OnSurvivalGameOver(int wavesCompleted)
    {
        gameOver = true;
        Debug.Log("Survival ended! Player completed " + wavesCompleted + " waves.");
        
        // You can show a specific survival game over screen here
        // Or update the existing game over panel with wave count info
    }

    public void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No more scenes in the build index.");
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit game called.");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop in editor
#endif
    }

    public void RestartToMainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void RestartCurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    // Method to pause the game (for menus, etc.)
    public void PauseGame()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Method to resume the game
    public void ResumeGame()
    {
        if (gameStarted && !gameOver)
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        // Handle pause menu toggle (optional)
        if (gameStarted && !gameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                // Show pause menu or panel1
                ShowPanel1();
                PauseGame();
            }
            else
            {
                // Resume game
                panel1.SetActive(false);
                ResumeGame();
            }
        }
    }

    // Utility methods for wave system integration
    public bool IsGameActive()
    {
        return gameStarted && !gameOver && Time.timeScale > 0;
    }

    public void SetGameOver(bool isGameOver)
    {
        gameOver = isGameOver;
        if (gameOver)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Additional method for debugging wave state
    public void LogCurrentWaveInfo()
    {
        if (playerStats != null)
        {
            Debug.Log($"Current Wave: {playerStats.GetCurrentWave()}, Time Remaining: {playerStats.GetCurrentWaveTime():F1}s, Wave Active: {playerStats.IsWaveActive()}");
        }
    }
}