using Assets.PatternDesigner.Scripts.Util;
using Scripts.Layout;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;

namespace Assets.PatternDesigner.Scripts.UI.RadialMenu
{
    /// <summary>
    ///     Represents the radial menu. Handles selection and clicking Selectables.
    /// </summary>
    public class RadialMenu : RadialLayout
    {
        public SteamVR_Action_Boolean clickAction = SteamVR_Input.__actions_default_in_TouchTouchPad;

        [SerializeField] private List<Selectable> hideOnClick;

        protected SteamInputHandler inputHandler;

        [SerializeField] private SteamVR_Input_Sources inputSource;
        public SteamVR_Action_Boolean touchedTouchpadAction = SteamVR_Input.__actions_default_in_TouchTouchPad;
        public SteamVR_Action_Vector2 touchpadAction = SteamVR_Input.__actions_default_in_TouchpadNavigate;

        private SteamVR_TrackedObject trackedObject;

        public event Action OnClick;

        protected override void OnEnable()
        {
            base.OnEnable();

            inputHandler = new SteamInputHandler(
                inputSource,
                SteamInputHandler.newKeyValuePair(clickAction, OnClickAction),
                SteamInputHandler.newKeyValuePair(touchpadAction, OnTrackpadPositionChanged),
                SteamInputHandler.newKeyValuePair(touchedTouchpadAction, OnTouchpadTouched)
            );

            inputHandler.RegisterActions();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            inputHandler.UnregisterActions();
        }

        private void OnClickAction(SteamVR_Action_In actionIn)
        {
            if (clickAction.GetStateDown(inputSource)) return;
            var selectedGo = EventSystem.current.currentSelectedGameObject;

            if (selectedGo == null)
                return;

            var selectable = selectedGo.GetComponent<Selectable>();

            if (selectable != null)
            {
                // Execute click on selectable
                ExecuteEvents.Execute(selectable.gameObject, new BaseEventData(EventSystem.current),
                    ExecuteEvents.submitHandler);
                OnClick?.Invoke();
            }

            var controllerUi = GetComponentInParent<ControllerUI>();
            if (controllerUi != null && hideOnClick.Contains(selectable))
                controllerUi.Hide();
        }

        private void OnTouchpadTouched(SteamVR_Action_In actionIn)
        {
            if (touchedTouchpadAction.GetStateUp(inputSource))
                EventSystem.current
                    .SetSelectedGameObject(null); //clear the old selection for directly selecting another one.
        }

        /// <summary>
        ///     Searches the closest button when trackpad touched and select it
        /// </summary>
        private void OnTrackpadPositionChanged(SteamVR_Action_In actionIn)
        {
            var axis = touchpadAction.GetAxis(inputSource);

            // WORKAROUND: jumps to (0,0) when release touchpad touch
            if (axis.x == 0 && axis.y == 0)
                return;

            var rect = ((RectTransform)gameObject.transform).rect;
            var goTransform = gameObject.transform;

            var searchedPos = goTransform.localPosition + new Vector3(axis.x * (rect.width / 2),
                axis.y * (rect.height / 2), goTransform.localPosition.z);

            var closest = GetClosestEnemy(searchedPos);
            if (closest == null) return;
            var selectable = closest.GetComponent<Selectable>();
            if (selectable == null) return;
            if (selectable.interactable)
                selectable.Select();
            else
                EventSystem.current.SetSelectedGameObject(null);
        }

        /// <summary>
        ///     Searches for the closest menu dot with trackpad touch position as origin.
        /// </summary>
        /// <param name="currentPosition"> trackpad touch position scaled to radial width and height.</param>
        /// <returns>Closes menu dot as transform component</returns>
        private Transform GetClosestEnemy(Vector3 currentPosition)
        {
            Transform tMin = null;
            var minDist = Mathf.Infinity;
            foreach (Transform t in transform)
            {
                var dist = Vector3.Distance(t.localPosition, currentPosition);
                if (!(dist < minDist)) continue;
                tMin = t;
                minDist = dist;
            }

            return tMin;
        }

        /// <summary>
        ///     Will disable selectables, if they are not usable in current situation.
        /// </summary>
        /// <param name="selectable">Not available selectables</param>
        protected void DisableSelectable(params Selectable[] selectable)
        {
            foreach (var s in selectable)
            {
                s.interactable = false;
                var text = s.gameObject.GetComponentInChildren<Text>(true);
                if (text != null)
                    text.color = Color.gray;
            }
        }

        /// <summary>
        ///     Will enable selectables, if they are usable in current situation.
        /// </summary>
        /// <param name="selectable">Available selectables</param>
        protected void EnableSelectable(params Selectable[] selectable)
        {
            foreach (var s in selectable)
            {
                s.interactable = true;
                var text = s.gameObject.GetComponentInChildren<Text>(true);
                if (text != null)
                    text.color = Color.white;
            }
        }
    }
}