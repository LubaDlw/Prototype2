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
        // Timer logic
        if (isTimerRunning)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isTimerRunning = false;  // Stop the timer once it reaches zero
                GameWin(); // Show the GameWin panel when the timer reaches zero
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
    void GameWin()
    {
        gameWinPanel.SetActive(true);  // Activate the GameWin panel
        PauseGame(); // Pause the game when the game is won
    }

    // Method to show the GameLose panel and pause the game
    void GameLose()
    {
        gameLosePanel.SetActive(true); // Activate the GameLose panel
        PauseGame(); // Pause the game when the game is lost
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
