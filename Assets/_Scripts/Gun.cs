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
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private bool isAutomatic = false;
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
    [Space(10)]
    [Header("ADS Recoil")]
    [Space(5)]
    public float recoilAimX;
    public float recoilAimY;
    public float recoilAimZ;

    private Camera mainCamera;
    private Animator animator;
    private Recoil recoilScript;
    private float nextShootTime = 0f;
    private int currentAmmo;
    private bool isReloading = false;

    private void OnEnable()
    {
        // these two fix the bug where if we switch the weapon during reloading you can't shoot anymore
        isReloading = false;
        animator.SetBool("isReloadingAnimation", false);
        animator.enabled = false;
    }

    private void Awake()
    {
        mainCamera = Camera.main;
        animator = FindObjectOfType<GunSwitcher>().gameObject.GetComponent<Animator>();
        recoilScript = FindObjectOfType<Recoil>().gameObject.GetComponent<Recoil>();
    }

    private void Start()
    {
        currentAmmo = maxAmmo; // change this with the ammo you want to start the game with
    }

    private void Update()
    {
        Reloading();
        Shooting();
    }

    private void Reloading()
    {
        if (isReloading) return;

        if (currentAmmo < maxAmmo) // can reload only if we don't have ammo, add reloading whenever?
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
            if (Input.GetButton("Fire1") && Time.time >= nextShootTime && currentAmmo >= 0)
            {
                nextShootTime = Time.time + 1f / fireRate; // the bigger the fire rate the less time between shots
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextShootTime && currentAmmo >= 0)
            {
                nextShootTime = Time.time + 1f / fireRate; // the bigger the fire rate the less time between shots
                Shoot();
            }
        }
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        animator.enabled = true;
        animator.SetBool("isReloadingAnimation", true);

        yield return new WaitForSeconds(reloadTime - .25f);

        animator.SetBool("isReloadingAnimation", false);

        yield return new WaitForSeconds(.25f);

        currentAmmo = maxAmmo;
        animator.enabled = false;
        isReloading = false;
    }

    private void Shoot()
    {
        muzzleFlashFX.Play();
        recoilScript.RecoilOnShoot();
        currentAmmo--;

        RaycastHit hit;

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            Enemy enemy = hit.transform.gameObject.GetComponent<Enemy>();

            if (enemy != null)
                enemy.TakeDamage(damage);

            if (hit.rigidbody != null)
                hit.rigidbody.AddForce(-hit.normal * impactForce);

            GameObject impactObj = Instantiate(impactFX, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactObj, 1f);
        }
    }
}