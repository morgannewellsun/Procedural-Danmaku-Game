using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystem : MonoBehaviour, IPlayerSystem
{
    IPlayerMovementManager playerMovementManager;
    IPlayerHealthManager playerHealthManager;

    GameObject playerBaseObject;

    void Awake()
    {
        playerBaseObject = new GameObject("Player");
        playerMovementManager = new PlayerMovementManager();
        playerHealthManager = new PlayerHealthManager();
    }

    void Start()
    {
        playerMovementManager.CreateLimbObject();
    }

    void Update()
    {
        playerMovementManager.UpdateMovement();
    }

    public void ApplyDamage(int amount)
    {
        playerHealthManager.ApplyDamage(amount);
    }

    public GameObject GetPlayerBaseObject()
    {
        return playerBaseObject;
    }

    public Vector2 GetPlayerPosition()
    {
        return new Vector2(
            playerBaseObject.transform.position.x,
            playerBaseObject.transform.position.y);
    }

    public int GetPlayerHealth()
    {
        return playerHealthManager.GetCurrentHealth();
    }
}
