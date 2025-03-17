using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    [SerializeField] [Range(.1f, 3f)] private float mouseSensitivity = 1f;

    private Transform player;
    private float cameraRotation = 0f;

    public float MouseSensitivity { get { return mouseSensitivity; } set { mouseSensitivity = value; } }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        RotateMouse();
    }

    private void RotateMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

        cameraRotation -= mouseY;
        cameraRotation = Mathf.Clamp(cameraRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(cameraRotation, 0f, 0f);
        player.Rotate(Vector3.up * mouseX);
    }
}