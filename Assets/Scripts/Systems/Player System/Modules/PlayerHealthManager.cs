using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : IPlayerHealthManager
{
    private int maximumHealth = 3;
    private int currentHealth = 3;

    public void ApplyDamage(int amount)
    {
        Debug.Log($"Player took {amount} damage.");
        currentHealth = currentHealth - amount;
        if (currentHealth == 0)
        {
            HandlePlayerDeath();
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    private void HandlePlayerDeath()
    {
        Debug.Log("Player died. Resetting health.");
        currentHealth = maximumHealth;
    }
}
