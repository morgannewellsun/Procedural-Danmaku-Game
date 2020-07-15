using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneshotAudioSourceManager : IOneshotAudioSourceManager
{
    HashSet<AudioSource> managedAudioSources = new HashSet<AudioSource>();

    public void AddAudioSource(GameObject carrier, AudioClip audioClip, AudioCurves audioCurves, 
        int priority, float volume, float pitch, bool ignorePause)
    {
        AudioSource addedAudioSource = AddAndConfigureAudioSource(
            carrier, audioClip, audioCurves, priority, volume, pitch, ignorePause);
        managedAudioSources.Add(addedAudioSource);
    }

    public void UpdateManaged()
    {
        CullFinishedAudioSources();
    }

    private AudioSource AddAndConfigureAudioSource(GameObject carrier, AudioClip audioClip, AudioCurves audioCurves,
        int priority, float volume, float pitch, bool ignorePause)
    {
        AudioSource newSource = carrier.AddComponent<AudioSource>();
        newSource.clip = audioClip;
        newSource.volume = volume;
        newSource.pitch = pitch;
        newSource.ignoreListenerPause = ignorePause;
        newSource.loop = false;
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

    private void CullFinishedAudioSources()
    {
        List<AudioSource> finishedAudioSources = new List<AudioSource>();
        foreach (AudioSource managedAudioSource in managedAudioSources)
        {
            if (!managedAudioSource.isPlaying)
            {
                finishedAudioSources.Add(managedAudioSource);
            }
        }
        foreach (AudioSource finishedAudioSource in finishedAudioSources)
        {
            managedAudioSources.Remove(finishedAudioSource);
        }
    }
}
