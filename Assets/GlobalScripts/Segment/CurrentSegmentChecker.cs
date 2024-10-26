using UnityEngine;

public class CurrentSegmentChecker : MonoBehaviour
{
    public AudioSource audioSource;
    public bool loggingEnabled;

    private AnalysisResult currentResult;
    private Segment lastSegment;
    private float timer = 0f;
    private float timerCheckInterval = 0.5f;

    public void SetCurrentResult(AnalysisResult result)
    {
        currentResult = result;
    }

    private void CheckSegementChanged(float seconds)
    {
        Segment segementAtSeconds = currentResult?.segments.Find(s =>
            s.start <= seconds && s.end >= seconds
        );

        if (segementAtSeconds != null && segementAtSeconds.label != lastSegment?.label)
        {
            if (loggingEnabled)
                Debug.Log("Segment changed from " + lastSegment?.label + " to " + segementAtSeconds?.label);
            lastSegment = segementAtSeconds;
            SongEvents.TriggerSegementEnter(segementAtSeconds.label);
        }
    }

    void Update()
    {
        if (audioSource.clip == null)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= timerCheckInterval)
        {
            CheckSegementChanged(audioSource.time + 1f);
            timer = 0f;
        }
    }

    private void OnCurrentSongChanged(AnalysisResult result)
    {
        currentResult = result;
        lastSegment = null;
    }

    void OnEnable()
    {
        SongEvents.OnCurrentSongChanged += OnCurrentSongChanged;
    }

    void OnDisable()
    {
        SongEvents.OnCurrentSongChanged -= OnCurrentSongChanged;
    }
}