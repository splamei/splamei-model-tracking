/*  Copyright 2026 Splamei
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

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

    public enum settingsData
    {
        scale = 0,
        smoothing = 1,
        deadzone = 2,
        maxMovement = 3,

        modelShowOnDisconnect = 4,
        headRotationFromBridge = 5,

        port = 6
    }

    public static float getSettingsDataFloat(SaveSystem saveSystem, settingsData settingsData)
    {
        if (settingsData == settingsData.scale)
        {
            if (saveSystem.modelScale == 0)
            {
                return 1;
            }

            return saveSystem.modelScale;
        }

        else if (settingsData == settingsData.smoothing)
        {
            if (saveSystem.modelSmoothing == 0)
            {
                return 0.05f;
            }

            return saveSystem.modelSmoothing;
        }

        else if (settingsData == settingsData.deadzone)
        {
            if (saveSystem.modelDeadzoneSize == 0)
            {
                return 0.005f;
            }

            return saveSystem.modelDeadzoneSize;
        }

        else if (settingsData == settingsData.maxMovement)
        {
            if (saveSystem.modelMaxMovement == 0)
            {
                return 0.4f;
            }

            return saveSystem.modelMaxMovement;
        }

        else if (settingsData == settingsData.port)
        {
            if (saveSystem.port == 0)
            {
                return 58080;
            }
            
            return saveSystem.port;
        }

        return 0;
    }

    public static bool getSettingsDataBool(SaveSystem saveSystem, settingsData settingsData)
    {
        if (settingsData == settingsData.modelShowOnDisconnect)
        {
            return saveSystem.modelShowOnDisconnect;
        }

        else if (settingsData == settingsData.headRotationFromBridge)
        {
            return saveSystem.modelUseBridgeHeadRotation;
        }

        return false;
    }

    public static void setSettingsDataFloat(SaveSystem saveSystem, settingsData settingsData, float data)
    {
        if (settingsData == settingsData.scale)
        {
            saveSystem.modelScale = data;
        }

        else if (settingsData == settingsData.smoothing)
        {
            saveSystem.modelSmoothing = data;
        }

        else if (settingsData == settingsData.deadzone)
        {
            saveSystem.modelDeadzoneSize = data;
        }

        else if (settingsData == settingsData.maxMovement)
        {
            saveSystem.modelMaxMovement = data;
        }

        else if (settingsData == settingsData.port)
        {
            saveSystem.port = int.Parse(data.ToString());
        }
    }

    public static void setSettingsDataBool(SaveSystem saveSystem, settingsData settingsData, bool data)
    {
        if (settingsData == settingsData.modelShowOnDisconnect)
        {
            saveSystem.modelShowOnDisconnect = data;
        }

        else if (settingsData == settingsData.headRotationFromBridge)
        {
            saveSystem.modelUseBridgeHeadRotation = data;
        }
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
