using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public Animation animationObj;
    public AnimationClip showAni;
    public AnimationClip closeAni;

    public bool showing = false;
    public bool isClosing = false;

    // Start is called before the first frame update
    void Start()
    {
        
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
