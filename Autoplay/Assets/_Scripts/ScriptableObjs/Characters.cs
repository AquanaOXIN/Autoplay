using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Character", menuName = "ScriptableObjects/Character", order = 1)]
public class Characters : ScriptableObject
{
    [Header("Basic Info")]
    public string tagName = default; // name used in the log file for [TAG]
    public string displayName = default;
    public string fullName = default;
    public string[] nameInOtherLanguage = default;
    public int nameDisplayPosition = default;

    [Header("Related Images")]
    public Sprite dialogBGSprite = default;
    public Sprite nameBGSprite = default;
    public Sprite[] emotionSprites = default;

    [Header("Animations")]
    public bool animated = default;
    public GameObject animatedCharacter = default;

    [Header("Related Audio")]
    public string audioFolder = default;
    public float mixerPitch = default;

    private AudioClip[] voiceClips = default;
    [SerializeField]
    private int defaultPosition = 999;
    private int currentPosition = 999;

    private void OnEnable()
    {
        currentPosition = defaultPosition;
    }

    public void LoadVoiceAudioFiles(int currentSceneNum)
    {
        voiceClips = Resources.LoadAll<AudioClip>("Audio/" + currentSceneNum.ToString() +"/" + audioFolder);
    }

    public AudioClip GetClip(int _currCounter)
    {
        return voiceClips[_currCounter];
    }

    public void SetCurrentPosition(int _pos)
    {
        currentPosition = _pos;
    }

    public int GetCurrentPosition()
    {
        return currentPosition;
    }
}
