using System;
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
    [Header("Scene Input Files")]
    [SerializeField]
    private TextAsset[] txtFiles = default;

    [Header("Characters")]
    [SerializeField]
    private string[] characterNames = default;

    [Header("UI Elements")]
    [SerializeField]
    private GameObject mainCanvas = default;
    [SerializeField]
    private GameObject foregroundUI = default;
    [SerializeField]
    private GameObject backgroundUI = default;
    [SerializeField]
    private Sprite[] backgroundImgs = default;
    [SerializeField]
    private GameObject[] characterUILoc = default;
    [SerializeField]
    private Sprite[] characterImgs = default;
    [SerializeField]
    private GameObject dialogUI = default;
    [SerializeField]
    private TextMeshProUGUI dialogDisplay = default;
    [SerializeField]
    private GameObject characterNameBox = default;
    [SerializeField]
    private TextMeshProUGUI nameDisplay = default;

    [Header("Audio Elements")]
    [SerializeField]
    private AudioSource audio = default;
    [SerializeField]
    private AudioMixer mainMixer = default;

    private int char0Counter = 0;
    private int char1Counter = 0;
    private AudioClip[] char0Clips;
    private AudioClip[] char1Clips;

    [Header("Variables")]
    public Material glitch2D = default;
    private string fullLog = default;
    private List<string> rawLines = default;
    private List<int> backgrounds = default;
    private List<string> characters = default;
    private List<int> characterPositions = default;
    private List<int> emotes = default;
    private List<string> sentences = default;
    private int index = 0;
    [Range(0f, 0.1f)]
    public float typingSpeed = 0.01f;
    private float readingSpeed = 1.5f;
    private bool lineComplete = false;

    private void Start()
    {
        foregroundUI.SetActive(true);
        char0Clips = Resources.LoadAll<AudioClip>("Audio/" + characterNames[0]);
        char1Clips = Resources.LoadAll<AudioClip>("Audio/" + characterNames[1]);

        rawLines = new List<string>();
        backgrounds = new List<int>();
        characters = new List<string>();
        characterPositions = new List<int>();
        emotes = new List<int>();
        sentences = new List<string>();

        dialogUI.SetActive(false);
        if(characterUILoc.Length > 0)
        {
            foreach(GameObject p in characterUILoc)
            {
                p.SetActive(false);
            }
        }

        // characterUILoc[0].SetActive(true);

        index = 0;
        lineComplete = false;

        // Greenal Mixer Pitch 1.02f
        // Shitou Mixer Pitch 0.92f

        StartCoroutine(LoadingScene());
    }

    private void Update()
    {
        if(dialogUI.activeInHierarchy)
        {
            if(dialogDisplay.text == sentences[index])
            {
                lineComplete = true;   
            }
            if(lineComplete)
            {
                StartCoroutine(NextLine());
            }
        }
    }

    private IEnumerator BlackIn(GameObject go, float duration)
    {
        float t = 0;
        float startAlpha = go.GetComponent<CanvasRenderer>().GetAlpha();

        while(t < duration)
        {
            go.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(startAlpha, 0f, t / duration));

            t += Time.deltaTime;
            yield return null;
        }
        // go.GetComponent<CanvasRenderer>().SetAlpha(0f);
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
        // go.GetComponent<Image>().material.SetFloat("Step Edge", endEdge);
    }

    private IEnumerator LoadingScene()
    {
        StartCoroutine(BlackIn(foregroundUI, 3f));
        yield return new WaitForSeconds(1f);
        ImportDialog(0);
    }

    private void ImportDialog(int scene_num)
    {
        if(txtFiles.Length > 0)
        {
            PromptDialog(txtFiles[scene_num]);
        }
    }

    private void ReadLog(TextAsset txt)
    {
        fullLog = txt.text;
        rawLines.AddRange(fullLog.Split("\n"[0]));

        string splitPattern0 = @"^\[";
        string splitPattern1 = @"\]\:";

        for (int i = 0; i < rawLines.Count; i++)
        {
            string[] lineParts0 = Regex.Split(rawLines[i], splitPattern0);
            string[] lineParts1 = Regex.Split(lineParts0[1], splitPattern1);

            characters.Add(lineParts1[0]);
            // Debug.Log(characters[i]);
            sentences.Add(lineParts1[1]);
        }
       
        //for(int i = 0; i< rawLines.Count; i += 5)
        //{
        //    bool formatCheck = int.TryParse(rawLines[i], out int result);
        //    if(formatCheck)
        //    {
        //        backgrounds.Add(result);
        //    }
        //    else
        //    {
        //        Debug.Log("Log content format error!");
        //    }
        //}
    }

    private void PromptDialog(TextAsset txt)
    {
        ReadLog(txt);
        dialogUI.SetActive(true);
        dialogDisplay.text = "";
        StartCoroutine(TypingSentence());
    }

    private IEnumerator TypingSentence()
    {
        if (index != 0)
        {
            //if (backgrounds[index] != backgrounds[index - 1])
            //{
            //    backgroundUI.GetComponent<Image>().sprite = backgroundImgs[backgrounds[index]];

            //}
            if (characters[index] != characters[index - 1])
            {
                nameDisplay.text = characters[index].ToString();
            }

            //if (characterPositions[index] != characterPositions[index - 1])
            //{
            //    characterUILoc[characterPositions[index]].SetActive(true);
            //}
            //if (emotes[index] != emotes[index - 1])
            //{
            //    characterUILoc[characterPositions[index]].GetComponent<Image>().sprite = characterImgs[emotes[index]];
            //}
        }
        else if (index == 0)
        {
            //backgroundUI.GetComponent<Image>().sprite = backgroundImgs[backgrounds[index]];
            nameDisplay.text = characters[index].ToString();

            // glitch2D.SetFloat("Vector1_AD7F5818", 1f);
            // characterUILoc[0].GetComponent<CanvasRenderer>().SetAlpha(0.3f);

            //characterUILoc[characterPositions[index]].SetActive(true);
            //characterUILoc[characterPositions[index]].GetComponent<Image>().sprite = characterImgs[emotes[index]];
        }

        // audio
        if (nameDisplay.text == "Greenal")
        {
            audio.clip = char0Clips[char0Counter];
            readingSpeed = char0Clips[char0Counter].length;
            mainMixer.SetFloat("Pitch", 1.02f);
            audio.Play();
            char0Counter++;
        }
        else if (nameDisplay.text == "石头")
        {
            audio.clip = char1Clips[char1Counter];
            readingSpeed = char1Clips[char1Counter].length;
            mainMixer.SetFloat("Pitch", 0.92f);
            audio.Play();
            char1Counter++;
        }

        foreach (char letter in sentences[index].ToCharArray())
        {
            dialogDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private IEnumerator NextLine()
    {
        lineComplete = false;

        if (index < sentences.Count - 1)
        {
            // Debug.Log(index);
            index++;
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
            index = 0;
            rawLines = new List<string>();
            backgrounds = new List<int>();
            characters = new List<string>();
            characterPositions = new List<int>();
            emotes = new List<int>();
            sentences = new List<string>();
        }
    }
}
