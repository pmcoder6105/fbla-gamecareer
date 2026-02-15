using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashInteract : MonoBehaviour
{
    public int type;
    [SerializeField] NewTrash trashManager;
    [SerializeField] SoundManager sM;
    [SerializeField] UIManager uIM;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collision) {
        transform.root.GetComponent<TrashBin>().canClose = true;
        sM.PlayTrashSound(collision.name);
        Debug.Log(type + "trigger type");
        Debug.Log(trashManager.trashType + "trash type");
        if (trashManager.trashType == type) {
            //correct
            Debug.Log("Correct choice");
            sM.PlayPointSound(true);
            uIM.AddPoint();
        }
        else {
            //wrong
            Debug.Log("Wrong choice");
            sM.PlayPointSound(false);
            uIM.RemovePoint();
            uIM.DisplayTooltip(type, true, uIM.uiDisableTime);
            uIM.canClick = false;
        }
        trashManager.MakeNewTrash(); //this will be used for making new trash
    }
}
