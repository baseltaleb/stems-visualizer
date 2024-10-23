using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework.Internal;
using UnityEngine;
using R3;

public class AudioController : MonoBehaviour
{
    public bool UseMainAudioTrack = false;

    public AudioSource main;
    public AudioSource vocals;
    public AudioSource drums;
    public AudioSource bass;
    public AudioSource other;

    public ReactiveProperty<AnalysisResult> CurrentAnalysisResult { get; private set; }

    private AudioPlaybackController playbackController = new();
    private AudioAnalysis analysis;
    private CancellationTokenSource analysisCancellation;

    public void Awake()
    {
        analysis = FindFirstObjectByType<AudioAnalysis>();
        CurrentAnalysisResult = new ReactiveProperty<AnalysisResult>();
        main.tag = StemNames.GetTag(StemNames.MAIN);
        vocals.tag = StemNames.GetTag(StemNames.VOCALS);
        drums.tag = StemNames.GetTag(StemNames.DRUMS);
        bass.tag = StemNames.GetTag(StemNames.BASS);
        other.tag = StemNames.GetTag(StemNames.OTHER);
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

        Observable
            .EveryValueChanged(this, value => value.UseMainAudioTrack)
            .Subscribe(value =>
                {
                    var sources = new[] { vocals, drums, bass, other };
                    if (value)
                        sources[^1] = main;

                    playbackController.SetAudioSources(sources);
                }
            );
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            playbackController.Skip(-10);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            playbackController.Skip(10);
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
        playbackController.PlayAudio();
    }

    public void StopAudio()
    {
        playbackController.StopAudio();
    }

    public void Mute(string stemName)
    {
        playbackController.ToggleMute(StemNames.GetTag(stemName));
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
    }

    private async UniTask HandleAudio(AnalysisResult analysisResult, CancellationToken ct)
    {
        StopAudio();

        var vocalsClip = await analysis.LoadAudioAsync(analysisResult.session_id, StemNames.VOCALS, ct);
        var drumClip = await analysis.LoadAudioAsync(analysisResult.session_id, StemNames.DRUMS, ct);
        var bassClip = await analysis.LoadAudioAsync(analysisResult.session_id, StemNames.BASS, ct);
        var otherClip = await analysis.LoadAudioAsync(analysisResult.session_id, StemNames.OTHER, ct);

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