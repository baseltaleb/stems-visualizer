using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

class AudioAnalysisController
{
    public readonly ReactiveProperty<List<AnalysisResult>> AnalyzedFiles = new(new List<AnalysisResult>());
    public readonly ReactiveProperty<AnalysisProgress> CurrentAnalysisProgress = new();

    private readonly AudioAnalysisApi analysisApi;
    private readonly ReactiveProperty<List<string>> analysisQueue = new(new List<string>());
    private CancellationTokenSource analysisQueueCancellation;
    public AudioAnalysisController(AudioAnalysisApi analysisApi)
    {
        this.analysisApi = analysisApi;
        analysisQueue
            .WhereNotNull()
            .DistinctUntilChanged()
            .SubscribeAwait(async (queue, ct) =>
            {
                analysisQueueCancellation?.Cancel();
                analysisQueueCancellation = new CancellationTokenSource();
                analysisQueueCancellation.AddTo(ct);

                foreach (var file in queue)
                {
                    var result = await StartAnalysisAsync(file, analysisQueueCancellation.Token);
                    var resultList = AnalyzedFiles.CurrentValue;
                    resultList.Add(result);
                    AnalyzedFiles.Value = resultList;
                    AnalyzedFiles.ForceNotify();
                }
            });
    }

    public void SetQueue(string[] files)
    {
        analysisQueue.Value = new List<string>(files);
    }

    private async UniTask<AnalysisResult> StartAnalysisAsync(string filePath, CancellationToken ct)
    {
        Debug.Log("Starting analysis for file " + filePath.GetFileName());
    
        var progress = new AnalysisProgress(0f, filePath);
        CurrentAnalysisProgress.Value = progress;

        var analysisResult = await analysisApi.AnalyzeAudioAsync(filePath, ct);
        analysisResult.mainFilePath = filePath;
        progress.IsFinished = true;
        CurrentAnalysisProgress.Value = progress;

        Debug.Log(
            $"Analysis result: File: {filePath.GetFileName()} Tempo: {analysisResult.tempo}, Number of segments: {analysisResult.segments.Count}"
        );
        SanitizeAnalysisResult(analysisResult);
        return analysisResult;
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
}

public struct AnalysisProgress
{
    public float Progress { get; }
    public string FileName { get; }
    public bool IsFinished { get; set; }

    public AnalysisProgress(float progress, string fileName)
    {
        Progress = progress;
        FileName = fileName;
        IsFinished = false;
    }
}