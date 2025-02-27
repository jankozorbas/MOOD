using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    private PlayerBehavior playerBehavior;

    public static Action<int> OnHealthPackPickedUp;

    private void Awake()
    {
        playerBehavior = FindObjectOfType<PlayerBehavior>().GetComponent<PlayerBehavior>();
    }

    public void RegenerateHealth()
    {
        int healthPackIncrease = UnityEngine.Random.Range(4, 7);
        playerBehavior.playerHealth = Mathf.Clamp(playerBehavior.playerHealth + healthPackIncrease, 0, playerBehavior.maxHealth);
        OnHealthPackPickedUp?.Invoke(playerBehavior.playerHealth);
    }
}