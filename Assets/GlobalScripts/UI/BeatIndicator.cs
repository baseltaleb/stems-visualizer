using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BeatIndicator : MonoBehaviour
{
    public AudioSource audioSource;
    public UnityEngine.UI.Image element;
    public float duration;
    
    private float timer = 0f;
    private AnalysisResult currentResult;

    void Awake()
    {
        element = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!audioSource.clip) return;
        
        if (IsBeatActive(audioSource.time))
        {
            element.CrossFadeAlpha(1f, 0.1f, true);
            // element.enabled = true;
        }
        else
        {
            element.CrossFadeAlpha(0f, 0.1f, true);
            // element.enabled = false;
        }
    }

    private bool IsBeatActive(float seconds)
    {
        var beats = currentResult.beats;
        return beats.Any(beat => beat >= seconds && beat <= seconds + duration);
    }
    private void OnCurrentSongChanged(AnalysisResult result)
    {
        currentResult = result;
    }

    void OnEnable()
    {
        SongEvents.OnCurrentSongChanged += OnCurrentSongChanged;
    }

    void OnDisable()
    {
        SongEvents.OnCurrentSongChanged -= OnCurrentSongChanged;
    }
}
