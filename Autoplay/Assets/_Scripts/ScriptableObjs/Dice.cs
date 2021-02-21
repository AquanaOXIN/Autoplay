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
    public Sprite dieSprite = default;

    [Header("Sound Effects")]
    public AudioClip[] rollDiceSFX = default;
}
