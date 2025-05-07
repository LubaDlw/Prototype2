using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public int health = 100;
    public int hunger;
    public int vitality;
    public int amount;

    public int enemiesKilled = 0;
    public TMP_Text enemiesKilledTxt;

    public Slider healthSlider; // Drag the Slider here in Inspector
    public float maxHealth = 100f;
    private float currentHealth;

    public TMP_Text healthTxt;  // Text for displaying health
    public TMP_Text timerTxt;   // Text for displaying the timer

    public TMP_Text waveNotificationTxt;

    public float timeRemaining = 600f;  // 10 minutes in seconds
    private bool isTimerRunning = true;

    public GameObject gameWinPanel; // Reference to the GameWin Panel
    public GameObject gameLosePanel; // Reference to the GameLose Panel\

    public GameObject damageEffectPrefab;

    [Header("Mission Settings")]
public GameObject[] zone2Blockers;  // Assign all blockers to Zone 2 in the Inspector
public GameObject missionCompletePanel; // UI panel to show when mission is complete

public bool hasCollectedResourcePack = false;
public int enemiesToKill = 5;  // Adjust per mission
private bool mission1Complete = false;


    GameManagerUI gameManager;

    // Player now loses when counter equals zero or dies
    private void Start()
    {
        maxHealth = 100f;
        waveNotificationTxt.text = "";
        health = 100;
        enemiesKilled = 0;
        currentHealth = maxHealth = health = 100;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        healthTxt.text = health.ToString();
        enemiesKilledTxt.text = enemiesKilled.ToString();

        // Initialize the timer display
        DisplayTime(timeRemaining);

        // Ensure both panels are hidden initially
        gameWinPanel.SetActive(false);
        gameLosePanel.SetActive(false);
    }

    public void DisplayWaveNotification(int waveNumber)
    {
        waveNotificationTxt.text = "Wave " + waveNumber + " starting!";
        StartCoroutine(ClearWaveNotification());
    }

    private IEnumerator ClearWaveNotification()
    {
        // Wait for 2 seconds before clearing the notification
        yield return new WaitForSeconds(2f);
        waveNotificationTxt.text = "";
    }


    private void Update()
    {

        // Check if mission is completed
if (!mission1Complete && hasCollectedResourcePack && enemiesKilled >= enemiesToKill)
{
    CompleteMission1();
}

        // Timer logic
        if (isTimerRunning)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isTimerRunning = false;  // Stop the timer once it reaches zero
                GameLose();
               // GameWin(); // Show the GameWin panel when the timer reaches zero
            }

            // Update the timer display each frame
            DisplayTime(timeRemaining);
        }

        enemiesKilledTxt.text = enemiesKilled.ToString();
        // Check if the player's health reaches zero
        if (health <= 0)
        {
            health = 0;
           // healthTxt.text = "GAME OVER"; // Optional: show "GAME OVER" when health is 0
            GameLose(); // Show the GameLose panel when health reaches zero
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        // Convert seconds to minutes and seconds
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        // Update the timer text
        timerTxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Method to deal damage and update health
    public void dealDamage(int amount)
    {
        health -= amount;
        healthSlider.value = health;
        healthTxt.text = health.ToString();

        if (damageEffectPrefab != null)
        {
            // Instantiate the damage effect prefab at the player's position
            Instantiate(damageEffectPrefab, transform.position, Quaternion.identity);
        }

    }

    public void healthPack()
    {
        if (health < maxHealth)
        {
            health += 5;
            healthSlider.value = health;
            healthTxt.text = health.ToString();
        }

    }

    // Method to show the GameWin panel and pause the game
  public   void GameWin()
    {
        gameWinPanel.SetActive(true);  // Activate the GameWin panel
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


        PauseGame(); // Pause the game when the game is won
    }

    // Method to show the GameLose panel and pause the game
    void GameLose()
    {
       
        gameLosePanel.SetActive(true); // Activate the GameLose panel
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PauseGame(); // Pause the game when the game is lost
    }

    void CompleteMission1()
{
    mission1Complete = true;

    // Unlock Zone 2
    foreach (GameObject blocker in zone2Blockers)
    {
        blocker.SetActive(false);
    }

    // Show mission complete panel
    if (missionCompletePanel != null)
    {
        missionCompletePanel.SetActive(true);
    }

    // Optional: stop timer or notify player
    Debug.Log("Mission 1 Complete! Zone 2 Unlocked.");
}


    // Method to pause the game
    void PauseGame()
    {
        Time.timeScale = 0;  // Stop the game time, effectively pausing the game
        // Optionally, you can show a "Paused" text or menu if you want
    }

    // Method to resume the game (e.g., if you have a "Resume" button in the UI)
    public void ResumeGame()
    {
        Time.timeScale = 1;  // Resume the game time
        gameWinPanel.SetActive(false);  // Hide the GameWin panel
        gameLosePanel.SetActive(false);  // Hide the GameLose panel
    }
}
