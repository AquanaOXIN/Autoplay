using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Character", menuName = "ScriptableObjects/Character", order = 1)]
public class Characters : ScriptableObject
{
    [Header("Basic Info")]
    public string tagName = default;
    public string displayName = default;
    public string fullName = default;

    [Header("Related Images")]
    public Sprite[] emotionSprites = default;
    public Sprite dialogBGSprite = default;
    // public Sprite nameBackground = default;

    [Header("Related Audio")]
    public string audioFolder = default;
    public float mixerPitch = default;
    // Greenal Mixer Pitch 1.02f
    // Shitou Mixer Pitch 0.92f

    private AudioClip[] voiceClips = default;
    private int currentPosition = 999;

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
