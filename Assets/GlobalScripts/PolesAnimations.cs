using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolesAnimations : MonoBehaviour
{
    public Animator segmentAnimator;

    void Awake() {
        segmentAnimator = GetComponent<Animator>();
    }

    void OnSegmentEnter(string label) {
        segmentAnimator.SetTrigger(label);
    }

    void OnEnable() {
        SongEvents.OnSegmentEntered += OnSegmentEnter;
    }

    void OnDisable() {
        SongEvents.OnSegmentEntered -= OnSegmentEnter;
    }
}
