using UnityEngine;
using TMPro;

public class TutorialPopup : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;  // Assign the UI Text in Inspector
    public Transform checkPosition;  // Empty object for detection area
    public Vector3 boxSize = new Vector3(2f, 2f, 2f); // Adjust area size per trigger
    public LayerMask playerLayer;  // Assign "Player" layer in Inspector
    public float displayTime = 2f; // Time text stays visible

    private bool hasTriggered = false;  // Ensures the text appears only once

    void Update()
    {
        if (hasTriggered) return; // If triggered, don't run again

        Collider[] colliders = Physics.OverlapBox(checkPosition.position, boxSize / 2, Quaternion.identity, playerLayer);
        if (colliders.Length > 0)
        {
            tutorialText.gameObject.SetActive(true);
            hasTriggered = true; // Mark this popup as "used"
            Invoke("HideText", displayTime); // Hide after set time
        }
    }

    void HideText()
    {
        tutorialText.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(checkPosition.position, boxSize);
    }
}
