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

public class ModelAvatarDriver : MonoBehaviour
{
    public SaveSystem saveSystem;
    public ModelPointMapper modelPointMapper;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (modelPointMapper == null) { return; }

        Vector3 hipPos = modelPointMapper.spine.transform.position;
        transform.position = hipPos;
    }

    void LateUpdate()
    {
        if (modelPointMapper == null || modelPointMapper.head == null || modelPointMapper.neck == null) { return; }

        Transform headBone = anim.GetBoneTransform(HumanBodyBones.Head);

        Vector3 forward = modelPointMapper.head.transform.position - modelPointMapper.neck.transform.position;

        Vector3 right = modelPointMapper.shoulderR.transform.position - modelPointMapper.shoulderL.transform.position;
        Vector3 up = Vector3.Cross(forward, right);

        if (GlobalData.getSettingsDataBool(saveSystem, GlobalData.settingsData.headRotationFromBridge))
        {
            headBone.rotation = modelPointMapper.head.transform.rotation;
        }
        else
        {
            if (forward.sqrMagnitude > 0.0001f)
            {
                headBone.rotation = Quaternion.LookRotation(forward, up);
            }
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (anim == null || modelPointMapper == null) return;

        // Hands
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);

        anim.SetIKPosition(AvatarIKGoal.LeftHand, modelPointMapper.handL.transform.position);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, modelPointMapper.handL.transform.rotation);

        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

        anim.SetIKPosition(AvatarIKGoal.RightHand, modelPointMapper.handR.transform.position);
        anim.SetIKRotation(AvatarIKGoal.RightHand, modelPointMapper.handR.transform.rotation);

        // Feet
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);

        anim.SetIKPosition(AvatarIKGoal.LeftFoot, modelPointMapper.footL.transform.position);
        anim.SetIKRotation(AvatarIKGoal.LeftFoot, modelPointMapper.footL.transform.rotation);

        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

        anim.SetIKPosition(AvatarIKGoal.RightFoot, modelPointMapper.footR.transform.position);
        anim.SetIKRotation(AvatarIKGoal.RightFoot, modelPointMapper.footR.transform.rotation);
    }
}
