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
