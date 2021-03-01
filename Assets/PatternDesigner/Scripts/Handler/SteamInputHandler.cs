using System;
using System.Collections.Generic;
using Valve.VR;

namespace Assets.PatternDesigner.Scripts.Util
{
    /// <summary>
    /// Manages steam actions
    /// </summary>
    public class SteamInputHandler : EventHandler<SteamVR_Action_In, Action<SteamVR_Action_In>>
    {
        public SteamInputHandler(SteamVR_Input_Sources inputSource,
            params KeyValuePair<SteamVR_Action_In, Action<SteamVR_Action_In>>[] actions)
            : base(
                (input, action) => input.AddOnChangeListener(action, inputSource),
                (input, action) => input.RemoveOnChangeListener(action, inputSource),
                actions
            )
        {
        }
    }
}