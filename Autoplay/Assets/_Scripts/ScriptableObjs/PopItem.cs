using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

[CreateAssetMenu(fileName = "New PopItem", menuName = "ScriptableObjects/PopItem", order = 4)]
public class PopItem : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName = default;

    [Header("Related Images")]
    public Sprite itemImg = default;
}
