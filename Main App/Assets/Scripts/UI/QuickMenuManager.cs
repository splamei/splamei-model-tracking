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
}
