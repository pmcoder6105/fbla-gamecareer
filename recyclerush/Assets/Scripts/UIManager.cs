using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] TMP_Text pointCounter;
    int points = 0;
    [SerializeField] TMP_Text timer;
    private float timeRemaining = 60f;
    private bool timerIsRunning = false;
    [SerializeField] GameObject[] tooltips;
    [SerializeField] TrashInteract trashManager;
    public bool canClick = false;
    public float uiDisableTime = 0.25f;

    [SerializeField] bool isMenu = false;

    
    void Awake() {
        if (isMenu)
            return;
        
        DisplayTooltip(4, true, 0f);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (isMenu)
            return;
        
        timerIsRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMenu)
            return;

        if (timerIsRunning) {
            if (timeRemaining > 0) {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else {
                Debug.Log("Time's up!");
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
    }

    //procedure for tooltip when gotten wrong
    //procedure for intial tooltip that plays at the start of the game

    public void AddPoint() {
        points++;
        pointCounter.text = points.ToString();
    }

    public void RemovePoint() {
        if (points-1 >= 0)
            points--;
        pointCounter.text = points.ToString();
    }

    public void ChangeClickBool(bool change) {
        canClick = change;
    }

    public void GoHome() {
        SceneManager.LoadScene(0);
    }

    private void DisplayTime(float timeToDisplay) {
        timeToDisplay++;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UIRemoveTooltip(int type) {
        StartCoroutine(WaitForTooltip(type, false, 0f));
    }

    public void DisplayTooltip(int type, bool action, float waitTime) {
        //int tooltipToDisplay = type-1;
        //int type = trashManager.type;
        StartCoroutine(WaitForTooltip(type, action, waitTime));
    }

    private IEnumerator WaitForTooltip(int type, bool action, float waitTime) {
        yield return new WaitForSeconds(waitTime);
        tooltips[type-1].SetActive(action);
        //Time.timeScale = action;
        if (action) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    public void MenuPlay() {
        SceneManager.LoadScene(1);
    }
    public void Quit() {
        Application.Quit();
    }
}
