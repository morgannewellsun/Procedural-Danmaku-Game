using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerHealthManager
{
    int GetCurrentHealth();
    void ApplyDamage(int amount);
}
