using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSlider : MonoBehaviour
{
    public AudioSource music;
    public float musicVolume = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        music.volume = musicVolume;
    }

    public void volumeUpdate( float volume)
    {
        musicVolume = volume;
    }
}
