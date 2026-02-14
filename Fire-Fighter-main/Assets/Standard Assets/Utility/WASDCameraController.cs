using UnityEngine;
using UnityEngine.InputSystem;

public class WASDCameraController : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        // Get input from WASD keys using new Input System
        float moveX = 0f;
        float moveZ = 0f;

        Keyboard keyboard = Keyboard.current;
        
        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed)
                moveZ += 1f;
            if (keyboard.sKey.isPressed)
                moveZ -= 1f;
            if (keyboard.aKey.isPressed)
                moveX -= 1f;
            if (keyboard.dKey.isPressed)
                moveX += 1f;
        }

        // Calculate movement direction
        Vector3 movement = new Vector3(moveX, 0f, moveZ);

        // Move the camera
        transform.position += movement * moveSpeed * Time.deltaTime;

        // Clamp camera position within boundaries
        Vector3 pos = transform.position;

        if (pos.x <= -5)
            pos.x = -5;
        if (pos.x >= 5)
            pos.x = 5;
        if (pos.z <= -14)
            pos.z = -14;
        if (pos.z >= -6)
            pos.z = -6;

        transform.position = pos;
    }
}