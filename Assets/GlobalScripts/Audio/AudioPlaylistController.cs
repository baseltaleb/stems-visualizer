using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;

public class AudioPlaylistController
{
    public readonly ReactiveProperty<List<string>> CurrentPlaylist = new();
    public readonly ReactiveProperty<string> CurrentFile = new();

    public void SetFiles(string[] files)
    {
        CurrentPlaylist.Value = new List<string>(files);
        CurrentFile.Value = CurrentPlaylist.Value.FirstOrDefault();
    }

    public void AddFiles(string[] files)
    {
        // Add new unique files to the current playlist
        CurrentPlaylist.Value = CurrentPlaylist.Value.Concat(
            files.Where(file => !CurrentPlaylist.Value.Contains(file))
        ).ToList();
    }

    public void RemoveFile(string file)
    {
        CurrentPlaylist.Value.Remove(file);
    }

    public void SetCurrentFile(string file)
    {
        if (CurrentPlaylist.Value.Contains(file))
            CurrentFile.Value = file;
        else
        {
            Debug.LogError("File not found in current playlist: " + file);
        }
    }

    public void MoveToNextFile()
    {
        if (CurrentPlaylist.Value.Count == 0)
        {
            Debug.LogError("No files in current playlist");
            return;
        }

        var currentIndex = CurrentPlaylist.Value.IndexOf(CurrentFile.Value);
        var nextIndex = (currentIndex + 1) % CurrentPlaylist.Value.Count;
        CurrentFile.Value = CurrentPlaylist.Value[nextIndex];
    }

    public void MoveToPreviousFile()
    {
        if (CurrentPlaylist.Value.Count == 0)
        {
            Debug.LogError("No files in current playlist");
            return;
        }

        var currentIndex = CurrentPlaylist.Value.IndexOf(CurrentFile.Value);
        var previousIndex = (currentIndex - 1 + CurrentPlaylist.Value.Count) % CurrentPlaylist.Value.Count;
        CurrentFile.Value = CurrentPlaylist.Value[previousIndex];
    }
}