using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioSourceCarrierManager
{
    GameObject GetOrCreateAudioSourceCarrier(GameObject trackedGameObject);
    void UpdateManaged();
}
