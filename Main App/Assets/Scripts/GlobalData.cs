using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GlobalData
{
    public static string getSavePath(string saveFile)
    {
        if (saveFile.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            Debug.LogError("[GlobalData] Unable to find safe path. Invalid characters in filename. File:" + saveFile);
            ErrorTrigger.triggerError("An access violation occured");
            throw new UnauthorizedAccessException("Unable to find safe path. Invalid characters in filename");
        }

        if (saveFile.Contains(Path.DirectorySeparatorChar) || saveFile.Contains(Path.AltDirectorySeparatorChar))
        {
            Debug.LogError("[GlobalData] Unable to find safe path. Directory separators in filename. File: " + saveFile);
            ErrorTrigger.triggerError("An access violation occured");
            throw new UnauthorizedAccessException("Unable to find safe path. Directory separators in filename");
        }

        string dir = Path.GetFullPath(
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Splamei",
                "Model Tracking",
                "Saves"
            )
        );

        string fullPath = Path.GetFullPath(Path.Combine(dir, saveFile));

        string baseDirWithSep = dir.EndsWith(Path.DirectorySeparatorChar.ToString()) ? dir : dir + Path.DirectorySeparatorChar;

        if (!fullPath.StartsWith(baseDirWithSep, StringComparison.OrdinalIgnoreCase) && fullPath != dir)
        {
            Debug.LogError("[GlobalData] Unable to find safe path. It did not start with the main directory. Full path: " + fullPath);
            ErrorTrigger.triggerError("An access violation occured");
            throw new UnauthorizedAccessException("Unable to find safe path. It did not start with the main directory");
        }

        return fullPath;
    }

    public static string getSafePath(string baseDir = null, params string[] segments)
    {
        string combinedPath = baseDir ?? "";

        foreach (var segment in segments)
        {
            if (string.IsNullOrEmpty(segment))
            {
                Debug.LogError("[GlobalData] Argument exception - Segments can't be null or empty");
                ErrorTrigger.triggerError("An access violation occured");
                throw new ArgumentException("Path segments can't be empty or null");
            }

            if (segment.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                Debug.LogError("[GlobalData] Access Violation - Invalid characters in path segment. Segment: " + segment);
                ErrorTrigger.triggerError("An access violation occured");
                throw new UnauthorizedAccessException("Unable to find safe path. Invalid characters in path segment.");
            }

            if (segment.Contains(Path.DirectorySeparatorChar) || segment.Contains(Path.AltDirectorySeparatorChar))
            {
                Debug.LogError("[GlobalData] Access Violation - Directory separators in path segment. Segment: " + segment);
                ErrorTrigger.triggerError("An access violation occured");
                throw new UnauthorizedAccessException("Unable to find safe path. Directory separators in path segment");
            }

            combinedPath = Path.Combine(combinedPath, segment);
        }

        string fullPath = Path.GetFullPath(combinedPath);

        if (!string.IsNullOrEmpty(baseDir))
        {
            string baseFullPath = Path.GetFullPath(baseDir);
            string baseDirWithSep = baseFullPath.EndsWith(Path.DirectorySeparatorChar.ToString()) ? baseFullPath : baseFullPath + Path.DirectorySeparatorChar;

            if (!fullPath.StartsWith(baseDirWithSep, StringComparison.OrdinalIgnoreCase) && fullPath != baseFullPath)
            {
                Debug.LogError("[GlobalData] Access Violation - The path did not start with the base directory. Full path: " + fullPath);
                ErrorTrigger.triggerError("An access violation occured");
                throw new UnauthorizedAccessException("Unable to find safe path. It did not start with the base directory.");
            }
        }

        return fullPath;
    }
}


internal static class ErrorTrigger
{
    internal static void triggerError(string msg)
    {
        PlayerPrefs.SetString("err", msg);
        SceneManager.LoadScene("Error");
    }
}
