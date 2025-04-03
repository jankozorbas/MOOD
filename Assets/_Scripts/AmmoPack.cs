using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPack : MonoBehaviour
{
    private Gun gun;
    private PlayerBehavior playerBehavior;
    private bool isPistol;
    private bool isRifle;

    private void Start()
    {
        gun = FindObjectOfType<Gun>().GetComponent<Gun>();
        playerBehavior = FindObjectOfType<PlayerBehavior>().GetComponent<PlayerBehavior>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Gun gun = other.GetComponentInChildren<Gun>();

        if (playerBehavior != null)
        {
            if (gun.currentReserveAmmo == gun.maxAmmo)
            {
                UIManager.Instance.ShowInteractionMessage("Max ammo reached");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerBehavior != null)
        {
            UIManager.Instance.HideInteractionMessage();
        }
    }
}