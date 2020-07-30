using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpriteAnimationSystem
{
    void Add(GameObject animatee, string layerName);
    void ApplyAnimation(GameObject animatee, string animationName, float speedMultiplier = 1);
    void SetAnimationSpeed(GameObject animatee, float speedMultiplier);
}
