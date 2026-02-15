using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// IMPORTANT: This script should be attached to an ACTIVE GameObject (like an empty "CatRescueManager")
/// NOT on the cat itself! The cat should be referenced in the catModel field.
/// </summary>
public class CatRescue : MonoBehaviour
{
    [Header("Cat Settings")]
    [SerializeField] private GameObject catModel; // Reference to the cat GameObject (can be inactive at start)
    [SerializeField] private Transform[] spawnPoints; // Array of possible spawn positions on the tree
    
    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnTime = 30f; // Minimum time before cat spawns (in seconds)
    [SerializeField] private float maxSpawnTime = 60f; // Maximum time before cat spawns (in seconds)
    
    [Header("Rescue Settings")]
    [SerializeField] private float rescueDistance = 3f; // How close player needs to be
    [SerializeField] private float holdTime = 2f; // How long to hold E to rescue
    [SerializeField] private int rescuePoints = 5; // Points awarded for rescue
    
    [Header("UI Settings")]
    [SerializeField] private GameObject rescueUI; // UI prompt (e.g., "Hold E to rescue cat")
    [SerializeField] private Image holdProgressBar; // Optional progress bar for hold duration
    [SerializeField] private GameObject rescueText; // Text showing "Hold E to rescue cat"
    
    [Header("Player Reference")]
    [SerializeField] private Transform playerCamera; // Reference to player's camera for raycast
    
    private bool catActive = false;
    private bool isRescuing = false;
    private float rescueProgress = 0f;
    private Transform currentSpawnPoint;
    private Coroutine spawnCoroutine;

    GameManager manager;

    void Start()
    {
        manager = GetComponent<GameManager>();
        // Hide cat and UI at start
        if (catModel != null)
        {
            catModel.SetActive(false);
        }
        
        if (rescueUI != null)
        {
            rescueUI.SetActive(false);
        }
        
        // Find player camera if not assigned
        if (playerCamera == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerCamera = player.GetComponentInChildren<Camera>().transform;
            }
        }
        
        // Start spawn timer
        StartSpawnTimer();
    }

    void Update()
    {
        if (!catActive)
            return;
        
        // Check if player is looking at the cat and within range
        bool canRescue = IsPlayerLookingAtCat();
        
        if (canRescue)
        {
            // Show UI prompt
            if (rescueUI != null)
            {
                rescueUI.SetActive(true);
            }
            
            // Check if player is holding E
            if (Keyboard.current != null && Keyboard.current.eKey.isPressed)
            {
                isRescuing = true;
                rescueText.SetActive(true);
                rescueProgress += Time.deltaTime;
                
                // Update progress bar if it exists
                if (holdProgressBar != null)
                {
                    holdProgressBar.fillAmount = rescueProgress / holdTime;
                }
                
                // Check if rescue is complete
                if (rescueProgress >= holdTime)
                {
                    RescueCat();
                    rescueText.SetActive(false);
                }
            }
            else
            {
                // Reset if player releases E
                if (isRescuing)
                {
                    isRescuing = false;
                    rescueText.SetActive(false);
                    rescueProgress = 0f;
                    
                    if (holdProgressBar != null)
                    {
                        holdProgressBar.fillAmount = 0f;
                    }
                }
            }
        }
        else
        {
            // Hide UI and reset progress when not looking at cat
            if (rescueUI != null)
            {
                rescueUI.SetActive(false);
                rescueText.SetActive(false);
            }
            
            if (isRescuing)
            {
                isRescuing = false;
                rescueProgress = 0f;
                
                if (holdProgressBar != null)
                {
                    holdProgressBar.fillAmount = 0f;
                    rescueText.SetActive(false);
                }
            }
        }
    }

    // Check if player is looking at the cat
    private bool IsPlayerLookingAtCat()
    {
        if (playerCamera == null || catModel == null)
            return false;
        
        // Raycast from camera
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, rescueDistance))
        {
            // Check if we hit the cat or its parent
            if (hit.collider.gameObject == catModel || hit.collider.transform.IsChildOf(catModel.transform))
            {
                return true;
            }
        }
        
        return false;
    }

    // Start the random spawn timer
    private void StartSpawnTimer()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        
        spawnCoroutine = StartCoroutine(SpawnCatAfterDelay());
    }

    // Coroutine to spawn cat after random delay
    private IEnumerator SpawnCatAfterDelay()
    {
        // Wait for random time
        float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
        yield return new WaitForSeconds(waitTime);
        
        // Spawn the cat
        SpawnCat();
    }

    // Spawn the cat at a random location
    private void SpawnCat()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned for cat!");
            return;
        }
        
        // Choose random spawn point
        int randomIndex = Random.Range(0, spawnPoints.Length);
        currentSpawnPoint = spawnPoints[randomIndex];
        
        // Position and activate cat
        if (catModel != null && currentSpawnPoint != null)
        {
            catModel.transform.position = currentSpawnPoint.position;
            catModel.transform.rotation = currentSpawnPoint.rotation;
            catModel.SetActive(true);
            catActive = true;
            
            Debug.Log("Cat spawned at position: " + currentSpawnPoint.name);
        }
    }

    // Rescue the cat and award points
    private void RescueCat()
    {
        // Award points through GameManager
        if (GameManager.instance != null)
        {
            //GameManager.instance.playerPoints += rescuePoints;
            GameManager.instance.SaveCat();
            Debug.Log("Cat rescued! Awarded " + rescuePoints + " points. Total: " + GameManager.instance.playerPoints);
        }
        
        // Hide cat and UI
        if (catModel != null)
        {
            catModel.SetActive(false);
        }
        
        if (rescueUI != null)
        {
            rescueUI.SetActive(false);
        }
        
        // Reset rescue state
        catActive = false;
        isRescuing = false;
        rescueProgress = 0f;
        
        if (holdProgressBar != null)
        {
            holdProgressBar.fillAmount = 0f;
        }
        
        // Start new spawn timer
        StartSpawnTimer();
    }

    // Optional: Visualize rescue distance in editor
    private void OnDrawGizmosSelected()
    {
        if (catModel != null && catModel.activeSelf)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(catModel.transform.position, rescueDistance);
        }
        
        // Draw spawn points
        if (spawnPoints != null)
        {
            Gizmos.color = Color.yellow;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, 0.3f);
                }
            }
        }
    }
}