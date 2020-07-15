using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimatedObjectSpriteSwitcher
{
    void Add(GameObject animatee);
    void ApplySequence(GameObject animatee, SpriteSequence sequence);
    void UpdateManaged();
}
