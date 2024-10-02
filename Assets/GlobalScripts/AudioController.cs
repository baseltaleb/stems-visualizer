using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
public class AudioController : MonoBehaviour
{
    public AudioSource vocals;
    public AudioSource drums;
    public AudioSource bass;
    public AudioSource other;

    public float[,] spectrogramData; // Your 2D spectrogram data for one stem

    private AudioAnalysis analysis;
    private AnalysisResult currentAnalysisResult;

    public void Awake()
    {
        analysis = FindFirstObjectByType<AudioAnalysis>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PlaybackSkip(-10);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            PlaybackSkip(10);
        }
    }

    void OnEnable() {
        FilePicker.OnFilesPickedEvent += OnFilesPicked;
    }

    void OnDisable() {
        FilePicker.OnFilesPickedEvent -= OnFilesPicked;
    }

    public void PlayAudio()
    {
        vocals.Play();
        drums.Play();
        bass.Play();
        other.Play();
    }

    public void StopAudio()
    {
        Debug.Log("CLICKY");
        vocals.Stop();
        drums.Stop();
        bass.Stop();
        other.Stop();
    }

    public void Mute(string clip)
    {
        if (clip == "vocals")
        {
            vocals.mute = !vocals.mute;
        }
        else if (clip == "drums")
        {
            drums.mute = !drums.mute;
        }
        else if (clip == "bass")
        {
            bass.mute = !bass.mute;
        }
        else if (clip == "other")
        {
            other.mute = !other.mute;
        }
    }

    public void PlaybackSkip(float seconds)
    {
        if (vocals.time + seconds < 0)
        {
            vocals.time = 0;
            drums.time = 0;
            bass.time = 0;
            other.time = 0;
            return;
        }
        if (vocals.time + seconds > vocals.clip.length)
        {
            vocals.time = vocals.clip.length;
            drums.time = drums.clip.length;
            bass.time = bass.clip.length;
            other.time = other.clip.length;
            return;
        }
        vocals.time += seconds;
        drums.time += seconds;
        bass.time += seconds;
        other.time += seconds;
    }

    private void OnFilesPicked(string[] paths) {
        if (paths.Length == 0) {
            Debug.Log("No files picked");
            return;
        }
        Debug.Log("Picked file: " + paths[0]);
        StartAnalysis(paths[0]);
    }

    // Call this method to start the analysis
    public void StartAnalysis(string filePath)
    {
        Debug.Log("Starting analysis...");

        var result = StartCoroutine(analysis.AnalyzeAudio(filePath, (result) =>
        {
            Debug.Log("Analysis finished");
            Debug.Log("Spectrogram data received");
            Debug.Log("Tempo: " + result.tempo);
            currentAnalysisResult = result;

            // Debug.Log("Other length: " + result.spectrogram.other.Length);
            Debug.Log("Number of segments: " + result.segments.Count);
            StartCoroutine(analysis.LoadAudio(result.session_id, "vocals", (audioClip) =>
            {
                vocals.clip = audioClip;
            }));
            StartCoroutine(analysis.LoadAudio(result.session_id, "drums", (audioClip) =>
            {
                drums.clip = audioClip;
            }));
            StartCoroutine(analysis.LoadAudio(result.session_id, "bass", (audioClip) =>
            {
                bass.clip = audioClip;
            }));
            StartCoroutine(analysis.LoadAudio(result.session_id, "other", (audioClip) =>
            {
                other.clip = audioClip;
            }));
            vocals.time = 0;
            drums.time = 0;
            bass.time = 0;
            other.time = 0;
            
            SongEvents.TriggerCurrentSongChange(result);
            // spectrogramData = ConvertToMultidimensionalArray(result.spectrogram.other);
            // timePerStep = 1 / result.spectrogram.fps;
            // spectrogramData = result.spectrogram.other;
        }));
    }

    private float[,] ConvertToMultidimensionalArray(List<List<float>> nestedList)
    {
        if (nestedList == null || nestedList.Count == 0)
            return new float[0, 0];

        int rows = nestedList.Count;
        int cols = nestedList[0].Count;

        float[,] result = new float[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = nestedList[i][j];
            }
        }

        return result;
    }
}
