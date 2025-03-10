using System;
using System.Collections;
using TMPro;
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
    [SerializeField] private float aimStandSpeed = 3f;
    [SerializeField] private float aimCrouchSpeed = 1f;
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float hitSpeedMultiplier = .35f;
    [SerializeField] private float hitSpeedTime = .3f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private LayerMask playerMask;

    public int playerHealth = 10;
    public int maxHealth = 10;

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
    private bool messageDisplayed = false;
    private bool canMove = true;
    private bool isDead = false;

    public static Action<int> OnDamageTaken;

    public bool CanMove { get { return canMove; }  set { canMove = value; } }

    //Getters
    public float RunSpeed => runSpeed;
    public float CrouchSpeed => crouchSpeed;
    public float AimStandSpeed => aimStandSpeed;
    public float AimCrouchSpeed => aimCrouchSpeed;

    public float SpeedMultiplier => speedMultiplier;

    public PlayerStance PlayerStanceGetter => playerStance;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        originalHeight = characterController.height;
        moveSpeed = runSpeed;
        canMove = true;
        canJump = true;
        isDead = false;
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
        if (!canMove) return;

        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        float verticalAxis = Input.GetAxisRaw("Vertical");

        Vector3 movementVector = transform.right * horizontalAxis + transform.forward * verticalAxis;

        characterController.Move(movementVector.normalized * moveSpeed * Time.deltaTime);
    }

    private void PlayerJump()
    {
        if (!canMove) return;
        
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
        if (!canMove) return;

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

        // Make sure to go back up if you are below something but have already let go of the key and then moved out of the way

        if (Input.GetKey(KeyCode.LeftControl) == false && playerStance == PlayerStance.Crouching)
        {
            if (!StanceCheck(playerStanceStanding.stanceCollider.height))
            {
                playerStance = PlayerStance.Standing;
                canJump = true;
                moveSpeed = runSpeed;
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
        if (isDead) return;

        playerHealth -= damage;

        StartCoroutine(SlowDown());

        OnDamageTaken?.Invoke(playerHealth);

        if (playerHealth <= 0) Die();
    }

    private IEnumerator SlowDown()
    {
        speedMultiplier = hitSpeedMultiplier;
        UpdateSpeed();
        yield return new WaitForSeconds(hitSpeedTime);
        speedMultiplier = 1f;
        UpdateSpeed();
    }

    private void UpdateSpeed()
    {
        if (playerStance == PlayerStance.Standing)
        {
            moveSpeed = runSpeed * speedMultiplier;
        }
        else if (playerStance == PlayerStance.Crouching)
        {
            moveSpeed = crouchSpeed * speedMultiplier;
        }
    }

    private void Die()
    {
        Lose();
        Debug.Log("You died.");
    }

    private bool CheckForInteractables()
    {
        isInteractable = Physics.CheckSphere(interactionPoint.position, interactionRadius, interactableMask);
        return isInteractable;
    }

    private void InvokeSetActiveFalse()
    {
        UIManager.Instance.interactionText.gameObject.SetActive(false);
    }

    private void Interact()
    {
        if (!CheckForInteractables()) return;

        else
        {
            if (!messageDisplayed)
            {
                UIManager.Instance.interactionText.gameObject.SetActive(true);
                Invoke("InvokeSetActiveFalse", 3f);
                messageDisplayed = true;
            }

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
                        if (CollectAmmo()) Destroy(collider.gameObject);
                        break;
                    }
                    else if (collider.gameObject.CompareTag("HealthPack"))
                    {
                        if (CollectHealth()) Destroy(collider.gameObject);
                        break;
                    }
                    else if (collider.gameObject.CompareTag("Bomb") && GameManager.Instance.GetKeyCount() >= GameManager.Instance.KeysNeeded)
                    {
                        Win();
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

    private bool CollectHealth()
    {
        HealthPack healthPack = FindObjectOfType<HealthPack>().GetComponent<HealthPack>();

        if (playerHealth == maxHealth) return false;

        healthPack.RegenerateHealth();
        return true;
    }

    private bool CollectAmmo()
    {
        Gun gun = FindObjectOfType<Gun>().GetComponent<Gun>();

        if (gun.currentReserveAmmo == gun.maxAmmo) return false;

        int ammoAmount = 9;
        gun.AddAmmo(ammoAmount);
        return true;
    }

    private void Win()
    {
        canMove = false;
        GameManager.Instance.WinGame();
    }

    private void Lose()
    {
        canMove = false;
        isDead = true;
        GameManager.Instance.LoseGame();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(interactionPoint.position, interactionRadius);
    }
}