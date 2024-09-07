using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class CurrentSegmentChecker : MonoBehaviour
{
    public AudioSource audioSource;

    private AnalysisResult currentResult;
    private Segment lastSegment;
    private float timer = 0f;
    private float timerCheckInterval = 0.5f;
    public void SetCurrentResult(AnalysisResult result)
    {
        currentResult = result;
    }

    private void CheckSegementChanged(float seconds)
    {
        Segment segementAtSeconds = currentResult?.segments.Find(s =>
            s.start <= seconds && s.end >= seconds
        );

        if (segementAtSeconds != null && segementAtSeconds != lastSegment)
        {
            Debug.Log("Segment changed from " + lastSegment?.label + " to " + segementAtSeconds?.label);
            lastSegment = segementAtSeconds;
            SongEvents.TriggerSegementEnter(segementAtSeconds.label);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timerCheckInterval)
        {
            CheckSegementChanged(audioSource.time + timerCheckInterval);
            timer = 0f;
        }
    }

    private void OnCurrentSongChanged(AnalysisResult result) {
        currentResult = result;
    }

    void OnEnable() {
        SongEvents.OnCurrentSongChanged += OnCurrentSongChanged;
    }

    void OnDisable() {
        SongEvents.OnCurrentSongChanged -= OnCurrentSongChanged;
    }
}
