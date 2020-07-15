using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioClipProvider
{
    void Prefetch();
    AudioClip GetAudioClip(string audioClipName);
}
