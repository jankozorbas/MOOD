using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPack : MonoBehaviour
{
    private Gun gun;
    private bool isPistol;
    private bool isRifle;

    private void Start()
    {
        gun = FindObjectOfType<Gun>().GetComponent<Gun>();
    }

    public void AddAmmoPack()
    {
        CheckAndSetGunType();

        int ammoIncrease;

        if (isPistol)
        {
            ammoIncrease = Random.Range(5, 11);
            gun.currentAmmo += ammoIncrease;
        }
            
        else if (isRifle)
        {
            ammoIncrease = Random.Range(15, 26);
            gun.currentAmmo += ammoIncrease;
        }
    }

    private void CheckAndSetGunType()
    {
        if (gun.CompareTag("Pistol"))
        {
            isRifle = false;
            isPistol = true;
            gun = FindObjectOfType<Gun>().GetComponent<Gun>();
        }
        else if (gun.CompareTag("Rifle"))
        {
            isPistol = false;
            isRifle = true;
            gun = FindObjectOfType<Gun>().GetComponent<Gun>();
        }
    }
}