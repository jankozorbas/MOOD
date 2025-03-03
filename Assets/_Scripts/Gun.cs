using System;
using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    [Space(10)]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 10f;
    [SerializeField] private float impactForce = 100f;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private bool isAutomatic = false;

    public int startAmmo = 30;
    public int currentAmmo;
    public int maxAmmo = 90;
    public int maxCurrentAmmo = 15;
    public int maxReserveAmmo = 90;
    public int currentReserveAmmo = 30;
    
    [Space(5)]
    [Header("Visual FX")]
    [Space(10)]
    [SerializeField] private ParticleSystem muzzleFlashFX;
    [SerializeField] private GameObject impactFX;
    [Space(5)]
    [Header("Recoil Settings")]
    [Space(10)]
    public float snappiness;
    public float returnSpeed;
    [Space(10)]
    [Header("Hipfire Recoil")]
    [Space(5)]
    public float recoilHipX;
    public float recoilHipY;
    public float recoilHipZ;

    private Camera mainCamera;
    private Animator animator;
    private Recoil recoilScript;
    private float nextShootTime = 0f;
    private bool isReloading = false;
    private bool isPistol;
    private bool isRifle;

    public static event Action<int, int> OnAmmoChanged;

    private void OnEnable()
    {
        // these two fix the bug where if we switch the weapon during reloading you can't shoot anymore
        isReloading = false;
        animator.SetBool("isReloadingAnimation", false);
        animator.enabled = false;
        GunSwitcher.OnWeaponChanged += FindCorrectAnimator;
    }

    private void OnDisable()
    {
        GunSwitcher.OnWeaponChanged -= FindCorrectAnimator;
    }

    private void Awake()
    {
        mainCamera = Camera.main;
        animator = FindObjectOfType<GunSwitcher>().gameObject.GetComponent<Animator>();
        recoilScript = FindObjectOfType<Recoil>().gameObject.GetComponent<Recoil>();
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
    }

    private void Reloading()
    {
        if (isReloading) return;

        if (currentAmmo < maxAmmo)
        {
            if (Input.GetKeyDown(KeyCode.R))
                StartCoroutine(ReloadRoutine());

            if (currentAmmo <= 0)
                return;
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
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextShootTime && currentAmmo > 0)
            {
                nextShootTime = Time.time + 1f / fireRate; // the bigger the fire rate the less time between shots
                Shoot();
            }
        }
    }

    private void FindCorrectAnimator(int one, int two)
    {
        animator = GetComponentInChildren<Animator>();
    }

    private IEnumerator ReloadRoutine()
    {
        if (isReloading || currentAmmo == maxCurrentAmmo) yield break;
        
        isReloading = true;
        animator.enabled = true;
        animator.SetBool("isReloading", true);

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
        muzzleFlashFX.Play();
        recoilScript.RecoilOnShoot();
        currentAmmo--;

        OnAmmoChanged?.Invoke(currentAmmo, currentReserveAmmo);

        RaycastHit hit;

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            EnemyBehavior enemy = hit.transform.gameObject.GetComponent<EnemyBehavior>();

            if (enemy != null && !enemy.IsDead)
                enemy.TakeDamage(damage);

            if (hit.rigidbody != null)
                hit.rigidbody.AddForce(-hit.normal * impactForce);

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
}