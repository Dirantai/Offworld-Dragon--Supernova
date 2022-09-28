using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : MonoBehaviour
{
   

    [System.Serializable]
    public class Sound{
        public string key = "";
        public AudioClip[] clips;
        public AudioSource audioSource;

        public bool playing;
        public float currentDuration;
    }

    public float globalVolumeMultiplier;

    public Sound[] sounds;

    void Update(){
        foreach(Sound sound in sounds){
            if(sound.playing && sound.currentDuration > 0){
                sound.currentDuration -= Time.deltaTime;
            }else{
                sound.playing = false;
            }
        }
    }
    
    public void PlaySounds(string key){
        AudioClip sound = null;
        AudioSource audioSource = null;

        int i = 0;
        while(sound == null && i < sounds.Length){
            if(sounds[i].key.ToLower().Contains(key.ToLower())){
                int r = Mathf.RoundToInt(Random.Range(0, sounds[i].clips.Length));
                r = Mathf.Clamp(r, 0, sounds[i].clips.Length - 1);
                sound = sounds[i].clips[r];
                audioSource = sounds[i].audioSource;
            }
            i++;
        }
        i--;
        if(sound != null && !sounds[i].playing){
            audioSource.PlayOneShot(sound);
            sounds[i].playing = true;
            sounds[i].currentDuration = sound.length / 3;
        }
    }

    public void PlaySounds(string key, float delay){
        AudioClip sound = null;
        AudioSource audioSource = null;

        int i = 0;
        while(sound == null && i < sounds.Length){
            if(sounds[i].key.ToLower().Contains(key.ToLower())){
                int r = Mathf.RoundToInt(Random.Range(0, sounds[i].clips.Length));
                r = Mathf.Clamp(r, 0, sounds[i].clips.Length - 1);
                sound = sounds[i].clips[r];
                audioSource = sounds[i].audioSource;
            }
            i++;
        }
        i--;
        if(sound != null && !sounds[i].playing){
            audioSource.PlayOneShot(sound);
            sounds[i].playing = true;
            sounds[i].currentDuration = delay;
        }
    }

     public void SetPitch(float pitch, string key){
        AudioSource sound = null;
        int i = 0;
        while(sound == null && i < sounds.Length){
            if(sounds[i].key.ToLower().Contains(key.ToLower())){
                sound = sounds[i].audioSource;
            }
            i++;
        }

        if(sound != null){
            sound.pitch = pitch;
        }
    }

    public void SetVolume(float volume, string key){
        AudioSource sound = null;
        int i = 0;
        while(sound == null && i < sounds.Length){
            if(sounds[i].key.ToLower().Contains(key.ToLower())){
                sound = sounds[i].audioSource;
            }
            i++;
        }

        if(sound != null){
            sound.volume = volume * (globalVolumeMultiplier / 100);
        }
    }
}
