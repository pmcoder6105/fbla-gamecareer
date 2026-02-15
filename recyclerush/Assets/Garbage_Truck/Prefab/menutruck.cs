using UnityEngine;

public class menutruck : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Starting position on the right side of the screen")]
    public Vector3 startPosition = new Vector3(20f, 0f, 0f);
    
    [Tooltip("Ending position on the left side of the screen")]
    public Vector3 endPosition = new Vector3(-20f, 0f, 0f);
    
    [Tooltip("Speed of the truck in units per second")]
    public float speed = 5f;
    
    [Tooltip("Delay before starting the return journey (in seconds)")]
    public float delayAtEnd = 0.5f;
    
    [Tooltip("If true, truck will instantly teleport back. If false, it smoothly moves back")]
    public bool instantReturn = false;
    
    [Header("Rotation Settings")]
    [Tooltip("Rotation speed when turning around (degrees per second)")]
    public float rotationSpeed = 180f;
    
    [Tooltip("Should the truck rotate to face movement direction?")]
    public bool rotateTowardsMovement = true;

    private float journeyLength;
    private float startTime;
    private bool movingToEnd = true;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private bool isRotating = false;
    private Quaternion targetRotation;
    private Quaternion startRotation;

    void Start()
    {
        // Set initial position
        transform.position = startPosition;
        
        // Calculate the journey length
        journeyLength = Vector3.Distance(startPosition, endPosition);
        
        // Set initial rotation to face the movement direction
        if (rotateTowardsMovement)
        {
            Vector3 direction = (endPosition - startPosition).normalized;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
        
        // Record the time when movement starts
        startTime = Time.time;
    }

    void Update()
    {
        // Handle rotation when changing direction
        if (isRotating)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation;
                isRotating = false;
                startTime = Time.time; // Reset timer for movement
            }
            return;
        }
        
        // Handle waiting at the end position
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= delayAtEnd)
            {
                isWaiting = false;
                waitTimer = 0f;
                
                // Start the return journey
                if (instantReturn)
                {
                    transform.position = startPosition;
                    movingToEnd = true;
                    
                    // Rotate to face forward again
                    if (rotateTowardsMovement)
                    {
                        Vector3 direction = (endPosition - startPosition).normalized;
                        if (direction != Vector3.zero)
                        {
                            transform.rotation = Quaternion.LookRotation(direction);
                        }
                    }
                    
                    startTime = Time.time;
                }
                else
                {
                    movingToEnd = false;
                    
                    // Start rotating to face the return direction
                    if (rotateTowardsMovement)
                    {
                        Vector3 returnDirection = (startPosition - endPosition).normalized;
                        if (returnDirection != Vector3.zero)
                        {
                            targetRotation = Quaternion.LookRotation(returnDirection);
                            isRotating = true;
                        }
                        else
                        {
                            startTime = Time.time;
                        }
                    }
                    else
                    {
                        startTime = Time.time;
                    }
                }
            }
            return;
        }

        // Calculate distance covered based on time and speed
        float distanceCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distanceCovered / journeyLength;

        // Move the truck
        if (movingToEnd)
        {
            // Move from start to end
            transform.position = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);

            // Check if reached the end
            if (fractionOfJourney >= 1f)
            {
                transform.position = endPosition;
                isWaiting = true;
            }
        }
        else
        {
            // Move from end back to start
            transform.position = Vector3.Lerp(endPosition, startPosition, fractionOfJourney);

            // Check if reached the start
            if (fractionOfJourney >= 1f)
            {
                transform.position = startPosition;
                movingToEnd = true;
                
                // Start rotating to face forward direction
                if (rotateTowardsMovement)
                {
                    Vector3 forwardDirection = (endPosition - startPosition).normalized;
                    if (forwardDirection != Vector3.zero)
                    {
                        targetRotation = Quaternion.LookRotation(forwardDirection);
                        isRotating = true;
                    }
                }
                else
                {
                    startTime = Time.time;
                }
            }
        }
    }

    // Helper method to visualize the path in the Scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPosition, 0.5f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(endPosition, 0.5f);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPosition, endPosition);
    }
}