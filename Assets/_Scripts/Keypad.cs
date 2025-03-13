using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keypad : MonoBehaviour
{
    [SerializeField] private Transform gatePivot;
    [SerializeField] private float openAngle = -90f;
    [SerializeField] private float openDuration = 1.5f;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void Start()
    {
        closedRotation = gatePivot.rotation;
        openRotation = Quaternion.Euler(gatePivot.eulerAngles.x + openAngle, gatePivot.eulerAngles.y, gatePivot.eulerAngles.z);
    }

    private IEnumerator OpenGate()
    {
        float elapsedTime = 0f;

        while (elapsedTime < openDuration)
        {
            float lerpProgress = elapsedTime / openDuration;
            gatePivot.rotation = Quaternion.Lerp(closedRotation, openRotation, lerpProgress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gatePivot.rotation = openRotation;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E) && !isOpen)
        {
            if (GameManager.Instance.GetKeyCount() > 0)
            {
                GameManager.Instance.RemoveKey();
                StartCoroutine(OpenGate());
                isOpen = true;
                Debug.Log("Opened");
            }
            else
            {
                //UI KEY NEEDED
                Debug.Log("Key needed");
            }
        }
    }
}
