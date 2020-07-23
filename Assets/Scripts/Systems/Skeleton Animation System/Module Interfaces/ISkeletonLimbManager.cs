using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkeletonLimbManager
{
    GameObject AttachLimb(GameObject baseObject, string limbLabel);
    GameObject GetLimb(GameObject baseObject, string limbLabel);
    void UpdateManaged();
}
