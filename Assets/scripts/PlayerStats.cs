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

    public int coins = 0; // Coins for purchasing upgrades
    public TMP_Text enemiesKilledTxt;
    public TMP_Text coinsTxt;
    public TMP_Text coinsTxt2;

    public Slider healthSlider;
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Armor System")]
    public int armor = 0;
    public int maxArmor = 100;
    public Slider armorSlider;
    public TMP_Text armorTxt;
    public int armorCost = 15; // Cost to buy armor upgrade

    public TMP_Text healthTxt;
    public TMP_Text timerTxt;
    public TMP_Text waveNotificationTxt;

    // Wave-based survival system
    public float waveTimer = 60f;  // Default base timer for waves beyond array
    private float currentWaveTime;
    private bool isWaveActive = true;
    private int currentWave = 1;

    public GameObject gameWinPanel;
    public GameObject gameLosePanel;
    public GameObject damageEffectPrefab;

    [Header("Mission Settings")]
    public GameObject[] zone2Blockers;
    public GameObject[] zone3Blockers;
    public GameObject missionCompletePanel;

    public bool hasCollectedResourcePack = false;
    public int enemiesToKill = 8;
    private bool mission1Complete = false;
    private bool mission2Complete = false;

    [Header("Mission UI")]
    public TMP_Text killEnemyMissionText;
    public TMP_Text collectItemMissionText;
    public TMP_Text collectItemMission5;
    public TMP_Text collectmission3;

    private bool enemyKillObjectiveComplete = false;
    private bool resourceObjectiveComplete = false;

    [Header("Wave Settings")]
    public float[] waveTimers = { 30f, 45f, 60f, 75f, 90f, 105f, 120f };
    public int[] waveUnlockZones = { 0, 1, 2, 3, 4 };

    [Header("Survival Settings")]
    public float waveBriefingDuration = 3f;
    public bool autoProgressWaves = true;

    [Header("Feedback UI System")]
    public GameObject feedbackUIPanel; // Panel containing the feedback message
    public TMP_Text feedbackMessageText; // Text component for displaying messages
    public float feedbackDisplayDuration = 3f; // How long to show feedback messages
    
    [Header("Coin Rewards")]
    public int mission1CoinReward = 25;
    public int mission2CoinReward = 50;
    public int mission3CoinReward = 75;
    public int waveCompletionCoinReward = 10;

    private PlayerShooting playerShooting; // Reference to PlayerShooting script
    private Coroutine currentFeedbackCoroutine; // To handle overlapping feedback messages

    private void Start()
    {
        InitializeGame();
        playerShooting = GetComponent<PlayerShooting>();
        if (playerShooting == null)
        {
            Debug.LogError("PlayerStats: playerShooting script not found on the player.");
        }
        
        // Initialize feedback UI
        if (feedbackUIPanel != null)
            feedbackUIPanel.SetActive(false);
    }

    private void InitializeGame()
    {
        enemiesToKill = 8;
        collectItemMission5.text = " ";
        collectmission3.text = " ";
        Time.timeScale = 1;
        maxHealth = 100f;
        waveNotificationTxt.text = "";
        health = 100;
        armor = 0; // Initialize armor
        enemiesKilled = 0;
        currentHealth = maxHealth = health = 100;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        healthTxt.text = health.ToString();
        
        // Initialize armor slider
        if (armorSlider != null)
        {
            armorSlider.maxValue = maxArmor;
            armorSlider.value = armor;
        }
        if (armorTxt != null)
            armorTxt.text = armor.ToString();
            
        enemiesKilledTxt.text = enemiesKilled.ToString();

        // Initialize wave system
        currentWave = 1;
        SetWaveTimer();
        DisplayWaveNotification(currentWave);

        gameWinPanel.SetActive(false);
        gameLosePanel.SetActive(false);
        if (missionCompletePanel != null)
            missionCompletePanel.SetActive(false);
    }

    // Feedback UI Methods
    public void ShowFeedbackMessage(string message)
    {
        if (feedbackUIPanel == null || feedbackMessageText == null)
        {
            Debug.LogWarning("Feedback UI components not assigned!");
            return;
        }

        // Stop any existing feedback coroutine
        if (currentFeedbackCoroutine != null)
        {
            StopCoroutine(currentFeedbackCoroutine);
        }

        // Start new feedback display
        currentFeedbackCoroutine = StartCoroutine(DisplayFeedbackMessage(message));
    }

    private IEnumerator DisplayFeedbackMessage(string message)
    {
        feedbackMessageText.text = message;
        feedbackUIPanel.SetActive(true);
        
        yield return new WaitForSeconds(feedbackDisplayDuration);
        
        feedbackUIPanel.SetActive(false);
        currentFeedbackCoroutine = null;
    }

    // Coin reward methods
    public void AddCoins(int amount, string reason = "")
    {
        coins += amount;
        string message = reason.Length > 0 ? reason : $"+{amount} Coins!";
        ShowFeedbackMessage(message);
        
        // Update coin display
        UpdateCoinDisplay();
    }

    private void UpdateCoinDisplay()
    {
        if (coinsTxt != null) coinsTxt.text = coins.ToString();
        if (coinsTxt2 != null) coinsTxt2.text = coins.ToString();
    }

    private void SetWaveTimer()
    {
        // Determine base time from array or default
        float baseTime;
        if (currentWave - 1 < waveTimers.Length)
        {
            baseTime = waveTimers[currentWave - 1];
        }
        else
        {
            baseTime = waveTimer + (currentWave - waveTimers.Length) * 10f;
        }

        // Add 15 seconds per wave beyond the first
        currentWaveTime = baseTime + (currentWave - 1) * 15f;

        DisplayTime(currentWaveTime);
    }

    public void DisplayWaveNotification(int waveNumber)
    {
        waveNotificationTxt.text = "Wave " + waveNumber + " - Survive the timer!";
        StartCoroutine(ClearWaveNotification());
    }

    private IEnumerator ClearWaveNotification()
    {
        yield return new WaitForSeconds(waveBriefingDuration);
        waveNotificationTxt.text = "";
    }

    private void Update()
    {
        // Update kill count text
        enemiesKilledTxt.text = enemiesKilled.ToString();
        UpdateCoinDisplay();
        
        // Update armor display
        if (armorTxt != null)
            armorTxt.text = armor.ToString();

        // Mission Objective: Kill Enemies
        if (!enemyKillObjectiveComplete && enemiesKilled >= enemiesToKill)
        {
            enemyKillObjectiveComplete = true;
            if (killEnemyMissionText != null)
                killEnemyMissionText.color = Color.green;
        }

        // Mission Objective: Resource Pack
        if (!resourceObjectiveComplete && hasCollectedResourcePack)
        {
            resourceObjectiveComplete = true;
        }

        // Check if mission is completed
        if (!mission1Complete && enemyKillObjectiveComplete && resourceObjectiveComplete)
        {
            CompleteMission1();
            Debug.Log("MISSION COMPLETE: Resource collected and enemies killed.");
        }

        // Check mission 2
        if (mission1Complete && enemiesKilled >= enemiesToKill && !mission2Complete && resourceObjectiveComplete)
        {
            Debug.Log("Mission 2 Underway");
            CompleteMission2();
        }

        // Wave timer logic
        if (isWaveActive)
        {
            currentWaveTime -= Time.deltaTime;

            if (currentWaveTime <= 0)
            {
                CompleteWave();
            }

            DisplayTime(currentWaveTime);
        }

        // Health check
        if (health <= 0)
        {
            health = 0;
            GameLose();
        }
    }

    private void CompleteWave()
    {
        Debug.Log("Wave " + currentWave + " completed! Player survived.");

        // Award coins for wave completion
        AddCoins(waveCompletionCoinReward, $"Wave {currentWave} Complete! +{waveCompletionCoinReward} Coins!");

        if (missionCompletePanel != null)
        {
            missionCompletePanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PauseGame();
        }

        currentWave++;
        HandleWaveBasedUnlocks();
        ScaleWaveDifficulty();
    }

    private void HandleWaveBasedUnlocks()
    {
        if (currentWave == 3 && zone2Blockers != null)
        {
            foreach (GameObject blocker in zone2Blockers)
                if (blocker != null)
                    blocker.SetActive(false);
            Debug.Log("Zone 2 unlocked at Wave " + currentWave);
            ShowFeedbackMessage("Zone 2 Unlocked!");
        }
        else if (currentWave == 5 && zone3Blockers != null)
        {
            foreach (GameObject blocker in zone3Blockers)
                if (blocker != null)
                    blocker.SetActive(false);
            Debug.Log("Zone 3 unlocked at Wave " + currentWave);
            ShowFeedbackMessage("Zone 3 Unlocked!");
        }
    }

    private void ScaleWaveDifficulty()
    {
        if (currentWave % 3 == 0)
        {
            enemiesToKill += 2;
            enemyKillObjectiveComplete = false;
            if (killEnemyMissionText != null)
            {
                killEnemyMissionText.text = "Kill " + enemiesToKill + " Zombies";
                killEnemyMissionText.color = Color.white;
            }
            Debug.Log("Difficulty increased! Now need to kill " + enemiesToKill + " enemies.");
            ShowFeedbackMessage("Difficulty Increased!");
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerTxt.text = string.Format("Wave {0} - {1:00}:{2:00}", currentWave, minutes, seconds);
    }

    public void dealDamage(int amount)
    {
        int remainingDamage = amount;
        
        // First, damage armor if available
        if (armor > 0)
        {
            if (armor >= remainingDamage)
            {
                // Armor absorbs all damage
                armor -= remainingDamage;
                remainingDamage = 0;
                ShowFeedbackMessage($"Armor absorbed {amount} damage!");
            }
            else
            {
                // Armor absorbs partial damage, remainder goes to health
                int armorDamage = armor;
                remainingDamage -= armor;
                armor = 0;
                ShowFeedbackMessage($"Armor destroyed! {armorDamage} damage blocked!");
            }
            
            // Update armor display
            if (armorSlider != null)
                armorSlider.value = armor;
            if (armorTxt != null)
                armorTxt.text = armor.ToString();
        }
        
        // Apply remaining damage to health
        if (remainingDamage > 0)
        {
            health -= remainingDamage;
            healthSlider.value = health;
            healthTxt.text = health.ToString();
            
            if (armor == 0) // Only show health damage message if no armor was involved
                ShowFeedbackMessage($"Took {remainingDamage} damage!");
        }

        if (damageEffectPrefab != null)
        {
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
            ShowFeedbackMessage("Health Restored!");
        }
        else
        {
            ShowFeedbackMessage("Health Already Full!");
        }
    }

    public void buyHealthUpgrade() //use coin count over enemies killed
    {
        if (health >= maxHealth)
        {
            ShowFeedbackMessage("Health Already at Maximum!");
            return;
        }
        
        if (coins < 10)
        {
            ShowFeedbackMessage("Not Enough Coins! Need 10 Coins");
            return;
        }

        Debug.Log("Health Upgrade Purchased!");
        coins -= 10; // Deduct cost
        health += 5;
        healthSlider.value = health;
        healthTxt.text = health.ToString();
        UpdateCoinDisplay();
        ShowFeedbackMessage("Health Upgrade Purchased! +5 Health");
    }

    public void buyAmmoUpgrade() //use coin count over enemies killed
    {
        if (coins < 10)
        {
            ShowFeedbackMessage("Not Enough Coins! Need 10 Coins");
            return;
        }

        Debug.Log("Ammo Upgrade Purchased!");
        coins -= 10; // Deduct cost
        UpdateCoinDisplay();
        
        if (playerShooting != null)
        {
            playerShooting.addAmmo(); // Call the method to add ammo
            ShowFeedbackMessage("Ammo Upgrade Purchased!");
        }
        else
        {
            Debug.LogError("PlayerShooting script not found on the player.");
            ShowFeedbackMessage("Error: Cannot Add Ammo!");
        }
    }

    public void buyArmorUpgrade()
    {
        if (armor >= maxArmor)
        {
            ShowFeedbackMessage("Armor Already at Maximum!");
            return;
        }
        
        if (coins < armorCost)
        {
            ShowFeedbackMessage($"Not Enough Coins! Need {armorCost} Coins");
            return;
        }

        Debug.Log("Armor Upgrade Purchased!");
        coins -= armorCost; // Deduct cost
        armor += 20; // Add armor points
        
        // Ensure armor doesn't exceed maximum
        if (armor > maxArmor)
            armor = maxArmor;
            
        // Update displays
        if (armorSlider != null)
            armorSlider.value = armor;
        if (armorTxt != null)
            armorTxt.text = armor.ToString();
        UpdateCoinDisplay();
        ShowFeedbackMessage("Armor Upgrade Purchased! +20 Armor");
    }

    public void armorPack()
    {
        if (armor < maxArmor)
        {
            armor += 10;
            if (armor > maxArmor)
                armor = maxArmor;
                
            if (armorSlider != null)
                armorSlider.value = armor;
            if (armorTxt != null)
                armorTxt.text = armor.ToString();
            ShowFeedbackMessage("Armor Restored!");
        }
        else
        {
            ShowFeedbackMessage("Armor Already Full!");
        }
    }

    public void GameWin()
    {
        Debug.Log("Survival Goal Achieved! Player reached Wave " + currentWave);
        gameWinPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PauseGame();
    }

    void GameLose()
    {
        Debug.Log("Game Over! Player survived " + (currentWave - 1) + " waves and reached Wave " + currentWave + ".");
        gameLosePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PauseGame();
    }

    void CompleteMission2()
    {
        hasCollectedResourcePack = false;
        mission2Complete = true;
        Debug.Log("Mission 2 Complete! All objectives finished.");
        
        // Award coins for mission completion
        AddCoins(mission2CoinReward, $"Mission 2 Complete! +{mission2CoinReward} Coins!");
        
        enemiesToKill = 24;
        enemyKillObjectiveComplete = false;
        resourceObjectiveComplete = false;
        collectItemMissionText.text = "Find Resource 3 and Return to Scientist";
        collectItemMission5.color = Color.white;
        collectItemMission5.text = "Find Resource 4 and Return to Scientist";
        collectmission3.text = "Find Resource 5 and Return to Scientist";
        collectmission3.color = Color.white;
        collectItemMissionText.color = Color.white;
        killEnemyMissionText.text = "Kill 24 Zombies";
        killEnemyMissionText.color = Color.white;
    }

    void CompleteMission1()
    {
        hasCollectedResourcePack = false;
        mission1Complete = true;
        Debug.Log("Mission 1 Complete! All objectives finished.");
        
        // Award coins for mission completion
        AddCoins(mission1CoinReward, $"Mission 1 Complete! +{mission1CoinReward} Coins!");
        
        enemiesToKill = 16;
        enemyKillObjectiveComplete = false;
        resourceObjectiveComplete = false;
        collectItemMissionText.text = "Find Resource 2 and Return to Scientist";
        collectItemMission5.color = Color.white;
        collectItemMission5.text = " ";
        collectItemMissionText.color = Color.white;
        killEnemyMissionText.text = "Kill 16 Zombies";
        killEnemyMissionText.color = Color.white;
    }

    void PauseGame()
    {
        Time.timeScale = 0;
        isWaveActive = false;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        isWaveActive = true;
        gameWinPanel.SetActive(false);
        gameLosePanel.SetActive(false);
        if (missionCompletePanel != null)
            missionCompletePanel.SetActive(false);
    }

    public void StartNextWave()
    {
        Debug.Log("Starting Wave " + currentWave);
        SetWaveTimer();
        DisplayWaveNotification(currentWave);
        Time.timeScale = 1;
        isWaveActive = true;
        if (missionCompletePanel != null)
            missionCompletePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ForceNextWave()
    {
        CompleteWave();
    }

    public void ToggleAutomaticWaveProgression(bool enabled)
    {
        autoProgressWaves = enabled;
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }

    public float GetCurrentWaveTime()
    {
        return currentWaveTime;
    }

    public bool IsWaveActive()
    {
        return isWaveActive;
    }

    public int GetWavesSurvived()
    {
        return currentWave - 1;
    }
}