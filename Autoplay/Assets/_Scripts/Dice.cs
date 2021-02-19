using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

[CreateAssetMenu(fileName = "New Die", menuName = "ScriptableObjects/Die", order = 3)]
public class Dice : ScriptableObject
{
    [Header("Basic Info")]
    // public string dieName = default;

    [Header("Related Images")]
    public Sprite dieImg = default;

    [Header("Sound Effects")]
    public AudioClip extraSuccess = default;
    public AudioClip successSFX = default;
    public AudioClip failSFX = default;
    public AudioClip extraFail = default;
}
