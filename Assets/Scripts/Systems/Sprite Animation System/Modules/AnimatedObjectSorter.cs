using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedObjectSorter : IAnimatedObjectSorter
{
    private readonly string[] layerNames = { "floor", "entities", "bullets", "effects" };

    private List<GameObject> animatees = new List<GameObject>();

    public void Add(GameObject animatee, string layerName)
    {
        SpriteRenderer addedRenderer = GetOrAddSpriteRendererToAnimatee(animatee);
        SetSpriteRendererLayer(addedRenderer, layerName);
        animatees.Add(animatee);
    }

    public void UpdateManaged()
    {
        CullDestroyedAnimatees();
        SetAnimateeHeights();
    }

    private SpriteRenderer GetOrAddSpriteRendererToAnimatee(GameObject animatee)
    {
        SpriteRenderer animateeRenderer = animatee.GetComponent<SpriteRenderer>();
        if (animateeRenderer == null)
        {
            animateeRenderer = animatee.AddComponent<SpriteRenderer>();
        }
        return animateeRenderer;
    }

    private void SetSpriteRendererLayer(SpriteRenderer configuree, string layerName)
    {
        for (short i = 0; i < layerNames.Length; i++)
        {
            if (layerName == layerNames[i])
            {
                configuree.sortingOrder = short.MinValue + i;
                break;
            }
        }
    }

    private void CullDestroyedAnimatees()
    {
        List<GameObject> destroyedAnimatees = new List<GameObject>();
        foreach(GameObject animatee in animatees)
        {
            if (animatee == null)
            {
                destroyedAnimatees.Add(animatee);
            }
        }
        foreach(GameObject destroyedAnimatee in destroyedAnimatees)
        {
            animatees.Remove(destroyedAnimatee);
        }
    }

    private void SetAnimateeHeights()
    {
        Camera mainCamera = GameDirector.cameraSystem.GetMainCamera();
        float zDelta = (mainCamera.nearClipPlane - mainCamera.farClipPlane) / animatees.Count;
        float zNext = mainCamera.transform.position.z + mainCamera.farClipPlane + 0.5f * zDelta;
        foreach(GameObject animatee in animatees)
        {
            animatee.transform.position = new Vector3(
                animatee.transform.position.x, 
                animatee.transform.position.y, 
                zNext);
            zNext += zDelta;
        }
    }
}
