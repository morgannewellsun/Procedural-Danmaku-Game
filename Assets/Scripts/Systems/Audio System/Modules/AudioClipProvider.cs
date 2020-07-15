using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipProvider : IAudioClipProvider
{
    private readonly string resourcesPathToAudioClips = "AudioClips";

    private Dictionary<string, AudioClip> audioClipCache = new Dictionary<string, AudioClip>();

    public void Prefetch()
    {
        LoadAndCacheClips();
    }

    public AudioClip GetAudioClip(string audioClipName)
    {
        return audioClipCache[audioClipName];
    }

    private void LoadAndCacheClips()
    {
        AudioClip[] clipList = Resources.LoadAll<AudioClip>(resourcesPathToAudioClips);
        for (int i = 0; i < clipList.Length; i++)
        {
            this.audioClipCache.Add(clipList[i].name, clipList[i]);
        }
    }
}
