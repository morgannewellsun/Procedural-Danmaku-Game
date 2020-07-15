using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioSystem
{
    void AttachListenerTo(GameObject listenerObject);
    void PlayLoop(GameObject sourceObject, string audioClipName, string audioCurvesName, int priority, 
        float volume = 1, float pitch = 1, bool ignorePause = false);
    void StopLoop(GameObject sourceObject, string audioClipName);
    void PlayOnce(GameObject sourceObject, string audioClipName, string audioCurvesName, int priority, 
        float volume = 1, float pitch = 1, bool ignorePause = false);
}
