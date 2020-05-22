using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour {
    public static AudioController Instance;
    public AudioMixer mixer;
    public AudioSource musicSource;
    public AudioSource sfxSource;

    //TO-DO: player prefs and adjust volume slider equation

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void setMusicVolume(float value) {
        mixer.SetFloat("musVol", (value - 1) * 80f);  //adjust
    }
    public void setSFXVolume(float value) {
        mixer.SetFloat("sfxVol", (value - 1) * 80f);  //adjust

    }
}
