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
using UnityEngine;
using UnityEngine.UI;

public class DebugStatsManager : MonoBehaviour
{
    public Text display;
    public ModelPointMapper modelPointMapper;

    public Toggle calibratedToggle;

    public float updateTimer = 0;

    public float peakLatencyTimer = 0;
    public float peakLatency = 0;

    public float averageLatency = 0;
    public int averageLatencyCount = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        updateTimer += Time.deltaTime;

        peakLatencyTimer += Time.deltaTime;
        if (peakLatencyTimer > 3)
        {
            peakLatency = 0;
            peakLatencyTimer = 0;
        }

        if (modelPointMapper.latency > peakLatency)
        {
            peakLatency = modelPointMapper.latency;
            peakLatencyTimer = 0;
        }

        if (updateTimer > 0.1f)
        {
            if (modelPointMapper.latency >= 0.000001f)
            {
                averageLatency = (averageLatency * averageLatencyCount + modelPointMapper.latency) / (averageLatencyCount + 1);
                averageLatencyCount++;
            }

            display.text = $"Latency: {modelPointMapper.latency}ms\nPeak latency: {peakLatency}ms (For {Math.Round(peakLatencyTimer, 1)}s)\nAverage latency: {Math.Round(averageLatency, 1)}ms\nBridge Frame: {modelPointMapper.bridgeFrame}\n\nBridge name: {modelPointMapper.bridgeName}\nBridge ID: {modelPointMapper.bridgeId}";
            updateTimer = 0;
        }

        if (!calibratedToggle.isOn)
        {
            modelPointMapper.reBaseCalibrate();
            calibratedToggle.isOn = true;
        }
    }
}
