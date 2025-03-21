using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSwitcher : MonoBehaviour
{
    private Gun currentGun;
    private int selectedWeapon = 0;

    public static Action<int, int> OnWeaponChanged;
    public static Action<Gun> OnGunSwitched;

    public Gun CurrentGun => currentGun;

    private void Awake()
    {
        if (transform.childCount > 0) SelectWeapon();
        else Debug.LogWarning("GunSwitcher has no weapons assigned!");
    }

    private void Update()
    {
        if (currentGun != null && currentGun.IsAiming) return;

        ScrollWeapons();
        NumberWeaponChange();
    }

    private void ScrollWeapons()
    {   
        int previousSelectedWeapon = selectedWeapon;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f) selectedWeapon = (selectedWeapon + 1) % transform.childCount;
        else if (scroll < 0f) selectedWeapon = (selectedWeapon - 1 + transform.childCount) % transform.childCount;

        if (previousSelectedWeapon != selectedWeapon) SelectWeapon();
    }

    private void NumberWeaponChange()
    {
        int previousSelectedWeapon = selectedWeapon;

        if (Input.GetKeyDown(KeyCode.Alpha1) && transform.childCount > 0) selectedWeapon = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount > 1) selectedWeapon = 1;

        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    private void SelectWeapon()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform weapon = transform.GetChild(i);
            weapon.gameObject.SetActive(i == selectedWeapon);
        }

        currentGun = transform.GetChild(selectedWeapon).GetComponent<Gun>();

        if (currentGun == null)
        {
            Debug.LogWarning("Selected weapon does not have a Gun component!");
            return;
        }

        OnWeaponChanged?.Invoke(currentGun.currentAmmo, currentGun.currentReserveAmmo);
        OnGunSwitched?.Invoke(currentGun);
    }
}