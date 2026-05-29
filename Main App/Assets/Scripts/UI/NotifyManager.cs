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

public class NotifyManager : MonoBehaviour
{
    public GameObject canvas;

    public Animation animationObj;
    public AnimationClip showAni;
    public AnimationClip closeAni;

    public Text titleText;
    public Text textText;
    public Text button1Text;
    public Text button2Text;

    [Serializable]
    public struct NotifyQueue
    {
        public string title;
        public string text;

        public string button1Text;
        public string button2Text;
        public Action<int> actionToRun;
    }

    public List<NotifyQueue> notifyQueue = new List<NotifyQueue>();

    public bool currentlyShowing = false;
    public bool currentlyClosing = false;
    private Action<int> actionToRunG = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentlyShowing && currentlyClosing)
        {
            if (!animationObj.isPlaying)
            {
                currentlyClosing = false;
                currentlyShowing = false;

                canvas.SetActive(false);

                if (notifyQueue.Count > 0)
                {
                    var nextItem = notifyQueue[0];
                    notifyQueue.RemoveAt(0);

                    internalShow(nextItem);
                }
            }
        }
    }

    public void button1Pressed()
    {
        if (!currentlyClosing)
        {
            if (actionToRunG != null)
            {
                actionToRunG(1);
            }

            internalClose();
        }
    }

    public void button2Pressed()
    {
        if (!currentlyClosing)
        {
            if (actionToRunG != null)
            {
                actionToRunG(2);
            }

            internalClose();
        }
    }

    public void show(Action<int> actionToRun, string title, string text, string button1, string button2)
    {
        if (currentlyShowing)
        {
            NotifyQueue notifyObject = new NotifyQueue
            {
                title = title,
                text = text,
                button1Text = button1,
                button2Text = button2,

                actionToRun = actionToRun
            };

            notifyQueue.Add(notifyObject);
        }
        else
        {
            NotifyQueue notifyObject = new NotifyQueue
            {
                title = title,
                text = text,
                button1Text = button1,
                button2Text = button2,

                actionToRun = actionToRun
            };

            internalShow(notifyObject);
        }
    }

    private void internalShow(NotifyQueue data)
    {
        currentlyShowing = true;
        canvas.SetActive(true);

        titleText.text = data.title;
        textText.text = data.text;

        if (string.IsNullOrEmpty(data.button1Text))
        {
            button1Text.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            button1Text.text = data.button1Text;
            button1Text.transform.parent.gameObject.SetActive(true);
        }

        if (string.IsNullOrEmpty(data.button2Text))
        {
            button2Text.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            button2Text.text = data.button2Text;
            button2Text.transform.parent.gameObject.SetActive(true);
        }

        actionToRunG = data.actionToRun;

        animationObj.clip = showAni;
        animationObj.Play();
    }

    private void internalClose()
    {
        animationObj.clip = closeAni;
        animationObj.Play();

        currentlyClosing = true;
    }
}
