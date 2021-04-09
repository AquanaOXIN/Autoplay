using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stage", menuName = "ScriptableObjects/Stage", order = 4)]
public class Stage : ScriptableObject
{
    [Header("Environment Elements")]
    public Sprite backgroundSprite = default;

    [Header("Initial Characters")]
    public Characters[] characters;

    [Header("Audio Elements")]
    public AudioClip backgroundMusic = default;
    public AudioClip ambienceMusic = default;
}
