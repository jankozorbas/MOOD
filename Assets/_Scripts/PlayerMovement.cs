using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField]
    private float runSpeed = 7f;
    [SerializeField]
    private float crouchSpeed = 4f;
    [SerializeField]
    private float jumpHeight = 5f;
    [SerializeField]
    private float gravity = -9.81f;
    [SerializeField]
    private float crouchHeight = 1.3f;

    [Header("Ground Variables")]
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float groundRadius = .5f;
    [SerializeField]
    private LayerMask groundMask;

    private CharacterController characterController;
    private Vector3 currentVelocity = Vector3.zero;
    private bool isGrounded;
    private bool isCrouching = false;
    private float originalHeight;
    private float moveSpeed;

    // ADD COYOTE TIME

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        originalHeight = characterController.height;
        moveSpeed = runSpeed;
    }

    private void Update()
    {
        CheckIsGrounded();
        PlayerMove();
        PlayerJump();
        ApplyGravity();
        Crouch();
    }

    private void CheckIsGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);

        if (isGrounded && currentVelocity.y < 0f)
        {
            currentVelocity.y = -2f; //ground check might register that we are on the ground a bit earlier so it's better not to have it at 0f
        }
    }

    private void PlayerMove()
    {
        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        float verticalAxis = Input.GetAxisRaw("Vertical");

        Vector3 movementVector = transform.right * horizontalAxis + transform.forward * verticalAxis;

        characterController.Move(movementVector.normalized * moveSpeed * Time.deltaTime);
    }

    private void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            currentVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // v = sqrt(h * -2 * g)
        }
    }

    private void ApplyGravity()
    {
        currentVelocity.y += gravity * Time.deltaTime;
        characterController.Move(currentVelocity * Time.deltaTime); //second deltaTime because of the formula: delta y = 1/2g * t^2
    }

    private void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;

            if (isCrouching)
            {
                characterController.height = crouchHeight;
                moveSpeed = crouchSpeed;
            }   
            else if (!isCrouching)
            {
                characterController.height = originalHeight;
                moveSpeed = runSpeed;
            }  
        }
    }
}