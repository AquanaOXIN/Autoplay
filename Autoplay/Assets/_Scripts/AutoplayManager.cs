﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Text.RegularExpressions;
using System.IO;

public class AutoplayManager : MonoBehaviour
{
    [Header("Scene Log Setup")]
    [SerializeField]
    private TextAsset[] txtFiles = default;
    [SerializeField]
    private int currentScene = default;

    [Header("Characters")]
    [SerializeField]
    private Characters[] characters = default;

    [Header("Dice")]
    [SerializeField]
    private Dice[] dice = default;

    [Header("Items")]
    [SerializeField]
    private PopItem[] items = default;

    [Header("Audio Elements")]
    [SerializeField]
    private AudioSource voiceAudio = default;
    [SerializeField]
    private AudioMixer voiceMixer = default;
    [SerializeField]
    private AudioSource SFXAudio = default;
    [SerializeField]
    private AudioSource BGMAudio = default;

    [Header("UI Elements")]
    [SerializeField]
    private GameObject foregroundUI = default;
    [SerializeField]
    private Image dialogBackground = default;
    [SerializeField]
    private GameObject dialogUI = default;
    [SerializeField]
    private TextMeshProUGUI dialogDisplay = default;
    [SerializeField]
    private TextMeshProUGUI nameDisplay = default;
    [SerializeField]
    private Image KPAvatar = default;
    [SerializeField]
    private GameObject ItemUI = default;
    [SerializeField]
    private Image ItemDisplay = default;

    [Header("Level Elements")]
    [SerializeField]
    private SpriteRenderer characterSR;

    [Header("Variables")]
    [SerializeField]
    private float loadingTime = default;
    [SerializeField, Range(0f, 0.1f)]
    private float typingSpeed = 0.01f;
    [SerializeField, Range(1f, 5f)]
    private float readingSpeed = 1.5f;

    // text line related variables
    private string fullLog = default;
    private List<string> rawLines = default;
    private List<string> orderedLineTags = default;
    private List<string> orderedLines = default;
    [SerializeField]
    private int progIndex = 0;
    [SerializeField]
    private string currTag = default;
    [SerializeField]
    private Characters currCharacter = default;
    private bool lineComplete = false;
    private Hashtable lineCounter = default;

    public bool debugMode = false;
    public Sprite KPstandby = default;

    private void Start()
    {
        foregroundUI.SetActive(true);
        ItemUI.SetActive(false);

        loadingTime = 2f;
        typingSpeed = 0.01f;
        readingSpeed = 3f;
        rawLines = new List<string>();
        orderedLineTags = new List<string>();
        orderedLines = new List<string>();

        // Initialize a counter for counting line prog of each character
        lineCounter = new Hashtable();
        foreach(Characters c in characters)
        {
            c.LoadVoiceAudioFiles(currentScene);
            lineCounter.Add(c.name, (int)0);
        }
        progIndex = 0;
        lineComplete = false;

        dialogUI.SetActive(false);

        StartCoroutine(LoadingScene());
    }

    private void Update()
    {
        if (dialogUI.activeInHierarchy)
        {
            if (dialogDisplay.text == orderedLines[progIndex])
            {
                lineComplete = true;
            }
            if (lineComplete)
            {
                StartCoroutine(NextLine());
            }
        }
    }

    private IEnumerator LoadingScene()
    {

        StartCoroutine(BlackIn(foregroundUI, loadingTime));
        yield return new WaitForSeconds(1f);
        ImportDialog(currentScene);
    }

    private void ImportDialog(int _currentSceneNum)
    {
        if (txtFiles.Length > 0)
        {
            InitiateDialog(txtFiles[_currentSceneNum]);
        }
    }

    private void ProcessLog(TextAsset txt)
    {
        fullLog = txt.text;
        rawLines.AddRange(fullLog.Split("\n"[0]));

        // Process raw lines
        string splitPattern0 = @"^\[";
        string splitPattern1 = @"\]\:";

        for (int i = 0; i < rawLines.Count; i++)
        {
            if (rawLines[i].Length > 0)
            {
                string[] lineParts0 = Regex.Split(rawLines[i], splitPattern0);
                string[] lineParts1 = Regex.Split(lineParts0[1], splitPattern1);
                orderedLineTags.Add(lineParts1[0]);
                orderedLines.Add(lineParts1[1]);
            }
        }
    }

