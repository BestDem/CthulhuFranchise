using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundsComponent : MonoBehaviour
{
    [SerializeField] private List<AudioData> sounds;
    private AudioSource source;
    private void Start()
    {
        source = GetComponent<AudioSource>();
    }
    public void Play(string id)
    {
        foreach(var audioData in sounds)
        {
            if (audioData.id == id) 
            {
                source.PlayOneShot(audioData.clip);
                break;
            }           
        }
    }
    public void Stop() => source.Stop();

    [Serializable]
    public class AudioData
    {
        [SerializeField] private string _id;
        [SerializeField] private AudioClip _clip;
        public string id => _id;
        public AudioClip clip => _clip;
    }
}
