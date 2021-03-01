//======= Copyright (c) Valve Corporation, All rights reserved. ===============

using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
    public class FloppyHand : MonoBehaviour
    {

        protected float fingerFlexAngle = 140;

        [SteamVR_DefaultAction("Squeeze")]
        public SteamVR_Action_Single squeezyAction;
        public SteamVR_Input_Sources inputSource;

        [System.Serializable]
        public class Finger
        {
            public float mass;

            [Range(0, 1)]
            public float pos;

            public Vector3 forwardAxis;

            public SkinnedMeshRenderer renderer;
            [HideInInspector]
            public SteamVR_Action_Single squeezyAction;
            public SteamVR_Input_Sources inputSource;

            public Transform[] bones;
            public Transform referenceBone;
            public Vector2 referenceAngles;

            public enum eulerAxis
            {
                X, Y, Z
            }
            public eulerAxis referenceAxis;


            [HideInInspector]
            public float flexAngle;

            private Vector3[] rotation;
            private Vector3[] velocity;
            private Transform[] boneTips;
            private Vector3[] oldTipPosition;
            private Vector3[] oldTipDelta;
            private Vector3[,] inertiaSmoothing;

            private float squeezySmooth;

            private int inertiaSteps = 10;
            private float k = 400;
            private float damping = 8;
            private Quaternion[] startRot;

            public void ApplyForce(Vector3 worldForce)
            {
                for (var i = 0; i < startRot.Length; i++)
                {
                    velocity[i] += worldForce / 50;
                }

            }


            public void Init()
            {
                startRot = new Quaternion[bones.Length];
                rotation = new Vector3[bones.Length];
                velocity = new Vector3[bones.Length];
                oldTipPosition = new Vector3[bones.Length];
                oldTipDelta = new Vector3[bones.Length];
                boneTips = new Transform[bones.Length];
                inertiaSmoothing = new Vector3[bones.Length, inertiaSteps];
                for (var i = 0; i < bones.Length; i++)
                {
                    startRot[i] = bones[i].localRotation;
                    if (i < bones.Length - 1)
                    {
                        boneTips[i] = bones[i + 1];
                    }
                }
            }

            public void UpdateFinger(float deltaTime)
            {
                if (deltaTime == 0)
                    return;

                float squeezeValue = 0;
                if (squeezyAction != null && squeezyAction.GetActive(inputSource))
                    squeezeValue = squeezyAction.GetAxis(inputSource);

                squeezySmooth = Mathf.Lerp(squeezySmooth, Mathf.Sqrt(squeezeValue), deltaTime * 10);

                if (renderer.sharedMesh.blendShapeCount > 0)
                {
                    renderer.SetBlendShapeWeight(0, squeezySmooth * 100);
                }

                float boneRot = 0;
                if (referenceAxis == eulerAxis.X)
                    boneRot = referenceBone.localEulerAngles.x;
                if (referenceAxis == eulerAxis.Y)
                    boneRot = referenceBone.localEulerAngles.y;
                if (referenceAxis == eulerAxis.Z)
                    boneRot = referenceBone.localEulerAngles.z;
                boneRot = FixAngle(boneRot);

                pos = Mathf.InverseLerp(referenceAngles.x, referenceAngles.y, boneRot);

                if (mass > 0)
                {
                    for (var boneIndex = 0; boneIndex < bones.Length; boneIndex++)
                    {
                        var useOffset = boneTips[boneIndex] != null;
                        if (useOffset) // inertia sim
                        {
                            var offset = (boneTips[boneIndex].localPosition - bones[boneIndex].InverseTransformPoint(oldTipPosition[boneIndex])) / deltaTime;
                            var inertia = (offset - oldTipDelta[boneIndex]) / deltaTime;
                            oldTipDelta[boneIndex] = offset;

                            var drag = offset * -2;
                            inertia *= -2f;

                            for (var offsetIndex = inertiaSteps - 1; offsetIndex > 0; offsetIndex--) // offset inertia steps
                            {
                                inertiaSmoothing[boneIndex, offsetIndex] = inertiaSmoothing[boneIndex, offsetIndex - 1];
                            }
                            inertiaSmoothing[boneIndex, 0] = inertia;

                            var smoothedInertia = Vector3.zero;
                            for (var offsetIndex = 0; offsetIndex < inertiaSteps; offsetIndex++) // offset inertia steps
                            {
                                smoothedInertia += inertiaSmoothing[boneIndex, offsetIndex];
                            }

                            smoothedInertia = smoothedInertia / inertiaSteps;
                            //if (boneIndex == 0 && Input.GetKey(KeyCode.Space))
                            //    Debug.Log(smoothedInertia);
                            smoothedInertia = PowVector(smoothedInertia / 20, 3) * 20;

                            var forward = forwardAxis;
                            var forwardDrag = forwardAxis + drag;
                            var forwardInertia = forwardAxis + smoothedInertia;
                            var dragQuaternion = Quaternion.FromToRotation(forward, forwardDrag);
                            var inertiaQuaternion = Quaternion.FromToRotation(forward, forwardInertia);
                            velocity[boneIndex] += FixVector(dragQuaternion.eulerAngles) * 2 * deltaTime;
                            velocity[boneIndex] += FixVector(inertiaQuaternion.eulerAngles) * 50 * deltaTime;
                            velocity[boneIndex] = Vector3.ClampMagnitude(velocity[boneIndex], 1000);

                        }

                        var targetPos = pos * Vector3.right * (flexAngle / bones.Length);

                        var springForce = -k * (rotation[boneIndex] - targetPos);
                        var dampingForce = damping * velocity[boneIndex];
                        var force = springForce - dampingForce;
                        var acceleration = force / mass;
                        velocity[boneIndex] += acceleration * deltaTime;
                        rotation[boneIndex] += velocity[boneIndex] * Time.deltaTime;
                        rotation[boneIndex] = Vector3.ClampMagnitude(rotation[boneIndex], 180);
                        if (useOffset)
                        {
                            oldTipPosition[boneIndex] = boneTips[boneIndex].position;
                        }
                    }
                }
                else
                {
                    Debug.LogError("finger mass is zero");
                }
            }

            public void ApplyTransforms()
            {
                for (var i = 0; i < bones.Length; i++)
                {
                    bones[i].localRotation = startRot[i];
                    bones[i].Rotate(rotation[i], Space.Self);
                }
            }

            private Vector3 FixVector(Vector3 ang)
            {
                return new Vector3(FixAngle(ang.x), FixAngle(ang.y), FixAngle(ang.z));
            }

            private float FixAngle(float ang)
            {
                if (ang > 180)
                    ang = -360 + ang;
                return ang;
            }

            private Vector3 PowVector(Vector3 vector, float power)
            {
                var sign = new Vector3(Mathf.Sign(vector.x), Mathf.Sign(vector.y), Mathf.Sign(vector.z));
                vector.x = Mathf.Pow(Mathf.Abs(vector.x), power) * sign.x;
                vector.y = Mathf.Pow(Mathf.Abs(vector.y), power) * sign.y;
                vector.z = Mathf.Pow(Mathf.Abs(vector.z), power) * sign.z;
                return vector;
            }
        }

        public Finger[] fingers;

        public Vector3 constforce;

        private void Start()
        {
            for (var fingerIndex = 0; fingerIndex < fingers.Length; fingerIndex++)
            {
                fingers[fingerIndex].Init();
                fingers[fingerIndex].flexAngle = fingerFlexAngle;
                fingers[fingerIndex].squeezyAction = squeezyAction;
                fingers[fingerIndex].inputSource = inputSource;
            }
        }

        private void Update()
        {
            for (var fingerIndex = 0; fingerIndex < fingers.Length; fingerIndex++)
            {
                fingers[fingerIndex].ApplyForce(constforce);
                fingers[fingerIndex].UpdateFinger(Time.deltaTime);
                fingers[fingerIndex].ApplyTransforms();
            }
        }
    }
}