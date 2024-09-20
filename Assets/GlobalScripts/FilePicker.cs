using UnityEngine;
using SFB;

public class FilePicker : MonoBehaviour
{
    public delegate void OnFilesPicked(string[] paths);
    public static event OnFilesPicked OnFilesPickedEvent;

    public void PickFile()
    {
        StandaloneFileBrowser.OpenFilePanelAsync(
            "Open File",
            "",
            "",
            false,
            (string[] paths) =>
            {
                OnFilesPickedEvent?.Invoke(paths);
            }
        );
    }
}


