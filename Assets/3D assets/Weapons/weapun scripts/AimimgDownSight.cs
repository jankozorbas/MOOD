using UnityEngine;

public class AimingDownSight : MonoBehaviour
{
    public Camera playerCamera;            // Reference to the player's camera
    public float aimDistanceX = -0.2f;     // Distance to move the gun on the X-axis (left-right) when aiming
    public float aimDistanceY = -0.1f;     // Distance to move the gun on the Y-axis (up-down) when aiming
    public float aimDistanceZ = -0.3f;     // Distance to move the gun on the Z-axis (forward-back) when aiming
    public float normalFOV = 60f;          // The normal FOV of the camera
    public float aimFOV = 30f;             // The FOV to switch to when aiming down sight
    public float aimSpeed = 8f;            // Speed of the gun movement when aiming
    public float fovSpeed = 5f;            // Speed of the FOV transition

    private Vector3 originalPosition;      // Store the original position of the gun
    private bool isAiming = false;         // Track if the player is aiming

    void Start()
    {
        // Save the original position of the gun
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        // Check if the right mouse button is pressed
        if (Input.GetMouseButton(1)) // 1 is the right mouse button
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }

        // If aiming, move the gun and change the FOV
        if (isAiming)
        {
            MoveGunDuringAim();
            AdjustFOV(aimFOV);
        }
        else
        {
            MoveGunBackToOriginalPosition();
            AdjustFOV(normalFOV);
        }
    }

    // Move the gun during aiming
    void MoveGunDuringAim()
    {
        Vector3 targetPosition = originalPosition + new Vector3(aimDistanceX, aimDistanceY, aimDistanceZ);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * aimSpeed);
    }

    // Move the gun back to its original position
    void MoveGunBackToOriginalPosition()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * aimSpeed);
    }

    // Adjust the camera's FOV
    void AdjustFOV(float targetFOV)
    {
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * fovSpeed);
    }
}
