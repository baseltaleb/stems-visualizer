
using System.Collections.Generic;

[System.Serializable]
public class Metadata
{
    public string name;
    public int priority;
}

[System.Serializable]
public class Segment
{
    public float start;
    public float end;
    public string label;
}

[System.Serializable]
public class Spectrogram
{
    // public List<List<float>> bass;
    // public List<List<float>> drums;
    // public List<List<float>> other;
    // public List<List<float>> vocals;
    public float[,] bass;
    public float[,] drums;
    public float[,] other;
    public float[,] vocals;
    public int fps;
}

[System.Serializable]
public class AnalysisResult
{
    public string session_id;
    public List<Segment> segments;
    // public Spectrogram spectrogram;
    public float tempo;
    public string mainFilePath = null;
}

public static class SegmentLabels {
    public const string INTRO = "intro";
    public const string OUTRO = "outro";
    public const string BREAK = "break";
    public const string BRIDGE = "bridge";
    public const string INST = "inst";
    public const string SOLO = "solo";
    public const string VERSE = "verse";
    public const string CHORUS = "chorus";
    public const string START = "start";
    public const string END = "end";
}

public static class StemNames
{
    public const string MAIN = "main";
    public const string BASS = "bass";
    public const string DRUMS = "drums";
    public const string OTHER = "other";
    public const string VOCALS = "vocals";

    public static string GetTag(string stemName)
    {
        return $"stem-{stemName}";
    }
}
