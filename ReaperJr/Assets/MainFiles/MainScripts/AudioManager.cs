﻿using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public Sound[] musicClips;
    public Sound[] sounds;

    // Start is called before the first frame update
    void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.Stop();
        }

        foreach (Sound s in musicClips)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.Stop();
        }
    }

    public void Play (AudioSource obj, string name, float distance)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        obj.clip = s.clip;
        obj.loop = s.loop;
        obj.volume = s.volume * _UI.soundFXSlider.value;
        obj.mute = s.source.mute;
        obj.maxDistance = distance;
        if(!obj.isPlaying)
            obj.Play();
    }

    public void StopPlay(GameObject obj)
    {
        obj.GetComponent<AudioSource>().Stop();
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicClips, musicClips => musicClips.name == name);
        _CAMERA.GetComponent<AudioSource>().clip = s.clip;
        _CAMERA.GetComponent<AudioSource>().loop = s.loop;
        _CAMERA.GetComponent<AudioSource>().volume = s.volume * _UI.musicSlider.value;
        _CAMERA.GetComponent<AudioSource>().mute = s.source.mute;
        if (!_CAMERA.GetComponent<AudioSource>().isPlaying)
            _CAMERA.GetComponent<AudioSource>().Play();
    }

    public void MuteMusic(bool toggle)
    {
        _CAMERA.GetComponent<AudioSource>().mute = toggle;
    }

    public void MuteSoundFX(bool toggle)
    {
        foreach (Sound s in sounds)
            s.source.mute = toggle;
    }

    public void MusicVolume (float volume)
    {
        foreach (Sound music in musicClips)
            _CAMERA.GetComponent<AudioSource>().volume = music.volume * volume;
    }

    public void SoundFXVolume(float volume)
    {
        foreach (Sound s in sounds)
            s.source.volume = s.volume * volume;
    }
}
