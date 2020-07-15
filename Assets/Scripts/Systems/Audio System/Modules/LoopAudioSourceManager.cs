using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopAudioSourceManager : ILoopAudioSourceManager
{
    public void AddAudioSource(GameObject carrier, AudioClip audioClip, AudioCurves audioCurves, 
        int priority, float volume, float pitch, bool ignorePause)
    {
        AddAndConfigureAudioSource(carrier, audioClip, audioCurves, priority, volume, pitch, ignorePause);
    }

    public void RemoveAudioSource(GameObject carrier, AudioClip audioClip)
    {
        AudioSource attachedAudioSourceToRemove = null;
        AudioSource[] attachedAudioSources = carrier.GetComponents<AudioSource>();
        foreach (AudioSource attachedAudioSource in attachedAudioSources)
        {
            if (attachedAudioSource.clip == audioClip)
            {
                attachedAudioSourceToRemove = attachedAudioSource;
            }
        }
        Object.Destroy(attachedAudioSourceToRemove);
    }

    private AudioSource AddAndConfigureAudioSource(GameObject carrier, AudioClip audioClip, AudioCurves audioCurves,
        int priority, float volume, float pitch, bool ignorePause)
    {
        AudioSource newSource = carrier.AddComponent<AudioSource>();
        newSource.clip = audioClip;
        newSource.volume = volume;
        newSource.pitch = pitch;
        newSource.ignoreListenerPause = ignorePause;
        newSource.loop = true;
        newSource.panStereo = 0;
        newSource.playOnAwake = true;
        newSource.priority = priority;
        newSource.maxDistance = audioCurves.maxDistance;
        newSource.rolloffMode = AudioRolloffMode.Custom;
        newSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, audioCurves.customRolloffCurve);
        newSource.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, audioCurves.reverbZoneMixCurve);
        newSource.SetCustomCurve(AudioSourceCurveType.SpatialBlend, audioCurves.spatialBlendCurve);
        newSource.SetCustomCurve(AudioSourceCurveType.Spread, audioCurves.spreadCurve);
        newSource.dopplerLevel = 0;
        newSource.Play();
        return newSource;
    }
}
