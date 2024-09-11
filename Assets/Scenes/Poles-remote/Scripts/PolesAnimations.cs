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
        Debug.Log("Sending trigger: " + label);
        segmentAnimator.SetTrigger(label);
    }

    void OnEnable() {
        SongEvents.OnSegmentEntered += OnSegmentEnter;
    }

    void OnDisable() {
        SongEvents.OnSegmentEntered -= OnSegmentEnter;
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            segmentAnimator.SetTrigger("intro");
        }
                if(Input.GetKeyDown(KeyCode.Alpha2)) {
            segmentAnimator.SetTrigger("solo");
        }
    }
}
