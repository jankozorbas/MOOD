using System;
using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum GunType { Pistol, Rifle }

    [Header("Gun Settings")]
    [Space(10)]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 10f;
    //[SerializeField] private float impactForce = 100f;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private bool isAutomatic = false;
    [SerializeField] private LayerMask shootMask;
    public GunType gunType;

    public int startAmmo = 30;
    public int currentAmmo;
    public int maxAmmo = 90;
    public int maxCurrentAmmo = 15;
    public int maxReserveAmmo = 90;
    public int currentReserveAmmo = 30;
    
    [Space(5)]
    [Header("Visuals")]
    [Space(10)]
    [SerializeField] private ParticleSystem muzzleFlashFX;
    [SerializeField] private GameObject impactFX;
    [SerializeField] private Canvas crosshairUI;
    [Space(5)]
    [Header("Recoil Settings")]
    [Space(10)]
    public float snappiness;
    public float returnSpeed;
    [Space(10)]
    [Header("Hipfire Recoil")]
    [Space(5)]
    [SerializeField] private float recoilHipX = -2f;
    [SerializeField] private float recoilHipY = 2f;
    [SerializeField] private float recoilHipZ = .35f;
    [SerializeField] private float adsRecoilX = -1f;
    [SerializeField] private float adsRecoilY = -1f;
    [SerializeField] private float adsRecoilZ = .35f;

    public float recoilX;
    public float recoilY;
    public float recoilZ;
    [Space(5)]

    [Header("ADS")]
    [Space(10)]
    [SerializeField] private float aimDistanceX = -.2f;
    [SerializeField] private float aimDistanceY = -.1f;
    [SerializeField] private float aimDistanceZ = -.3f;
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float aimFOV = 30f;
    [SerializeField] private float aimSpeed = 8f;
    [SerializeField] private float fovSpeed = 5f;
    [SerializeField] private float aimSmoothing = .3f;
    [SerializeField] private float aimSmoothRotation = .3f;
    [SerializeField] private float normalSmoothing = 10f;
    [SerializeField] private float normalSmoothRotation = 12f;

    private Vector3 originalPosition;
    private bool isAiming = false;

    private Camera mainCamera;
    private Animator animator;
    private Recoil recoilScript;
    private GunMovement gunMovement;
    private GunSwitcher gunSwitcher;
    private PlayerBehavior playerBehavior;
    private float nextShootTime = 0f;
    private bool isReloading = false;

    public static event Action<int, int> OnAmmoChanged;

    public bool IsAiming => isAiming;

    private void OnEnable()
    {
        // these two fix the bug where if we switch the weapon during reloading you can't shoot anymore
        isReloading = false;
        GunSwitcher.OnWeaponChanged += FindCorrectAnimator;
        //animator.SetBool("isReloading", false);
        animator.enabled = true;
        originalPosition = transform.localPosition;
    }

    private void OnDisable()
    {
        GunSwitcher.OnWeaponChanged -= FindCorrectAnimator;
    }

    private void Awake()
    {
        mainCamera = Camera.main;
        animator = FindObjectOfType<GunSwitcher>().gameObject.GetComponentInChildren<Animator>();
        recoilScript = FindObjectOfType<Recoil>().gameObject.GetComponent<Recoil>();
        gunMovement = FindObjectOfType<GunMovement>().gameObject.GetComponent<GunMovement>();
        gunSwitcher = FindObjectOfType<GunSwitcher>().GetComponent<GunSwitcher>();
        playerBehavior = FindObjectOfType<PlayerBehavior>().gameObject.GetComponent<PlayerBehavior>();
    }

    private void Start()
    {
        currentAmmo = startAmmo; // change this with the ammo you want to start the game with
        OnAmmoChanged?.Invoke(currentAmmo, currentReserveAmmo);
    }

    private void Update()
    {
        Reloading();
        Shooting();
        AimDownSight();
    }

    private void Reloading()
    {
        if (isReloading) return;

        if (currentAmmo < maxAmmo)
        {
            if (Input.GetKeyDown(KeyCode.R) && currentReserveAmmo > 0)
            {
                StartCoroutine(ReloadRoutine());
                // Play correct sound based on what gun it is
            }
                
        }
    }

    private void Shooting()
    {
        // SHOOTING BEHAVIOR BASED ON IF THE GUN IS AUTOMATIC OR SEMI AUTOMATIC
        if (isAutomatic)
        {
            if (Input.GetButton("Fire1") && Time.time >= nextShootTime && currentAmmo > 0)
            {
                nextShootTime = Time.time + 1f / fireRate; // the bigger the fire rate the less time between shots
                Shoot();
            }
            else if (Input.GetButtonDown("Fire1") && currentAmmo == 0)
                AudioManager.Instance.PlaySound("NoAmmo");
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextShootTime && currentAmmo > 0)
            {
                nextShootTime = Time.time + 1f / fireRate; // the bigger the fire rate the less time between shots
                Shoot();
            }
            else if (Input.GetButtonDown("Fire1") && currentAmmo == 0)
                AudioManager.Instance.PlaySound("NoAmmo");
        }
    }

    private void FindCorrectAnimator(int one, int two)
    {
        animator = GetComponent<Animator>();
    }

    private IEnumerator ReloadRoutine()
    {
        if (isReloading || currentAmmo == maxCurrentAmmo) yield break;
        
        isReloading = true;
        animator.enabled = true;
        animator.SetBool("isReloading", true);

        switch (gunType)
        {
            case GunType.Pistol:
                AudioManager.Instance.PlaySound("ReloadPistol");
                break;
            case GunType.Rifle:
                AudioManager.Instance.PlaySound("ReloadRifle");
                break;
        }

        yield return new WaitForSeconds(reloadTime - .25f);

        animator.SetBool("isReloading", false);

        yield return new WaitForSeconds(.25f);

        int ammoNeeded = maxCurrentAmmo - currentAmmo;
        int ammoToLoad = Mathf.Min(ammoNeeded, currentReserveAmmo);

        currentAmmo = Mathf.Clamp(currentAmmo + ammoToLoad, 0, maxCurrentAmmo);
        currentReserveAmmo -= ammoToLoad;
        currentReserveAmmo = Mathf.Clamp(currentReserveAmmo, 0, maxReserveAmmo);

        animator.enabled = false;
        isReloading = false;

        OnAmmoChanged?.Invoke(currentAmmo, currentReserveAmmo);
    }

    private void Shoot()
    {
        if (isReloading) return;

        switch (gunType)
        {
            case GunType.Pistol:
                AudioManager.Instance.PlaySound("ShootPistol");
                break;
            case GunType.Rifle:
                AudioManager.Instance.PlaySound("ShootRifle");
                break;
        }

        muzzleFlashFX.Play();
        recoilScript.RecoilOnShoot();
        currentAmmo--;

        OnAmmoChanged?.Invoke(currentAmmo, currentReserveAmmo);

        RaycastHit hit;

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, range, shootMask))
        {
            Debug.Log(hit.transform.name);

            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyBehavior enemy = hit.transform.gameObject.GetComponent<EnemyBehavior>();
                if (enemy != null && !enemy.IsDead)
                {
                    enemy.TakeDamage(damage);
                    return;
                }
                    
            }

            /*if (hit.rigidbody != null)
                hit.rigidbody.AddForce(-hit.normal * impactForce);*/
            
            GameObject impactObj = Instantiate(impactFX, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactObj, 1f);
        }
    }

    public void AddAmmo(int amount)
    {
        currentReserveAmmo = Mathf.Clamp(currentReserveAmmo + amount, 0, maxReserveAmmo);
        OnAmmoChanged?.Invoke(currentAmmo, currentReserveAmmo);
    }

    public int GetAmmoCount() { return currentAmmo; }

    private void AimDownSight()
    {
        if (Input.GetMouseButton(1)) isAiming = true;
        else isAiming = false;

        if (isAiming)
        {
            // Set the correct speed
            if (playerBehavior.PlayerStanceGetter == PlayerBehavior.PlayerStance.Standing)
                playerBehavior.moveSpeed = playerBehavior.AimStandSpeed * playerBehavior.SpeedMultiplier;
            else if (playerBehavior.PlayerStanceGetter == PlayerBehavior.PlayerStance.Crouching)
                playerBehavior.moveSpeed = playerBehavior.AimCrouchSpeed * playerBehavior.SpeedMultiplier;

            // Set the correct smoothing of the gun
            gunMovement.Smoothing = aimSmoothing;
            gunMovement.SmoothRotation = aimSmoothRotation;

            // Set the proper recoil amount

            recoilX = adsRecoilX;
            recoilY = adsRecoilY;
            recoilZ = adsRecoilZ;

            // Disable the crosshair
            crosshairUI.gameObject.SetActive(false);

            MoveGunDuringAim();
            AdjustFOV(aimFOV);
        }
        else
        {
            // Set the correct speed
            if (playerBehavior.PlayerStanceGetter == PlayerBehavior.PlayerStance.Standing)
                playerBehavior.moveSpeed = playerBehavior.RunSpeed * playerBehavior.SpeedMultiplier;
            else if (playerBehavior.PlayerStanceGetter == PlayerBehavior.PlayerStance.Crouching)
                playerBehavior.moveSpeed = playerBehavior.CrouchSpeed * playerBehavior.SpeedMultiplier;

            // Set the correct smoothing of the gun
            gunMovement.Smoothing = normalSmoothing;
            gunMovement.SmoothRotation = normalSmoothRotation;

            // Set the proper recoil amount

            recoilX = recoilHipX;
            recoilY = recoilHipY;
            recoilZ = recoilHipZ;

            // Enable the crosshair
            crosshairUI.gameObject.SetActive(true);

            MoveGunBack();
            AdjustFOV(normalFOV);
        }
    }

    private void MoveGunDuringAim()
    {
        Vector3 aimDistance = new Vector3(aimDistanceX, aimDistanceY, aimDistanceZ);
        Vector3 targetPosition = originalPosition + aimDistance;

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * aimSpeed);

        // fixes the weird positioning of the gun if you switch the weapon before it finishes lerping
        if (Vector3.Distance(transform.localPosition, targetPosition) < 0.01f) gunSwitcher.enabled = false;
    }

    private void MoveGunBack()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * aimSpeed);

        // fixes the weird positioning of the gun if you switch the weapon before it finishes lerping
        if (Vector3.Distance(transform.localPosition, originalPosition) < 0.01f) gunSwitcher.enabled = true;
    }

    private void AdjustFOV(float targetFOV)
    {
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * fovSpeed);
    }
}