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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickMenuManager : MonoBehaviour
{
    public NotifyManager notifyManager;
    public ModelPointMapper modelPointMapper;

    public Animation quickMenuAnimation;
    public AnimationClip quickMenuShow;
    public AnimationClip quickMenuHide;

    public Button[] uiButtons = new Button[0];

    public GameObject quickMenuObj;

    private bool hidingMenu = false;

    private float clickTimer = 0;

    [Header("Menu option Refs")]
    public AviModelImporterSpawner modelImporterSpawner;

    public GameObject calibratingObj;
    public float calibratedTimer = 0;
    public bool currentlyCalibrating = false;

    public bool showingDebugMode = false;
    public GameObject debugModeObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hidingMenu && !quickMenuAnimation.isPlaying)
        {
            hidingMenu = false;
            quickMenuObj.SetActive(false);
        }

        if (clickTimer > 0)
        {
            clickTimer -= Time.deltaTime;
        }

        if (currentlyCalibrating)
        {
            calibratedTimer += Time.deltaTime;
            if (calibratedTimer > 5)
            {
                currentlyCalibrating = false;
                calibratedTimer = 0;

                modelPointMapper.endCalibration();
                calibratingObj.SetActive(false);
            }
        }
    }

    public void showQuickMenu()
    {
        quickMenuAnimation.clip = quickMenuShow;

        foreach (var button in uiButtons)
        {
            button.interactable = true;
        }

        quickMenuAnimation.Play();

        quickMenuObj.SetActive(true);
    }

    public void hideQuickMenu()
    {
        quickMenuAnimation.clip = quickMenuHide;

        foreach (var button in uiButtons)
        {
            button.interactable = false;
        }

        quickMenuAnimation.Play();

        hidingMenu = true;
    }

    public void showQuickMenuButtonPressed()
    {
        if (clickTimer > 0)
        {
            showQuickMenu();
            clickTimer = 0;
        }
        else
        {
            clickTimer = 0.2f;
        }
    }

#region Menu functions

    public void selectNewModel()
    {
        modelImporterSpawner.triggerModelSwap(true, true);
    }

    public void calibrateModel()
    {
        modelPointMapper.reBaseCalibrate();
        modelPointMapper.beginCalibration();

        calibratedTimer = 0;
        currentlyCalibrating = true;
        calibratingObj.SetActive(true);
    }

    public void toggleDebugModePressed(int pressed = -1)
    {
        if (showingDebugMode)
        {
            showingDebugMode = false;
            debugModeObj.SetActive(false);

            foreach (var obj in modelPointMapper.jointObjects)
            {
                MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.enabled = false;
                }
            }
        }
        else
        {
            if (pressed == -1)
            {
                notifyManager.show(toggleDebugModePressed, "Enable debug mode?", "Do you want to enable debug mode? This mode shows extra statistics on screen to help you debug issues with your bridge or the app.\n\nUnless something is wrong, you shouldn't enable this mode", "Yes", "No");
            }
            else if (pressed == 1)
            {
                showingDebugMode = true;
                debugModeObj.SetActive(true);

                foreach (var obj in modelPointMapper.jointObjects)
                {
                    MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        meshRenderer.enabled = true;
                    }
                }
            }
        }
    }

    public void showNotImplemented()
    {
        notifyManager.show(null, "Not implemented!", "This quick menu option hasn't been implemented yet! Please wait while we implement everything for the app.\n\nWe're sorry for any issues this may cause", "OK", "");
    }

#endregion
}
