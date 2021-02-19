using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;


[CreateAssetMenu(fileName ="New Character", menuName = "ScriptableObjects/Character", order = 1)]
public class Characters : ScriptableObject
{
    [Header("Basic Info")]
    public string characterName = default;
    public string characterFullName = default;

    [Header("Related Images")]
    public Sprite[] poses = default;
    public Sprite dialogBackground = default;

    [Header("Prefabs to be Setup")]
    public GameObject characterPosObj = default;
    public GameObject dialogUIPosObj = default;
    public GameObject nameUIPosObj = default;  

    [Header("Related Audio")]
    public string audioFolder = default;
    public float mixerPitch = default;

}
