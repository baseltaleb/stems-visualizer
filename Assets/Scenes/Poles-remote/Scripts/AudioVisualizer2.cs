using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
public class AudioVisualizer2 : MonoBehaviour
{
    public AudioSource vocals;
    public AudioSource drums;
    public AudioSource bass;
    public AudioSource other;

    public float[,] spectrogramData; // Your 2D spectrogram data for one stem
    public int frequencyBinToVisualize = 40; // Choose which frequency bin to visualize
    public float scaleFactor = 10f; // Adjust this to change the scale of the visualization
    public float smoothing = 0.1f; // Smoothing factor for transitions

    private int currentTimeStep = 0;
    private float timePerStep = 0.01f; // 100 FPS
    private float timeSinceLastStep = 0f;

    private AudioAnalysis analysis;
    private bool readyToPlay = false;

    public void Awake()
    {
        analysis = FindObjectOfType<AudioAnalysis>();
    }

    public void Play() {
        vocals.Play();
        drums.Play();
        bass.Play();
        other.Play();
    }

    public void Stop() {
        vocals.Stop();
        drums.Stop();
        bass.Stop();
        other.Stop();
    }

    public void Mute(string clip) {
        if (clip == "vocals") {
            vocals.mute = !vocals.mute;
        } else if (clip == "drums") {
            drums.mute = !drums.mute;
        } else if (clip == "bass") {
            bass.mute = !bass.mute;
        } else if (clip == "other") {
            other.mute = !other.mute;
        }
    }

    public void PickFile()
    {
        if (NativeFilePicker.IsFilePickerBusy())
        {
            return;
        }

        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        {
            if (path == null)
            {
                Debug.Log("Operation cancelled");
                return;
            }
            Debug.Log("Picked file: " + path);
            StartAnalysis(path);
        }, new string[] { "*" });
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
            // Debug.Log("Other length: " + result.spectrogram.other.Length);
            Debug.Log("Number of segments: " + result.segments.Count);
            StartCoroutine(LoadAudio(result.stem_links["vocals.wav"], (audioClip) =>
            {
                vocals.clip = audioClip;
            }));
            StartCoroutine(LoadAudio(result.stem_links["drums.wav"], (audioClip) =>
            {
                drums.clip = audioClip;
            }));
            StartCoroutine(LoadAudio(result.stem_links["bass.wav"], (audioClip) =>
            {
                bass.clip = audioClip;
            }));
            StartCoroutine(LoadAudio(result.stem_links["other.wav"], (audioClip) =>
            {
                other.clip = audioClip;
            }));
            readyToPlay = true;
            // spectrogramData = ConvertToMultidimensionalArray(result.spectrogram.other);
            // timePerStep = 1 / result.spectrogram.fps;
            // spectrogramData = result.spectrogram.other;
        }));
    }

    private IEnumerator LoadAudio(string url, System.Action<AudioClip> downloadComplete)
    {
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(analysis.serverUrl + "/" + url, AudioType.WAV);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            // www.SendWebRequest();            
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
            downloadComplete(audioClip);
        }
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
