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
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public SaveSystem saveSystem;

    public GameObject backgroundImageCanvas;
    public RawImage backgroundImage;

    // Start is called before the first frame update
    void Start()
    {
        updateBackground(saveSystem.backgroundType);
    }

    public void updateBackground(SaveGlobal.backgroundType backgroundType)
    {
        Camera cameraObj = Camera.main;

        cameraObj.clearFlags = CameraClearFlags.Skybox;
        if (backgroundType == SaveGlobal.backgroundType.skybox)
        {
            backgroundImageCanvas.SetActive(false);
        }
        else if (backgroundType == SaveGlobal.backgroundType.image)
        {
            backgroundImageCanvas.SetActive(true);

            Texture2D tex = loadImageFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Splamei", "Model Tracking", "Background.png"));
            backgroundImage.texture = tex;
        }
        else if (backgroundType == SaveGlobal.backgroundType.transparent)
        {
            backgroundImageCanvas.SetActive(false);
            cameraObj.clearFlags = CameraClearFlags.SolidColor;

            IntPtr hWnd = GetActiveWindow();
            MARGINS margins = new MARGINS { cxLeftWidth = -1 };
            DwmExtendFrameIntoClientArea(hWnd, ref margins);
        }
    }

    public Texture2D loadImageFile(string path)
    {
        try
        {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(path))
            {
                fileData = File.ReadAllBytes(path);
                tex = new Texture2D(1, 1);
                tex.LoadImage(fileData);
            }

            return tex;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[BackgroundManager] Unable to load image file '{path}'! - " + ex);
            return null;
        }
    }

    [DllImport("user32.dll")] private static extern IntPtr GetActiveWindow();

    [DllImport("Dwmapi.dll")] private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    private struct MARGINS
    {
        public int cxLeftWidth, cxRightWidth, cyTopHeight, cyBottomHeight;
    }
}
