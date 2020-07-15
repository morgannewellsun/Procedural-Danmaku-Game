using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimatedObjectSorter
{
    void Add(GameObject animatee, string layerName);

    void UpdateManaged();
}
