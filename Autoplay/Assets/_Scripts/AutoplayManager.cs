using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class AutoplayManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField]
    private GameObject dialogUI = default;
    [SerializeField]
    private GameObject backgroundUI = default;
    [SerializeField]
    private GameObject[] charactersUI = default;

    [Header("Files")]
    [SerializeField]
    private TextAsset[] txtFiles = default;
}
