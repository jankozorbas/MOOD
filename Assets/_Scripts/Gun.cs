using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun Stats")]
    [SerializeField]
    private float damage = 10f;
    [SerializeField]
    private float range = 100f;
    [SerializeField]
    private float fireRate = 10f;
    [SerializeField]
    private float impactForce = 100f;
    [SerializeField]
    private int maxAmmo = 30;
    [SerializeField]
    private float reloadTime = 2f;
    [Header("Visual FX")]
    [SerializeField]
    private ParticleSystem muzzleFlashFX;
    [SerializeField]
    private GameObject impactFX;

    private Camera mainCamera;
    private float nextShootTime = 0f;
    private int currentAmmo;
    private bool isReloading = false;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        currentAmmo = maxAmmo; //change this with the ammo you want to start the game with
    }

    private void Update()
    {
        if (isReloading) return;

        if (currentAmmo <= 0) //can reload only if we don't have ammo, add reloading whenever?
        {
            if (Input.GetKeyDown(KeyCode.R))
                StartCoroutine(ReloadRoutine());

            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= nextShootTime) //getbuttondown for semi automatic getbutton for automatic weapons
        {
            nextShootTime = Time.time + 1f / fireRate; //the bigger the fire rate the less time between shots
            Shoot();
        }
    }

    private IEnumerator ReloadRoutine()
    {
        //add animation 5:45 video
        isReloading = true;
        Debug.Log("Reloading..");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    private void Shoot()
    {
        muzzleFlashFX.Play();
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