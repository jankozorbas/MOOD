using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinimapTutorial : MonoBehaviour
{
    public GameObject tutorialPanel;  // Assign the UI Panel in Inspector
    public TextMeshProUGUI continueText;  // Assign "Press any button to continue" text
    public Collider spawnBlocker; // Assign your invisible collider in Inspector

    private bool canClose = false;

    void Start()
    {
        GameManager.Instance.IsTimerActive = false;

        tutorialPanel.SetActive(true);  // Show the tutorial popup
        continueText.text = "";  // Hide text initially
        Invoke("EnableCloseMessage", 5f);  // Enable close message after 5 sec

        if (spawnBlocker != null)
            spawnBlocker.gameObject.SetActive(true);  // Ensure the blocker is active
    }

    void Update()
    {
        if (canClose && Input.anyKeyDown)
        {
            GameManager.Instance.IsTimerActive = true;

            tutorialPanel.SetActive(false);  // Hide the tutorial
            if (spawnBlocker != null)
                spawnBlocker.gameObject.SetActive(false);  // Remove movement blocker
        }
    }

    void EnableCloseMessage()
    {
        continueText.text = "Press any button to continue";  // Show closing message
        canClose = true;
    }
}
