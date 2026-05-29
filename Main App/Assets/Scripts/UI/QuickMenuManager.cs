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

    public void showNotImplemented()
    {
        notifyManager.show(null, "Not implemented!", "This quick menu option hasn't been implemented yet! Please wait while we implement everything for the app.\n\nWe're sorry for any issues this may cause", "OK", "");
    }

#endregion
}
