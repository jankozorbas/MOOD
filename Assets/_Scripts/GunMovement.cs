using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMovement : MonoBehaviour
{
    // https://www.youtube.com/watch?v=DR4fTllQnXg I used this tutorial to make this happen, had to adjust some things but the idea comes from here.

    [Header("Settings")]
    [Space(10)]
    [SerializeField] private bool shouldSwing = true;
    [SerializeField] private bool shouldSwingRotate = true;
    [SerializeField] private bool shouldBobOffset = true;
    [SerializeField] private bool shouldBob = true;
    [Space(5)]
    [SerializeField] private float smoothing = 10f;
    [SerializeField] private float smoothRotation = 12f;
    [Space(10)]
    [Header("Swing Settings")]
    [Space(10)]
    [SerializeField] private float step = .01f;
    [SerializeField] private float maxStepDistance = .06f;
    private Vector3 swingPosition;
    [Space(5)]
    [SerializeField] private float rotationStep = 4f;
    [SerializeField] private float maxRotationStep = 5f;
    private Vector3 swingEulerRotation;
    [Space(10)]
    [Header("Bobbing Settings")]
    [Space(10)]
    [SerializeField] private Vector3 moveLimit = Vector3.one * 0.025f;
    [SerializeField] private Vector3 bobLimit = Vector3.one * 0.01f;
    private Vector3 bobPosition;
    [Space(5)]
    [SerializeField] private float speedCurve;
    private float curveSin { get => Mathf.Sin(speedCurve); }
    private float curveCos { get => Mathf.Cos(speedCurve); }
    [Space(10)]
    [SerializeField] private Vector3 multiplier;
    private Vector3 bobEulerRotation;

    private PlayerMovement playerMovement; //for groundchecking and movement speed
    private Vector2 moveInput;
    private Vector2 mouseInput;

    private void Awake()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        GetInput();
        Swing();
        SwingRotation();
        BobOffset();
        BobRotation();

        BobAndSwing();
    }

    private void GetInput()
    {
        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        float verticalAxis = Input.GetAxisRaw("Vertical");

        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        moveInput = new Vector2(horizontalAxis, verticalAxis).normalized;
        mouseInput = new Vector2(mouseX, mouseY);
    }

    private void Swing()
    {
        if (!shouldSwing) { swingPosition = Vector3.zero; return; }

        Vector3 invertMouse = mouseInput * -step;
        invertMouse.x = Mathf.Clamp(invertMouse.x, -maxStepDistance, maxStepDistance);
        invertMouse.y = Mathf.Clamp(invertMouse.y, -maxStepDistance, maxStepDistance);

        swingPosition = invertMouse;
    }

    private void SwingRotation()
    {
        if (!shouldSwingRotate) { swingEulerRotation = Vector3.zero; return; }

        Vector2 invertMouse = mouseInput * -rotationStep;
        invertMouse.x = Mathf.Clamp(invertMouse.x, -maxRotationStep, maxRotationStep);
        invertMouse.y = Mathf.Clamp(invertMouse.y, -maxRotationStep, maxRotationStep);

        swingEulerRotation = new Vector3(invertMouse.y, invertMouse.x, invertMouse.x);
    }

    private void BobOffset()
    {
        speedCurve += Time.deltaTime * (playerMovement.isGrounded ? playerMovement.moveSpeed : 1f) + 0.01f;

        if (!shouldBobOffset) { bobPosition = Vector3.zero; return; }

        bobPosition.x = (curveCos * bobLimit.x * (playerMovement.isGrounded ? 1 : 0)) - (moveInput.x * moveLimit.x);
        bobPosition.y = (curveSin * bobLimit.y) - (moveInput.y * moveLimit.y);
        bobPosition.z = -(moveInput.y * moveLimit.z); 
    }

    private void BobRotation()
    {
        if (!shouldBob) { bobEulerRotation = Vector3.zero; return; }

        bobEulerRotation.x = (moveInput != Vector2.zero ? multiplier.x * (Mathf.Sin(2 * speedCurve)) : multiplier.x * Mathf.Sin(2 * speedCurve) / 2);
        bobEulerRotation.y = (moveInput != Vector2.zero ? multiplier.y * curveCos : 0);
        bobEulerRotation.z = (moveInput != Vector2.zero ? multiplier.z * curveCos * moveInput.x : 0);
    }

    private void BobAndSwing()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, swingPosition + bobPosition, Time.deltaTime * smoothing);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, 
                                                    Quaternion.Euler(swingEulerRotation) * Quaternion.Euler(bobEulerRotation),
                                                    Time.deltaTime * smoothRotation);
    }
}