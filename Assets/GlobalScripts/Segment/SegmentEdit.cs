using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

public class SegmentEdit : MonoBehaviour
{
    public AnalysisResult currentResult;
    
    private readonly AudioAnalysisApi analysisApi = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioController
            .CurrentAnalysisResult
            .WhereNotNull()
            .DistinctUntilChanged()
            .Subscribe(analysisResult =>
            {
                currentResult = analysisResult;
            });
    }

    public void UpdateResult(AnalysisResult result)
    {
        var resultLabels = string.Join(", ", currentResult.segments.Select(s => s.label));
        Debug.Log($"Updating: {resultLabels}");
        analysisApi.UpdateResult(result).Forget();
    }
}
