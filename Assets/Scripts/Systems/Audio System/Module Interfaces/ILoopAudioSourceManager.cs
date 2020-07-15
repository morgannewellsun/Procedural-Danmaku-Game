using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoopAudioSourceManager
{
    void AddAudioSource(GameObject carrier, AudioClip audioClip, AudioCurves audioCurves, int priority, float volume, float pitch, bool ignorePause);
    void RemoveAudioSource(GameObject carrier, AudioClip audioClip);
}
