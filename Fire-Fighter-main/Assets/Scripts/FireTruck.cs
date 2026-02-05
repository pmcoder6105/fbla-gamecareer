using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class FiretruckController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float turnSpeed = 50f;
    
    private Rigidbody rb;
    private Vector2 moveInput;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        // Get WASD input
        moveInput.y = 0f;
        moveInput.x = 0f;
        
        if (Keyboard.current.wKey.isPressed) moveInput.y = 1f;
        if (Keyboard.current.sKey.isPressed) moveInput.y = -1f;
        if (Keyboard.current.aKey.isPressed) moveInput.x = -1f;
        if (Keyboard.current.dKey.isPressed) moveInput.x = 1f;
        
        // Move forward/backward
        transform.Translate(Vector3.forward * moveInput.y * speed * Time.deltaTime);
        
        // Turn left/right
        transform.Rotate(Vector3.up * moveInput.x * turnSpeed * Time.deltaTime);
    }
}