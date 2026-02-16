using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class ExecutableLauncher : MonoBehaviour
{
    [Header("Executable Settings")]
    [Tooltip("Path to the executable file")]
    public string executablePath = "";
    
    [Tooltip("Launch the executable in fullscreen mode")]
    public bool launchFullscreen = true;
    
    [Tooltip("Optional command line arguments")]
    public string commandLineArguments = "";
    
    [Header("Interaction Settings")]
    [Tooltip("Tag of the player object")]
    public string playerTag = "Player";
    
    [Tooltip("Allow multiple launches")]
    public bool allowMultipleLaunches = false;
    
    [Header("Teleport Settings")]
    [Tooltip("Distance to push the player back")]
    public float pushBackDistance = 2f;
    
    [Tooltip("Use player's facing direction instead of collision direction")]
    public bool usePlayerFacingDirection = true;
    
    private bool hasLaunched = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that touched this has the player tag
        if (other.CompareTag(playerTag))
        {
            LaunchExecutable(other.transform);
        }
    }
    
    // Alternative: Use OnCollisionEnter if using regular colliders (not triggers)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            LaunchExecutable(collision.transform);
        }
    }

    private void LaunchExecutable(Transform playerTransform)
    {
        // Check if we've already launched and multiple launches aren't allowed
        if (hasLaunched && !allowMultipleLaunches)
        {
            UnityEngine.Debug.Log("Executable already launched. Multiple launches disabled.");
            return;
        }

        // Validate executable path
        if (string.IsNullOrEmpty(executablePath))
        {
            UnityEngine.Debug.LogError("Executable path is not set!");
            return;
        }

        if (!System.IO.File.Exists(executablePath))
        {
            UnityEngine.Debug.LogError($"Executable not found at path: {executablePath}");
            return;
        }

        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = executablePath;
            
            // Add command line arguments for fullscreen if applicable
            if (launchFullscreen && !string.IsNullOrEmpty(commandLineArguments))
            {
                startInfo.Arguments = commandLineArguments;
            }
            else if (launchFullscreen)
            {
                // Common fullscreen arguments for various applications
                // Modify based on your executable's requirements
                startInfo.Arguments = "-fullscreen";
            }
            else if (!string.IsNullOrEmpty(commandLineArguments))
            {
                startInfo.Arguments = commandLineArguments;
            }

            startInfo.UseShellExecute = true;
            
            Process.Start(startInfo);
            
            hasLaunched = true;
            UnityEngine.Debug.Log($"Successfully launched: {executablePath}");
            
            // Teleport the player backward
            TeleportPlayerBack(playerTransform);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"Failed to launch executable: {e.Message}");
        }
    }
    
    private void TeleportPlayerBack(Transform playerTransform)
    {
        Vector3 pushDirection;
        
        if (usePlayerFacingDirection)
        {
            // Push player backward based on their facing direction
            pushDirection = -playerTransform.forward;
        }
        else
        {
            // Push player away from this object
            pushDirection = (playerTransform.position - transform.position).normalized;
        }
        
        // Calculate new position
        Vector3 newPosition = playerTransform.position + (pushDirection * pushBackDistance);
        
        // Teleport the player
        // If player has a CharacterController, we need to disable and re-enable it
        CharacterController charController = playerTransform.GetComponent<CharacterController>();
        if (charController != null)
        {
            charController.enabled = false;
            playerTransform.position = newPosition;
            charController.enabled = true;
        }
        else
        {
            // For Rigidbody or simple transforms
            Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.position = newPosition;
            }
            else
            {
                playerTransform.position = newPosition;
            }
        }
        
        UnityEngine.Debug.Log($"Player teleported back by {pushBackDistance} units");
    }
}