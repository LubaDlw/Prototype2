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

    public Slider healthSlider;
    public float maxHealth = 100f;
    private float currentHealth;

    public TMP_Text healthTxt;
    public TMP_Text timerTxt;
    public TMP_Text waveNotificationTxt;

    public float timeRemaining = 600f;  // 10 minutes
    private bool isTimerRunning = true;

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
   private bool  mission2Complete = false;

    [Header("Mission UI")]  //Change upon newLevel
    public TMP_Text killEnemyMissionText;
    public TMP_Text collectItemMissionText;
    //public TMP_Text collectItemMission4;
    public TMP_Text collectItemMission5;

    public TMP_Text collectmission3;

    private bool enemyKillObjectiveComplete = false;
    private bool resourceObjectiveComplete = false;

    private void Start()
    {enemiesToKill = 8;
        collectItemMission5.text = " ";
        collectmission3.text = " ";
         Time.timeScale = 1;
        maxHealth = 100f;
        waveNotificationTxt.text = "";
        health = 100;
        enemiesKilled = 0;
        currentHealth = maxHealth = health = 100;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        healthTxt.text = health.ToString();
        enemiesKilledTxt.text = enemiesKilled.ToString();

        DisplayTime(timeRemaining);

        gameWinPanel.SetActive(false);
        gameLosePanel.SetActive(false);
        if (missionCompletePanel != null)
            missionCompletePanel.SetActive(false);
    }

    public void DisplayWaveNotification(int waveNumber)
    {
        waveNotificationTxt.text = "Wave " + waveNumber + " starting!";
        StartCoroutine(ClearWaveNotification());
    }

    private IEnumerator ClearWaveNotification()
    {
        yield return new WaitForSeconds(2f);
        waveNotificationTxt.text = "";
    }
    
    public void displayCorrectUI()
    {
        enemiesKilled = enemiesKilled/2;
    }
    private void Update()
    {
        // Update kill count text
        enemiesKilledTxt.text = enemiesKilled.ToString();

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
            Debug.Log("MISSION COMPLETE: Resource collected and 5 enemies killed.");
        }

//check mission 2
        if(mission1Complete && enemiesKilled >= enemiesToKill && !mission2Complete && resourceObjectiveComplete) 
        {
            Debug.Log("Mission 2 Underway");
            CompleteMission2();
        }

        // Timer logic
        if (isTimerRunning)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isTimerRunning = false;
                GameLose();
            }

            DisplayTime(timeRemaining);
        }

        // Health check
        if (health <= 0)
        {
            health = 0;
            GameLose();
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerTxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void dealDamage(int amount)
    {
        health -= amount;
        healthSlider.value = health;
        healthTxt.text = health.ToString();

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
        }
    }

    public void GameWin()
    {
        gameWinPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PauseGame();
    }

    void GameLose()
    {
        gameLosePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PauseGame();
    }
void CompleteMission2()
{
    hasCollectedResourcePack = false;
    //level3
    mission2Complete = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (GameObject blocker in zone3Blockers)
        {
            blocker.SetActive(false);
        }

        if (missionCompletePanel != null)
        {
            missionCompletePanel.SetActive(true);
            //mission1Complete = false;
            enemiesToKill = 24;
            enemyKillObjectiveComplete = false;
            resourceObjectiveComplete = false;
           // !mission1Complete && enemyKillObjectiveComplete && resourceObjectiveComplete
            //set checkpoint
            // Set New Missions
            //New Resource Text
            collectItemMissionText.text = "Find Resource 3 and Return to Scientist";
             //Set text 3 Active
           // collectItemMission5.SetActive(true);
            collectItemMission5.color = Color.white;
            collectItemMission5.text = "Find Resource 4 and Return to Scientist";
            collectmission3.text = " Find Resource 5 and Return to Scientist";
            collectmission3.color = Color.white;
            collectItemMissionText.color = Color.white;
            killEnemyMissionText.text = "Kill 24 Zombies";
            killEnemyMissionText.color = Color.white;
            //COllect Item 2
        }

        Debug.Log("Mission 2 Complete! Zone 3 Unlocked.");
        //mission1Complete = false;
}
    void CompleteMission1()
    {
        hasCollectedResourcePack = false;
        mission1Complete = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (GameObject blocker in zone2Blockers)
        {
            blocker.SetActive(false);
        }

        if (missionCompletePanel != null)
        {
            missionCompletePanel.SetActive(true);
            //mission1Complete = false;
            enemiesToKill = 16;
            enemyKillObjectiveComplete = false;
            resourceObjectiveComplete = false;
           // !mission1Complete && enemyKillObjectiveComplete && resourceObjectiveComplete
            //set checkpoint
            // Set New Missions
            //New Resource Text

            // This is level 2
            collectItemMissionText.text = "Find Resource 2 and Return to Scientist";
            //Set text 3 Active
            
            collectItemMission5.color = Color.white;
            collectItemMission5.text = " ";
            collectItemMissionText.color = Color.white;
            killEnemyMissionText.text = "Kill 16 Zombies";
            killEnemyMissionText.color = Color.white;
            //COllect Item 2
        }

        Debug.Log("Mission 1 Complete! Zone 2 Unlocked.");
        //mission1Complete = false;
    }

    void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        gameWinPanel.SetActive(false);
        gameLosePanel.SetActive(false);
    }
}
