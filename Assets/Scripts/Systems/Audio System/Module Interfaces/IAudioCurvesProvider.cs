using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioCurvesProvider
{
    void Prefetch();
    AudioCurves GetAudioCurves(string audioCurvesName);
}
