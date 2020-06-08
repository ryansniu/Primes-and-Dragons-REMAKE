using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour {
    public static AudioController Instance;
    public AudioMixer mixer;
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }
        mixer.SetFloat("musVol", volumeEq(PlayerPrefs.GetFloat("musVol", 1f)));
        mixer.SetFloat("sfxVol", volumeEq(PlayerPrefs.GetFloat("sfxVol", 1f)));
    }

    public void setMusicVolume(float value) {
        PlayerPrefs.SetFloat("musVol", value);
        mixer.SetFloat("musVol", volumeEq(value));
    }
    public void setSFXVolume(float value) {
        PlayerPrefs.SetFloat("sfxVol", value);
        mixer.SetFloat("sfxVol", volumeEq(value));
    }

    private float volumeEq(float value) {  // TO-DO: Fix this LUL
        return (value - 1) * 80f;
    }
}
