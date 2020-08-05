using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSystem : MonoBehaviour, IBulletSystem
{
    private const string BULLET_LAYER_NAME = "bullets";

    private class BulletState
    {
        public float radius = 0;
        public Vector2 velocity = Vector2.zero;
        public Vector2 referencePoint = Vector2.zero;
        public float radialAcceleration = 0;
        public float angularAcceleration = 0;
    }

    private class BulletChange
    {
        public float changeTime;
        public GameObject bulletObjectToChange;
        public string newAnimationName = null;
        public float? newAnimationSpeedMultiplier = null;
        public float? newRadius = null;
        public Vector2? newPosition = null;
        public Vector2? newVelocity = null;
        public Vector2? newReferencePoint = null;
        public float? newRadialAcceleration = null;
        public float? newAngularAcceleration = null;
    }

    Dictionary<GameObject, int> bulletIndices = new Dictionary<GameObject, int>();
    Queue<int> deletedIndices = new Queue<int>();

    List<BulletState> bulletStates = new List<BulletState>();
    List<BulletChange> scheduledBulletChanges = new List<BulletChange>();

    void Awake()
    {

    }

    void Start()
    {
        
    }

    void Update()
    {
        CullDestroyedBulletObjects();
        ApplyBulletChanges();
        StepBulletStates();
        CheckForPlayerCollision();
    }

    public GameObject SpawnBullet(
        string animationName, float animationSpeedMultiplier,
        float radius, 
        Vector2 initialPosition, Vector2 initialVelocity, 
        float radialAcceleration = 0, float angularAcceleration = 0)
    {
        GameObject bulletObject = new GameObject();
        bulletObject.name = $"Bullet {bulletObject.GetInstanceID()}";
        bulletObject.transform.position = initialPosition;
        GameDirector.spriteAnimationSystem.Add(bulletObject, BULLET_LAYER_NAME);
        GameDirector.spriteAnimationSystem.ApplyAnimation(bulletObject, animationName, animationSpeedMultiplier);
        int index = GetOrAddBulletIndex(bulletObject);
        BulletState bulletState = new BulletState();
        bulletState.radius = radius;
        bulletState.velocity = initialVelocity;
        bulletState.radialAcceleration = radialAcceleration;
        bulletState.angularAcceleration = angularAcceleration;
        bulletStates[index] = bulletState;
        return bulletObject;
    }

    public void ChangeBulletParams(
        GameObject bulletObject, float delay = 0, 
        string newAnimationName = null, float? newAnimationSpeedMultiplier = null,
        float? newRadius = null, 
        Vector2? newPosition = null, Vector2? newVelocity = null, 
        float? newRadialAcceleration = null, float? newAngularAcceleration = null, 
        Vector2? newReferencePoint = null)
    {
        BulletChange bulletChange = new BulletChange();
        bulletChange.changeTime = Time.time + delay;
        bulletChange.bulletObjectToChange = bulletObject;
        bulletChange.newAnimationName = newAnimationName;
        bulletChange.newAnimationSpeedMultiplier = newAnimationSpeedMultiplier;
        bulletChange.newRadius = newRadius;
        bulletChange.newPosition = newPosition;
        bulletChange.newVelocity = newVelocity;
        bulletChange.newRadialAcceleration = newRadialAcceleration;
        bulletChange.newAngularAcceleration = newAngularAcceleration;
        bulletChange.newReferencePoint = newReferencePoint;
        scheduledBulletChanges.Add(bulletChange);
    }

    public void DestroyBullet(GameObject bulletObject)
    {
        int index = bulletIndices[bulletObject];
        bulletIndices.Remove(bulletObject);
        deletedIndices.Enqueue(index);
    }

    public void ClearBullets()
    {
        foreach (GameObject bulletObject in bulletIndices.Keys)
        {
            Destroy(bulletObject);
        }
        bulletIndices = new Dictionary<GameObject, int>();
        deletedIndices = new Queue<int>();
        bulletStates = new List<BulletState>();
        scheduledBulletChanges = new List<BulletChange>();
    }

    private int GetOrAddBulletIndex(GameObject bulletObject)
    {
        int index;
        if (bulletObject == null)
        {
            throw new Exception("Bullet object must not be null or destroyed.");
        }
        else if (bulletIndices.TryGetValue(bulletObject, out index))
        {
            return index;
        }
        else 
        {
            try
            {
                index = deletedIndices.Dequeue();
                bulletIndices.Add(bulletObject, index);
            }
            catch (InvalidOperationException) 
            {
                index = bulletStates.Count;
                bulletIndices.Add(bulletObject, index);
            }
            return index;
        }
    }

    private void CullDestroyedBulletObjects()
    {
        List<GameObject> deletedBullets = new List<GameObject>();
        foreach (KeyValuePair<GameObject, int> entry in bulletIndices)
        {
            deletedBullets.Add(entry.Key);
            deletedIndices.Enqueue(entry.Value);
        }
        foreach (GameObject deletedBullet in deletedBullets)
        {
            bulletIndices.Remove(deletedBullet);
        }
    }

    private void ApplyBulletChanges()
    {
        foreach (BulletChange bulletChange in scheduledBulletChanges)
        {
            if (bulletChange.changeTime <= Time.time)
            {
                int index = bulletIndices[bulletChange.bulletObjectToChange];
                if (bulletChange.newAnimationName != null)
                {
                    GameDirector.spriteAnimationSystem.ApplyAnimation(
                        bulletChange.bulletObjectToChange,
                        bulletChange.newAnimationName,
                        bulletChange.newAnimationSpeedMultiplier ?? 1);
                }
                if (bulletChange.newAnimationSpeedMultiplier != null)
                {
                    GameDirector.spriteAnimationSystem.SetAnimationSpeed(
                        bulletChange.bulletObjectToChange,
                        bulletChange.newAnimationSpeedMultiplier.GetValueOrDefault());
                }
                if (bulletChange.newRadius != null)
                {
                    bulletStates[index].radius = bulletChange.newRadius.GetValueOrDefault();
                }
                if (bulletChange.newPosition != null)
                {
                    bulletChange.bulletObjectToChange.transform.position = bulletChange.newPosition.GetValueOrDefault();
                }
                if (bulletChange.newVelocity != null)
                {
                    bulletStates[index].velocity = bulletChange.newVelocity.GetValueOrDefault();
                }
                if (bulletChange.newReferencePoint != null)
                {
                    bulletStates[index].referencePoint = bulletChange.newReferencePoint.GetValueOrDefault();
                }
                if (bulletChange.newRadialAcceleration != null)
                {
                    bulletStates[index].radialAcceleration = bulletChange.newRadialAcceleration.GetValueOrDefault();
                }
                if (bulletChange.newAngularAcceleration != null)
                {
                    bulletStates[index].angularAcceleration = bulletChange.newAngularAcceleration.GetValueOrDefault();
                }
            }
        }
    }

    private void StepBulletStates()
    {
        throw new NotImplementedException();
    }

    private void CheckForPlayerCollision()
    {
        throw new NotImplementedException();
    }
}
