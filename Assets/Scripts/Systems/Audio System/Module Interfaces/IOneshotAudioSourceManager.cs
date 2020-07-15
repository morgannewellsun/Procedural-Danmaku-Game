using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOneshotAudioSourceManager
{
    void AddAudioSource(GameObject carrier, AudioClip audioClip, AudioCurves audioCurves, int priority, 
        float volume, float pitch, bool ignorePause);
    void UpdateManaged();
}
