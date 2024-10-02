using UnityEngine;

public class AudioMapper : MonoBehaviour
{
    public TrackName track;
    public enum TrackName
    {
        Main, Vocals, Drums, Bass, Other
    }
    // Use awake so that the spectrums are guaranteed to have a source on start.
    void Awake()
    {
        MapAudio();
    }
    
    void MapAudio()
    {
        var consnsumer = GetComponent<IAudioSourceConsumer>();
        var audioParent = GameObject.FindGameObjectWithTag("audioSourceParent");
        var audioSource = audioParent.transform.Find(track.ToString().ToLower()).GetComponent<AudioSource>();
        if (consnsumer != null)
            consnsumer.SetAudioSource(audioSource);
    }
}
