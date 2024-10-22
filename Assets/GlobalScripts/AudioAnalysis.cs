using System;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;

class AudioAnalysis : MonoBehaviour
{
    public string serverUrl = "http://localhost:5000";

    private const string AudioExtension = ".mp3";
    private CancellationTokenSource fileDownloadCts;

    public async UniTask<AnalysisResult> AnalyzeAudioAsync(string filePath)
    {
        Debug.Log("Starting request for file " + filePath);
        WWWForm form = new WWWForm();

        // Add audio file
        byte[] audioData = await File.ReadAllBytesAsync(filePath);
        form.AddBinaryData("audio", audioData, Path.GetFileName(filePath), "audio/mp3");

        UnityWebRequest www = UnityWebRequest.Post(serverUrl + "/segment", form);
        www.SetRequestHeader("Accept", "application/json");

        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Audio analysis error: " + www.error);
            return null;
        }

        string jsonResult = www.downloadHandler.text;
        AnalysisResult result = JsonConvert.DeserializeObject<AnalysisResult>(jsonResult);

        return result;
    }
    public void ClearFiles()
    {
        // Look for 
        
    }
    public async UniTask<AudioClip> LoadAudioAsync(string sessionId, string stemName, CancellationToken ct)
    {
        var fileName = stemName + AudioExtension;
        var cacheFilePath = FileCacheManager.GetFileCachePath(sessionId, fileName);

        if (!FileCacheManager.IsCached(cacheFilePath))
        {
            fileDownloadCts?.Cancel();
            fileDownloadCts = new CancellationTokenSource();
            await DownloadStemFileAsync(
                sessionId: sessionId,
                fileName: fileName,
                storagePath: cacheFilePath,
                fileDownloadCts.Token
            );
        }
        
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(cacheFilePath, AudioType.MPEG);
        await www.SendWebRequest().WithCancellation(ct);
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            return null;
        }

        AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
        return audioClip;
    }

    private async UniTask<string> DownloadStemFileAsync(
        string sessionId,
        string fileName,
        string storagePath,
        CancellationToken ct
    )
    {
        var url = serverUrl + "/" + "file/" + sessionId + "/" + fileName;
        var uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
        uwr.downloadHandler = new DownloadHandlerFile(storagePath);
        await uwr.SendWebRequest().WithCancellation(ct);

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(uwr.error);
            throw new InvalidOperationException("Download Failed");
        }

        Debug.Log("File successfully downloaded and saved to " + storagePath);
        return storagePath;
    }
}