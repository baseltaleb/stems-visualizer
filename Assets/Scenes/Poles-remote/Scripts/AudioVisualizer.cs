using System;
using System.Collections.Generic;
using UnityEngine;
public class AudioVisualizer : MonoBehaviour
{
    public float[,] spectrogramData; // Your 2D spectrogram data for one stem
    public int frequencyBinToVisualize = 40; // Choose which frequency bin to visualize
    public float scaleFactor = 10f; // Adjust this to change the scale of the visualization
    public float smoothing = 0.1f; // Smoothing factor for transitions

    private int currentTimeStep = 0;
    private float timePerStep = 0.01f; // 100 FPS
    private float timeSinceLastStep = 0f;

    private void Update()
    {
        if (spectrogramData == null || spectrogramData.GetLength(1) <= frequencyBinToVisualize)
        {
            // Debug.LogError("Spectrogram data is null or frequency bin is out of range");
            return;
        }
        timeSinceLastStep += Time.deltaTime;

        if (timeSinceLastStep >= timePerStep)
        {
            UpdateVisualization();
            timeSinceLastStep -= timePerStep;
            currentTimeStep = (currentTimeStep + 1) % spectrogramData.GetLength(0);
        }
    }

    private void UpdateVisualization()
    {
        float amplitude = spectrogramData[currentTimeStep, frequencyBinToVisualize];

        // Calculate target scale
        Vector3 targetScale = transform.localScale;
        targetScale.y = amplitude * scaleFactor;

        // Apply smoothing
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, smoothing);
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
        }, new string[] { "audio/*" });
    }
    // Call this method to start the analysis
    public void StartAnalysis(string filePath)
    {
        Debug.Log("Starting analysis...");
        var analysis = FindObjectOfType<AudioAnalysis>();
        var result = StartCoroutine(analysis.AnalyzeAudio(filePath, (result) =>
        {
            Debug.Log("Analysis finished");
            Debug.Log("Spectrogram data received");
            Debug.Log("Tempo: " + result.tempo);
            // Debug.Log("Other length: " + result.spectrogram.other.Length);
            // Debug.Log("Number of segments: " + result.segments.Count);
            
            // spectrogramData = ConvertToMultidimensionalArray(result.spectrogram.other);
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
