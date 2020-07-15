using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class AudioCurvesProvider : IAudioCurvesProvider
{
    private readonly string resourcesPathToAudioCurvesSpecs = "AudioCurvesSpecs";

    private Dictionary<string, AudioCurves> audioCurvesCache = new Dictionary<string, AudioCurves>();

    public void Prefetch()
    {
        LoadAndParseAudioCurvesSpecs();
    }

    public AudioCurves GetAudioCurves(string audioCurvesName)
    {
        return audioCurvesCache[audioCurvesName];
    }

    private void LoadAndParseAudioCurvesSpecs()
    {
        AudioCurvesSpec[] loadedSpecs = Resources.LoadAll<AudioCurvesSpec>(resourcesPathToAudioCurvesSpecs);
        foreach (AudioCurvesSpec loadedSpec in loadedSpecs)
        {
            AudioCurves createdCurves = CreateCurvesFromSpecs(loadedSpec);
            audioCurvesCache.Add(loadedSpec.name, createdCurves);
        }
    }

    private AudioCurves CreateCurvesFromSpecs(AudioCurvesSpec specs)
    {
        AudioCurves createdCurves = new AudioCurves();
        float maxDistance = Mathf.Max(
            specs.customRolloffEndDistance, specs.reverbZoneMixEndDistance,
            specs.spatialBlendEndDistance, specs.spreadEndDistance);
        createdCurves.maxDistance = maxDistance;
        if (specs.customRolloffStartLevel == 0 && specs.customRolloffStartDistance == 0
            && specs.customRolloffEndLevel == 0 && specs.customRolloffEndDistance == 0)
        {
            createdCurves.customRolloffCurve = AnimationCurve.Constant(0f, maxDistance, 1f);
        }
        else
        {
            createdCurves.customRolloffCurve = AnimationCurve.EaseInOut(
                specs.customRolloffStartDistance, specs.customRolloffStartDistance,
                specs.customRolloffEndDistance, specs.customRolloffEndLevel);
            createdCurves.customRolloffCurve.AddKey(maxDistance, specs.customRolloffEndLevel);
        }
        if (specs.reverbZoneMixStartLevel == 0 && specs.reverbZoneMixStartDistance == 0
            && specs.reverbZoneMixEndLevel == 0 && specs.reverbZoneMixEndDistance == 0)
        {
            createdCurves.reverbZoneMixCurve = AnimationCurve.Constant(0f, maxDistance, 1f);
        }
        else
        {
            createdCurves.reverbZoneMixCurve = AnimationCurve.EaseInOut(
                specs.reverbZoneMixStartDistance, specs.reverbZoneMixStartDistance,
                specs.reverbZoneMixEndDistance, specs.reverbZoneMixEndLevel);
            createdCurves.reverbZoneMixCurve.AddKey(maxDistance, specs.reverbZoneMixEndLevel);
        }
        if (specs.spatialBlendStartLevel == 0 && specs.spatialBlendStartDistance == 0
            && specs.spatialBlendEndLevel == 0 && specs.spatialBlendEndDistance == 0)
        {
            createdCurves.spatialBlendCurve = AnimationCurve.Constant(0f, maxDistance, 1f);
        }
        else
        {
            createdCurves.spatialBlendCurve = AnimationCurve.EaseInOut(
                specs.spatialBlendStartDistance, specs.spatialBlendStartDistance,
                specs.spatialBlendEndDistance, specs.spatialBlendEndLevel);
            createdCurves.spatialBlendCurve.AddKey(maxDistance, specs.spatialBlendEndLevel);
        }
        if (specs.spreadStartLevel == 0 && specs.spreadStartDistance == 0
            && specs.spreadEndLevel == 0 && specs.spreadEndDistance == 0)
        {
            createdCurves.spreadCurve = AnimationCurve.Constant(0f, maxDistance, 1f);
        }
        else
        {
            createdCurves.spreadCurve = AnimationCurve.EaseInOut(
                specs.spreadStartDistance, specs.spreadStartDistance,
                specs.spreadEndDistance, specs.spreadEndLevel);
            createdCurves.spreadCurve.AddKey(maxDistance, specs.spreadEndLevel);
        }
        return createdCurves;
    }
}
