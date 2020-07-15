using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class SpriteSequenceProvider : ISpriteSequenceProvider
{
    private readonly string resourcesPathToSpecs = "SpriteSequenceSpecs";
    private readonly string resourcesPathToSprites = "Sprites";

    private Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

    private Dictionary<string, SpriteSequence> spriteSequenceCache = new Dictionary<string, SpriteSequence>();

    private Dictionary<string, float> cachedTotalDurations = new Dictionary<string, float>();
    private Dictionary<string, float> defaultTotalDurations = new Dictionary<string, float>();

    public void Preload()
    {
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>(resourcesPathToSprites);
        foreach (Sprite loadedSprite in loadedSprites)
        {
            spriteCache.Add(loadedSprite.name, loadedSprite);
        }

        SpriteSequenceSpec[] loadedSpecs = Resources.LoadAll<SpriteSequenceSpec>(resourcesPathToSpecs);
        foreach (SpriteSequenceSpec loadedSpec in loadedSpecs)
        {
            SpriteSequence defaultSequence = CreateDefaultSequenceFromSpec(loadedSpec);
            spriteSequenceCache.Add(loadedSpec.name, defaultSequence);

            float defaultDuration = defaultSequence.durations.Sum();
            cachedTotalDurations.Add(loadedSpec.name, defaultDuration);
            defaultTotalDurations.Add(loadedSpec.name, defaultDuration);
        }
    }

    public SpriteSequence GetSequence(string sequenceName, float sequenceDuration = -1)
    {
        if (sequenceDuration == -1)
        {
            sequenceDuration = defaultTotalDurations[sequenceName];
        }
        if (cachedTotalDurations[sequenceName] == sequenceDuration)
        {
            return spriteSequenceCache[sequenceName];
        }
        else
        {
            return RescaleCachedSequence(sequenceName, sequenceDuration); ;
        }
    }

    private SpriteSequence CreateDefaultSequenceFromSpec(SpriteSequenceSpec sequenceSpec)
    {
        SpriteSequence newDefaultSequence = new SpriteSequence();
        for (int i = 0; i < sequenceSpec.spriteNames.Length; i++)
        {
            Sprite nextSprite = spriteCache[sequenceSpec.spriteNames[i]];
            float nextDuration = sequenceSpec.defaultDurations[i];
            newDefaultSequence.sprites.Add(nextSprite);
            newDefaultSequence.durations.Add(nextDuration);
        }
        return newDefaultSequence;
    }

    private SpriteSequence RescaleCachedSequence(string sequenceName, float newTotalDuration)
    {
        SpriteSequence cachedSequence = spriteSequenceCache[sequenceName];
        float oldTotalDuration = cachedTotalDurations[sequenceName];
        for (int i = 0; i < cachedSequence.sprites.Count; i++)
        {
            cachedSequence.durations[i] *= (newTotalDuration / oldTotalDuration);
        }
        cachedTotalDurations[sequenceName] = newTotalDuration;
        return cachedSequence;
    }
}
