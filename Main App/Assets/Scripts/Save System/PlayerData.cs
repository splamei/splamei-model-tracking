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

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string gameVer;
    public int saveVer;

    public float modelScale;
    public float modelSmoothing;
    public float modelDeadzoneSize;
    public float modelMaxMovement;

    public bool modelShowOnDisconnect;
    public bool modelUseBridgeHeadRotation;

    public SaveGlobal.backgroundType backgroundType;
    public Vector3 cameraPos;
    public Vector3 cameraRot;

    public int port;

    public PlayerData (SaveSystem player)
    {
        gameVer = player.gameVer;
        saveVer = player.saveVer;

        modelScale = player.modelScale;
        modelSmoothing = player.modelSmoothing;
        modelDeadzoneSize = player.modelDeadzoneSize;
        modelMaxMovement = player.modelMaxMovement;

        modelShowOnDisconnect = player.modelShowOnDisconnect;
        modelUseBridgeHeadRotation = player.modelUseBridgeHeadRotation;

        backgroundType = player.backgroundType;
        cameraPos = player.cameraPos;
        cameraRot = player.cameraRot;

        port = player.port;
    }
}
