using UnityEngine;

public class PhysicsPickup : MonoBehaviour
{
    public Transform objectToMove; // The object to move
    public float planeDistance = 5f; // Distance of the plane from the camera

    // Update is called once per frame
    void Update()
    {
        // Check if the left mouse button is clicked
        if (Input.GetMouseButton(0))
        {
            // Get the main camera
            Camera mainCamera = Camera.main;

            if (mainCamera == null)
            {
                Debug.LogError("Main camera not found. Assign a camera to the MainCamera tag.");
                return;
            }

            // Get the mouse position
            Vector3 mousePosition = Input.mousePosition;

            // Create a ray from the mouse position
            Ray rayToMouse = mainCamera.ScreenPointToRay(mousePosition);

            // Define a plane parallel to the camera's view at a fixed distance
            Vector3 planeOrigin = mainCamera.transform.position + mainCamera.transform.forward * planeDistance;
            Plane cameraAlignedPlane = new Plane(-mainCamera.transform.forward, planeOrigin);

            // Check if the ray intersects the plane
            if (cameraAlignedPlane.Raycast(rayToMouse, out float enter))
            {
                // Calculate the intersection point
                Vector3 hitPoint = rayToMouse.GetPoint(enter);

                // Move the object to the intersection point
                if (objectToMove != null)
                {
                    //objectToMove.position = hitPoint;
                    objectToMove.GetComponent<Rigidbody>().MovePosition(new Vector3(hitPoint.x, hitPoint.y, objectToMove.transform.position.z));
                    Debug.Log($"Object moved to: {hitPoint}");
                }

                // Visualize the ray in the Scene view
                Debug.DrawRay(rayToMouse.origin, rayToMouse.direction * enter, Color.green, 2f);
            }
            else
            {
                Debug.Log("Ray did not intersect the camera-aligned plane.");
            }
        }
    }
}