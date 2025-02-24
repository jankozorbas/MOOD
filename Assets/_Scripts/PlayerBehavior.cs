using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [Serializable]
    public class CharacterStance
    {
        public float cameraHeight;
        public CapsuleCollider stanceCollider;
    }

    public enum PlayerStance { Standing, Crouching };

    [Header("Player Settings")]
    [Space(10)]
    public float moveSpeed;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float crouchSpeed = 4f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private LayerMask playerMask;

    public int playerHealth = 10;

    [Header("Ground Settings")]
    [Space(10)]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = .5f;
    [SerializeField] private LayerMask groundMask;

    [HideInInspector] public bool isGrounded;

    //new crouching - put in correct places later
    [Header("Crouching Settings")]
    [Space(10)]
    [SerializeField] private float stanceSmoothing;
    [SerializeField] private CharacterStance playerStanceStanding;
    [SerializeField] private CharacterStance playerStanceCrouching;
    [SerializeField] private Transform feet;
    [SerializeField] private float stanceCheckErrorMargin = .05f;
    [SerializeField] private bool isToggle = false;

    private PlayerStance playerStance;
    
    private float cameraHeight;
    private float cameraVelocity;

    private Vector3 stanceColliderCenterVelocity;
    private float stanceColliderHeightVelocity;

    private CharacterStance currentStance;
    //end of crouching

    private CharacterController characterController;
    private Vector3 currentVelocity = Vector3.zero;
    private Camera mainCamera;
    private bool canJump = false;
    private float originalHeight;

    //Interactions
    [Header("Interactions")]
    [Space(10)]
    [SerializeField] private float interactionRadius = 1f;
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private LayerMask interactableMask;

    private bool isInteractable;

    public static Action<int> OnDamageTaken;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        originalHeight = characterController.height;
        moveSpeed = runSpeed;
        canJump = true;
        cameraHeight = mainCamera.transform.localPosition.y;
    }

    private void Update()
    {
        CheckIsGrounded();
        PlayerMove();
        PlayerJump();
        ApplyGravity();
        Crouch();
        CalculateStance();
        Interact();
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
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && canJump)
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
        if (isToggle)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (playerStance == PlayerStance.Standing)
                {
                    if (StanceCheck(playerStanceCrouching.stanceCollider.height))
                        return;

                    playerStance = PlayerStance.Crouching;
                    canJump = false;
                    moveSpeed = crouchSpeed;
                }
                else if (playerStance == PlayerStance.Crouching)
                {
                    if (StanceCheck(playerStanceStanding.stanceCollider.height))
                        return;

                    playerStance = PlayerStance.Standing;
                    canJump = true;
                    moveSpeed = runSpeed;
                }
            }

            return;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (playerStance == PlayerStance.Standing)
            {
                if (StanceCheck(playerStanceCrouching.stanceCollider.height))
                    return;

                playerStance = PlayerStance.Crouching;
                canJump = false;
                moveSpeed = crouchSpeed;
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (playerStance == PlayerStance.Crouching)
            {
                if (StanceCheck(playerStanceStanding.stanceCollider.height))
                    return;

                playerStance = PlayerStance.Standing;
                canJump = true;
                moveSpeed = runSpeed;
            }
        }
    }

    private void CalculateStance()
    {
        currentStance = playerStanceStanding;

        switch (playerStance)
        {
            case PlayerStance.Standing:
                currentStance = playerStanceStanding;
                break;
            case PlayerStance.Crouching:
                currentStance = playerStanceCrouching;
                break;
        }

        cameraHeight = Mathf.SmoothDamp(mainCamera.transform.localPosition.y, currentStance.cameraHeight, ref cameraVelocity, stanceSmoothing);
        mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, cameraHeight, mainCamera.transform.localPosition.z);

        characterController.height = Mathf.SmoothDamp(characterController.height,
                                                        currentStance.stanceCollider.height,
                                                        ref stanceColliderHeightVelocity,
                                                        stanceSmoothing);
        characterController.center = Vector3.SmoothDamp(characterController.center,
                                                        currentStance.stanceCollider.center,
                                                        ref stanceColliderCenterVelocity,
                                                        stanceSmoothing);
    }

    private bool StanceCheck(float stanceCheckHeight)
    {
        var startVector = new Vector3(feet.position.x, feet.position.y + characterController.radius + stanceCheckErrorMargin, feet.position.z);
        var endVector = new Vector3(feet.position.x,
                                    feet.position.y - characterController.radius - stanceCheckErrorMargin + stanceCheckHeight, 
                                    feet.position.z);

        return Physics.CheckCapsule(startVector, endVector, characterController.radius, playerMask);
    }

    public void TakeDamage(int damage)
    {
        playerHealth -= damage;
        OnDamageTaken?.Invoke(playerHealth);
        if (playerHealth <= 0) Die();
    }

    private void Die()
    {
        Debug.Log("You died.");
    }

    private bool CheckForInteractables()
    {
        isInteractable = Physics.CheckSphere(interactionPoint.position, interactionRadius, interactableMask);
        return isInteractable;
    }

    private void Interact()
    {
        if (!CheckForInteractables()) return;

        else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Collider[] colliders = Physics.OverlapSphere(interactionPoint.position, interactionRadius, interactableMask);

                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject.CompareTag("Key"))
                    {
                        CollectKey();
                        Destroy(collider.gameObject);
                        break;
                    }
                    else if (collider.gameObject.CompareTag("Ammo"))
                    {
                        CollectAmmo();
                        Destroy(collider.gameObject);
                        break;
                    }
                    else if (collider.gameObject.CompareTag("HealthPack"))
                    {
                        CollectHealth();
                        Destroy(collider.gameObject);
                        break;
                    }
                    else if (collider.gameObject.CompareTag("Bomb"))
                    {
                        GameManager.Instance.WinGame();
                        Destroy(collider.gameObject);
                        break;
                    }
                    else
                        return;
                }
            }
        }
    }

    private void CollectKey()
    {
        GameManager.Instance.AddKey();
    }

    private void CollectHealth()
    {
        FindObjectOfType<HealthPack>().GetComponent<HealthPack>().RegenerateHealth();
    }

    private void CollectAmmo()
    {
        FindObjectOfType<AmmoPack>().GetComponent < AmmoPack>().AddAmmoPack();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(interactionPoint.position, interactionRadius);
    }
}