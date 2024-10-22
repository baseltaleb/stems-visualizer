using System.IO;
using JetBrains.Annotations;
using UnityEngine;

public static class FileCacheManager
{
    public static string GetFileCachePath(string sessionId, string fileName)
    {
        var path = Path.Combine(GetCachePath(), sessionId);
        return Path.Combine(path, fileName);
    }

    public static bool IsCached(string path)
    {
        var filePath = Path.Combine(GetCachePath(), path);
        return File.Exists(filePath);
    }

    public static void ClearCache()
    {
        Directory.Delete(GetCachePath(), true);
        Directory.CreateDirectory(GetCachePath());
    }
    
    private static string GetCachePath()
    {
        var path = Path.Combine(Application.persistentDataPath, "cache");
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        return path;
    }
}