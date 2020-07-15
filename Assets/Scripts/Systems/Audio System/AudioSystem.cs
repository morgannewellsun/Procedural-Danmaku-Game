using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour, IAudioSystem
{
    private IAudioClipProvider audioClipProvider;
    private IAudioCurvesProvider audioCurvesProvider;
    private IAudioSourceCarrierManager audioSourceCarrierManager;
    private IOneshotAudioSourceManager oneshotAudioSourceManager;
    private ILoopAudioSourceManager loopAudioSourceManager;

    void Awake()
    {
        audioClipProvider = new AudioClipProvider();
        audioCurvesProvider = new AudioCurvesProvider();
        audioSourceCarrierManager = new AudioSourceCarrierManager();
        oneshotAudioSourceManager = new OneshotAudioSourceManager();
        loopAudioSourceManager = new LoopAudioSourceManager();
        audioClipProvider.Prefetch();
        audioCurvesProvider.Prefetch();
    }

    void Start()
    {
        AttachListenerTo(GameDirector.cameraSystem.GetMainCamera().gameObject);
    }

    void Update()
    {
        audioSourceCarrierManager.UpdateManaged();
        oneshotAudioSourceManager.UpdateManaged();
    }

    public void AttachListenerTo(GameObject listenerObject)
    {
        listenerObject.AddComponent<AudioListener>();
    }

    public void PlayLoop(GameObject sourceObject, string audioClipName, string audioCurvesName, int priority, 
        float volume = 1, float pitch = 1, bool ignorePause = false)
    {
        AudioClip audioClip = audioClipProvider.GetAudioClip(audioClipName);
        AudioCurves audioCurves = audioCurvesProvider.GetAudioCurves(audioCurvesName);
        GameObject carrier = audioSourceCarrierManager.GetOrCreateAudioSourceCarrier(sourceObject);
        loopAudioSourceManager.AddAudioSource(carrier, audioClip, audioCurves, priority, volume, pitch, ignorePause);
    }

    public void PlayOnce(GameObject sourceObject, string audioClipName, string audioCurvesName, int priority, 
        float volume = 1, float pitch = 1, bool ignorePause = false)
    {
        AudioClip audioClip = audioClipProvider.GetAudioClip(audioClipName);
        AudioCurves audioCurves = audioCurvesProvider.GetAudioCurves(audioCurvesName);
        GameObject carrier = audioSourceCarrierManager.GetOrCreateAudioSourceCarrier(sourceObject);
        oneshotAudioSourceManager.AddAudioSource(carrier, audioClip, audioCurves, priority, volume, pitch, ignorePause);
    }

    public void StopLoop(GameObject sourceObject, string audioClipName)
    {
        AudioClip audioClip = audioClipProvider.GetAudioClip(audioClipName);
        GameObject carrier = audioSourceCarrierManager.GetOrCreateAudioSourceCarrier(sourceObject);
        loopAudioSourceManager.RemoveAudioSource(carrier, audioClip);
    }
}
