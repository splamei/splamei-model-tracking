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
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public SaveSystem saveSystem;
    public Camera cameraObj;

    public GameObject settingsObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (settingsObj.activeSelf)
        {
            return;
        }

        Vector3 cameraPos = saveSystem.cameraPos;

        float moveSpeed = 2f;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            cameraPos.x += moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            cameraPos.x -= moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            cameraPos.y -= moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            cameraPos.y += moveSpeed * Time.deltaTime;
        }

        saveSystem.cameraPos = cameraPos;

        cameraPos.z -= 2;
        cameraObj.transform.position = cameraPos;
    }
}
