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
}