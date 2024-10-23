using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongEvents : MonoBehaviour
{
    public delegate void OnSegmentEnter(string label);
    public static event OnSegmentEnter OnSegmentEntered;

    public delegate void OnCurrentSongChange(AnalysisResult result);
    public static event OnCurrentSongChange OnCurrentSongChanged;

    public static void TriggerSegementEnter(string label) {
        OnSegmentEntered?.Invoke(label);
    }

    public static void TriggerCurrentSongChange(AnalysisResult result) {
        OnCurrentSongChanged?.Invoke(result);
    }

}