    private void InitiateDialog(TextAsset txt)
    {
        ProcessLog(txt);
        dialogUI.SetActive(true);
        dialogDisplay.text = "";
        StartCoroutine(TypingSentence());
    }

    private IEnumerator TypingSentence()
    {
        nameDisplay.text = "";
        // Current "[???]:" => currTag 
        currTag = orderedLineTags[progIndex];

        if (currTag == "ROLL")
        {
            KPAvatar.sprite = dice[0].dieImg;
            SFXAudio.clip = dice[0].successSFX;
            SFXAudio.PlayDelayed(0.5f);
            readingSpeed = 3.0f;
        }
        else if (currTag == "BG")
        {

        }
        else if (currTag == "Item")
        {
            ItemUI.SetActive(true);
            ItemDisplay.sprite = items[0].itemImg;
            ItemDisplay.SetNativeSize();
            DelayedSetActive(ItemUI, false, 3.0f);
        }
        else
        {
            for (int i = 0; i < characters.Length; i++)
            {
                if (currTag == characters[i].name)
                {
                    // Visuals
                    if (progIndex == 0)
                    {
                        currCharacter = characters[i];
                        if(currTag == "KP")
                        {
                            KPAvatar.sprite = characters[i].poses[0];
                        }
                        else
                        {
                            KPAvatar.sprite = KPstandby;
                            nameDisplay.text = characters[i].name.ToString();
                            characterSR.sprite = characters[i].poses[0];
                        }
                        dialogBackground.sprite = characters[i].dialogBackground;
                    }
                    else if (progIndex != 0)
                    {
                        if (currTag != currCharacter.name) // Later should be added another condition: nowPose != currPose etc
                        {                        
                            currCharacter = characters[i];
                            if (currTag == "KP")
                            {
                                KPAvatar.sprite = characters[i].poses[0];
                            }
                            else
                            {
                                KPAvatar.sprite = KPstandby;
                                nameDisplay.text = characters[i].name.ToString();
                                characterSR.sprite = characters[i].poses[0];
                            }
                            dialogBackground.sprite = characters[i].dialogBackground;
                        }
                    }
                    // Voice
                    int currCounter = (int)lineCounter[characters[i].name];
                    voiceAudio.clip = characters[i].GetClip(currCounter);
                    if (voiceAudio.clip.length > 0)
                    {
                        readingSpeed = voiceAudio.clip.length;
                    }
                    else
                    {
                        readingSpeed = 2.0f;
                    }
                    voiceMixer.SetFloat("Pitch", characters[i].mixerPitch);
                    voiceAudio.PlayDelayed(0.5f);
                    lineCounter[characters[i].name] = currCounter + 1;
                }
            }
        }
        foreach (char letter in orderedLines[progIndex].ToCharArray())
        {
            dialogDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private IEnumerator NextLine()
    {
        lineComplete = false;

        if (progIndex < orderedLines.Count - 1)
        {
            progIndex++;
            yield return new WaitForSeconds(readingSpeed);
            dialogDisplay.text = "";
            StartCoroutine(TypingSentence());
        }
        else
        {
            yield return new WaitForSeconds(readingSpeed);
            dialogDisplay.text = "";
            lineComplete = false;
            dialogUI.SetActive(false);
            progIndex = 0;
            rawLines = new List<string>();
            orderedLineTags = new List<string>();
            orderedLines = new List<string>();
        }
    }

    private IEnumerator DelayedSetActive(GameObject _go, bool _active, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        _go.SetActive(_active);
    }

    /// Visual Effects

    private IEnumerator BlackIn(GameObject go, float duration)
    {
        float t = 0;
        float startAlpha = go.GetComponent<CanvasRenderer>().GetAlpha();

        while (t < duration)
        {
            go.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(startAlpha, 0f, t / duration));

            t += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator NoiseTVEffect(GameObject go, float duration)
    {
        float t = 0;
        float startEdge = 0.25f;
        float endEdge = 0f;
        go.GetComponent<Image>().material.SetFloat("Step Edge", startEdge);

        while (t < duration)
        {
            go.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(startEdge, endEdge, t / duration));

            t += Time.deltaTime;
            yield return null;
        }
    }
}
