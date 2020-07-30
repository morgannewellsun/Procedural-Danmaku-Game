using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimationSystem : MonoBehaviour, ISpriteAnimationSystem
{
    private IAnimatedObjectSorter animatedObjectSorter;
    private IAnimatedObjectSpriteSwitcher animatedObjectSpriteSwitcher;
    private ISpriteSequenceProvider spriteSequenceProvider;

    void Awake()
    {
        animatedObjectSorter = new AnimatedObjectSorter();
        animatedObjectSpriteSwitcher = new AnimatedObjectSpriteSwitcher();
        spriteSequenceProvider = new SpriteSequenceProvider();
        spriteSequenceProvider.Preload();
    }

    void Start()
    {
        
    }

    void Update()
    {
        animatedObjectSorter.UpdateManaged();
        animatedObjectSpriteSwitcher.UpdateManaged();
    }

    public void Add(GameObject animatee, string layerName)
    {
        animatedObjectSorter.Add(animatee, layerName);
        animatedObjectSpriteSwitcher.Add(animatee);
    }

    public void ApplyAnimation(GameObject animatee, string animationName, float speedMultiplier = 1)
    {
        SpriteSequence sequence = spriteSequenceProvider.GetSequence(animationName);
        animatedObjectSpriteSwitcher.ApplySequence(animatee, sequence, speedMultiplier);
    }

    public void SetAnimationSpeed(GameObject animatee, float speedMultiplier)
    {
        animatedObjectSpriteSwitcher.SetSequenceSpeed(animatee, speedMultiplier);
    }
}
