using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinimapTutorial : MonoBehaviour
{
    public GameObject tutorialPanel;  // Assign the UI Panel in Inspector
    public TextMeshProUGUI continueText;  // Assign TextMeshPro object
    private bool canClose = false;

    void Start()
    {
        tutorialPanel.SetActive(true);  // Show the tutorial popup
        continueText.text = "";  // Hide text initially
        Invoke("EnableCloseMessage", 5f);  // Enable close message after 5 sec
    }

    void Update()
    {
        if (canClose && Input.anyKeyDown)
        {
            tutorialPanel.SetActive(false);  // Hide the tutorial when any key is pressed
        }
    }

    void EnableCloseMessage()
    {
        continueText.text = "Press any button to continue";  // Show closing message
        canClose = true;
    }
}
