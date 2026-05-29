using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickMenuManager : MonoBehaviour
{
    public Animation quickMenuAnimation;
    public AnimationClip quickMenuShow;
    public AnimationClip quickMenuHide;

    public Button[] uiButtons = new Button[0];

    public GameObject quickMenuObj;

    private bool hidingMenu = false;

    private float clickTimer = 0;

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
}
