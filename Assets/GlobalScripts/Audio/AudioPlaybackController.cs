using System.Linq;
using UnityEngine;

public class AudioPlaybackController
{
    private AudioSource[] audioSources;

    public void SetAudioSources(AudioSource[] sources)
    {
        audioSources = sources;
    }

    public void PlayAudio()
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.Play();
        }
    }

    public void StopAudio()
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.Stop();
            audioSource.time = 0;
        }
    }

    public void PauseAudio()
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.Pause();
        }
    }

    public void ToggleMute(string tag)
    {
        var source = audioSources.First(source => source.CompareTag(tag));
        source.mute = !source.mute;
    }

    public void Skip(float seconds)
    {
        var source = audioSources[0];
        var currentTime = source.time;
       
        var targetTime = Mathf.Clamp(currentTime + seconds, 0, source.clip.length);

        foreach (var audioSource in audioSources)
        {
            audioSource.time = targetTime;
        }
    }
}