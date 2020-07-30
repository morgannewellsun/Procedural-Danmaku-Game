using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedObjectSpriteSwitcher : IAnimatedObjectSpriteSwitcher
{
    private Dictionary<GameObject, int> animateeIndices = new Dictionary<GameObject, int>();
    private List<SpriteRenderer> animateeRenderers = new List<SpriteRenderer>();
    private List<SpriteSequence> animateeSequences = new List<SpriteSequence>();
    private List<int> animateeCurrentSpriteIndices = new List<int>();
    private List<float> animateeNextSpriteStartTimes = new List<float>();
    private List<float> animateeSpeedMultipliers = new List<float>();

    public void Add(GameObject animatee)
    {
        animateeIndices.Add(animatee, animateeRenderers.Count);
        SpriteRenderer animateeRenderer = GetOrAddSpriteRendererToAnimatee(animatee);
        animateeRenderers.Add(animateeRenderer);
        animateeSequences.Add(null);
        animateeCurrentSpriteIndices.Add(-1);
        animateeNextSpriteStartTimes.Add(-1);
        animateeSpeedMultipliers.Add(-1);
    }

    public void ApplySequence(GameObject animatee, SpriteSequence sequence, float speedMultiplier)
    {
        int animateeIndex = animateeIndices[animatee];
        animateeRenderers[animateeIndex].sprite = sequence.sprites[0];
        animateeSequences[animateeIndex] = sequence;
        animateeCurrentSpriteIndices[animateeIndex] = 0;
        animateeNextSpriteStartTimes[animateeIndex] = Time.time + (sequence.durations[0] / speedMultiplier);
        animateeSpeedMultipliers[animateeIndex] = speedMultiplier;
    }

    public void SetSequenceSpeed(GameObject animatee, float speedMultiplier)
    {
        int animateeIndex = animateeIndices[animatee];
        animateeSpeedMultipliers[animateeIndex] = speedMultiplier;
    }

    public void UpdateManaged()
    {
        CullDestroyedAnimatees();
        UpdateAnimateeSprites();
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

    private void CullDestroyedAnimatees()
    {
        List<GameObject> destroyedAnimatees = new List<GameObject>();
        List<int> destroyedAnimateeIndices = new List<int>();
        foreach (KeyValuePair<GameObject, int> entry in animateeIndices)
        {
            if (entry.Key == null)
            {
                destroyedAnimatees.Add(entry.Key);
                destroyedAnimateeIndices.Add(entry.Value);
            }
        }
        foreach (GameObject destroyedAnimatee in destroyedAnimatees)
        {
            animateeIndices.Remove(destroyedAnimatee);
        }
        foreach (int destroyedAnimateeIndex in destroyedAnimateeIndices)
        {
            animateeRenderers.RemoveAt(destroyedAnimateeIndex);
            animateeSequences.RemoveAt(destroyedAnimateeIndex);
            animateeCurrentSpriteIndices.RemoveAt(destroyedAnimateeIndex);
            animateeNextSpriteStartTimes.RemoveAt(destroyedAnimateeIndex);
            animateeSpeedMultipliers.RemoveAt(destroyedAnimateeIndex);
        }
    }

    private void UpdateAnimateeSprites()
    {
        for (int animateeIndex = 0; animateeIndex < animateeRenderers.Count; animateeIndex++)
        {
            if (animateeSequences[animateeIndex] != null) 
            { 
                if (Time.time > animateeNextSpriteStartTimes[animateeIndex])
                {
                    int nextSpriteIndex = animateeCurrentSpriteIndices[animateeIndex] + 1;
                    if (nextSpriteIndex >= animateeSequences[animateeIndex].sprites.Count)
                    {
                        nextSpriteIndex = 0;
                    }
                    animateeRenderers[animateeIndex].sprite = animateeSequences[animateeIndex].sprites[nextSpriteIndex];
                    animateeCurrentSpriteIndices[animateeIndex] = nextSpriteIndex;
                    animateeNextSpriteStartTimes[animateeIndex] = 
                        Time.time 
                        + (animateeSequences[animateeIndex].durations[nextSpriteIndex] / animateeSpeedMultipliers[animateeIndex]);
                }
            }
        }
    }
}
