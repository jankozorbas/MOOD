using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSwing : MonoBehaviour
{
    [Header("Swing Settings")]
    [Space(10)]
    [SerializeField] private float swingSmoothness = 5f;
    [SerializeField] private float swingMultiplier = 1f;

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * swingMultiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * swingMultiplier;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right); //negative mouseY because otherwise it is inverted
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);
        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, swingSmoothness * Time.deltaTime);
    }
}