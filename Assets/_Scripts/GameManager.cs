using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 1f;
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private TMP_Text keyCountText;

    private bool isInteractable;
    private float keyCount = 0f;

    private void Update()
    {
        Interact();
    }

    private bool IsInteractable()
    {
        isInteractable = Physics.CheckSphere(interactionPoint.position, interactionRadius, interactableMask);
        return isInteractable;
    }

    private void Interact()
    {
        if (!IsInteractable()) return;

        else
        {
            Collider[] colliders = Physics.OverlapSphere(interactionPoint.position, interactionRadius, interactableMask);

            if (Input.GetKeyDown(KeyCode.E))
            {
                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject.CompareTag("Key"))
                    {
                        // CollectKey();
                        // UpdateUI();
                        keyCount++;
                        keyCountText.text = "Keys: " + keyCount.ToString();
                        Destroy(collider.gameObject);
                    }
                    else if (collider.gameObject.CompareTag("Ammo"))
                    {
                        // CollectAmmo();
                        //ammoCount += a number
                        Destroy(collider.gameObject);
                    }
                    else if (collider.gameObject.CompareTag("HealthPack"))
                    {
                        // CollectHealth();
                        //playerBehavior.health += 10f;
                        Destroy(collider.gameObject);
                    }
                    else
                        return;
                }
            }
        }
    }

    private void TimerCountdown()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(interactionPoint.position, interactionRadius);
    }
}