using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioControl : MonoBehaviour {
    // assign audio clip to associate
    // set dafault properties in audio source component
    private AudioSource audio_source;
    private float defaultVolume;

    private void Start()
    {
        audio_source = gameObject.GetComponent<AudioSource>();
        defaultVolume = audio_source.volume;

    }
    private void HotspotDown(string go_name)
    {
        Debug.Log("play audio");
        if ( audio_source != null ){
            audio_source.volume = 1;
            if(!audio_source.isPlaying){
                audio_source.Play();
            };
        }   
    }

    private void HotspotUp(string go_name)
    {
        if ( audio_source != null ){
            audio_source.volume = defaultVolume;
            if(!audio_source.playOnAwake){
                // should not be present in bg
                audio_source.Stop();
            };
        }  
    }
}
