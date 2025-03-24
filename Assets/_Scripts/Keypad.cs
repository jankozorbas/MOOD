using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keypad : MonoBehaviour
{
    [SerializeField] private List<Transform> gatePivots;
    [SerializeField] private Color closedColor;
    [SerializeField] private Color openColor;
    [SerializeField] private GameObject keypad;
    [SerializeField] private float openAngle = -90f;
    [SerializeField] private float openDuration = 1.5f;

    private bool isOpen = false;
    private bool playerInRange = false;
    private Light keypadLight;

    private void Awake()
    {
        keypadLight = keypad.GetComponentInChildren<Light>();
        keypadLight.type = LightType.Point;
    }

    private void Start()
    {
        keypadLight.color = closedColor;
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E) && !isOpen)
        {
            if (GameManager.Instance.GetKeyCount() > 0)
            {
                GameManager.Instance.RemoveKey();
                StartCoroutine(OpenGate());
                isOpen = true;
                keypadLight.color = openColor;
                Debug.Log("Opened");
                AudioManager.Instance.PlaySound("KeypadOpen");
            }
            else
            {
                //UI KEY NEEDED
                Debug.Log("Key needed");
                AudioManager.Instance.PlaySound("KeypadLocked");
            }
        }
    }

    private IEnumerator OpenGate()
    {
        foreach (Transform gate in gatePivots)
        {
            gate.GetComponentInChildren<BoxCollider>().enabled = false;
        }
        
        float elapsedTime = 0f;
        Dictionary<Transform, Quaternion> closedRotations = new();
        Dictionary<Transform, Quaternion> openRotations = new();

        for (int i = 0; i < gatePivots.Count; i++)
        {
            Transform gate = gatePivots[i];
            float adjustedAngle = i % 2  == 0 ? openAngle : -openAngle;
            closedRotations[gate] = gate.rotation;
            openRotations[gate] = Quaternion.Euler(gate.eulerAngles.x + adjustedAngle, gate.eulerAngles.y, gate.eulerAngles.z);
        }

        yield return new WaitForSeconds(1f);
        AudioManager.Instance.PlaySound("OpenGate");

        while (elapsedTime < openDuration)
        {
            float lerpProgress = elapsedTime / openDuration;
            
            foreach (Transform gate in gatePivots)
            { 
                gate.rotation = Quaternion.Lerp(closedRotations[gate], openRotations[gate], lerpProgress);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (Transform gate in gatePivots)
        {
            gate.rotation = openRotations[gate];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
