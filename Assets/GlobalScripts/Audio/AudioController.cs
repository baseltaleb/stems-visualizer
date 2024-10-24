using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public AudioMixerSnapshot mainEnabledSnapshot;
    public AudioMixerSnapshot mainDisabledSnapshot;

    public AudioSource main;
    public AudioSource vocals;
    public AudioSource drums;
    public AudioSource bass;
    public AudioSource other;

    public ReactiveProperty<AnalysisResult> CurrentAnalysisResult { get; private set; }

    private AudioMixerSnapshot activeSnapshot;
    private readonly AudioPlaybackController playbackController = new();
    private readonly AudioPlaylistController playlistController = new();
    private readonly AudioAnalysisApi analysisApi = new();

    private CancellationTokenSource analysisCancellation;
    private bool isMainTrackAvailable = false;

    public void Awake()
    {
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

        playbackController.SetAudioSources(new[] { vocals, drums, bass, other, main });

        activeSnapshot = mainEnabledSnapshot;
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

        if (isMainTrackAvailable)
        {
            var sources = new[] { vocals, drums, bass, other };
            if (activeSnapshot == mainEnabledSnapshot)
            {
                activeSnapshot = mainDisabledSnapshot;
            }

            // If none of the sources are muted, mute all sources and unmute main
            if (sources.All(source => !source.mute))
            {
                activeSnapshot = mainEnabledSnapshot;
            }

            activeSnapshot.TransitionTo(0.0f);
        }
    }

    private void OnFilesPicked(string[] paths)
    {
        if (paths.Length == 0)
        {
            Debug.Log("No files picked");
            return;
        }

        Debug.Log("Picked file: " + paths[0]);
        playlistController.SetFiles(paths);
        StartAnalysis(paths[0]); // TODO: move to AAC
    }

    private void StartAnalysis(string filePath)
    {
        StartAnalysisAsync(filePath).Forget();
    }

    private async UniTaskVoid StartAnalysisAsync(string filePath)
    {
        analysisCancellation?.Cancel();

        Debug.Log("Starting analysis...");

        var analysisResult = await analysisApi.AnalyzeAudioAsync(filePath);
        analysisResult.mainFilePath = filePath;

        Debug.Log(
            $"Analysis result: Tempo: {analysisResult.tempo}, Number of segments: {analysisResult.segments.Count}"
        );

        CurrentAnalysisResult.Value = analysisResult;
    }

    private async UniTask HandleAudio(AnalysisResult analysisResult, CancellationToken ct)
    {
        StopAudio();

        foreach (var stem in new[] { StemNames.VOCALS, StemNames.DRUMS, StemNames.BASS, StemNames.OTHER })
        {
            var filePath = await analysisApi.GetCachedFilePath(sessionId: analysisResult.session_id, stem, ct);
            var audioClip = await analysisApi.GetAudioClip(filePath, ct);

            if (audioClip != null)
            {
                playbackController.SetClip(audioClip, StemNames.GetTag(stem));
            }
            else
            {
                Debug.LogWarning($"Could not find audio clip: {filePath}");
            }
        }

        try
        {
            var mainClip = await analysisApi.GetAudioClip(analysisResult.mainFilePath, ct);
            playbackController.SetClip(mainClip, StemNames.GetTag(StemNames.MAIN));
            isMainTrackAvailable = true;
        }
        catch (Exception ex)
        {
            isMainTrackAvailable = false;
            activeSnapshot = mainDisabledSnapshot;
            activeSnapshot.TransitionTo(0.0f);
            Debug.LogError($"Error getting main clip: {ex.Message}");
        }
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