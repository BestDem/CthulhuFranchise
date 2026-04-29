using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicValueChanged : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixerSound;
    [SerializeField] private Slider volumeAudioSound;
    private float soundVolume = 3f;

    private void Start()
    {
        ResetValue();
    }

    public void ResetValue()
    {
        soundVolume = PlayerPrefs.GetFloat("SoundVolume", soundVolume);

        volumeAudioSound.value = soundVolume;
        audioMixerSound.SetFloat("MusicVolume", soundVolume);
    }
    public void SetVolumeSound(float newVolume)
    {
        soundVolume = Mathf.Clamp01(newVolume);
        if (soundVolume <= 0)
        {
            soundVolume = -80;
        }
        else
        {
            soundVolume = Mathf.Log10(soundVolume) * 20;
        }
        audioMixerSound.SetFloat("SoundVolume", soundVolume);

        PlayerPrefs.SetFloat("SoundVolume", newVolume);
        PlayerPrefs.Save();
    }
}