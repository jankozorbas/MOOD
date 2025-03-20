using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSwitcher : MonoBehaviour
{
    private Gun gun;
    private int selectedWeapon = 0;

    public static Action<int, int> OnWeaponChanged;

    private void Awake()
    {
        gun = FindObjectOfType<Gun>();
    }

    private void Start()
    {
        SelectWeapon();
    }

    private void Update()
    {
        ScrollWeapons();
        NumberWeaponChange();
    }

    private void ScrollWeapons()
    {
        if (gun.IsAiming) return;
        
        int previousSelectedWeapon = selectedWeapon;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon >= transform.childCount - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon <= 0)
                selectedWeapon = transform.childCount - 1;
            else
                selectedWeapon--;
        }

        if (previousSelectedWeapon != selectedWeapon) 
            SelectWeapon();

        gun = FindObjectOfType<Gun>();
        OnWeaponChanged?.Invoke(gun.currentAmmo, gun.currentReserveAmmo);
    }

    private void NumberWeaponChange()
    {
        if (gun.IsAiming) return;
        
        int previousSelectedWeapon = selectedWeapon;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            selectedWeapon = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
            selectedWeapon = 1;

        if (previousSelectedWeapon != selectedWeapon)
            SelectWeapon();

        gun = FindObjectOfType<Gun>();
        OnWeaponChanged?.Invoke(gun.currentAmmo, gun.currentReserveAmmo);
    }

    private void SelectWeapon()
    {
        int i = 0;

        foreach(Transform weapon in transform)
        {
            if (i == selectedWeapon)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);

            i++;
        }
    }
}