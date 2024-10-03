using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour {
    public static SoundMixerManager Instance;

    [SerializeField] AudioMixer audioMixer;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this manager persistent across scenes
        } else {
            Destroy(gameObject); // make sure theres only one instance of SoundMixerManager
        }
    }

    public void SetMasterVolume(float level) {
        //audioMixer.SetFloat("masterVolume", level);    not correct function gotta use weird ass mathf clamp
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20);
        SaveSettings();
    }

    public void SetSoundFXVolume(float level) {
        audioMixer.SetFloat("soundFXVolume", Mathf.Log10(level) * 20);
        SaveSettings();
    }

    public void SetMusicVolume(float level) {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20);
        SaveSettings();
    }

    // Save volume settings to PlayerPrefs
    public void SaveSettings() {
        PlayerPrefs.SetFloat("MasterVolume", Mathf.Pow(10, GetVolume("masterVolume") / 20)); // Convert back from log scale
        PlayerPrefs.SetFloat("SFXVolume", Mathf.Pow(10, GetVolume("soundFXVolume") / 20));
        PlayerPrefs.SetFloat("MusicVolume", Mathf.Pow(10, GetVolume("musicVolume") / 20));
        PlayerPrefs.Save();
    }

    // Load saved volume settings from PlayerPrefs and apply them
    public void LoadSettings() {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f); // Default to 1 if not found
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        audioMixer.SetFloat("masterVolume", Mathf.Log10(masterVolume) * 20);
        audioMixer.SetFloat("soundFXVolume", Mathf.Log10(sfxVolume) * 20);
        audioMixer.SetFloat("musicVolume", Mathf.Log10(musicVolume) * 20);
    }

    private float GetVolume(string parameterName) {
        float value;
        audioMixer.GetFloat(parameterName, out value);
        return value;
    }
}