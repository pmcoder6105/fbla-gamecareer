using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Cursor = UnityEngine.Cursor;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Revamped Point System:
    /// - +25 points per fire extinguished
    /// - +45 points per cat saved
    /// - -1 point per second elapsed
    /// - -1 point per 1% water lost
    /// </summary>

    // Levels manager
    private int curLevel = 0;
    [SerializeField] private GameObject[] level;
    private GameObject currentBoard;

    // Fire tracking
    private int activeFire = 0;
    private int firesExtinguished = 0; // NEW: Track fires put out
    
    // NEW: Point values
    [SerializeField] private int pointsPerFireExtinguished = 25;
    [SerializeField] private int pointsPerCatSaved = 45;
    private int catsSaved = 0; // NEW: Track cats saved

    // Timer
    [SerializeField] private Text fires;
    [SerializeField] private Text txtTimer;
    private float timePassed = 0;
    private float startingTime = 0;
    
    // Legacy variable for compatibility with Levels.cs (not used in new point system)
    public int averageTime;
    
    // Score
    public float playerPoints = 0;
    [SerializeField] private GameObject endLevelScreen;

    // Victory flag
    private bool victoryManager;

    // Water system
    [HideInInspector] public float waterStart;
    public float waterAmount;
    private float waterPercentage;

    // Damage system (for game over condition)
    private float damage = 0;
    [SerializeField] private float damageSpeed = 0.5f;

    // Best score tracking
    private int bestScore = 0;

    // UI Buttons
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    public string sceneToReload = "FireFighter";

    // Point breakdown for display
    private float firePoints;
    private float catPoints;
    private float timePenalty;
    private float waterPenalty;
    private float levelTotalPoints;

    // UI Text elements
    [SerializeField] private Text txtVictory;
    [SerializeField] private Text txtTotalPoints;
    [SerializeField] private Text txtFirePoints;
    [SerializeField] private Text txtCatPoints;
    [SerializeField] private Text txtTimePenalty;
    [SerializeField] private Text txtWaterPenalty;
    [SerializeField] private Text txtDamage;
    [SerializeField] public Text txtWater;
    [SerializeField] private Text txtBestScore;
    [SerializeField] private Text txtNextLevelButton;
    
    // Buttons
    [SerializeField] private GameObject nextLevelButton;
    [SerializeField] private GameObject mainMenuButton;
    [SerializeField] private GameObject quitButton;
    
    // Singleton
    public static GameManager instance = null;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    // Level loading
    public void LoadLevel()
    {
        Invoke(nameof(Initiate), 0);

        if (currentBoard)
        {
            Destroy(currentBoard);
        }

        // Reset level-specific counters
        activeFire = 0;
        firesExtinguished = 0;
        catsSaved = 0;
        damage = 0;

        currentBoard = Instantiate(level[curLevel]);
        Time.timeScale = 1;
        startingTime = Time.time;
        endLevelScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Water function
    public void DecreaseWater(float amount)
    {
        waterAmount -= amount;
        waterPercentage = (waterAmount * 100) / waterStart;
        if (waterPercentage < 0)
        {
            waterPercentage = 0;
            FinishingLevel(false);
        }
    }
    
    // Quit Button
    public void QuitButton()
    {
        Application.Quit();
    }

    // Restart Button
    public void RestartButton()
    {
        if (victoryManager)
        {
            curLevel--;
        }
        
        LoadLevel();
    }
    
    // Button for go to main menu
    public void LoadMainMenuScene()
    {
        SceneManager.LoadSceneAsync("Main Menu");
    }
    
    // Increase Fire Counter
    public void MoreFire()
    {
        activeFire++;
    }
    
    // NEW: Call this when a cat is saved
    public void SaveCat()
    {
        catsSaved++;
        Debug.Log("Cats Saved: " + catsSaved);
    }
    
    // NEW: Simplified point calculation
    private void CalculatePoints()
    {
        // Points earned from extinguishing fires
        firePoints = firesExtinguished * pointsPerFireExtinguished;
        
        // Points earned from saving cats
        catPoints = catsSaved * pointsPerCatSaved;
        
        // Time penalty (1 point per second)
        timePenalty = Mathf.Floor(timePassed);
        
        // Water penalty (percentage of water lost)
        float waterLostPercentage = 100f - waterPercentage;
        waterPenalty = waterLostPercentage;
        
        // Calculate level total
        levelTotalPoints = firePoints + catPoints - timePenalty - waterPenalty;
        
        // Ensure level points don't go below 0
        if (levelTotalPoints < 0)
        {
            levelTotalPoints = 0;
        }
        
        // Add to overall player points
        playerPoints += levelTotalPoints;
        
        // Ensure total player points don't go below 0
        if (playerPoints < 0)
        {
            playerPoints = 0;
        }
        
        // Update best score if needed
        if (playerPoints > bestScore)
        {
            PlayerPrefs.SetFloat("Score", playerPoints);
            PlayerPrefs.Save();
        }

        // Debug output
        Debug.Log("=== POINT BREAKDOWN ===");
        Debug.Log("Fires Extinguished: " + firesExtinguished + " x " + pointsPerFireExtinguished + " = +" + firePoints);
        Debug.Log("Cats Saved: " + catsSaved + " x " + pointsPerCatSaved + " = +" + catPoints);
        Debug.Log("Time Penalty: -" + timePenalty + " seconds");
        Debug.Log("Water Penalty: -" + waterPenalty + "% lost");
        Debug.Log("Level Total: " + levelTotalPoints);
        Debug.Log("Overall Total: " + playerPoints);
    }

    public List<GameObject> LightUpFiresRandomly(List<GameObject> notUsedFire, int i)
    {
        notUsedFire[i].SetActive(true);
        notUsedFire.Remove(notUsedFire[i]);
        Debug.Log("Lighting up a fire");
        return notUsedFire;
    }

    // Initialize level
    private void Initiate()
    {
        GameObject[] checkbox = GameObject.FindGameObjectsWithTag("Checkbox");
        damage = 0;

        foreach (var check in checkbox)
        {
            check.SetActive(false);
        }
        
        waterPercentage = 100f;
    }
    
    // Show Victory Screen
    private void Victorytxt()
    {
        txtVictory.text = "Congratulations, You Just Won The Level!!!";
        
        if (txtFirePoints != null)
            txtFirePoints.text = "Fire Points: +" + firePoints + " (" + firesExtinguished + " fires x " + pointsPerFireExtinguished + ")";
        
        if (txtCatPoints != null)
            txtCatPoints.text = "Cat Points: +" + catPoints + " (" + catsSaved + " cats x " + pointsPerCatSaved + ")";
        
        if (txtTimePenalty != null)
            txtTimePenalty.text = "Time Penalty: -" + timePenalty + " seconds";
        
        if (txtWaterPenalty != null)
            txtWaterPenalty.text = "Water Penalty: -" + waterPenalty + "% lost";
        
        txtTotalPoints.text = "Level Score: " + levelTotalPoints + "\nTotal Score: " + playerPoints;
        
        if (txtNextLevelButton != null)
            txtNextLevelButton.text = "Next Level";
        
        nextLevelButton.SetActive(true);
        mainMenuButton.SetActive(false);
        quitButton.SetActive(true);
        
        if (playerPoints > bestScore)
        {
            txtBestScore.text = "[HIGH SCORE!]";
        }
        else
        {
            txtBestScore.text = "";
        }
    }
    
    // Show Defeat Screen
    private void Defeattxt()
    {
        txtVictory.text = "Defeated! Try again!";
        
        if (txtFirePoints != null)
            txtFirePoints.text = "Fire Points: +" + firePoints + " (" + firesExtinguished + " fires x " + pointsPerFireExtinguished + ")";
        
        if (txtCatPoints != null)
            txtCatPoints.text = "Cat Points: +" + catPoints + " (" + catsSaved + " cats x " + pointsPerCatSaved + ")";
        
        if (txtTimePenalty != null)
            txtTimePenalty.text = "Time Penalty: -" + timePenalty + " seconds";
        
        if (txtWaterPenalty != null)
            txtWaterPenalty.text = "Water Penalty: -" + waterPenalty + "% lost";
        
        txtTotalPoints.text = "Level Score: " + levelTotalPoints + "\nTotal Score: " + playerPoints;
        
        nextLevelButton.SetActive(false);
        quitButton.SetActive(false);
        mainMenuButton.SetActive(true);
        
        txtBestScore.text = "";
    }
    
    // Ends the level
    private void FinishingLevel(bool victory)
    {
        victoryManager = victory;
        CalculatePoints();
        endLevelScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        
        if (victory)
        {
            curLevel++;
            Victorytxt();
        }
        else
        {
            Defeattxt();
        }
    }

    // Decrease Fire Counter (called when fire is extinguished)
    public void KillFire()
    {
        activeFire--;
        firesExtinguished++; // NEW: Track fires extinguished
        Debug.Log("Active Fires: " + activeFire + " | Fires Extinguished: " + firesExtinguished);
        
        if (activeFire <= 0)
        {
            FinishingLevel(true);
        }
    }

    void Start()
    {
        bestScore = PlayerPrefs.GetInt("Score", 0);
        LoadLevel();
    }
    
    void Update()
    {
        fires.text = activeFire.ToString();

        timePassed = Time.time - startingTime;

        string minutes = ((int)timePassed / 60).ToString();
        string seconds = (timePassed % 60).ToString("00");
        txtTimer.text = minutes + ":" + seconds;
        
        // Damage update
        if (damage <= 100)
        {
            damage += (damageSpeed * activeFire) * Time.deltaTime;
        }
        else
        {
            damage = 100;
        }
        
        if (txtDamage != null)
            txtDamage.text = damage.ToString("00") + "%";
        
        // Water display
        if (txtWater != null)
            txtWater.text = waterPercentage.ToString("00") + "%";
        
        // Game over condition
        if (damage >= 100)
        {
            FinishingLevel(false);
        }
    }
}