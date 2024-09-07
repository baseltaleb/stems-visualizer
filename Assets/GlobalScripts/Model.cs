
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
    public List<Segment> segments;
    // public Spectrogram spectrogram;
    public float tempo;
    public Dictionary<string, string> stem_links;
}

public static class SegmentLabels {
    public static string INTRO = "intro";
    public static string OUTRO = "outro";
    public static string BREAK = "break";
    public static string BRIDGE = "bridge";
    public static string INST = "inst";
    public static string SOLO = "solos";
    public static string VERSE = "verse";
    public static string CHORUS = "chorus";
    public static string START = "start";
    public static string END = "end";
}