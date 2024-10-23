using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework.Internal;
using UnityEngine;
using R3;

public class AudioController : MonoBehaviour
{
    public AudioSource vocals;
    public AudioSource drums;
    public AudioSource bass;
    public AudioSource other;
    public ReactiveProperty<AnalysisResult> CurrentAnalysisResult { get; private set; }

    public float[,] spectrogramData; // Your 2D spectrogram data for one stem

    private AudioAnalysis analysis;
    private CancellationTokenSource analysisCancellation;

    public void Awake()
    {
        analysis = FindFirstObjectByType<AudioAnalysis>();
        CurrentAnalysisResult = new ReactiveProperty<AnalysisResult>();
    }

    void Start()
    {
        CurrentAnalysisResult
            .SubscribeAwait(async (analysisResult, ct) =>
            {
                if (analysisResult != null)
                {
                    SanitizeAnalysisResult(analysisResult);
                    await HandleAudio(analysisResult, ct);
                    SongEvents.TriggerCurrentSongChange(analysisResult);
                }
            });
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

    void OnEnable()
    {
        FilePicker.OnFilesPickedEvent += OnFilesPicked;
    }

    void OnDisable()
    {
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
        AudioSource[] sources = { vocals, drums, bass, other };
        foreach (var audioSource in sources)
        {
            audioSource.Stop();
            audioSource.time = 0;
        }
    }

    public void Mute(string clip)
    {
        switch (clip)
        {
            case "vocals":
                vocals.mute = !vocals.mute;
                break;
            case "drums":
                drums.mute = !drums.mute;
                break;
            case "bass":
                bass.mute = !bass.mute;
                break;
            case "other":
                other.mute = !other.mute;
                break;
        }
    }

    private void PlaybackSkip(float seconds)
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

    private void OnFilesPicked(string[] paths)
    {
        if (paths.Length == 0)
        {
            Debug.Log("No files picked");
            return;
        }

        Debug.Log("Picked file: " + paths[0]);
        StartAnalysis(paths[0]);
    }

    private void StartAnalysis(string filePath)
    {
        StartAnalysisAsync(filePath).Forget();
    }

    private async UniTaskVoid StartAnalysisAsync(string filePath)
    {
        analysisCancellation?.Cancel();

        Debug.Log("Starting analysis...");

        var analysisResult = await analysis.AnalyzeAudioAsync(filePath);

        Debug.Log(
            $"Analysis result: Tempo: {analysisResult.tempo}, Number of segments: {analysisResult.segments.Count}"
        );

        CurrentAnalysisResult.Value = analysisResult;

        // spectrogramData = ConvertToMultidimensionalArray(result.spectrogram.other);
        // timePerStep = 1 / result.spectrogram.fps;
        // spectrogramData = result.spectrogram.other;
    }

    private async UniTask HandleAudio(AnalysisResult analysisResult, CancellationToken ct)
    {
        StopAudio();
        var vocalsClip = await analysis.LoadAudioAsync(analysisResult.session_id, "vocals", ct);
        var drumClip = await analysis.LoadAudioAsync(analysisResult.session_id, "drums", ct);
        var bassClip = await analysis.LoadAudioAsync(analysisResult.session_id, "bass", ct);
        var otherClip = await analysis.LoadAudioAsync(analysisResult.session_id, "other", ct);

        vocals.clip = vocalsClip;
        drums.clip = drumClip;
        bass.clip = bassClip;
        other.clip = otherClip;
    }

    private void SanitizeAnalysisResult(AnalysisResult analysisResult1)
    {
        // Replace inst with solo if solo is not present
        if (analysisResult1.segments.All(segment => segment.label != SegmentLabels.SOLO))
        {
            foreach (var segment in analysisResult1.segments.Where(segment => segment.label == SegmentLabels.INST))
            {
                segment.label = SegmentLabels.SOLO;
            }
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