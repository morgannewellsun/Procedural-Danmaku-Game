using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonLimbManager : ISkeletonLimbManager
{
    private Dictionary<Tuple<GameObject, string>, GameObject> managedLimbObjects = 
        new Dictionary<Tuple<GameObject, string>, GameObject>();

    public GameObject AttachLimb(GameObject baseObject, string limbLabel)
    {
        GameObject newLimbObject = new GameObject(baseObject.name + "_" + limbLabel);
        managedLimbObjects.Add(new Tuple<GameObject, string>(baseObject, limbLabel), newLimbObject);
        return newLimbObject;
    }

    public GameObject GetLimb(GameObject baseObject, string limbLabel)
    {
        return managedLimbObjects[new Tuple<GameObject, string>(baseObject, limbLabel)];
    }

    public void UpdateManaged()
    {
        CullDeletedBaseObjects();
    }

    private void CullDeletedBaseObjects()
    {
        List<Tuple<GameObject, string>> deletedKeys = new List<Tuple<GameObject, string>>();
        foreach (Tuple<GameObject, string> key in managedLimbObjects.Keys)
        {
            if (key.Item1 == null)
            {
                deletedKeys.Add(key);
            }
        }
        foreach (Tuple<GameObject, string> deletedKey in deletedKeys)
        {
            managedLimbObjects.Remove(deletedKey);
        }
    }
}
