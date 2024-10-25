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
            true,
            (string[] paths) =>
            {
                OnFilesPickedEvent?.Invoke(paths);
            }
        );
    }
}


