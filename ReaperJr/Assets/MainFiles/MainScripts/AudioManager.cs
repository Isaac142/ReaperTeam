using UnityEngine.Audio;
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
        }

        foreach (Sound s in musicClips)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start()
    {
        RestartSetting(); 
    }

    public void RestartSetting()
    {
        PlayMusic("Theme");
    }

    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }
    public void StopPlay(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicClips, sound => sound.name == name);
        s.source.Play();
    }
    public void StopPlayMusic(string name)
    {
        Sound s = Array.Find(musicClips, sound => sound.name == name);
        s.source.Stop();
    }

    public void MuteMusic(bool toggle)
    {
        foreach (Sound music in musicClips)
            music.source.mute = toggle;
    }

    public void MuteSoundFX(bool toggle)
    {
        foreach (Sound s in sounds)
            s.source.mute = toggle;
    }

    public void MusicVolume (float volume)
    {
        foreach (Sound music in musicClips)
            music.source.volume = music.volume * volume;
    }

    public void SoundFXVolume(float volume)
    {
        foreach (Sound s in sounds)
            s.source.volume = s.volume * volume;
    }
}
