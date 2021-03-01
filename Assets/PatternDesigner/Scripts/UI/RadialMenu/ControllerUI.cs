using Assets.PatternDesigner.Scripts.Util;
using Assets.PatternDesigner.Scripts.Values.Resources;
using HTC.UnityPlugin.Vive;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using Valve.VR;

namespace Assets.PatternDesigner.Scripts.UI.RadialMenu
{
    /// <summary>
    ///     Represents all UI on controller. Will show or hide radial menu when touching trackpad.
    /// </summary>
    [DisallowMultipleComponent]
    public class ControllerUI : MonoBehaviour
    {
        public const string PATH = "ViveCameraRig/RightHand/ControllerUI";

        [SerializeField] public Hand hand;

        public SteamInputHandler inputHandler;

        [SerializeField] private SteamVR_Input_Sources inputSource;

        private bool isUiActive;

        [SerializeField] public SteamVR_Action_Boolean showUIAction;

        [SerializeField] public TextMesh trackpadDescription;

        [SerializeField] public List<GameObject> uisToShow, uisToHide;

        private ViveInput viveInput;

        private void Awake()
        {
            inputHandler = new SteamInputHandler(
                inputSource,
                SteamInputHandler.newKeyValuePair(showUIAction, OnShowActionChange)
            );
            inputHandler.RegisterActions();
        }

        private void Update()
        {
            if (ViveInput.GetPressDown(HandRole.LeftHand, ControllerButton.DPadDownTouch)) OnShowActionChange(null);
        }

        private void OnDestroy()
        {
            inputHandler.UnregisterActions();
        }

        private void OnShowActionChange(SteamVR_Action_In actionIn)
        {
            if (showUIAction.GetStateDown(inputSource))
                Show();
            else
                Hide();
        }

        public void Hide()
        {
            if (!isUiActive)
                return;
            uisToShow.ForEach(go => go.SetActive(false));
            uisToHide.ForEach(go => go.SetActive(true));
            EventSystem.current.SetSelectedGameObject(null);
            if (trackpadDescription.text == ResourcesManager.Get(Strings.CLICK_ACTION))
                trackpadDescription.text = ResourcesManager.Get(Strings.SHOW_MENU);
            isUiActive = false;
        }

        public void Show()
        {
            if (isUiActive)
                return;
            uisToHide.ForEach(go => go.SetActive(false));
            uisToShow.ForEach(go => go.SetActive(true));
            trackpadDescription.text = ResourcesManager.Get(Strings.CLICK_ACTION);
            isUiActive = true;
        }
    }
}