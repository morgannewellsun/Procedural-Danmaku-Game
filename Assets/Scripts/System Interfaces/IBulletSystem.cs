using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBulletSystem
{
    GameObject SpawnBullet(
        string animationName, float animationSpeedMultiplier,
        float radius, 
        Vector2 initialPosition, Vector2 initialVelocity,
        float radialAcceleration = 0, float angularAcceleration = 0);
    void ChangeBulletParams(
        GameObject bulletObject, float delay = 0,
        string newAnimationName = null, float? newAnimationSpeedMultiplier = null,
        float? newRadius = null,
        Vector2? newPosition = null, Vector2? newVelocity = null,
        float? newRadialAcceleration = null, float? newAngularAcceleration = null,
        Vector2? newReferencePoint = null);
    void DestroyBullet(GameObject bulletObject);
    void ClearBullets();
}
