using Assets.PatternDesigner.Scripts.Util;
using Assets.PatternDesigner.Scripts.Values;
using System;
using UnityEngine;
using Valve.VR;

namespace Assets.PatternDesigner.Scripts.UI.RadialMenu
{
    /// <summary>
    ///     Uses the VRProgressBar. Represents a slider on controller.
    /// </summary>
    public class VRSlider : VRProgressBar
    {
        private readonly float trackpadRange = 1.8f;
        [SerializeField] public SteamVR_Action_Vector2 changeSliderAction;

        private float initValue = -1;

        private SteamInputHandler inputHandler;

        [SerializeField] private SteamVR_Input_Sources inputSource;

        [SerializeField] private Orientation orientation;

        private int sliderStarted = -1;

        [SerializeField] public float snapValue = -1f;

        [SerializeField] public SteamVR_Action_Boolean touchSliderAction, clickedSliderAction;

        public event Action<float, bool> OnSliderExit;

        protected override void OnEnable()
        {
            base.OnEnable();

            inputHandler = new SteamInputHandler(
                inputSource,
                SteamInputHandler.newKeyValuePair(changeSliderAction, OnChangeSliderAction),
                SteamInputHandler.newKeyValuePair(touchSliderAction, OnSliderTouched),
                SteamInputHandler.newKeyValuePair(clickedSliderAction, OnSliderClicked)
            );

            inputHandler.RegisterActions();
        }

        private void OnDisable()
        {
            inputHandler.UnregisterActions();
        }

        /// <summary>
        ///     Show the slider with an initial value.
        /// </summary>
        /// <param name="initValue">The value that should be shown at the beginning.</param>
        public void Show(float initValue)
        {
            sliderStarted = Environment.TickCount;
            gameObject.SetActive(true);
            setValue(initValue);
            this.initValue = initValue;
        }

        /// <summary>
        ///     Cancels the slider and set the value to initValue from Show(float).
        /// </summary>
        public void Cancel()
        {
            setValue(initValue);
            initValue = -1;
            Hide(false);
        }

        /// <summary>
        ///     Hides slider without changing any value. That should be handled by callback
        /// </summary>
        /// <param name="saveValue">Should save the value? Will call callback with that</param>
        public void Hide(bool saveValue)
        {
            sliderStarted = -1;

            gameObject.SetActive(false);

            OnSliderExit?.Invoke(lastValue, saveValue);
        }

        /// <summary>
        ///     Sets the percentage value by consider snap value.
        /// </summary>
        /// <param name="value"></param>
        public override void setValue(float value)
        {
            if (snapValue != -1 && snapValue + 0.05f >= value && snapValue - 0.05f <= value)
                value = snapValue;
            base.setValue(value);
        }

        private void OnChangeSliderAction(SteamVR_Action_In actionIn)
        {
            var axis = changeSliderAction.GetAxis(inputSource);
            // WORKAROUND: jumps to (0,0) when release touchpad touch
            if (Environment.TickCount - sliderStarted <= Constants.SLIDER_NO_MANUAL_CANCEL_TIME ||
                axis.x == 0 && axis.y == 0)
                return;

            var value = axis.x;

            if (orientation == Orientation.Vertical)
                value = axis.y;
            value = Math.Max(value, -trackpadRange / 2f);
            value = Math.Min(value, trackpadRange / 2f);

            setValue(value / trackpadRange + 0.5f);
        }

        private void OnSliderTouched(SteamVR_Action_In actionIn)
        {
            if (Environment.TickCount - sliderStarted <= Constants.SLIDER_NO_MANUAL_CANCEL_TIME)
                return;
            if (touchSliderAction.GetStateUp(inputSource))
                Cancel();
        }

        private void OnSliderClicked(SteamVR_Action_In actionIn)
        {
            if (clickedSliderAction.GetStateUp(inputSource))
                Hide(true);
        }

        private enum Orientation
        {
            Vertical,
            Horizontal
        }
    }
}