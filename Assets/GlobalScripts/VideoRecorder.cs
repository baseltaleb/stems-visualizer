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
    private RecorderController recorderController;
    private MovieRecorderSettings recordSettings;

    public void Start()
    {
        AudioPlaybackController
            .SongEnded
            .DistinctUntilChanged()
            .SubscribeAwait(async (_, ct) =>
            {
                await StopRecordingWithDelay(ct);
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
            // Codec = CoreEncoderSettings.OutputCodec.MP4,
            // EncodingProfile = CoreEncoderSettings.H264EncodingProfile.High,
            // EncodingQuality = CoreEncoderSettings.VideoEncodingQuality.High,
            // TargetBitRate = 32
        };
        recordSettings.EncoderSettings = encoderSettings;
        recordSettings.CaptureAudio = true;

        // recordSettings.OutputFormat = MovieRecorderSettings.VideoRecorderOutputFormat.MP4;
        // recordSettings.VideoBitRateMode = VideoBitrateMode.High;
        recordSettings.ImageInputSettings = new GameViewInputSettings
        {
            OutputWidth = 3840,
            OutputHeight = 2160
        };

        var mediaOutputFolder = new DirectoryInfo(Path.Combine(Application.dataPath, "..", "Recordings"));
        // Set output file
        recordSettings.OutputFile =
            Path.Combine(mediaOutputFolder.FullName, $"video_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}");

        controllerSettings.AddRecorderSettings(recordSettings);
        controllerSettings.SetRecordModeToManual();
        controllerSettings.FrameRate = 60;
    }

    public void ToggleRecording()
    {
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
        recorderController.PrepareRecording();
        recorderController.StartRecording();
        Debug.Log("Started Recording");
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
        await UniTask.Delay(3000, cancellationToken: ct);
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