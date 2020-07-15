using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpriteSequenceProvider
{
    void Preload();
    SpriteSequence GetSequence(string sequenceName, float sequenceDuration = -1);
}
