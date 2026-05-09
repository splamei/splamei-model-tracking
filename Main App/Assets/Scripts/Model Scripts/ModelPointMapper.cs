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

public class ModelPointMapper : MonoBehaviour
{
    public UdpReceiver udpReceiver;
    public GameObject modelRoot;
    public Animator modelAni;

    private Vector3 rootOffset;
    private Quaternion lastTargetRotation;
    private bool calibrated = false;

    public float scale = 1.0f;
    public float headOffset = 0.15f;
    public float smoothSpeed = 10.0f;

    public float latency = 0;
    public int bridgeFrame;
    public string bridgeName;
    public string bridgeId;

    private Dictionary<GameObject, Vector3> velocities = new Dictionary<GameObject, Vector3>();

    public GameObject head, neck, spine;
    public GameObject shoulderL, shoulderR;
    public GameObject elbowL, elbowR;
    public GameObject wristL, wristR;
    public GameObject handL, handR;

    public GameObject hipL, hipR;
    public GameObject kneeL, kneeR;
    public GameObject ankleL, ankleR;
    public GameObject footL, footR;

    // Start is called before the first frame update
    void Start()
    {
        head = createSphere();
        neck = createSphere();
        spine = createSphere();

        shoulderL = createSphere();
        shoulderR = createSphere();

        elbowL = createSphere();
        elbowR = createSphere();

        wristL = createSphere();
        wristR = createSphere();

        handL = createSphere();
        handR = createSphere();

        hipL = createSphere();
        hipR = createSphere();

        kneeL = createSphere();
        kneeR = createSphere();

        ankleL = createSphere();
        ankleR = createSphere();

        footL = createSphere();
        footR = createSphere();
    }

    // Update is called once per frame
    void Update()
    {
        if (udpReceiver == null || !udpReceiver.hasData() || modelRoot == null)
        {
            return;
        }

        if (!udpReceiver.hadValidData)
        {
            return;
        }

        var p = udpReceiver.getLatest();

        latency = (float)Math.Round((TimeSpan.FromTicks(DateTime.UtcNow.Ticks) - TimeSpan.FromTicks(p.timestampG)).TotalMilliseconds, 2);
        bridgeFrame = p.frame;
        bridgeId = p.identifier;
        bridgeName = p.friendlyName;

        Vector3 hipPos = new Vector3(p.spineBase.x, p.spineBase.y, p.spineBase.z);

        if (!calibrated)
        {
            rootOffset = transform.position - hipPos;
            calibrated = true;
        }

        Vector3 spineL = toUnityPos(p.spineBase);
        Vector3 neckL  = toUnityPos(p.neck);
        Vector3 headPos = neckL + (neckL - spineL).normalized * headOffset;
        head.transform.position = Vector3.Lerp(
            head.transform.position,
            headPos + rootOffset,
            Time.deltaTime * 15f
        );

        Vector3 forward = modelRoot.transform.forward;

        Vector3 neckForward = (neckL - spineL).normalized;

        Vector3 blendedForward = Vector3.Slerp(forward, neckForward, 0.3f);

        Quaternion targetRot = Quaternion.LookRotation(blendedForward, Vector3.up);

        head.transform.rotation = Quaternion.Slerp(
            head.transform.rotation,
            targetRot,
            Time.deltaTime * 10f
        );

        //setSpherePos(head, p.head, rootOffset);
        setSpherePos(neck, p.neck, rootOffset);
        setSpherePos(spine, p.spineBase, rootOffset);

        setSpherePos(shoulderL, p.shoulderLeft, rootOffset);
        setSpherePos(shoulderR, p.shoulderRight, rootOffset);

        setSpherePos(elbowL, p.elbowLeft, rootOffset);
        setSpherePos(elbowR, p.elbowRight, rootOffset);

        setSpherePos(wristL, p.wristLeft, rootOffset);
        setSpherePos(wristR, p.wristRight, rootOffset);

        setSpherePos(handL, p.handLeft, rootOffset);
        setSpherePos(handR, p.handRight, rootOffset);

        setSpherePos(hipL, p.hipLeft, rootOffset);
        setSpherePos(hipR, p.hipRight, rootOffset);

        setSpherePos(kneeL, p.kneeLeft, rootOffset);
        setSpherePos(kneeR, p.kneeRight, rootOffset);

        setSpherePos(ankleL, p.ankleLeft, rootOffset);
        setSpherePos(ankleR, p.ankleRight, rootOffset);

        setSpherePos(footL, p.ankleLeft, rootOffset);
        setSpherePos(footR, p.ankleRight, rootOffset);
        footL.transform.rotation = Quaternion.Euler(0, 180, 0);
        footR.transform.rotation = Quaternion.Euler(0, 180, 0);

        Vector3 hip = toUnityPos(p.spineBase);

        Vector3 shoulderDir = (shoulderR.transform.position - shoulderL.transform.position).normalized;
        Vector3 hipForward = (hip - (toUnityPos(p.hipLeft) + toUnityPos(p.hipRight)) * 0.5f).normalized;

        forward = Vector3.Slerp(hipForward, Vector3.Cross(shoulderDir, Vector3.up), 0.7f);

        if (forward.sqrMagnitude > 0.0001f)
        {
            lastTargetRotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        modelRoot.transform.rotation = Quaternion.Slerp(modelRoot.transform.rotation, lastTargetRotation, Time.deltaTime * smoothSpeed);
    }

    private GameObject createSphere()
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.localScale = Vector3.one * 0.15f;
        return obj;
    }

    private void setSpherePos(GameObject obj, UdpReceiver.Vec3 v, Vector3 offset)
    {
        if (obj == null) { return; }

        float deadzone = 0.005f;
        float maxMovement = 0.4f;
        float smoothTime = 0.05f;

        Vector3 target = (new Vector3(-v.x, v.y, v.z) + offset) * scale;

        if (!velocities.ContainsKey(obj))
        {
            velocities[obj] = Vector3.zero;
        }

        Vector3 currentPos = obj.transform.position;

        Vector3 movementSize = target - currentPos;
        if (movementSize.magnitude < deadzone)
        {
            return;
        }

        if (movementSize.magnitude > maxMovement)
        {
            target = currentPos + movementSize.normalized * maxMovement;
        }

        Vector3 vel = velocities[obj];

        Vector3 smoothed = Vector3.SmoothDamp(
            currentPos,
            target,
            ref vel,
            smoothTime
        );

        obj.transform.position = smoothed;

        velocities[obj] = vel;
    }

    private Vector3 toUnityPos(UdpReceiver.Vec3 v)
    {
        return new Vector3(v.x, v.y, -v.z) * scale;
    }

    public void reBaseCalibrate()
    {
        calibrated = false;
        rootOffset = Vector3.zero;
    }
}
