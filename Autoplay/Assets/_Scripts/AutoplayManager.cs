using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class AutoplayManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField]
    private GameObject mainCanvas = default;
    [SerializeField]
    private GameObject backgroundUI = default;
    [SerializeField]
    private Sprite[] backgroundImgs = default;
    [SerializeField]
    private GameObject[] characterUILoc = default;
    //[SerializeField]
    //private Sprite[][] characterImgs = default;
    //[SerializeField]
    //private Sprite[] character1_Imgs = default;
    //[SerializeField]
    //private Sprite[] character2_Imgs = default;
    //[SerializeField]
    //private Sprite[] character3_Imgs = default;
    //[SerializeField]
    //private Sprite[] character4_Imgs = default;
    //[SerializeField]
    //private Sprite[] character5_Imgs = default;
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

    [Header("Files")]
    [SerializeField]
    private TextAsset[] txtFiles = default;

    [Header("Variables")]
    private string fullLog = default;
    private List<string> rawLines = default;
    private List<int> backgrounds = default;
    private List<string> characters = default;
    private List<int> characterPositions = default;
    private List<int> emotes = default;
    private List<string> sentences = default;
    private int index = 0;
    public float typingSpeed = 0.01f;
    private float readingSpeed = 1.5f;
    private bool lineComplete = false;

    public GameObject nextButton;

    private void Start()
    {
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

        nextButton.SetActive(false);
        index = 0;
        lineComplete = false;

        ImportScene(0);
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

    private void ImportScene(int scene_num)
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

        for(int i = 0; i< rawLines.Count; i += 5)
        {
            bool formatCheck = int.TryParse(rawLines[i], out int result);
            if(formatCheck)
            {
                backgrounds.Add(result);
            }
            else
            {
                Debug.Log("Log content format error!");
            }
        }

        for (int i = 1; i < rawLines.Count; i += 5)
        {
            characters.Add(rawLines[i]);
        }

        for (int i = 2; i < rawLines.Count; i += 5)
        {
            bool formatCheck = int.TryParse(rawLines[i], out int result);
            if (formatCheck)
            {
                characterPositions.Add(result);
            }
            else
            {
                Debug.Log("Log content format error!");
            }
        }
        for (int i = 3; i < rawLines.Count - 1; i += 5)
        {
            bool formatCheck = int.TryParse(rawLines[i], out int result);
            if (formatCheck)
            {
                emotes.Add(result);
            }
            else
            {
                Debug.Log("Log content format error!");
            }
        }
        for (int i = 4; i < rawLines.Count ; i += 5)
        {
            sentences.Add(rawLines[i]);
        }
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
            if (backgrounds[index] != backgrounds[index - 1])
            {
                backgroundUI.GetComponent<Image>().sprite = backgroundImgs[backgrounds[index]];

            }
            if (characters[index] != characters[index - 1])
            {
                nameDisplay.text = characters[index].ToString();
            }
            if (characterPositions[index] != characterPositions[index - 1])
            {
                characterUILoc[characterPositions[index]].SetActive(true);
            }
            if (emotes[index] != emotes[index - 1])
            {
                characterUILoc[characterPositions[index]].GetComponent<Image>().sprite = characterImgs[emotes[index]];
            }
        }
        else if (index == 0)
        {
            backgroundUI.GetComponent<Image>().sprite = backgroundImgs[backgrounds[index]];
            nameDisplay.text = characters[index].ToString();
            characterUILoc[characterPositions[index]].SetActive(true);
            characterUILoc[characterPositions[index]].GetComponent<Image>().sprite = characterImgs[emotes[index]];
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
