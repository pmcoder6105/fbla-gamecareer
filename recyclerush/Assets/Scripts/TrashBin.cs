using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBin : MonoBehaviour
{

    Animator animator;
    bool m_isOpen = false;

    Camera mainCamera;
    Vector3 screenCenter;
    Ray rayFromCenter;

    Transform lid;
    [SerializeField] float speed = 0.5f;
    [SerializeField] float xRotation;
    public bool canClose = true;
    
    [SerializeField] NewTrash trashManager;
    [SerializeField] UIManager uIM;
    

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found. Assign a camera to the MainCamera tag.");
            return;
        }
        // Get the center of the screen
        screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        rayFromCenter = mainCamera.ScreenPointToRay(screenCenter);
        lid = transform.Find("TrashbinLid");
    }

    // Update is called once per frame
    void Update()
    {
        if (!uIM.canClick)
            return;

        Vector3 mousePosition = Input.mousePosition;

        // Convert both positions to world space using the camera
            
        Ray rayToMouse = mainCamera.ScreenPointToRay(mousePosition);

            // Visualize the rays in the Scene view
        //Debug.DrawRay(rayFromCenter.origin, rayFromCenter.direction * 100, Color.red, 2f);
        Debug.DrawRay(rayToMouse.origin, rayToMouse.direction * 100, Color.blue, 2f);
        RaycastHit hitInfo;


        if (Physics.Raycast(rayToMouse, out hitInfo, Mathf.Infinity)) {
            if (hitInfo.collider == this.gameObject.GetComponent<Collider>()) {
                //m_isOpen = true;
                // animator.SetBool("isOpen", true);
                //Debug.Log(hitInfo.collider.gameObject.transform.name);
                lid.localRotation = Quaternion.Lerp(lid.localRotation, Quaternion.Euler(xRotation, 0, 0), Time.deltaTime * speed);

                if (Input.GetMouseButtonDown(0)) {
                    canClose = false;
                    //check which bin and animate accordingly
                    int index = 0;
                    for (int i = 0; i < trashManager.bins.Length; i++){
                        if (trashManager.bins[i] == this.gameObject)
                            index = i;
                    }
                    trashManager.trash.GetComponent<Animator>().Play(trashManager.anims[index]);
                }
            }
            else
            {
                // check if person has clicked if not then close if so 
                StartCoroutine(Close());
                //m_isOpen = false;
            }
        
        }
        
    }

    IEnumerator Close() {
        yield return new WaitUntil(() => canClose);
        lid.localRotation = Quaternion.Lerp(lid.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * speed);

    }

    // void OnTriggerEnter(Collider collision) {
    //     Debug.Log("entered collision with " + collision.gameObject.name);

    //     if (collision.gameObject.tag == "Trash") {
    //         animator.SetBool("isOpen", true);
    //     }
    // }

    // void OnTriggerExit(Collider collision) {
    //     if (collision.gameObject.tag == "Trash") {
    //         animator.SetBool("isOpen", false);
    //     }
    // }
}
