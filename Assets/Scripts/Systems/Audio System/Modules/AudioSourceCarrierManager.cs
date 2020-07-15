using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceCarrierManager : IAudioSourceCarrierManager
{
    private Dictionary<GameObject, GameObject> audioSourceCarriers = new Dictionary<GameObject, GameObject>();

    public GameObject GetOrCreateAudioSourceCarrier(GameObject sourceObject)
    {
        GameObject storedCarrier;
        if (audioSourceCarriers.TryGetValue(sourceObject, out storedCarrier))
        {
            return storedCarrier;
        }
        else
        {
            GameObject createdCarrier = CreateAudioSourceCarrier(sourceObject);
            audioSourceCarriers.Add(sourceObject, createdCarrier);
            return createdCarrier;
        }
    }
    public void UpdateManaged()
    {
        CullDestroyedSourceObjects();
        TrackSourceObjects();
    }

    private GameObject CreateAudioSourceCarrier(GameObject sourceObject)
    {
        GameObject createdCarrier = new GameObject(sourceObject.name + " AudioSource Carrier");
        createdCarrier.transform.position = new Vector3(
            sourceObject.transform.position.x,
            sourceObject.transform.position.y,
            GameDirector.cameraSystem.GetMainCamera().transform.position.z);
        return createdCarrier;
    }

    private void CullDestroyedSourceObjects()
    {
        List<GameObject> destroyedSourceObjects = new List<GameObject>();
        List<GameObject> orphanedCarriers = new List<GameObject>();
        foreach (KeyValuePair<GameObject, GameObject> entry in audioSourceCarriers)
        {
            if (entry.Key == null)
            {
                destroyedSourceObjects.Add(entry.Key);
                orphanedCarriers.Add(entry.Value);
            }
        }
        foreach (GameObject destroyedSourceObject in destroyedSourceObjects)
        {
            audioSourceCarriers.Remove(destroyedSourceObject);
        }
        foreach (GameObject orphanedCarrier in orphanedCarriers)
        {
            Object.Destroy(orphanedCarrier);
        }
    }

    private void TrackSourceObjects()
    {
        foreach (KeyValuePair<GameObject, GameObject> entry in audioSourceCarriers)
        {
            entry.Value.transform.position = new Vector3(
                entry.Key.transform.position.x,
                entry.Key.transform.position.y,
                GameDirector.cameraSystem.GetMainCamera().transform.position.z);
        }
    }
}
