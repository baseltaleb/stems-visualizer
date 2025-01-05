using System;
using System.Collections;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEditor.Recorder;
using UnityEngine;
using UnityEditor.Recorder.Encoder;
using UnityEditor.Recorder.Input;

public class VideoRecorder : MonoBehaviour
{
    public bool landscape = false;
    public string outputPath = null;
    public bool autoStart = false;
    public bool autoStop = true;
    public int stopRecordingDelay = 3;

    private RecorderController recorderController;
    private MovieRecorderSettings recordSettings;

    public void Start()
    {
        AudioPlaybackController
            .IsPlaying
            .DistinctUntilChanged()
            .SubscribeAwait(async (isPlaying, ct) =>
            {
                Debug.Log($"IsPlaying: {isPlaying}");
                if (isPlaying && autoStart)
                {
                    StartRecording();
                }
                else if (!isPlaying && autoStop)
                {
                    await StopRecordingWithDelay(ct);
                }
            });

        var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
        recorderController = new RecorderController(controllerSettings);

        recordSettings = ScriptableObject.CreateInstance<MovieRecorderSettings>();
        recordSettings.name = "My Movie Recorder";
        recordSettings.Enabled = true;

        // Configure video settings
        var encoderSettings = new ProResEncoderSettings()
        {
            Format = ProResEncoderSettings.OutputFormat.ProRes422Proxy,
        };
        recordSettings.EncoderSettings = encoderSettings;
        recordSettings.CaptureAudio = true;

        recordSettings.ImageInputSettings = new GameViewInputSettings
        {
            OutputWidth = landscape ? 2160 : 3840,
            OutputHeight = landscape ? 3840 : 2160
        };

        controllerSettings.AddRecorderSettings(recordSettings);
        controllerSettings.SetRecordModeToManual();
        controllerSettings.FrameRate = 60;
    }

    public void ToggleRecording()
    {
        Debug.Log($"Toggle Recording");
        if (recorderController.IsRecording())
        {
            StopRecording();
        }
        else
        {
            StartRecording();
        }
    }

    public void StartRecording()
    {
        var file = AudioPlaylistController.CurrentFile.CurrentValue;
        DirectoryInfo mediaOutputFolder;
        if (outputPath == null)
        {
            mediaOutputFolder = new DirectoryInfo(Path.Combine(Application.dataPath, "..", "Recordings"));
        }
        else
        {
            mediaOutputFolder = new DirectoryInfo(outputPath);
        }

        var fileName = Path.GetFileNameWithoutExtension(file);
        var fullPath = Path.Combine(mediaOutputFolder.FullName, fileName);

        recordSettings.OutputFile = fullPath;

        recorderController.PrepareRecording();

        if (recorderController.StartRecording())
        {
            Debug.Log($"Started Recording {fileName}");
        }
        else
        {
            Debug.Log("Failed to start recording");
        }
    }

    public void StopRecording()
    {
        if (recorderController != null && recorderController.IsRecording())
        {
            recorderController.StopRecording();
            Debug.Log("Stopped Recording");
        }
    }

    async UniTask StopRecordingWithDelay(CancellationToken ct)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(stopRecordingDelay), cancellationToken: ct);
        StopRecording();
    }
}

// #if UNITY_EDITOR
//
// using System.ComponentModel;
// using System.IO;
// using UnityEditor;
// using UnityEditor.Recorder;
// using UnityEditor.Recorder.Encoder;
// using UnityEditor.Recorder.Input;
//
// namespace UnityEngine.Recorder.Examples
// {
//     /// <summary>
//     /// This example shows how to set up a recording session via script, for an MP4 file.
//     /// To use this example, add the MovieRecorderExample component to a GameObject.
//     ///
//     /// Enter the Play Mode to start the recording.
//     /// The recording automatically stops when you exit the Play Mode or when you disable the component.
//     ///
//     /// This script saves the recording outputs in [Project Folder]/SampleRecordings.
//     /// </summary>
//     public class MovieRecorderExample : MonoBehaviour
//     {
//         RecorderController m_RecorderController;
//         public bool m_RecordAudio = true;
//         internal MovieRecorderSettings m_Settings = null;
//
//         public FileInfo OutputFile
//         {
//             get
//             {
//                 var fileName = m_Settings.OutputFile + ".mp4";
//                 return new FileInfo(fileName);
//             }
//         }
//
//         void OnEnable()
//         {
//             Initialize();
//         }
//
//         internal void Initialize()
//         {
//             var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
//             m_RecorderController = new RecorderController(controllerSettings);
//
//             var mediaOutputFolder = new DirectoryInfo(Path.Combine(Application.dataPath, "..", "SampleRecordings"));
//
//             // Video
//             m_Settings = ScriptableObject.CreateInstance<MovieRecorderSettings>();
//             m_Settings.name = "My Video Recorder";
//             m_Settings.Enabled = true;
//
//             // This example performs an MP4 recording
//             m_Settings.EncoderSettings = new CoreEncoderSettings
//             {
//                 EncodingQuality = CoreEncoderSettings.VideoEncodingQuality.High,
//                 Codec = CoreEncoderSettings.OutputCodec.MP4
//             };
//             m_Settings.CaptureAlpha = true;
//
//             m_Settings.ImageInputSettings = new GameViewInputSettings
//             {
//                 OutputWidth = 1920,
//                 OutputHeight = 1080
//             };
//
//             // Simple file name (no wildcards) so that FileInfo constructor works in OutputFile getter.
//             m_Settings.OutputFile = mediaOutputFolder.FullName + "/" + "video";
//
//             // Setup Recording
//             controllerSettings.AddRecorderSettings(m_Settings);
//             controllerSettings.SetRecordModeToManual();
//             controllerSettings.FrameRate = 60.0f;
//
//             RecorderOptions.VerboseMode = false;
//         }
//
//         public void StartRecording()
//         {
//             m_RecorderController.PrepareRecording();
//             m_RecorderController.StartRecording();
//             Debug.Log($"Started recording for file {OutputFile.FullName}");
//         }
//
//         public void StopRecording()
//         {
//             m_RecorderController.StopRecording();
//             Debug.Log("Stopped Recording");
//         }
//
//         public void ToggleRecording()
//         {
//             if (m_RecorderController.IsRecording())
//             {
//                 StopRecording();
//             }
//             else
//             {
//                 StartRecording();
//             }
//         }
//         void OnDisable()
//         {
//             m_RecorderController.StopRecording();
//         }
//     }
// }
//
// #endif