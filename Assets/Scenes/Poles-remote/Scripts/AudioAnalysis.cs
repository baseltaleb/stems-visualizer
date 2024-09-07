
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

class AudioAnalysis : MonoBehaviour
{
    public string serverUrl = "http://localhost:5000";

    public IEnumerator AnalyzeAudio(string filePath, System.Action<AnalysisResult> onAnalysisComplete)
    {
        Debug.Log("Starting request for file " + filePath);
        WWWForm form = new WWWForm();

        // Add audio file
        byte[] audioData = File.ReadAllBytes(filePath);
        form.AddBinaryData("audio", audioData, Path.GetFileName(filePath), "audio/mp3");

        UnityWebRequest www = UnityWebRequest.Post(serverUrl + "/segment", form);
        www.SetRequestHeader("Accept", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            Debug.Log("Response: success");
            string jsonResult = www.downloadHandler.text;
            AnalysisResult result = JsonConvert.DeserializeObject<AnalysisResult>(jsonResult);

            // Now you can use the 'result' object which contains all the analyzed data
            Debug.Log("Tempo: " + result.tempo);
            Debug.Log("Number of segments: " + result.segments.Count);
            
            // Access other properties as needed
            yield return null;
            onAnalysisComplete(result);
        }
    }
}