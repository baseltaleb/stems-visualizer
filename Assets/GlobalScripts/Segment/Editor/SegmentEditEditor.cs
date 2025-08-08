using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;

[CustomEditor(typeof(SegmentEdit))]
public class SegmentEditEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SegmentEdit segmentEdit = (SegmentEdit)target;
        DrawDefaultInspector();
        
        if (GUILayout.Button("Update Segments"))
        {
            AnalysisResult analysisResult = segmentEdit.currentResult;
            segmentEdit.UpdateResult(analysisResult);
        }
    }
}