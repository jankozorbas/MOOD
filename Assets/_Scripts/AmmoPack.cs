using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPack : MonoBehaviour
{
    private Gun gun;
    private PlayerBehavior playerBehavior;
    private bool isPistol;
    private bool isRifle;
    private bool playerInsideTrigger = false;

    private void Start()
    {
        gun = FindObjectOfType<Gun>().GetComponent<Gun>();
        playerBehavior = FindObjectOfType<PlayerBehavior>().GetComponent<PlayerBehavior>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideTrigger = true;
            Gun gun = FindObjectOfType<GunSwitcher>().GetComponent<GunSwitcher>().CurrentGun;

            if (playerBehavior != null)
            {
                CheckAmmo(gun);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideTrigger = false;
            UIManager.Instance.HideInteractionMessage();
        }
    }

    private void OnGunSwitched(Gun newGun)
    {
        if (playerInsideTrigger)
            CheckAmmo(newGun);
    }

    private void CheckAmmo(Gun gun)
    {
        if (gun.currentReserveAmmo == gun.maxAmmo && playerInsideTrigger)
        {
            UIManager.Instance.ShowInteractionMessage("Max ammo reached");
        }
        else
        {
            UIManager.Instance.HideInteractionMessage();
        }
    }

    private void OnEnable()
    {
        GunSwitcher.OnGunSwitched += OnGunSwitched;
    }

    private void OnDisable()
    {
        GunSwitcher.OnGunSwitched -= OnGunSwitched;
    }

    /*private void OnTriggerEnter(Collider other)
    {
        Gun gun = FindObjectOfType<GunSwitcher>().GetComponent<GunSwitcher>().CurrentGun;

        if (playerBehavior != null)
        {
            if (gun.currentReserveAmmo == gun.maxAmmo)
            {
                UIManager.Instance.ShowInteractionMessage("Max ammo reached");
            }
        }
    }*/

    /*private void OnTriggerExit(Collider other)
    {
        if (playerBehavior != null)
        {
            UIManager.Instance.HideInteractionMessage();
        }
    }*/
}