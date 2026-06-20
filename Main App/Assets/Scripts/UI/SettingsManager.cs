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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public SaveSystem saveSystem;
    public NotifyManager notify;

    public bool changingValues = false;

    [Serializable]
    public struct BackgroundEnumToStruct
    {
        public string name;
        public SaveGlobal.backgroundType backgroundType;
    }

    public List<BackgroundEnumToStruct> backgroundEnumToStruct = new List<BackgroundEnumToStruct>();

    [Header("Model Control")]
    public Animation animationObj;
    public AnimationClip showAni;
    public AnimationClip closeAni;

    public InputField modelScaleInput;
    public InputField smoothingInput;
    public InputField deadzoneInput;
    public InputField maxMovementInput;

    public Toggle showModelOnDisconnect;
    public Toggle getHeadRotationBridge;

    [Header("Visuals")]
    public Dropdown backgroundDropdown;
    public InputField cameraPosX, cameraPosY, cameraPosZ;
    public InputField cameraRotX, cameraRotY, cameraRotZ;

    [Header("Network")]
    public InputField portInput;

    [Header("About")]
    public Text versionText;

    [Header("Other")]
    public bool showing = false;
    public bool isClosing = false;

    // Start is called before the first frame update
    void Start()
    {
        versionText.text = $"Version: {Application.version}";
    }

    void OnEnable()
    {
        changingValues = true;

        modelScaleInput.text = GlobalData.getSettingsDataFloat(saveSystem, GlobalData.settingsData.scale).ToString();
        smoothingInput.text = GlobalData.getSettingsDataFloat(saveSystem, GlobalData.settingsData.smoothing).ToString();
        deadzoneInput.text = GlobalData.getSettingsDataFloat(saveSystem, GlobalData.settingsData.deadzone).ToString();
        maxMovementInput.text = GlobalData.getSettingsDataFloat(saveSystem, GlobalData.settingsData.maxMovement).ToString();

        showModelOnDisconnect.isOn = GlobalData.getSettingsDataBool(saveSystem, GlobalData.settingsData.modelShowOnDisconnect);
        getHeadRotationBridge.isOn = GlobalData.getSettingsDataBool(saveSystem, GlobalData.settingsData.headRotationFromBridge);

        portInput.text = GlobalData.getSettingsDataFloat(saveSystem, GlobalData.settingsData.port).ToString();

        cameraPosX.text = saveSystem.cameraPos.x.ToString();
        cameraPosY.text = saveSystem.cameraPos.y.ToString();
        cameraPosZ.text = saveSystem.cameraPos.z.ToString();

        cameraRotX.text = saveSystem.cameraRot.x.ToString();
        cameraRotY.text = saveSystem.cameraRot.y.ToString();
        cameraRotZ.text = saveSystem.cameraRot.z.ToString();

        for (int i = 0; i < backgroundDropdown.options.Count; i++)
        {
            string currentOption = backgroundDropdown.options[i].text;
            for (int i2 = 0; i2 < backgroundEnumToStruct.Count; i2++)
            {
                if (backgroundEnumToStruct[i2].name == currentOption)
                {
                    if (backgroundEnumToStruct[i2].backgroundType == saveSystem.backgroundType)
                    {
                        backgroundDropdown.value = i;
                        break;
                    }
                }
            }
        }

        changingValues = false;
    }

    void OnDisable()
    {
        GlobalData.setSettingsDataFloat(saveSystem, GlobalData.settingsData.scale, float.Parse(!string.IsNullOrEmpty(modelScaleInput.text) ? modelScaleInput.text : "0"));
        GlobalData.setSettingsDataFloat(saveSystem, GlobalData.settingsData.smoothing, float.Parse(!string.IsNullOrEmpty(smoothingInput.text) ? smoothingInput.text : "0"));
        GlobalData.setSettingsDataFloat(saveSystem, GlobalData.settingsData.deadzone, float.Parse(!string.IsNullOrEmpty(deadzoneInput.text) ? deadzoneInput.text : "0"));
        GlobalData.setSettingsDataFloat(saveSystem, GlobalData.settingsData.maxMovement, float.Parse(!string.IsNullOrEmpty(maxMovementInput.text) ? maxMovementInput.text : "0"));

        GlobalData.setSettingsDataBool(saveSystem, GlobalData.settingsData.modelShowOnDisconnect, showModelOnDisconnect.isOn);
        GlobalData.setSettingsDataBool(saveSystem, GlobalData.settingsData.headRotationFromBridge, getHeadRotationBridge.isOn);

        GlobalData.setSettingsDataFloat(saveSystem, GlobalData.settingsData.port, float.Parse(!string.IsNullOrEmpty(portInput.text) ? portInput.text : "0"));

        saveSystem.cameraPos = new Vector3(zeroIfNull(cameraPosX.text), zeroIfNull(cameraPosY.text), zeroIfNull(cameraPosZ.text));
        saveSystem.cameraRot = new Vector3(zeroIfNull(cameraRotX.text), zeroIfNull(cameraRotY.text), zeroIfNull(cameraRotZ.text));

        for (int i = 0; i < backgroundEnumToStruct.Count; i++)
        {
            string currentOption = backgroundEnumToStruct[i].name;
            Debug.Log("1 - " + currentOption);
            Debug.Log("2 - " + backgroundDropdown.options[backgroundDropdown.value].text);
            if (currentOption == backgroundDropdown.options[backgroundDropdown.value].text)
            {
                saveSystem.backgroundType = backgroundEnumToStruct[i].backgroundType;
                break;
            }
        }
    }

    float zeroIfNull(string input, string defaultNum = "0")
    {
        try
        {
            string data = !string.IsNullOrEmpty(input) ? input : defaultNum;
            return float.Parse(data);
        }
        catch
        {
            return float.Parse(defaultNum);
        }
    }

    public void updateCameraPosRot()
    {
        Vector3 cameraPos = new Vector3(zeroIfNull(cameraPosX.text), zeroIfNull(cameraPosY.text), zeroIfNull(cameraPosZ.text) - 2f);
        Vector3 cameraRot = new Vector3(zeroIfNull(cameraRotX.text), zeroIfNull(cameraRotY.text), zeroIfNull(cameraRotZ.text));

        var camera = Camera.main.gameObject;
        camera.transform.position = cameraPos;
        camera.transform.rotation = Quaternion.Euler(cameraRot);
    }

    // Update is called once per frame
    void Update()
    {
        if (isClosing && !animationObj.isPlaying)
        {
            isClosing = false;
            showing = false;

            this.gameObject.SetActive(false);
        }
    }

    public void bridgeHeadRotation()
    {
        if (!changingValues && getHeadRotationBridge.isOn)
        notify.show(null, "Note", "Using the head rotation from any connected bridge apps may not work if the bridge doesn't support head rotations.\n\nUnless you know the bridge supports head rotation, this should stay disabled.", "OK", "");
    }

    public void showSettings()
    {
        animationObj.clip = showAni;
        animationObj.Play();

        showing = true;
    }

    public void closeSettings()
    {
        animationObj.clip = closeAni;
        animationObj.Play();

        isClosing = true;
    }
}
