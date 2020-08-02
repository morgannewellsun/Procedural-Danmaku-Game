using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerSystem
{
    void ApplyDamage(int amount);
    GameObject GetPlayerBaseObject();
    Vector2 GetPlayerPosition();
    int GetPlayerHealth();
}
