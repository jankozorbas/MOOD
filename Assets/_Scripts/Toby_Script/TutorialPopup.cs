using UnityEngine;
using TMPro;

public class TutorialPopup : MonoBehaviour
{
    public TextMeshProUGUI tutorialText; // Assign this in Inspector
    public Transform checkPosition; // Empty object placed where the trigger should be
    public Vector3 boxSize = new Vector3(2f, 2f, 2f); // Adjust this to fit your area
    public LayerMask playerLayer; // Assign "Player" layer in Inspector

    private bool playerInside = false;

    void Update()
    {
        // Check if player is inside the area
        Collider[] colliders = Physics.OverlapBox(checkPosition.position, boxSize / 2, Quaternion.identity, playerLayer);
        bool isPlayerHere = colliders.Length > 0;

        if (isPlayerHere && !playerInside)
        {
            tutorialText.gameObject.SetActive(true);
            playerInside = true;
        }
        else if (!isPlayerHere && playerInside)
        {
            tutorialText.gameObject.SetActive(false);
            playerInside = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(checkPosition.position, boxSize);
    }
}
