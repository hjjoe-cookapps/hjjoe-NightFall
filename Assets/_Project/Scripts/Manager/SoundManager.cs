using System;
using System.Collections.Generic;
using _Project.Scripts.Defines;
using UnityEngine;

public class SoundManager : SingletonBehaviour<SoundManager>
{
    private AudioSource[] _audioSources = new AudioSource[Enum.GetValues(typeof(Sound)).Length];
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    public static readonly int MinVolume = 0;
    public static readonly int MaxVolume = 100;

    public void Play(string path, Sound type = Sound.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = null;//GetOrAddAudioClip(path);
        Play(audioClip, type, pitch);
    }

    public void Play(AudioClip audioClip, Sound type = Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        AudioSource audioSource = _audioSources[(int)type];
        audioSource.pitch = pitch;

        if (type == Sound.BGM)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else if (type == Sound.Effect)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void Mute(bool isMute)
    {
        foreach(AudioSource audioSource in _audioSources)
        {
            audioSource.mute = isMute;
        }
    }

    /*
    AudioClip GetOrAddAudioClip(string path)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";
        AudioClip audioClip = null;
        if (audioClips.TryGetValue(path, out audioClip) == false)
        {
            audioClip = Managers.Resource.Load<AudioClip>(path);
            if (audioClip == null)
            {
                Debug.Log($"AudioClip Missing ! {path}");
            }
            else
            {
                audioClips.Add(path, audioClip);
            }
        }

        return audioClip;
    }
    */
}
