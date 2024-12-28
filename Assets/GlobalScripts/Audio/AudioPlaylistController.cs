using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using IngameDebugConsole;
using R3;
using UnityEngine;

public class AudioPlaylistController
{
    public static readonly ReactiveProperty<List<string>> CurrentPlaylist = new(new List<string>());
    public static readonly ReactiveProperty<string> CurrentFile = new();

    public AudioPlaylistController()
    {
        DebugLogConsole.AddCommandInstance("playlist", "Logs current playlist", "LogCurrentPlaylist", this);
    }

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
            Debug.LogError("APC: File not found in current playlist: " + file);
        }
    }

    public void MoveToNextFile()
    {
        if (CurrentPlaylist.Value.Count < 1)
        {
            Debug.LogWarning("Cannot move to next file");
            return;
        }

        var currentIndex = CurrentPlaylist.Value.IndexOf(CurrentFile.Value);
        var nextIndex = (currentIndex + 1) % CurrentPlaylist.Value.Count;
        CurrentFile.Value = CurrentPlaylist.Value[nextIndex];
        Debug.Log(
            $"APC: Current file: {CurrentFile.Value.GetFileName()}. Remaining files: {CurrentPlaylist.Value.Count - 1 - nextIndex}");
    }

    public void MoveToPreviousFile()
    {
        if (CurrentPlaylist.Value.Count < 1)
        {
            Debug.LogWarning("APC: Cannot move to next file");
            return;
        }

        var currentIndex = CurrentPlaylist.Value.IndexOf(CurrentFile.Value);
        var previousIndex = (currentIndex - 1 + CurrentPlaylist.Value.Count) % CurrentPlaylist.Value.Count;
        CurrentFile.Value = CurrentPlaylist.Value[previousIndex];
        Debug.Log(
            $"APC: Current file: {CurrentFile.Value.GetFileName()}. Remaining files: {CurrentPlaylist.Value.Count - 1 - previousIndex}");
    }

    public bool HasNextFile()
    {
        return CurrentPlaylist.Value.IndexOf(CurrentFile.Value) < CurrentPlaylist.Value.Count - 1;
    }

    public void LogCurrentPlaylist()
    {
        // Log current file name
        Debug.Log("APC: Current file: " + CurrentFile.Value?.GetFileName());
        // Log file names of current playlist
        Debug.Log("APC: Current playlist: \n" +
                  string.Join("\n", CurrentPlaylist.Value.Select(file => file.GetFileName())));
    }
}