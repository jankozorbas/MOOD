using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private float damage = 10f;
    [SerializeField]
    private float range = 100f;
    [SerializeField]
    private ParticleSystem muzzleFlashFX;
    [SerializeField]
    private GameObject impactFX;

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        muzzleFlashFX.Play();

        RaycastHit hit;

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            Enemy enemy = hit.transform.gameObject.GetComponent<Enemy>();

            if (enemy != null)
                enemy.TakeDamage(damage);
            else
                return;

            //Instantiate(impactFX, hit.point, Quaternion.LookRotation(hit.normal)); //stopped at 10:00 mark
        }
        else
            return;
    }
}