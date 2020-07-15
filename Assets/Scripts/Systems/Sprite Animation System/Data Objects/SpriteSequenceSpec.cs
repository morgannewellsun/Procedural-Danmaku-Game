using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom ScriptableObjects/SpriteSequenceSpec")]
public class SpriteSequenceSpec : ScriptableObject
{
    [Space(10)]
    [SerializeField]
    public string[] spriteNames;
    [Space(10)]
    [SerializeField]
    public float[] defaultDurations;
}