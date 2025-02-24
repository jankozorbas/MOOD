using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] private float healthPackIncrease = 10f;

    private PlayerBehavior playerBehavior;

    public static Action<float> OnHealthPackPickedUp;

    private void Awake()
    {
        playerBehavior = FindObjectOfType<PlayerBehavior>().GetComponent<PlayerBehavior>();
    }

    public void RegenerateHealth()
    {
        int healthPackIncrease = UnityEngine.Random.Range(8, 13);
        playerBehavior.playerHealth += healthPackIncrease;
        OnHealthPackPickedUp?.Invoke(playerBehavior.playerHealth);
    }
}