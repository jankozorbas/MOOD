using UnityEngine;
using TMPro;

public class TutorialPopup : MonoBehaviour
{
    public TextMeshProUGUI tutorialText; // Assign in Inspector
    public Transform checkPosition; // Empty object for detection position
    public Vector3 boxSize = new Vector3(2f, 2f, 2f); // Detection area size
    public LayerMask playerLayer; // Assign "Player" layer in Inspector
    public float displayTime = 2f; // Time in seconds before text disappears

    private bool hasTriggered = false; // Prevents text from showing again

    void Update()
    {
        if (hasTriggered) return; // If already triggered, do nothing

        Collider[] colliders = Physics.OverlapBox(checkPosition.position, boxSize / 2, Quaternion.identity, playerLayer);
        if (colliders.Length > 0) // Player enters trigger area
        {
            tutorialText.gameObject.SetActive(true);
            hasTriggered = true; // Mark as triggered
            Invoke("HideText", displayTime); // Hide after displayTime seconds
        }
    }

    void HideText()
    {
        tutorialText.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(checkPosition.position, boxSize);
    }
}
