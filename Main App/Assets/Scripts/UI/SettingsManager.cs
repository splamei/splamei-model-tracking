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
 
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public SaveSystem saveSystem;
    public NotifyManager notify;

    public bool changingValues = false;

    public Animation animationObj;
    public AnimationClip showAni;
    public AnimationClip closeAni;

    public InputField modelScaleInput;
    public InputField smoothingInput;
    public InputField deadzoneInput;
    public InputField maxMovementInput;

    public Toggle showModelOnDisconnect;
    public Toggle getHeadRotationBridge;

    public InputField portInput;

    public Text versionText;

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
