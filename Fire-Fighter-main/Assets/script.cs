using UnityEngine;
using UnityEngine.UI; // for Button (if using uGUI)

public class ToggleImage : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject imageObject; // UI Image GameObject (or any GameObject you want to hide/show)
    public Button toggleButton;     // UI Button

    private bool isVisible = false;

    void Awake()
    {
        // Hook up the button click
        toggleButton.onClick.AddListener(Toggle);

        // Optional: ensure starting state matches
        imageObject.SetActive(isVisible);
    }

    public void Toggle()
    {
        isVisible = !isVisible;
        imageObject.SetActive(isVisible);

        // Optional: update button text (if it has a child Text component)
        var text = toggleButton.GetComponentInChildren<Text>();
        if (text != null)
            text.text = isVisible ? "Hide" : "Show";
    }
}