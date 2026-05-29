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
    public Text display1;
    public Text display2;
    public ModelPointMapper modelPointMapper;

    public float updateTimer = 0;

    public float peakLatencyTimer = 0;
    public float peakLatency = 0;

    public float averageLatency = 0;
    public int averageLatencyCount = 0;

    public GameObject latencyMeterObj;
    public GameObject currentLatancyObj;
    public GameObject peakLatencyObj;

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

            display1.text = $"Latency: {modelPointMapper.latency}ms\nAverage latency: {Math.Round(averageLatency, 1)}ms\nPeak latency: {peakLatency}ms (for {Math.Round(peakLatencyTimer, 1)}s)";
            display2.text = $"Bridge name: {modelPointMapper.bridgeName}\nBridge ID: {modelPointMapper.bridgeId}\nBridge Frame: {modelPointMapper.bridgeFrame}";
            updateTimer = 0;

            float latency2 = modelPointMapper.latency;
            if (latency2 >= 2000) { latency2 = 2000; }
            float currentLatencyYPos = (0.17f * latency2) - 340f;
            currentLatancyObj.transform.localPosition = new Vector3(currentLatancyObj.transform.localPosition.x, currentLatencyYPos, currentLatancyObj.transform.localPosition.z);

            float peakLatency2 = peakLatency;
            if (peakLatency2 >= 2000) { peakLatency2 = 2000; }
            float peakLatencyYPos = (0.17f * peakLatency2) - 170f;
            peakLatencyObj.transform.localPosition = new Vector3(peakLatencyObj.transform.localPosition.x, peakLatencyYPos, peakLatencyObj.transform.localPosition.z);
        }
    }
}
