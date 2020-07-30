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
        }
    }

    public SpriteSequence GetSequence(string sequenceName)
    {
        return spriteSequenceCache[sequenceName];
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
}
