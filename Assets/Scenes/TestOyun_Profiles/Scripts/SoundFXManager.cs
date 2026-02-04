using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlayRandomSoundFXClip(
    AudioClip[] clips,
    Transform spawnTransform,
    float volume,
    Vector2 pitchRange
)
    {
        if (clips == null || clips.Length == 0)
            return;

        int rand = UnityEngine.Random.Range(0, clips.Length);
        PlaySoundFXClip(clips[rand], spawnTransform, volume, pitchRange);
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume , Vector2 pitchRange)
    {

        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);

        audioSource.Play();
        float clipLength = audioSource.clip.length;
    }

}
