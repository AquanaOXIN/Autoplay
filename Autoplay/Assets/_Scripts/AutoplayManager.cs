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
    [Header("Scene Log Setup")]
    [SerializeField]
    private TextAsset[] logFiles = default;
    [SerializeField]
    private int currentSceneNum = default;
    [SerializeField]
    private string KPTAG = default;
    [SerializeField]
    private string ROLLTAG = default; // tag in log for rolling dice
    [SerializeField]
    private string BGTAG = default; // tag in log for background
    [SerializeField]
    private string ESTAG = default; // tag in log for scene sprite
    [SerializeField]
    private string UISTAG = default; // tag in log for UI sprite

    [Header("Characters")]
    [SerializeField]
    private bool KP_as_PC = false; // KP on UI as default, check this if you want KP also as a scene Sprite
    [SerializeField]
    private Characters[] characters = default;

    [Header("Dice")]
    [SerializeField]
    private Dice[] dice = default;

    [Header("Basic UI Elements")]
    [SerializeField]
    private GameObject dialogueUI = default;
    [SerializeField]
    private GameObject[] nameUIs = default;
    [SerializeField]
    private GameObject[] characterUIs = default;
    [SerializeField]
    private GameObject diceUI = default;
    [SerializeField]
    private GameObject itemUI = default;
    [SerializeField]
    private GameObject foregroundUI = default;

    private Image dialogImg = default;
    private TextMeshProUGUI dialogDisplay = default;
    private List<Image> nameImgs = default;
    private List<TextMeshProUGUI> nameDisplays = default;
    private Image diceImg = default;
    private List<Image> characterImgs = default;
    private Image itemImg = default;
    private Image foregroundImg = default;

    [Header("Basic Environment Elements")]
    //[SerializeField]
    //private GameObject addLights = default;
    [SerializeField]
    private GameObject envBackground = default;
    [SerializeField]
    private GameObject[] envCharacters = default;
    [SerializeField]
    private GameObject[] envItems = default;

    private SpriteRenderer envBGSR = default;
    private List<SpriteRenderer> envCharacterSRs = default;
    private List<SpriteRenderer> envItemSRs = default;

    [Header("Other Sprite Images")]
    [SerializeField]
    private Sprite[] BackgroundSprites = default;
    [SerializeField]
    private Sprite[] envSprites = default;
    [SerializeField]
    private Sprite[] UISprites = default;

    [Header("Audio Elements")]
    [SerializeField]
    private AudioSource voiceAudio = default;
    [SerializeField]
    private AudioMixer voiceMixer = default;
    [SerializeField]
    private AudioSource SFXAudio = default;
    [SerializeField]
    private AudioSource BGMAudio = default;

    [Header("Variables")]
    private bool sceneSettled = false;
    [SerializeField]
    private float loadingTime = default;
    [SerializeField, Range(0f, 0.1f)]
    private float typingSpeed = 0.01f;
    [SerializeField, Range(1f, 5f)]
    private float readingSpeed = 1.5f;

    // Text Line related variables
    private string fullLog = default;
    private List<string> rawLines = default;
    [SerializeField]
    private int progIndex = 0;
    [SerializeField]
    private string currTag = default;
    [SerializeField]
    private string currDialog = default;
    private List<string> lineTagSeq = default;
    private List<string> lineDialogSeq = default;
    private ArrayList procdLines = default;
    private bool lineComplete = false;
    private Hashtable speechSeqCounter = default;

    // KP Specials
    private Characters theKP = default;

    // Formatting Params
    private int bgInParamNum = 3;
    private int uisInParamNum = 4;
    private int esInParamNum = 4;
    private int rollInParamNum = 4;
    private int dialogInParamNum = 8;

    private void Start()
    {
        // Curtain blocking stuff...
        foregroundUI.SetActive(true);

        // Initialize UI Components
        dialogImg = dialogueUI.GetComponent<Image>();      
        dialogDisplay = dialogueUI.GetComponentInChildren<TextMeshProUGUI>();
        dialogDisplay.text = "";
        dialogueUI.SetActive(false);

        nameImgs = new List<Image>();
        nameDisplays = new List<TextMeshProUGUI>();
        foreach(GameObject go in nameUIs)
        {
            Image img = go.GetComponent<Image>();
            nameImgs.Add(img);
            TextMeshProUGUI textPro = go.GetComponentInChildren<TextMeshProUGUI>();
            textPro.text = "";
            nameDisplays.Add(textPro);
            go.SetActive(false);
        }

        characterImgs = new List<Image>();
        foreach (GameObject go in characterUIs)
        {
            Image img = go.GetComponent<Image>();
            characterImgs.Add(img);
            go.SetActive(false);
        }

        diceImg = diceUI.GetComponent<Image>();
        diceUI.SetActive(false);

        itemImg = itemUI.GetComponent<Image>();
        itemUI.SetActive(false);

        foregroundImg = foregroundUI.GetComponent<Image>();

        // Initialize Environment Components
        envBGSR = envBackground.GetComponentInChildren<SpriteRenderer>();
        envBackground.SetActive(false);

        envCharacterSRs = new List<SpriteRenderer>();
        foreach (GameObject go in envCharacters)
        {
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            envCharacterSRs.Add(sr);
            go.SetActive(false);
        }

        envItemSRs = new List<SpriteRenderer>();
        foreach (GameObject go in envItems)
        {
            SpriteRenderer sr = go.GetComponentInChildren<SpriteRenderer>();
            envItemSRs.Add(sr);
            go.SetActive(false);
        }

        // Initialize log-line related things
        fullLog = "";
        rawLines = new List<string>();
        //progIndex = 0;
        //currTag = "";
        //currDialog = "";
        lineTagSeq = new List<string>();
        lineDialogSeq = new List<string>();
        procdLines = new ArrayList();
        lineComplete = false;
        // Initialize a counter for counting line prog of each character
        speechSeqCounter = new Hashtable();
        foreach (Characters c in characters)
        {
            c.LoadVoiceAudioFiles(currentSceneNum);
            speechSeqCounter.Add(c.tagName, (int)0);
        }

        sceneSettled = false;
        StartCoroutine(LoadingScene());
    }

    private void Update()
    {
        //if(dialogueUI.activeInHierarchy)
        //{
        //    if (dialogDisplay.text == lineDialogSeq[progIndex])
        //    {
        //        lineComplete = true;
        //    }
        //    if (lineComplete)
        //    {
        //        // StartCoroutine(NextLine());
        //        NextLine();
        //    }
        //}
        if (lineComplete)
        {
            // StartCoroutine(NextLine());
            NextLine();
        }
    }

    private IEnumerator LoadingScene()
    {

        StartCoroutine(BlackIn(foregroundUI, loadingTime));
        yield return new WaitForSeconds(1f);
        ImportDialog(currentSceneNum);
    }

    private void ImportDialog(int _currentSceneNum)
    {
        if (logFiles.Length > 0)
        {
            InitiateDialog(logFiles[_currentSceneNum]);
        }
        else
        {
            Debug.Log("Log File Format Error.");
        }
    }

    private void InitiateDialog(TextAsset txt)
    {
        ProcessLog(txt);

        dialogueUI.SetActive(true);
        dialogDisplay.text = "";
        foreach(TextMeshProUGUI textPro in nameDisplays)
        {
            textPro.text = "";
        }

        // ExecutingLine();
        StartCoroutine(ExecutingLine());
    }

    private void ProcessLog(TextAsset txt)
    {
        fullLog = txt.text;
        rawLines.AddRange(fullLog.Split("\n"[0]));

        // Process raw lines
        string rawLineFormat = @"^\[\S+\]\:\[\S+\].*";
        string bgLineFormat = @".*\|.*\|.*";
        string ssLineFormat = @".*\|.*\|.*\|.*";
        string uisLineFormat = @".*\|.*\|.*\|.*";
        string rolLineFormat = @".*\|.*\|.*\|.*";
        string dialogLineFormat = @".*\|.*\|.*\|.*\|.*\|.*\|.*\|.*";

        string splitPattern0 = @"^\[";
        string splitPattern1 = @"\]\:";

        string splitPattern2 = @"\[";
        string splitPattern3 = @"\]\s*";
        string splitPattern4 = @"\|";

        for (int i = 0; i < rawLines.Count; i++)
        {
            Match m = Regex.Match(rawLines[i], rawLineFormat);

            if (m.Length > 0) // if overall format correct, then start processing
            {
                string tmpTag;
                string tmpDialog;

                // ↓ splitPattern0 => delete the 1st "["  
                string[] segdLine0 = Regex.Split(rawLines[i], splitPattern0);
                // ↑ null and the rest without the 1st "["
                // ↑ [0]          [1]

                // ↓ splitPattern1 => split by "]:" and [0] is TAG 
                string[] segdLine1 = Regex.Split(segdLine0[1], splitPattern1);
                tmpTag = segdLine1[0];
                // ↑ TAG and the rest (variables + dialog(if any), etc)
                // ↑ [0]          [1]

                // ↓ splitPattern2 => delete the initial "[" again
                string[] segdLine2 = Regex.Split(segdLine1[1], splitPattern2);
                // ↑ null and the rest without the initial "["
                // ↑ [0]          [1]

                // ↓ splitPattern3 => split by "]" and [1] is dialog(if any)
                string[] segdLine3 = Regex.Split(segdLine2[1], splitPattern3);
                tmpDialog = segdLine3[1];
                // ↑ variables and dialog seg
                // ↑ [0]          [1]

                // ↓ splitPattern4 => split by "|" and get variables of the line
                // But Format Check accordingly first
                
                if(tmpTag == BGTAG)
                {
                    Match _m = Regex.Match(segdLine3[0], bgLineFormat);
                    if(_m.Length > 0)
                    {
                        string[] in_params = Regex.Split(segdLine3[0], splitPattern4);

                        int?[] intParams = new int?[bgInParamNum];
                        int counter = 0;
                        while(counter < intParams.Length)
                        {
                            intParams[counter] = ToNullableInt(in_params[counter]);
                            counter++;
                        }
                        BGLine _line = new BGLine(BGTAG, intParams[0], intParams[1], intParams[2]);
                        lineTagSeq.Add(tmpTag);
                        lineDialogSeq.Add("");
                        procdLines.Add(_line);
                    }
                    else
                    {
                        Debug.Log("Format Error @ line " + i.ToString());
                        break;
                    }
                }
                else if (tmpTag == ESTAG)
                {
                    Match _m = Regex.Match(segdLine3[0], ssLineFormat);
                    if (_m.Length > 0)
                    {
                        string[] in_params = Regex.Split(segdLine3[0], splitPattern4);
                        //
                        int?[] intParams = new int?[esInParamNum];
                        int counter = 0;
                        while (counter < intParams.Length)
                        {
                            intParams[counter] = ToNullableInt(in_params[counter]);
                            counter++;
                        }
                        ESLine _line = new ESLine(ESTAG, intParams[0], intParams[1], intParams[2], intParams[3]);
                        lineTagSeq.Add(tmpTag);
                        lineDialogSeq.Add("");
                        procdLines.Add(_line);
                    }
                    else
                    {
                        Debug.Log("Format Error @ line " + i.ToString());
                        break;
                    }
                }
                else if (tmpTag == UISTAG)
                {
                    Match _m = Regex.Match(segdLine3[0], uisLineFormat);
                    if (_m.Length > 0)
                    {
                        string[] in_params = Regex.Split(segdLine3[0], splitPattern4);
                        //
                        int?[] intParams = new int?[uisInParamNum];
                        int counter = 0;
                        while (counter < intParams.Length)
                        {
                            intParams[counter] = ToNullableInt(in_params[counter]);
                            counter++;
                        }
                        UISLine _line = new UISLine(UISTAG, intParams[0], intParams[1], intParams[2], intParams[3]);
                        lineTagSeq.Add(tmpTag);
                        lineDialogSeq.Add("");
                        procdLines.Add(_line);
                    }
                    else
                    {
                        Debug.Log("Format Error @ line " + i.ToString());
                        break;
                    }
                }
                else if (tmpTag == ROLLTAG)
                {
                    Match _m = Regex.Match(segdLine3[0], rolLineFormat);
                    if (_m.Length > 0)
                    {
                        string[] in_params = Regex.Split(segdLine3[0], splitPattern4);
                        //
                        int?[] intParams = new int?[rollInParamNum];
                        int counter = 0;
                        while (counter < intParams.Length)
                        {
                            intParams[counter] = ToNullableInt(in_params[counter]);
                            counter++;
                        }
                        ROLLine _line = new ROLLine(ROLLTAG, intParams[0], intParams[1], intParams[2], intParams[3], tmpDialog);
                        lineTagSeq.Add(tmpTag);
                        lineDialogSeq.Add(tmpDialog);
                        procdLines.Add(_line);
                    }
                    else
                    {
                        Debug.Log("Format Error @ line " + i.ToString());
                        break;
                    }
                }
                else
                {
                    foreach (Characters c in characters)
                    {
                        if(tmpTag == c.tagName)
                        {
                            int seqCounter = (int)speechSeqCounter[c.tagName];
                            
                            Match _m = Regex.Match(segdLine3[0], dialogLineFormat);
                            if (_m.Length > 0)
                            {
                                string[] in_params = Regex.Split(segdLine3[0], splitPattern4);

                                //
                                int?[] intParams = new int?[dialogInParamNum];
                                int counter = 0;
                                while (counter < intParams.Length)
                                {
                                    intParams[counter] = ToNullableInt(in_params[counter]);
                                    counter++;
                                }

                                DialogLine _line = new DialogLine(c.tagName, intParams[0], intParams[1], intParams[2], intParams[3], seqCounter, tmpDialog,
                                    intParams[4], intParams[5], intParams[6], intParams[7]);
                                lineTagSeq.Add(tmpTag);
                                lineDialogSeq.Add(tmpDialog);
                                procdLines.Add(_line);
                                seqCounter++;
                                speechSeqCounter[c.tagName] = seqCounter;
                            }
                            else
                            {
                                Debug.Log("Format Error @ line " + i.ToString());
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Format Error @ line " + i.ToString());
                break;
            }
        }
    }


    private IEnumerator ExecutingLine()
    {
        currTag = lineTagSeq[progIndex];
        currDialog = lineDialogSeq[progIndex];
        // sceneSettled?

        if (currTag == BGTAG)
        {
            readingSpeed = 0.5f;

            BGLine currLine = (BGLine)procdLines[progIndex];
            
            if (currLine.spriteSelect != null)
            {
                // can be changed in other ways
                envBGSR.sprite = BackgroundSprites[(int)currLine.spriteSelect];
            }
            else
            {
                // show default background
                envBGSR.sprite = BackgroundSprites[0];
            }

            if(currLine.status != null)
            {
                // Moving-in or Moving-out? Moving should change the envBackground Gameobject
            }

            if(currLine.vfxSelect != null)
            {
                // ... with which kind of VFX?
                if (currLine.vfxSelect == 0)
                {
                    StartCoroutine(BlackIn(foregroundUI, 0.3f));
                }
                else if (currLine.vfxSelect == 1)
                {
                    StartCoroutine(BlackOut(foregroundUI, 0.3f));
                }
            }
            
        }
        else if (currTag == ESTAG)
        {
            readingSpeed = 1f;

            Sprite sprite2Show = default;
            ESLine currLine = (ESLine)procdLines[progIndex];
            if(currLine.spriteSelect != null)
            {
                sprite2Show = envSprites[(int)currLine.spriteSelect];
            }

            if(currLine.posSelect != null)
            {
                envItemSRs[(int)currLine.posSelect].sprite = sprite2Show;
            }

            if(currLine.status != null)
            {

            }

            if(currLine.vfxSelect != null)
            {

            }
        }
        else if (currTag == UISTAG)
        {
            readingSpeed = 0.5f;
            Sprite sprite2Show = default;
            UISLine currLine = (UISLine)procdLines[progIndex];

            if (currLine.status == 1)
            {
                if (currLine.spriteSelect != null)
                {
                    itemUI.SetActive(true);
                    sprite2Show = UISprites[(int)currLine.spriteSelect];
                    itemImg.sprite = sprite2Show;
                    itemImg.SetNativeSize();
                }
                
            }
            else if (currLine.status == 0)
            {
                itemUI.SetActive(false);
            }
            else
            {
                sprite2Show = null;
                itemImg.sprite = sprite2Show;
                itemUI.SetActive(false);
            }
            //if (currLine.posSelect != null)
            //{
            //    itemImg.sprite = sprite2Show;
            //    itemImg.SetNativeSize();
            //}
            //else
            //{
            //    // default Image UI
            //    itemImg.sprite = sprite2Show;
            //    itemImg.SetNativeSize();
            //}

            if (currLine.vfxSelect != null)
            {

            }

        }
        else if (currTag == ROLLTAG)
        {
            readingSpeed = 3f;
            Sprite sprite2Show = default;
            ROLLine currLine = (ROLLine)procdLines[progIndex];

            if (currLine.diceSelect != null)
            {
                sprite2Show = dice[(int)currLine.diceSelect].dieSprite;
            }
            else
            {
                // show default die
                sprite2Show = dice[0].dieSprite;
            }

            if (currLine.posSelect != null)
            {
                diceImg.sprite = sprite2Show;
                diceImg.SetNativeSize();
            }
            else
            {
                diceImg.sprite = sprite2Show;
                diceImg.SetNativeSize();
            }
            

            if (currLine.sfxSelect != null)
            {
                SFXAudio.clip = dice[(int)currLine.diceSelect].rollDiceSFX[(int)currLine.sfxSelect];
                SFXAudio.PlayDelayed(0.5f);
            }
            else
            {
                SFXAudio.clip = dice[0].rollDiceSFX[0];
                SFXAudio.PlayDelayed(0.5f);
            }

            if (currLine.vfxSelect != null)
            {

            }
            else
            {

            }

            StartCoroutine(TypingDialogue(dialogDisplay, currLine.dialogContent, typingSpeed));
        }
        else // Character TAG situation ...
        {
            foreach(Characters c in characters)
            {
                if(currTag == c.tagName)
                {
                    dialogImg.sprite = c.dialogBGSprite;

                    DialogLine currLine = (DialogLine)procdLines[progIndex];
                    if (!KP_as_PC && currTag == KPTAG)
                    {
                        // KP line 
                        // Assign our the most special KP
                        if(theKP == null)
                        {
                            theKP = c;
                        }

                        if (currLine.status == 1) // character in
                        {
                            if (currLine.emoSelect != null && currLine.posSelect != null)
                            {
                                characterImgs[(int)currLine.posSelect].sprite = c.emotionSprites[(int)currLine.emoSelect];
                                c.SetCurrentPosition((int)currLine.posSelect);
                                characterImgs[theKP.GetCurrentPosition()].SetNativeSize();
                            }
                            else if (currLine.emoSelect != null && currLine.posSelect == null)
                            {
                                characterImgs[0].sprite = c.emotionSprites[(int)currLine.emoSelect];
                                c.SetCurrentPosition(0);
                                characterImgs[theKP.GetCurrentPosition()].SetNativeSize();
                            }
                            else if (currLine.emoSelect == null && currLine.posSelect != null)
                            {
                                characterImgs[(int)currLine.posSelect].sprite = c.emotionSprites[1];
                                c.SetCurrentPosition((int)currLine.posSelect);
                                characterImgs[theKP.GetCurrentPosition()].SetNativeSize();
                            }
                            else
                            {
                                characterImgs[0].sprite = c.emotionSprites[1];
                                c.SetCurrentPosition(0);
                                characterImgs[theKP.GetCurrentPosition()].SetNativeSize();
                            }
                        }
                        else if (currLine.status == 2) // character leave
                        {
                            if (c.GetCurrentPosition() != 999)
                            {
                                characterImgs[c.GetCurrentPosition()].sprite = null;
                            }
                        }
                        else
                        {
                            // should be changed
                            characterImgs[0].sprite = c.emotionSprites[1];
                            c.SetCurrentPosition(0);
                            characterImgs[theKP.GetCurrentPosition()].SetNativeSize();
                        }
                    }
                    else
                    {
                        // Regular PC line
                        if(theKP)
                        {
                            if (theKP.GetCurrentPosition() != 999)
                            {
                                characterImgs[theKP.GetCurrentPosition()].sprite = theKP.emotionSprites[0];
                                characterImgs[theKP.GetCurrentPosition()].SetNativeSize();
                            }
                        }

                        //characterImgs[0].sprite = characters[0].emotionSprites[0];
                        // should be changed 
                        nameDisplays[0].text = c.displayName.ToString();
                        if(currLine.status != null || currLine.status != 0)
                        {
                            if(currLine.status == 1) // character in
                            {
                                if(currLine.emoSelect != null && currLine.posSelect != null)
                                {
                                    envCharacterSRs[(int)currLine.posSelect].sprite = c.emotionSprites[(int)currLine.emoSelect];
                                    c.SetCurrentPosition((int)currLine.posSelect);
                                }
                                else if(currLine.emoSelect != null && currLine.posSelect == null)
                                {
                                    envCharacterSRs[0].sprite = c.emotionSprites[(int)currLine.emoSelect];
                                    c.SetCurrentPosition(0);
                                }
                                else if (currLine.emoSelect == null && currLine.posSelect != null)
                                {
                                    envCharacterSRs[(int)currLine.posSelect].sprite = c.emotionSprites[0];
                                    c.SetCurrentPosition((int)currLine.posSelect);
                                }
                                else
                                {
                                    envCharacterSRs[0].sprite = c.emotionSprites[0];
                                    c.SetCurrentPosition(0);
                                }
                            }
                            else if(currLine.status == 2) // character leave
                            {
                               if(c.GetCurrentPosition()!= 999)
                                {
                                    envCharacterSRs[c.GetCurrentPosition()].sprite = null;
                                }
                            }
                            else if(currLine.emoSelect != null)
                            {
                                envCharacterSRs[0].sprite = c.emotionSprites[(int)currLine.emoSelect];
                            }
                            else
                            {
                                // should be changed
                                if(c.emotionSprites.Length > 0)
                                {
                                    envCharacterSRs[0].sprite = c.emotionSprites[0];
                                }                              
                            }

                        }
                    }
                    voiceAudio.clip = c.GetClip(currLine.speechIndex);
                    if (voiceAudio.clip.length > 0)
                    {
                        readingSpeed = voiceAudio.clip.length + 0.6f;
                    }
                    else
                    {
                        readingSpeed = 2.0f;
                    }
                    voiceMixer.SetFloat("Pitch", c.mixerPitch);
                    voiceAudio.PlayDelayed(0.5f);

                    StartCoroutine(TypingDialogue(dialogDisplay, currLine.dialogContent, typingSpeed));
                }
            }
        }

        yield return new WaitForSeconds(readingSpeed);
        lineComplete = true;
    }

    private void NextLine()
    {
        lineComplete = false;
        if (progIndex < procdLines.Count)
        {
            progIndex++;
            // yield return new WaitForSeconds(readingSpeed);
            dialogDisplay.text = "";
            foreach (TextMeshProUGUI textPro in nameDisplays)
            {
                textPro.text = "";
            }
            StartCoroutine(ExecutingLine());
        }
        else
        {
            // yield return new WaitForSeconds(readingSpeed);
            dialogDisplay.text = "";
            foreach (TextMeshProUGUI textPro in nameDisplays)
            {
                textPro.text = "";
            }
            lineComplete = false;
            dialogueUI.SetActive(false);
            progIndex = 0;
            fullLog = "";
            rawLines = new List<string>();
            lineTagSeq = new List<string>();
            procdLines = new ArrayList();
            speechSeqCounter = new Hashtable();
            sceneSettled = false;
            // Other lists ...
        }
    }

    private IEnumerator DelayedSetActive(GameObject _go, bool _active, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        _go.SetActive(_active);
    }

    /// TypingSentence
    private IEnumerator TypingDialogue(TextMeshProUGUI _textDisplay, string _dialog, float _typingSpeed)
    {
        foreach (char letter in _dialog)
        {
            _textDisplay.text += letter;
            yield return new WaitForSeconds(_typingSpeed);
        }
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
    private IEnumerator BlackOut(GameObject go, float duration)
    {
        float t = 0;
        float startAlpha = go.GetComponent<CanvasRenderer>().GetAlpha();

        while (t < duration)
        {
            go.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(startAlpha, 1f, t / duration));

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

    private IEnumerator LineCompleteCounter(float _waitingTime)
    {
        yield return new WaitForSeconds(_waitingTime);
        lineComplete = true;
    }

    /// Convert Type
    public int? ToNullableInt(string s)
    {
        int i;
        if (int.TryParse(s, out i))
            return i;
        return null;
    }
}
