using TMPro;
using UnityEngine;
using R3;

public class AudioPlaybackProgress : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public AudioSource audioSource;
    public bool hideIfRecording = true;

    private string currentSegment;
    
    void Start()
    {
        VideoRecorder
            .IsRecording
            .Subscribe(isRecording =>
            {
                if (isRecording && hideIfRecording)
                {
                    timeText.gameObject.SetActive(false);
                }
                else
                {
                    timeText.gameObject.SetActive(true);
                }
            });
    }

    void Update()
    {
        if (audioSource.clip)
        {
            timeText.text = $"{currentSegment ?? "None"} {audioSource.time:F2}";
        }
    }
    
    void OnSegmentEnter(string label) {
        currentSegment = label;
    }

    void OnEnable() {
        SongEvents.OnSegmentEntered += OnSegmentEnter;
    }

    void OnDisable() {
        SongEvents.OnSegmentEntered -= OnSegmentEnter;
    }
}