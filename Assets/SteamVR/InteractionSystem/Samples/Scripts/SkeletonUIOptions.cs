//======= Copyright (c) Valve Corporation, All rights reserved. ===============

using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
    public class SkeletonUIOptions : MonoBehaviour
    {

        public void AnimateHandWithController()
        {
            for (var handIndex = 0; handIndex < Player.instance.hands.Length; handIndex++)
            {
                var hand = Player.instance.hands[handIndex];
                if (hand != null)
                {
                    hand.SetSkeletonRangeOfMotion(Valve.VR.EVRSkeletalMotionRange.WithController);
                }
            }
        }

        public void AnimateHandWithoutController()
        {
            for (var handIndex = 0; handIndex < Player.instance.hands.Length; handIndex++)
            {
                var hand = Player.instance.hands[handIndex];
                if (hand != null)
                {
                    hand.SetSkeletonRangeOfMotion(Valve.VR.EVRSkeletalMotionRange.WithoutController);
                }
            }
        }

        public void ShowController()
        {
            for (var handIndex = 0; handIndex < Player.instance.hands.Length; handIndex++)
            {
                var hand = Player.instance.hands[handIndex];
                if (hand != null)
                {
                    hand.ShowController(true);
                }
            }
        }

        public void SetRenderModel(RenderModelHolder prefabs)
        {
            for (var handIndex = 0; handIndex < Player.instance.hands.Length; handIndex++)
            {
                var hand = Player.instance.hands[handIndex];
                if (hand != null)
                {
                    if (hand.handType == SteamVR_Input_Sources.RightHand)
                        hand.SetRenderModel(prefabs.rightPrefab);
                    if (hand.handType == SteamVR_Input_Sources.LeftHand)
                        hand.SetRenderModel(prefabs.leftPrefab);
                }
            }
        }

        public void HideController()
        {
            for (var handIndex = 0; handIndex < Player.instance.hands.Length; handIndex++)
            {
                var hand = Player.instance.hands[handIndex];
                if (hand != null)
                {
                    hand.HideController(true);
                }
            }
        }
    }
}