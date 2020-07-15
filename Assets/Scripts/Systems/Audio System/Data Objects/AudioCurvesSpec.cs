using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom ScriptableObjects/AudioCurvesSpec")]
public class AudioCurvesSpec : ScriptableObject
{
    [Space(10)]

    [SerializeField]
    public float customRolloffStartDistance;
    [SerializeField]
    public float customRolloffStartLevel;
    [SerializeField]
    public float customRolloffEndDistance;
    [SerializeField]
    public float customRolloffEndLevel;

    [Space(10)]

    [SerializeField]
    public float reverbZoneMixStartDistance;
    [SerializeField]
    public float reverbZoneMixStartLevel;
    [SerializeField]
    public float reverbZoneMixEndDistance;
    [SerializeField]
    public float reverbZoneMixEndLevel;

    [Space(10)]

    [SerializeField]
    public float spatialBlendStartDistance;
    [SerializeField]
    public float spatialBlendStartLevel;
    [SerializeField]
    public float spatialBlendEndDistance;
    [SerializeField]
    public float spatialBlendEndLevel;

    [Space(10)]

    [SerializeField]
    public float spreadStartDistance;
    [SerializeField]
    public float spreadStartLevel;
    [SerializeField]
    public float spreadEndDistance;
    [SerializeField]
    public float spreadEndLevel;
}
