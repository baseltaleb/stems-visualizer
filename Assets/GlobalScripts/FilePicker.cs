// using UnityEngine;
// using UnityEngine.UI;
// using System.Windows.Input;
// using System.IO;

// public class FilePicker : MonoBehaviour
// {
//     public Button pickButton;
//     public AudioAnlysis audioAnalyzer;

//     void Start()
//     {
//         pickButton.onClick.AddListener(PickFile);
//     }

//     void PickFile()
//     {
//         FileDialog
//         var openFileDialog = new OpenFileDialog
//         {
//             Filter = "Audio Files (*.mp3;*.wav)|*.mp3;*.wav|All files (*.*)|*.*",
//             FilterIndex = 1,
//             RestoreDirectory = true
//         };

//         if (openFileDialog.ShowDialog() == DialogResult.OK)
//         {
//             string filePath = openFileDialog.FileName;
//             Debug.Log("Selected file: " + filePath);
            
//             // Start the analysis with the selected file
//             audioAnalyzer.StartAnalysis(filePath);
//         }
//     }
// }


