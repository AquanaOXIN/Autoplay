using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[CreateAssetMenu(menuName = "Audio/AudioPreviewer")]
public class AudioPreviewer : ScriptableObject
{
    public AudioClip my_audioClip;
    public AudioMixerGroup audioOutput;

    public void PlayPreview(AudioSource source)
    {
        if (!my_audioClip)
            return;
        source.clip = my_audioClip;
        source.outputAudioMixerGroup = audioOutput;
        source.Play();
    }
}
