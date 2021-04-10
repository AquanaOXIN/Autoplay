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
    [Header("Debug View")]
    [SerializeField]
    private int progIndex = 0;
    [SerializeField]
    private string currTag = default;
    [SerializeField]
    private string currDialog = default;

    [Header("Scene Log Setup")]
    [SerializeField]
    private TextAsset[] logFiles = default;
    [SerializeField]
    private int currentSceneNum = default;
    [SerializeField]
    private string STAGETAG = default;
    [SerializeField]
    private string[] KPTAG = default;
    [SerializeField]
    private string ROLLTAG = default; // tag in log for rolling dice
    [SerializeField]
    private string BGTAG = default; // tag in log for background
    [SerializeField]
    private string CGTAG = default; 
    [SerializeField]
    private string ESTAG = default; // tag in log for scene sprite
    [SerializeField]
    private string UISTAG = default; // tag in log for UI sprite

    [Header("Setted Stages")]
    [SerializeField]
    private Stage[] stages = default;

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
    private Canvas mainCanvas = default;
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
    [SerializeField]
    private GameObject cgUI = default;
    [SerializeField]
    private GameObject cgDialogUI = default;

    private Image dialogImg = default;
    [SerializeField]
    private TextMeshProUGUI dialogDisplay = default;
    private List<Image> nameImgs = default;
    private List<TextMeshProUGUI> nameDisplays = default;
    private Image diceImg = default;
    private TextMeshProUGUI diceDisplay = default;
    private List<Image> characterImgs = default;
    private Image itemImg = default;
    private Image foregroundImg = default;
    private Image cgImg = default;

    [Header("Decoration UI Elements")]
    [SerializeField]
    private GameObject leftCurtain = default;
    [SerializeField]
    private GameObject rightCurtain = default;

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
    [SerializeField]
    private Sprite[] CGSprites = default;

    [Header("Audio Elements")]
    [SerializeField, Range(0, 1)]
    private float bgmVolumeCap = default;
    [SerializeField]
    private AudioClip[] bgmClips = default;
    [SerializeField]
    private AudioClip[] sfxClips = default;
    [SerializeField]
    private AudioSource voiceAudio = default;
    [SerializeField]
    private AudioMixer voiceMixer = default;
    [SerializeField]
    private AudioSource SFXAudio = default;
    [SerializeField]
    private AudioSource BGMAudio = default;

    [Header("Variables")]
    // private bool sceneSettled = false;
    [SerializeField]
    private float loadingTime = default;
    [SerializeField, Range(0f, 0.1f)]
    private float typingSpeed = 0.01f;
    [SerializeField, Range(1f, 5f)]
    private float readingSpeed = 1.5f;

    // Text Line related variables
    private string fullLog = default;
    private List<string> rawLines = default;

    private List<string> lineTagSeq = default;
    private List<string> lineDialogSeq = default;
    private ArrayList procdLines = default;
    private bool lineComplete = false;
    private Hashtable speechSeqCounter = default;
    private Hashtable UICharPositionStatus = default;
    private Hashtable envCharPositionStatus = default;

    // KP Specials
    [SerializeField]
    private List<Characters> theKPs = default;

    // Formatting Params
    private int bgInParamNum = 3;
    private int cgInParamNum = 2;
    private int uisInParamNum = 4;
    private int esInParamNum = 4;
    private int rollInParamNum = 6;
    private int dialogInParamNum = 9;
    private int stageInParamNum = 2;

    // Character Display Controls
    private CharacterDisplayController characterDisplayControls = default;

    // UI Controls
    private UIController UIControls = default;
    
    // Effects
    private TransitionEffectsController transitionEffects = default;

    // Other
    private bool inCG = default;
    private bool stageDialogPrompted = default;
    private bool curtainPulled = default;

    private void Awake()
    {
        // Curtain blocking stuff...
        foregroundUI.SetActive(true);
        leftCurtain.SetActive(true);
        rightCurtain.SetActive(true);
        // Initialize UI Components
        dialogImg = dialogueUI.GetComponent<Image>();
        // dialogDisplay = dialogueUI.GetComponentInChildren<TextMeshProUGUI>();
        dialogDisplay.text = "";
        dialogueUI.SetActive(false);

        cgUI.SetActive(false);
        cgDialogUI.SetActive(false);
        inCG = false;
        stageDialogPrompted = false;
        curtainPulled = false;

        nameImgs = new List<Image>();
        nameDisplays = new List<TextMeshProUGUI>();
        foreach (GameObject go in nameUIs)
        {
            Image img = go.GetComponent<Image>();
            nameImgs.Add(img);
            TextMeshProUGUI textPro = go.GetComponentInChildren<TextMeshProUGUI>();
            textPro.text = "";
            nameDisplays.Add(textPro);
            go.SetActive(false);
        }

        characterImgs = new List<Image>();
        UICharPositionStatus = new Hashtable();
        foreach (GameObject go in characterUIs)
        {
            UICharPositionStatus.Add(go, (int)0);
            Image img = go.GetComponent<Image>();
            characterImgs.Add(img);
            go.SetActive(false);
        }

        diceImg = diceUI.GetComponent<Image>();
        diceDisplay = diceUI.GetComponentInChildren<TextMeshProUGUI>();
        diceDisplay.text = "";
        diceUI.SetActive(false);

        itemImg = itemUI.GetComponent<Image>();
        itemUI.SetActive(false);

        cgImg = cgUI.GetComponent<Image>();

        foregroundImg = foregroundUI.GetComponent<Image>();

        // Initialize Environment Components
        envBGSR = envBackground.GetComponentInChildren<SpriteRenderer>();
        envBackground.SetActive(true);

        envCharacterSRs = new List<SpriteRenderer>();
        envCharPositionStatus = new Hashtable();
        foreach (GameObject go in envCharacters)
        {
            envCharPositionStatus.Add(go, (int)0);
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
    }


    private void Start()
    {
        characterDisplayControls = this.GetComponent<CharacterDisplayController>();
        UIControls = this.GetComponent<UIController>();
        transitionEffects = this.GetComponent<TransitionEffectsController>();

        characterDisplayControls.InitializeEnvCharacterList(envCharacters);

        // Initialize log-line related things
        fullLog = "";
        rawLines = new List<string>();
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

        theKPs = new List<Characters>();

        // sceneSettled = false;
        StartCoroutine(LoadingScene());
    }

    private void Update()
    {
        if (lineComplete)
        {
            NextLine();
        }
    }

    private IEnumerator LoadingScene()
    {
        // StartCoroutine(transitionEffects.UIFadeOut(foregroundUI, loadingTime));
        yield return new WaitForSeconds(loadingTime - 1.5f);
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

        // should be changed
        //dialogueUI.SetActive(true);
        //foreach(GameObject go in nameUIs)
        //{
        //    go.SetActive(true);
        //}

        dialogDisplay.text = "";
        foreach(TextMeshProUGUI textPro in nameDisplays)
        {
            textPro.text = "";
        }

        // PullAwayCurtain();
        StartCoroutine(ExecutingLine());
    }

    private void ProcessLog(TextAsset txt)
    {
        fullLog = txt.text;
        rawLines.AddRange(fullLog.Split("\n"[0]));

        // Process raw lines
        string rawLineFormat = @"^\[\S+\]\:\[\S+\].*";
        string bgLineFormat = @".*\|.*\|.*";
        string esLineFormat = @".*\|.*\|.*\|.*";
        string uisLineFormat = @".*\|.*\|.*\|.*";
        string rolLineFormat = @".*\|.*\|.*\|.*";
        string dialogLineFormat = @".*\|.*\|.*\|.*\|.*\|.*\|.*\|.*";
        string stageLineFormat = @".*|.*";
        string cgLineFormat = @".*|.*";

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
                else if(tmpTag == CGTAG)
                {
                    Match _m = Regex.Match(segdLine3[0], cgLineFormat);
                    if (_m.Length > 0)
                    {
                        string[] in_params = Regex.Split(segdLine3[0], splitPattern4);
                        int?[] intParams = new int?[stageInParamNum];
                        int counter = 0;
                        while (counter < intParams.Length)
                        {
                            intParams[counter] = ToNullableInt(in_params[counter]);
                            counter++;
                        }
                        CGLine _line = new CGLine(STAGETAG, intParams[0], intParams[1]);
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
                else if (tmpTag == STAGETAG)
                {
                    Match _m = Regex.Match(segdLine3[0], stageLineFormat);
                    if(_m.Length > 0)
                    {
                        string[] in_params = Regex.Split(segdLine3[0], splitPattern4);
                        int?[] intParams = new int?[stageInParamNum];
                        int counter = 0;
                        while (counter < intParams.Length)
                        {
                            intParams[counter] = ToNullableInt(in_params[counter]);
                            counter++;
                        }
                        STAGELine _line = new STAGELine(STAGETAG, intParams[0], intParams[1]);
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
                    Match _m = Regex.Match(segdLine3[0], esLineFormat);
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
                        ROLLine _line = new ROLLine(ROLLTAG, intParams[0], intParams[1], intParams[2], intParams[3], intParams[4], intParams[5], tmpDialog);
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
                                    intParams[4], intParams[5], intParams[6], intParams[7], intParams[8]);
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
            readingSpeed = 1f;
            BGLine currLine = (BGLine)procdLines[progIndex];
            if(currLine.spriteSelect != null)
            {
                if(!envBackground.activeInHierarchy)
                {
                    envBackground.SetActive(true);
                }
                if(envBGSR.sprite)
                {
                    StartCoroutine(transitionEffects.FadeOut(envBackground, 0.5f));
                    yield return new WaitForSeconds(0.51f);
                }
                envBGSR.sprite = BackgroundSprites[(int)currLine.spriteSelect];
                StartCoroutine(transitionEffects.FadeIn(envBackground, 0.5f));
                yield return new WaitForSeconds(0.52f);
            }
            else
            {
                Debug.Log("BGTAG: No background sprite selected!");
            }
            
        }
        else if (currTag == CGTAG)
        {
            readingSpeed = 0.5f;
            CGLine currLine = (CGLine)procdLines[progIndex];
            if (currLine.cgSelect != null)
            {
                if(currLine.status == 1)
                {
                    inCG = true;
                    cgImg.sprite = CGSprites[(int)currLine.cgSelect];
                    StartCoroutine(DelayedSetActive(cgUI, true, 0.1f));
                    StartCoroutine(transitionEffects.UIFadeIn(cgUI, 0.5f));
                    yield return new WaitForSeconds(0.51f);
                }
            }
            else if(currLine.status == 0)
            {
                inCG = false;
                cgDialogUI.SetActive(false);
                StartCoroutine(transitionEffects.UIFadeOut(cgUI, 0.5f));
                yield return new WaitForSeconds(0.51f);
                cgUI.SetActive(false);
            }
        }
        else if (currTag == STAGETAG)
        {
            readingSpeed = 4f;
            STAGELine currLine = (STAGELine)procdLines[progIndex];

            // text display parts ...
            dialogDisplay.text = "";
            foreach(TextMeshProUGUI t in nameDisplays)
            {
                t.text = "";
            }
            UIControls.UISlide(dialogueUI, mainCanvas, UIController.SlideType.toButtom, LeanTweenType.easeInBack);
            StartCoroutine(DelayedSetActive(dialogueUI, false, 1.1f));
            StartCoroutine(UIControls.DelayedResetUIPosition(dialogueUI, 1.12f));

            if(currLine.stageSelect != null)
            {
                // Audio

                if(stages[(int)currLine.stageSelect].backgroundMusic)
                {
                    if (BGMAudio.clip != null)
                    {
                        StartCoroutine(transitionEffects.AudioFadeOut(BGMAudio, 2f));
                        yield return new WaitForSeconds(2.01f);
                        BGMAudio.Stop();
                    }

                    BGMAudio.clip = stages[(int)currLine.stageSelect].backgroundMusic;
                }
                else
                {
                    // BGMAudio.clip = bgmClips[0];
                }
                
                StartCoroutine(transitionEffects.AudioFadeIn(BGMAudio, 5f, bgmVolumeCap));
                BGMAudio.Play();

                // Background
                if(!envBackground.activeInHierarchy)
                {
                    envBackground.SetActive(true);
                }
                if (envBGSR.sprite != null)
                {
                    characterDisplayControls.ResetAlpha(1);
                    characterDisplayControls.UpdateCharacterPositions();
                    StartCoroutine(transitionEffects.UIFadeIn(foregroundUI, 1f));
                    StartCoroutine(transitionEffects.FadeOut(envBackground, 1f));
                    yield return new WaitForSeconds(1.01f);
                }

                // character display parts ... (Leave)

                for(int i = 0; i < envCharacters.Length; i++)
                {
                    foreach(Characters c in characters)
                    {
                        if(c.isKP == false)
                        {
                            if (c.GetCurrentPosition() == i)
                            {
                                envCharPositionStatus[envCharacters[c.GetCurrentPosition()]] = (int)0;
                                characterDisplayControls.DecreaseOneCharacter(c.GetCurrentPosition());
                                StartCoroutine(DelayedSetActive(envCharacters[c.GetCurrentPosition()], false, 0.95f)); // for UpdateNumOnScreen() keep this time bigger
                                c.SetCurrentPosition(999);
                                yield return new WaitForSeconds(0.05f);
                            }
                        }
                    }
                }

                yield return new WaitForSeconds(1.1f); // 1.51f

                // Characters (Import)
                int posCounter = 0;
                foreach (Characters c in stages[(int)currLine.stageSelect].characters)
                {
                    c.SetCurrentPosition(posCounter);
                    envCharacterSRs[posCounter].sprite = c.emotionSprites[0];
                    envCharPositionStatus[envCharacters[posCounter]] = (int)1;
                    envCharacters[posCounter].transform.localPosition = Vector3.up * (c.offsetY);
                    characterDisplayControls.IncreaseOneCharacter(c.GetCurrentPosition());
                    StartCoroutine(DelayedSetActive(envCharacters[c.GetCurrentPosition()], true, 0.1f));
                    yield return new WaitForSeconds(0.1f);
                    posCounter++;
                }

                if (stages[(int)currLine.stageSelect].backgroundSprite)
                {
                    envBGSR.sprite = stages[(int)currLine.stageSelect].backgroundSprite;
                }
                else
                {
                    envBGSR.sprite = BackgroundSprites[0];
                }
                StartCoroutine(transitionEffects.UIFadeOut(foregroundUI, 1f));
                StartCoroutine(transitionEffects.FadeIn(envBackground, 1f));
                yield return new WaitForSeconds(1.2f);

                if(!curtainPulled)
                {
                    PullAwayCurtain();
                    curtainPulled = true;
                }
                
            }
            else
            {
                Debug.Log("STAGETAG: No stage selected!");
            }
        }

        else if (currTag == ESTAG)
        {
            readingSpeed = 1f;
            ESLine currLine = (ESLine)procdLines[progIndex];
            
            // Dev...
        }
        else if (currTag == UISTAG)
        {          
            readingSpeed = 0.5f;
            UISLine currLine = (UISLine)procdLines[progIndex];

            if(currLine.spriteSelect!= null)
            {
                itemImg.sprite = UISprites[(int)currLine.spriteSelect];
                itemImg.SetNativeSize();
            }

            if(currLine.status == 1)
            {
                dialogueUI.SetActive(false);
                StartCoroutine(transitionEffects.UIFadeIn(itemUI, 0.45f));                        
                itemUI.SetActive(true);
            }
            else if(currLine.status == 0)
            {
                StartCoroutine(transitionEffects.UIFadeOut(itemUI, 0.3f));
                StartCoroutine(DelayedSetActive(itemUI, false, 0.31f));
            }       
        }
        else if (currTag == ROLLTAG)
        {
            ROLLine currLine = (ROLLine)procdLines[progIndex];
            readingSpeed = 1f * (currLine.dialogContent.ToCharArray().Length) / 10f;

            Sprite _dialogSprite = default;
            if (currLine.diceSelect != null)
            {
                diceImg.sprite = dice[(int)currLine.diceSelect].dieSprite;
                _dialogSprite = dice[(int)currLine.diceSelect].dialogBGSprite;
                diceImg.SetNativeSize();
            }
            else
            {
                diceImg.sprite = dice[0].dieSprite;
                _dialogSprite = dice[0].dialogBGSprite;
                diceImg.SetNativeSize();
            }
            dialogImg.sprite = _dialogSprite;

            if(inCG)
            {
                cgDialogUI.SetActive(true);
            }

            if (!dialogueUI.activeInHierarchy && !inCG)
            {
                // dialogueUI.SetActive(true);
                UIControls.UISlide(dialogueUI, mainCanvas, UIController.SlideType.fromButtom, LeanTweenType.easeOutBack);
                StartCoroutine(DelayedSetActive(dialogueUI, true, 0.1f));
            }

            //StartCoroutine(transitionEffects.UIFadeIn(diceUI, 0.15f));
            //diceUI.SetActive(true);
            UIControls.UISlide(diceUI, mainCanvas, UIController.SlideType.fromTop, LeanTweenType.easeOutBounce);
            StartCoroutine(DelayedSetActive(diceUI, true, 0.1f));

            if (currLine.sfxSelect != null)
            {
                SFXAudio.clip = dice[(int)currLine.diceSelect].rollDiceSFX[(int)currLine.sfxSelect];
                SFXAudio.PlayDelayed(0.25f);
            }
            else
            {
                SFXAudio.clip = dice[0].rollDiceSFX[0];
                SFXAudio.PlayDelayed(0.25f);
            }
            StartCoroutine(TypingDialogue(dialogDisplay, currLine.dialogContent, typingSpeed));

            yield return new WaitForSeconds(0.25f);
            if(currLine.result != null)
            {
                diceDisplay.text = currLine.result.ToString();
            }
            float diceStayTime = (readingSpeed > 1.8f) ? readingSpeed - 1.2f : readingSpeed; 
            yield return new WaitForSeconds(diceStayTime);
            UIControls.UISlide(diceUI, mainCanvas, UIController.SlideType.toTop, LeanTweenType.easeInOutBack);
            StartCoroutine(DelayedSetActive(diceUI, false, 1.1f));
            StartCoroutine(UIControls.DelayedResetUIPosition(diceUI, 1.15f));
            //StartCoroutine(transitionEffects.UIFadeOut(diceUI, 0.1f));
            //diceUI.SetActive(false);
            diceDisplay.text = "";
            yield return new WaitForSeconds(1.2f);
        }
        else // Character TAG situation ...
        {
            foreach(Characters c in characters)
            {
                // Debug.Log(inCG);
                if(currTag == c.tagName)
                {
                    if(c.GetCurrentPosition() != 999) // if haven't in scene, wait for VFX to pull up the UIs
                    {
                        if (!dialogueUI.activeInHierarchy && !inCG)
                        {
                            dialogueUI.SetActive(true);
                            //UIControls.UISlide(dialogueUI, mainCanvas, UIController.SlideType.fromButtom, LeanTweenType.easeOutBack);
                            //StartCoroutine(DelayedSetActive(dialogueUI, true, 0.1f));
                            //yield return new WaitForSeconds(1f);
                        }
                        else if(inCG)
                        {
                            cgDialogUI.SetActive(true);
                        }
                        if (!nameUIs[c.nameDisplayPosition].activeInHierarchy)
                        {
                            nameUIs[c.nameDisplayPosition].SetActive(true);
                        }
                    }
                    else
                    {
                        dialogueUI.SetActive(false);
                        nameUIs[c.nameDisplayPosition].SetActive(false);
                    }
                    dialogImg.sprite = c.dialogBGSprite;

                    DialogLine currLine = (DialogLine)procdLines[progIndex];

                    int? _status = currLine.status;

                    bool isKP = false;
                    foreach (string tag in KPTAG)
                    {
                        // determine if KP
                        if (currTag == tag)
                        {
                            isKP = true;
                            bool knownKP = false;
                            foreach (Characters kp in theKPs)
                            {
                                if (kp.tagName == currTag)
                                    knownKP = true;
                            }
                            if (!knownKP)
                            {
                                theKPs.Add(c); // initialize kp list
                            }
                            break;
                        }                       
                    }

                    // If it's tagged as KP ...
                    if(isKP)
                    {
                        characterDisplayControls.ResetAlpha(1);

                        if (!KP_as_PC)
                        {
                            // KP line 
                            Sprite sprite2Display = default;

                            // emoSelect
                            if (currLine.emoSelect != null)
                            {
                                sprite2Display = c.emotionSprites[(int)currLine.emoSelect];
                            }
                            else
                            {
                                if (c.GetCurrentPosition() == 999) // if not displayed, then select a sprite to display
                                {
                                    sprite2Display = c.emotionSprites[1];
                                }
                                else
                                {
                                    // sprite2Display = characterImgs[c.GetCurrentPosition()].sprite;
                                    sprite2Display = c.emotionSprites[1];
                                }
                            }

                            // posSelect
                            if (currLine.posSelect != null)
                            {
                                if ((int)UICharPositionStatus[characterUIs[(int)currLine.posSelect]] == 0)
                                {
                                    c.SetCurrentPosition((int)currLine.posSelect);
                                    characterImgs[(int)currLine.posSelect].sprite = sprite2Display;
                                    characterImgs[(int)currLine.posSelect].SetNativeSize();
                                    UICharPositionStatus[characterUIs[(int)currLine.posSelect]] = (int)1;
                                }
                            }
                            else
                            {
                                if (c.GetCurrentPosition() == 999)
                                {
                                    for (int i = 0; i < characterUIs.Length; i++)
                                    {
                                        if ((int)UICharPositionStatus[characterUIs[i]] == 0)
                                        {
                                            c.SetCurrentPosition(i);
                                            characterImgs[i].sprite = sprite2Display;
                                            characterImgs[i].SetNativeSize();
                                            UICharPositionStatus[characterUIs[i]] = (int)1;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    characterImgs[c.GetCurrentPosition()].sprite = sprite2Display;
                                    characterImgs[c.GetCurrentPosition()].SetNativeSize();
                                }
                            }

                            if (_status == 1)
                            {
                                StartCoroutine(transitionEffects.UIFadeIn(characterUIs[c.GetCurrentPosition()], 0.5f));
                                characterUIs[c.GetCurrentPosition()].SetActive(true);
                                // dialogueUI.SetActive(true);
                                // Add LeanTween ...
                                UIControls.UISlide(dialogueUI, mainCanvas, UIController.SlideType.fromButtom, LeanTweenType.easeOutBack);
                                if(characterUIs[c.GetCurrentPosition()].GetComponent<RectTransform>().anchoredPosition.x > 0)
                                {
                                    UIControls.UISlide(characterUIs[c.GetCurrentPosition()], mainCanvas, UIController.SlideType.fromRight, LeanTweenType.easeOutSine);
                                }
                                else
                                {
                                    UIControls.UISlide(characterUIs[c.GetCurrentPosition()], mainCanvas, UIController.SlideType.fromLeft, LeanTweenType.easeOutSine);
                                }                             
                                StartCoroutine(DelayedSetActive(dialogueUI, true, 0.1f));
                                StartCoroutine(DelayedSetActive(characterUIs[c.GetCurrentPosition()], true, 0.1f));
                                yield return new WaitForSeconds(1f);
                                nameUIs[c.nameDisplayPosition].SetActive(true);
                            }
                            else if (_status == null)
                            {
                                // Debug.Log(c.GetCurrentPosition());
                                if (!characterUIs[c.GetCurrentPosition()].activeInHierarchy)
                                {
                                    
                                    UIControls.UISlide(characterUIs[c.GetCurrentPosition()], mainCanvas, UIController.SlideType.fromButtom, LeanTweenType.easeInBounce);                                  
                                    StartCoroutine(DelayedSetActive(characterUIs[c.GetCurrentPosition()], true, 0.1f));
                                    if(!dialogueUI.activeInHierarchy && !inCG)
                                    {
                                        UIControls.UISlide(dialogueUI, mainCanvas, UIController.SlideType.fromButtom, LeanTweenType.easeOutBack);
                                        StartCoroutine(DelayedSetActive(dialogueUI, true, 0.1f));
                                        yield return new WaitForSeconds(1f);
                                        nameUIs[c.nameDisplayPosition].SetActive(true);
                                    }
                                    else if(inCG)
                                    {
                                        cgDialogUI.SetActive(true);
                                    }
                                }
                            }

                            voiceAudio.clip = c.GetClip(currLine.speechIndex);
                            if (voiceAudio.clip.length > 0)
                            {
                                readingSpeed = voiceAudio.clip.length + 0.18f;
                            }
                            else
                            {
                                readingSpeed = 2.0f;
                            }
                            voiceMixer.SetFloat("Pitch", c.mixerPitch);
                            voiceAudio.PlayDelayed(0.15f);

                            // 2nd Emotion Sprite Change
                            if (currLine.charNum != null && currLine.emoSelect2 != null && currLine.timing != null)
                            {
                                if (characters[(int)currLine.charNum].GetCurrentPosition() != 999)
                                {
                                    StartCoroutine(SecondaryEmotionChange((int)currLine.charNum, (int)currLine.emoSelect2, (int)currLine.timing, currLine.dialogContent, readingSpeed));
                                }
                            }

                            if (c.displayName.Length > 0)
                            {
                                nameDisplays[c.nameDisplayPosition].text = c.displayName.ToString();
                            }
                            StartCoroutine(TypingDialogue(dialogDisplay, currLine.dialogContent, typingSpeed));

                            // SHOULD BE CHANGED
                            if(_status == 0 && currLine.emoSelect != null)
                            {
                                characterImgs[c.GetCurrentPosition()].sprite = sprite2Display;
                            }

                            yield return new WaitForSeconds(readingSpeed);

                            if (_status == 0)
                            {
                                StartCoroutine(transitionEffects.UIFadeOut(characterUIs[c.GetCurrentPosition()], 1f));
                                StartCoroutine(DelayedSetActive(characterUIs[c.GetCurrentPosition()], false, 1.1f));
                                c.SetCurrentPosition(999);
                                // dialogueUI.SetActive(false); // should be LeanTween VFX
                                UIControls.UISlide(dialogueUI, mainCanvas, UIController.SlideType.toLeft, LeanTweenType.easeInBack);
                                StartCoroutine(DelayedSetActive(dialogueUI, false, 1.1f));
                                StartCoroutine(UIControls.DelayedResetUIPosition(dialogueUI, 1.12f));
                                yield return new WaitForSeconds(1.15f);
                            }
                            // lineComplete = true;
                        }
                    }
                    else // NOT TAGGED as KP
                    {
                        // Regular PC line
                        Sprite sprite2Display = default;

                        // KP stand-by DELETED FEATURE
                        //foreach (Characters kp in theKPs)
                        //{
                        //    if (kp.GetCurrentPosition() != 999)
                        //    {
                        //        characterImgs[kp.GetCurrentPosition()].sprite = kp.emotionSprites[0];
                        //        characterImgs[kp.GetCurrentPosition()].SetNativeSize();
                        //    }
                        //}

                        // emoSelect
                        if (currLine.emoSelect != null)
                        {
                            sprite2Display = c.emotionSprites[(int)currLine.emoSelect];
                        }
                        else
                        {
                            if (c.GetCurrentPosition() == 999) // if not displayed, then select a sprite to display
                            {
                                sprite2Display = c.emotionSprites[0];
                            }
                            else
                            {
                                // current displayed sprite
                                sprite2Display = envCharacterSRs[(int)c.GetCurrentPosition()].sprite;
                            }
                        }

                        // posSelect
                        if (currLine.posSelect != null)
                        {
                            if (c.GetCurrentPosition() == 999) // if character hasn't been add to the scene yet
                            {
                                if ((int)envCharPositionStatus[envCharacters[(int)currLine.posSelect]] == 1)
                                {
                                    Debug.Log("Wrong Position: Position occupied!");
                                }
                                else
                                {
                                    // then add it to the scene
                                    int _pos = (int)currLine.posSelect;
                                    c.SetCurrentPosition(_pos);
                                    envCharacterSRs[_pos].sprite = sprite2Display;
                                    envCharPositionStatus[envCharacters[_pos]] = (int)1;
                                    envCharacters[_pos].transform.localPosition = Vector3.up * (c.offsetY);
                                }
                            }
                            else // if already in the scene
                            {
                                int _curPos = c.GetCurrentPosition();
                                // if occupied
                                if ((int)envCharPositionStatus[envCharacters[(int)currLine.posSelect]] == 1)
                                {
                                    // don't set the current SELECTED position to the current character
                                }
                                // if vacant
                                else if ((int)envCharPositionStatus[envCharacters[(int)currLine.posSelect]] == 0)
                                {

                                    c.SetCurrentPosition((int)currLine.posSelect);
                                    envCharacterSRs[(int)currLine.posSelect].sprite = sprite2Display;
                                    envCharPositionStatus[envCharacters[(int)currLine.posSelect]] = (int)1;
                                    envCharacters[(int)currLine.posSelect].transform.localPosition = Vector3.up * (c.offsetY);
                                }
                                characterDisplayControls.MoveCharacterPosition(_curPos, (int)currLine.posSelect);
                            }
                        }
                        else
                        {
                            if (c.GetCurrentPosition() == 999)
                            {
                                for (int i = 0; i < envCharacters.Length; i++)
                                {
                                    if ((int)envCharPositionStatus[envCharacters[i]] == 0)
                                    {
                                        c.SetCurrentPosition(i);
                                        envCharacterSRs[i].sprite = c.emotionSprites[0];
                                        envCharPositionStatus[envCharacters[i]] = (int)1;
                                        envCharacters[i].transform.localPosition = Vector3.up * (c.offsetY);
                                        // envCharacters[i].SetActive(true);
                                        break;
                                    }
                                }
                            }
                        }

                        if (_status == 1)
                        {
                            characterDisplayControls.IncreaseOneCharacter(c.GetCurrentPosition());
                            StartCoroutine(DelayedSetActive(envCharacters[c.GetCurrentPosition()], true, 0.1f)); // for UpdateNumOnScreen(), keep this number small
                            // dialogueUI.SetActive(true);
                            // Add LeanTween for dialogueUI...
                            UIControls.UISlide(dialogueUI, mainCanvas, UIController.SlideType.fromButtom, LeanTweenType.easeOutBack);
                            StartCoroutine(DelayedSetActive(dialogueUI, true, 0.1f));
                            yield return new WaitForSeconds(1f);
                            nameUIs[c.nameDisplayPosition].SetActive(true);

                        }
                        else if (_status == null)
                        {
                            if (!envCharacters[c.GetCurrentPosition()].activeInHierarchy)
                            {
                                characterDisplayControls.IncreaseOneCharacter(c.GetCurrentPosition());
                                StartCoroutine(DelayedSetActive(envCharacters[c.GetCurrentPosition()], true, 0.1f)); // for UpdateNumOnScreen(), keep this number small
                                if (!dialogueUI.activeInHierarchy && !inCG)
                                {
                                    UIControls.UISlide(dialogueUI, mainCanvas, UIController.SlideType.fromButtom, LeanTweenType.easeOutBack);
                                    StartCoroutine(DelayedSetActive(dialogueUI, true, 0.1f));
                                    yield return new WaitForSeconds(1f);
                                    nameUIs[c.nameDisplayPosition].SetActive(true);
                                }
                                else if (inCG)
                                {
                                    cgDialogUI.SetActive(true);
                                }
                            }
                            envCharacterSRs[c.GetCurrentPosition()].sprite = sprite2Display;
                        }
                        // Speaking...
                        if (c.GetCurrentPosition() != 999)
                        {
                            characterDisplayControls.CharacterSpeaking(c.GetCurrentPosition());
                        }

                        // ===================Audio=======================

                        voiceAudio.clip = c.GetClip(currLine.speechIndex);
                        if (voiceAudio.clip.length > 0)
                        {
                            readingSpeed = voiceAudio.clip.length + 0.52f;
                        }
                        else
                        {
                            readingSpeed = 2.0f;
                        }
                        voiceMixer.SetFloat("Pitch", c.mixerPitch);
                        voiceAudio.PlayDelayed(0.5f);

                        // 2nd Emotion Sprite Change
                        if (currLine.charNum != null && currLine.emoSelect2 != null && currLine.timing != null)
                        {
                            if (characters[(int)currLine.charNum].GetCurrentPosition() != 999)
                            {
                                StartCoroutine(SecondaryEmotionChange((int)currLine.charNum, (int)currLine.emoSelect2, (int)currLine.timing, currLine.dialogContent, readingSpeed));
                            }
                        }

                        nameDisplays[c.nameDisplayPosition].text = c.displayName.ToString();

                        StartCoroutine(TypingDialogue(dialogDisplay, currLine.dialogContent, typingSpeed));

                        //// Timing Stuff
                        //if (currLine.timing != null)
                        //{
                        //    float theTime = ((int)currLine.timing / currDialog.Length) * readingSpeed;

                        //    yield return new WaitForSeconds(theTime);
                        //    // 2nd Emotion Change...

                        //}

                        // SHOULD BE CHANGED
                        if (_status == 0 && currLine.emoSelect != null)
                        {
                            envCharacterSRs[c.GetCurrentPosition()].sprite = sprite2Display;
                        }

                        yield return new WaitForSeconds(readingSpeed);
                        // 
                        if (_status == 0)
                        {
                            envCharPositionStatus[envCharacters[c.GetCurrentPosition()]] = (int)0;
                            dialogDisplay.text = "";
                            nameDisplays[c.nameDisplayPosition].text = "";
                            characterDisplayControls.DecreaseOneCharacter(c.GetCurrentPosition());
                            StartCoroutine(DelayedSetActive(envCharacters[c.GetCurrentPosition()], false, 0.95f)); // for UpdateNumOnScreen() keep this time bigger (if buggy then -> 1.5f)
                            c.SetCurrentPosition(999);
                            // dialogueUI.SetActive(false); // should be LeanTween VFX
                            UIControls.UISlide(dialogueUI, mainCanvas, UIController.SlideType.toButtom, LeanTweenType.easeInBack);
                            StartCoroutine(DelayedSetActive(dialogueUI, false, 1.1f));
                            StartCoroutine(UIControls.DelayedResetUIPosition(dialogueUI, 1.12f));
                            yield return new WaitForSeconds(1.51f);
                        }
                        // lineComplete = true;
                    }
                }
            }

            if (stageDialogPrompted == false)
            {
                stageDialogPrompted = true;
            }
        }

        lineComplete = true;
    }

    private void NextLine()
    {
        lineComplete = false;
        if (progIndex < procdLines.Count - 1)
        {
            progIndex++;
            // yield return new WaitForSeconds(readingSpeed);
            dialogDisplay.text = "";
            foreach (TextMeshProUGUI textPro in nameDisplays)
            {
                textPro.text = "";
            }
            diceDisplay.text = "";
            StartCoroutine(ExecutingLine());
        }
        else
        {
            // yield return new WaitForSeconds(readingSpeed);
            foreach(GameObject go in characterUIs)
            {
                if(go.activeInHierarchy)
                {
                    if(go.GetComponent<RectTransform>().anchoredPosition.x > 0)
                    {
                        UIControls.UISlide(go, mainCanvas, UIController.SlideType.toRight, LeanTweenType.easeInOutSine);
                    }
                    else
                    {
                        UIControls.UISlide(go, mainCanvas, UIController.SlideType.toLeft, LeanTweenType.easeInOutSine);
                    }                  
                    StartCoroutine(DelayedSetActive(go, false, 1.1f));
                    StartCoroutine(UIControls.DelayedResetUIPosition(go, 1.12f));
                }
            }

            PullCloseCurtain();
            dialogDisplay.text = "";
            foreach (TextMeshProUGUI textPro in nameDisplays)
            {
                textPro.text = "";
            }
            diceDisplay.text = "";
            lineComplete = false;
            // dialogueUI.SetActive(false);
            UIControls.UISlide(dialogueUI, mainCanvas, UIController.SlideType.toButtom, LeanTweenType.easeInBack);
            StartCoroutine(DelayedSetActive(dialogueUI, false, 1.1f));
            StartCoroutine(UIControls.DelayedResetUIPosition(dialogueUI, 1.12f));

            progIndex = 0;
            fullLog = "";
            rawLines = new List<string>();
            lineTagSeq = new List<string>();
            procdLines = new ArrayList();
            speechSeqCounter = new Hashtable();
            envCharPositionStatus = new Hashtable();
            // sceneSettled = false;
            // Other lists ...
            theKPs = new List<Characters>();
        }
    }

    private IEnumerator SecondaryEmotionChange(int charNum, int emoSelect, int timing, string dialog, float voiceLength)
    {
        int stringChars = dialog.ToCharArray().Length;
        float theTime = ((float)timing / (float)stringChars) * voiceLength;
        // Debug.Log("timing: " + timing + ";dialog length: " + stringChars + ";voice length: " + voiceLength);
        yield return new WaitForSeconds(theTime);
        envCharacterSRs[characters[charNum].GetCurrentPosition()].sprite = characters[charNum].emotionSprites[emoSelect];
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

    private IEnumerator LineCompleteCounter(float _waitingTime)
    {
        yield return new WaitForSeconds(_waitingTime);
        lineComplete = true;
    }

    private void PullAwayCurtain()
    {
        UIControls.UISlide(leftCurtain, mainCanvas, UIController.SlideType.toLeft, LeanTweenType.easeInOutSine);
        UIControls.UISlide(rightCurtain, mainCanvas, UIController.SlideType.toRight, LeanTweenType.easeInOutSine);
    }

    private void PullCloseCurtain()
    {
        UIControls.UISlide(leftCurtain, mainCanvas, UIController.SlideType.toRight, LeanTweenType.easeInOutSine);
        UIControls.UISlide(rightCurtain, mainCanvas, UIController.SlideType.toLeft, LeanTweenType.easeInOutSine);
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
