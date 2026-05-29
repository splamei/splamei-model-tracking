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
    public AviModelImporterSpawner modelImporterSpawner;
    public GameObject modelRoot;
    public Animator modelAni;

    private Vector3 rootOffset;
    private Quaternion lastTargetRotation;
    private bool calibrated = false;
    private bool currentlyCalibrating = false;

    public float scale = 1.0f;
    public float headOffset = 0.15f;
    public float smoothSpeed = 10.0f;

    public Vector3 trackerAnchor;
    public Vector3 unityAnchor;

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

    public List<GameObject> jointObjects = new List<GameObject>();

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

        if (currentlyCalibrating)
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
        
        MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }

        jointObjects.Add(obj);

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

    private Vector3 toUnityPosNoScale(UdpReceiver.Vec3 v)
    {
        return new Vector3(v.x, v.y, -v.z);
    }

    public void reBaseCalibrate()
    {
        calibrated = false;
        rootOffset = Vector3.zero;
    }

    public void beginCalibration()
    {
        currentlyCalibrating = true;
        modelImporterSpawner.triggerModelSwap(false, false);
    }

    public void endCalibration()
    {
        currentlyCalibrating = false;
        scale = 1;

        var p = udpReceiver.getLatest();

        modelRoot.transform.rotation = Quaternion.identity;
        modelRoot.transform.position = (toUnityPosNoScale(p.ankleLeft) + toUnityPosNoScale(p.ankleRight)) / 2f;

        float trackerShoulderY = (toUnityPosNoScale(p.shoulderLeft).y + toUnityPosNoScale(p.shoulderRight).y) / 2f;
        float trackerFootY = (toUnityPosNoScale(p.ankleLeft).y + toUnityPosNoScale(p.ankleRight).y) / 2f;
        float trackerHeight = trackerShoulderY - trackerFootY;

        float modelShoulderY = (getModelBonePos(HumanBodyBones.LeftUpperArm).y + getModelBonePos(HumanBodyBones.RightUpperArm).y) / 2f;
        float modelFootY = (getModelBonePos(HumanBodyBones.LeftFoot).y + getModelBonePos(HumanBodyBones.RightFoot).y) / 2f;
        float modelHeight = modelShoulderY - modelFootY;

        scale = (modelHeight / trackerHeight) + 0.2f;

        trackerAnchor = new Vector3(
            (toUnityPosNoScale(p.ankleLeft).x + toUnityPosNoScale(p.ankleRight).x) / 2f,
            trackerFootY,
            (toUnityPosNoScale(p.ankleLeft).z + toUnityPosNoScale(p.ankleRight).z) / 2f
        );

        unityAnchor = new Vector3(
            (getModelBonePos(HumanBodyBones.LeftFoot).x + getModelBonePos(HumanBodyBones.RightFoot).x) / 2f,
            modelFootY,
            (getModelBonePos(HumanBodyBones.LeftFoot).z + getModelBonePos(HumanBodyBones.RightFoot).z) / 2f
        );

        /**if (p.supportedJoints.headAndNeck)
        {
            Vector3 trackerHead = toUnityPosNoScale(p.head);
            Vector3 modelHead = getModelBonePos(HumanBodyBones.Head);
            headOffset2 = (trackerHead - modelHead) + rootOffset;

            Vector3 trackerNeck = toUnityPosNoScale(p.neck);
            Vector3 modelNeck = getModelBonePos(HumanBodyBones.Neck);
            neckOffset = (trackerNeck - modelNeck) + rootOffset;
        }

        if (p.supportedJoints.spine)
        {
            Vector3 trackerSpine = toUnityPosNoScale(p.spineBase);
            Vector3 modelSpine = getModelBonePos(HumanBodyBones.Spine);

            spineOffset = (trackerSpine - modelSpine) + rootOffset;
        }

        Vector3 trackerShoulderL = toUnityPosNoScale(p.shoulderLeft);
        Vector3 trackerShoulderR = toUnityPosNoScale(p.shoulderRight);

        Vector3 modelShoulderL = getModelBonePos(HumanBodyBones.LeftUpperArm);
        Vector3 modelShoulderR = getModelBonePos(HumanBodyBones.RightUpperArm);

        shoulderLOffset = (trackerShoulderL - modelShoulderL) + rootOffset;
        shoulderROffset = (trackerShoulderR - modelShoulderR) + rootOffset;

        if (p.supportedJoints.elbow)
        {
            Vector3 trackerElbowL = toUnityPosNoScale(p.elbowLeft);
            Vector3 trackerElbowR = toUnityPosNoScale(p.elbowRight);

            Vector3 modelElbowL = getModelBonePos(HumanBodyBones.LeftLowerArm);
            Vector3 modelElbowR = getModelBonePos(HumanBodyBones.RightLowerArm);

            elbowLOffset = (trackerElbowL - modelElbowL) + rootOffset;
            elbowROffset = (trackerElbowR - modelElbowR) + rootOffset;
        }

        if (p.supportedJoints.wrist)
        {
            Vector3 trackerWristL = toUnityPosNoScale(p.wristLeft);
            Vector3 trackerWristR = toUnityPosNoScale(p.wristRight);

            Vector3 modelWristL = getModelBonePos(HumanBodyBones.LeftHand);
            Vector3 modelWristR = getModelBonePos(HumanBodyBones.RightHand);

            wristLOffset = (trackerWristL - modelWristL) + rootOffset;
            wristROffset = (trackerWristR - modelWristR) + rootOffset;
        }

        Vector3 trackerHandL = toUnityPosNoScale(p.handLeft);
        Vector3 trackerHandR = toUnityPosNoScale(p.handRight);

        Vector3 modelHandL = getModelBonePos(HumanBodyBones.LeftHand);
        Vector3 modelHandR = getModelBonePos(HumanBodyBones.RightHand);

        Debug.Log("Model Hand L: " + modelHandL);
        Debug.Log("Tracker Hand L: " + trackerHandL);

        handLOffset = (trackerHandL - modelHandL) + rootOffset;
        Debug.Log("Hand L Offset: " + handLOffset);

        handROffset = (trackerHandR - modelHandR) + rootOffset;

        // hip cause it's required

        Vector3 trackerHipL = toUnityPosNoScale(p.hipLeft);
        Vector3 trackerHipR = toUnityPosNoScale(p.hipRight);

        Vector3 modelHipL = getModelBonePos(HumanBodyBones.LeftUpperLeg);
        Vector3 modelHipR = getModelBonePos(HumanBodyBones.RightUpperLeg);

        hipLOffset = (trackerHipL - modelHipL) + rootOffset;
        hipROffset = (trackerHipR - modelHipR) + rootOffset;

        if (p.supportedJoints.knee)
        {
            Vector3 trackerKneeL = toUnityPosNoScale(p.kneeLeft);
            Vector3 trackerKneeR = toUnityPosNoScale(p.kneeRight);

            Vector3 modelKneeL = getModelBonePos(HumanBodyBones.LeftLowerLeg);
            Vector3 modelKneeR = getModelBonePos(HumanBodyBones.RightLowerLeg);

            kneeLOffset = (trackerKneeL - modelKneeL) + rootOffset;
            kneeROffset = (trackerKneeR - modelKneeR) + rootOffset;
        }

        // ankle cause it's also required

        Vector3 trackerAnkleL = toUnityPosNoScale(p.ankleLeft);
        Vector3 trackerAnkleR = toUnityPosNoScale(p.ankleRight);

        Vector3 modelAnkleL = getModelBonePos(HumanBodyBones.LeftFoot);
        Vector3 modelAnkleR = getModelBonePos(HumanBodyBones.RightFoot);

        ankleLOffset = (trackerAnkleL - modelAnkleL) + rootOffset;
        ankleROffset = (trackerAnkleR - modelAnkleR) + rootOffset;

        if (p.supportedJoints.foot)
        {
            Vector3 trackerFootL = toUnityPosNoScale(p.footLeft);
            Vector3 trackerFootR = toUnityPosNoScale(p.footRight);

            Vector3 modelFootL = getModelBonePos(HumanBodyBones.LeftFoot);
            Vector3 modelFootR = getModelBonePos(HumanBodyBones.RightFoot);

            footLOffset = (trackerFootL - modelFootL) + rootOffset;
            footROffset = (trackerFootR - modelFootR) + rootOffset;
        }**/

        modelImporterSpawner.triggerModelSwap(true, false);
    }

    private Vector3 getModelBonePos(HumanBodyBones bone)
    {
        Vector3 ogPos =  modelAni.GetBoneTransform(bone).position;
        return new Vector3(ogPos.x, ogPos.y, ogPos.z);
    }
}
