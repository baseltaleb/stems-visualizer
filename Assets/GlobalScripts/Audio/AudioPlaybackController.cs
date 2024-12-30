using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

public class AudioPlaybackController
{
    public static readonly SynchronizedReactiveProperty<bool> IsPlaying = new(false);
    public static readonly ReactiveProperty<int> SongEnded = new();
    public string CurrentSongFile;

    private AudioSource[] audioSources;
    private AudioSource sampleSource;
    
    public void SetAudioSources(AudioSource[] sources)
    {
        audioSources = sources;
        IsPlaying
            .DistinctUntilChanged()
            .SubscribeAwait(async (playing, ct) =>
            {
                if (playing)
                    await PlayAudioAsync(ct);
                else
                {
                    foreach (var audioSource in audioSources)
                    {
                        audioSource.Stop();
                        audioSource.time = 0;
                    }
                }
            });

        Observable
            .EveryUpdate()
            .Subscribe(_ =>
            {
                if (IsPlaying.CurrentValue && GetCurrentTime() >= (sampleSource?.clip?.length ?? 0) - 1)
                {
                    Debug.Log("Song ended");
                    IsPlaying.Value = false;
                    SongEnded.Value++;
                    SongEnded.ForceNotify();
                }
            });
    }

    public void SetClip(AudioClip clip, string tag, string fileName)
    {
        try
        {
            var source = audioSources.First(source => source.CompareTag(tag));
            if (source.clip != null)
                AudioClip.Destroy(source.clip);
            source.clip = clip;
            sampleSource = source;
            CurrentSongFile = fileName;
        }
        catch (InvalidOperationException e)
        {
            Debug.LogError("Audio source not found for tag: " + tag);
        }
    }

    public void PlayAudio()
    {
        IsPlaying.Value = true;
    }

    public void StopAudio()
    {
        IsPlaying.Value = false;
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

    public void SetMute(string tag, bool mute)
    {
        var source = audioSources.First(source => source.CompareTag(tag));
        source.mute = mute;
    }

    public void Skip(float seconds)
    {
        var source = audioSources[0];
        var targetTime = source.time + seconds;
        
        if (targetTime < 0)
        {
            targetTime = 0;
        }

        if (targetTime > source.clip.length - seconds)
        {
            targetTime = source.clip.length - seconds;
        }

        foreach (var audioSource in audioSources)
        {
            audioSource.time = targetTime;
        }
    }

    public float GetCurrentTime()
    {
        return sampleSource?.time ?? 0f;
    }

    private async UniTask PlayAudioAsync(CancellationToken ct)
    {
        
        foreach (var audioSource in audioSources)
        {
            while (audioSource.clip.loadState != AudioDataLoadState.Loaded)
            {
                await UniTask.Delay(100, cancellationToken: ct);
            }
        }

        foreach (var audioSource in audioSources)
        {
            Debug.Log("Playing audio" + audioSource.clip.name);
            audioSource.Play();
        }
    }
}