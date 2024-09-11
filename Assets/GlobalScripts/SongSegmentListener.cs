using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongSegmentListener : MonoBehaviour
{
    public string label;
    public string segmentLabel
    {
        get
        {
            return label;
        }
        set
        {
            label = value;
        }
    }
    // Start is called before the first frame update
    void OnSegmentEnter(string label) {
        Debug.Log("Segment: " + label);
        if (label == segmentLabel) {
            Debug.Log("Segment: " + label);
        }
    }

    void OnEnable() {
        SongEvents.OnSegmentEntered += OnSegmentEnter;
    }

    void OnDisable() {
        SongEvents.OnSegmentEntered -= OnSegmentEnter;
    }
}
