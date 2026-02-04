using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] public AudioMixer mainMixer;

    public void SetMasterVolume(float level)
    {
        mainMixer.SetFloat("MasterVolume", level);
    }
    public void SetSoundFXVolume(float level)
    {
        mainMixer.SetFloat("SoundFXVolume", level);
    }
    public void SetMusicVolume(float level)
    {
        mainMixer.SetFloat("MusicVolume", level);

    }
}
