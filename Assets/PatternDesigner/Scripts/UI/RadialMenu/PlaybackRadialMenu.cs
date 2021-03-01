using Assets.PatternDesigner.Scripts.PaintSystem.Player;
using Assets.PatternDesigner.Scripts.PaintSystem.Stroke;
using Assets.PatternDesigner.Scripts.Util;
using Assets.PatternDesigner.Scripts.Values;
using Assets.PatternDesigner.Scripts.Values.Resources;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PatternDesigner.Scripts.UI.RadialMenu
{
    /// <summary>
    ///     Represents the secondary radial menu for controlling player and more.
    /// </summary>
    public class PlaybackRadialMenu : PatternRadialMenu
    {
        [SerializeField] public Button btnSpeed, btnTime;

        private ButtonHandler buttonHandler;

        [SerializeField] private ControllerUI controllerUI;
        private TactilePlayer player;

        [SerializeField] private VRSlider slider;
        private ToggleHandler toggleHandler;

        [SerializeField] public Toggle toggleLoop, togglePlay, toggleRecord;

        [SerializeField] public TextMesh trackpadDescText;
        #region Initialization
        protected override void Awake()
        {
            base.Awake();

            paintMode.pattern.StrokeAdded += OnStrokeAdded;
            paintMode.pattern.StrokeRemoved += OnStrokeRemoved;

            player = paintMode.player;

            buttonHandler = new ButtonHandler(
                ButtonHandler.newKeyValuePair(btnSpeed, OnSpeedClicked),
                ButtonHandler.newKeyValuePair(btnTime, OnTimeClicked)
            );

            toggleHandler = new ToggleHandler(
                ToggleHandler.newKeyValuePair(togglePlay, OnPlayClicked),
                ToggleHandler.newKeyValuePair(toggleLoop, OnLoopClicked),
                ToggleHandler.newKeyValuePair(toggleRecord, OnRecordClicked)
            );
        }

        // initialization
        protected override void Start()
        {
            base.Start();

            slider.OnSliderExit += OnSliderExit;

            player.PlayStarted += OnPatternPlay;
            player.PlayStopped += OnPatternStop;

            buttonHandler.RegisterActions();
            toggleHandler.RegisterActions();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            UpdateToggleText();

            EnableButtons(paintMode.pattern.strokes.Count);
            DisableButtons(paintMode.pattern.strokes.Count);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            slider.OnSliderExit -= OnSliderExit;
            player.PlayStarted -= OnPatternPlay;
            player.PlayStopped -= OnPatternStop;
            buttonHandler.UnregisterActions();
            toggleHandler.UnregisterActions();
        }

        #endregion

        #region buttonPressEvents
        /// <summary>
        /// Updates the text on the controller.
        /// </summary>
        public void UpdateToggleText()
        {
            var text = ResourcesManager.Get(Strings.PLAY);
            if (player.IsPlaying())
                text = ResourcesManager.Get(Strings.STOP);
            togglePlay.GetComponentInChildren<Text>().text = text;

            var loopText = ResourcesManager.Get(Strings.ENABLE_LOOP);
            if (player.IsLooping())
            {
                toggleLoop.isOn = true;
                loopText = ResourcesManager.Get(Strings.DISABLE_LOOP);
            }

            toggleLoop.GetComponentInChildren<Text>().text = loopText;
        }

        protected override void OnStrokeAdded(Stroke stroke)
        {
            EnableButtons(paintMode.pattern.strokes.Count);
        }

        protected override void OnStrokeRemoved(Stroke stroke)
        {
            DisableButtons(paintMode.pattern.strokes.Count);
        }

        private void EnableButtons(int strokeAmount)
        {
            if (strokeAmount > 0)
                EnableSelectable(btnTime, togglePlay);
        }

        private void DisableButtons(int strokeAmount)
        {
            if (strokeAmount <= 0)
                DisableSelectable(btnTime, togglePlay);
        }

        private void OnPlayClicked(bool isOn)
        {
            if (player.IsPlaying() && !isOn)
                player.Stop();
            else if (isOn)
                player.Play();
            UpdateToggleText();
        }

        public void OnPatternPlay()
        {
            togglePlay.GetComponentInChildren<Text>().text = ResourcesManager.Get(Strings.STOP);
        }

        public void OnPatternStop()
        {
            togglePlay.GetComponentInChildren<Text>().text = ResourcesManager.Get(Strings.PLAY);
            togglePlay.isOn = false;
        }

        private void OnLoopClicked(bool isOn)
        {
            player.SetLooping(isOn);
            UpdateToggleText();
        }

        private void OnSpeedClicked()
        {
            var sliderValue = calcSpeedSliderValue(player.GetSpeed());
            slider.snapValue = calcSpeedSliderValue(1f);
            slider.OnValueChanged += OnSpeedSliderChanged;
            ShowSlider(sliderValue);
        }

        private float calcSpeedSliderValue(float speed)
        {
            return (speed - Constants.PLAYER_MIN_SPEED) / (Constants.PLAYER_MAX_SPEED - Constants.PLAYER_MIN_SPEED);
        }

        private void OnTimeClicked()
        {
            player.Stop();

            var sliderValue = player.GetTime() / player.GetDuration();

            slider.OnValueChanged += OnTimeSliderChanged;
            ShowSlider(sliderValue);
        }

        private void OnRecordClicked(bool isOn)
        {
            player.isRecording = isOn;
        }

        private void ShowSlider(float value)
        {
            inputHandler.UnregisterActions();
            controllerUI.inputHandler.UnregisterActions();
            slider.Show(value);
        }

        private void OnSpeedSliderChanged(float value)
        {
            var speed = value * (Constants.PLAYER_MAX_SPEED - Constants.PLAYER_MIN_SPEED) + Constants.PLAYER_MIN_SPEED;
            player.SetSpeed(speed);
            trackpadDescText.text = ResourcesManager.Get(Strings.SLIDER_SPEED, speed);
        }

        private void OnTimeSliderChanged(float value)
        {
            var time = value * player.GetDuration();
            if (time < 0)
                time = 0f;
            player.SetTime(time);
            trackpadDescText.text = ResourcesManager.Get(Strings.SLIDER_TIME_DESC);
        }

        private void OnSliderExit(float value, bool shouldSave)
        {
            slider.snapValue = -1f;
            controllerUI.inputHandler.RegisterActions();
            if (shouldSave)
                controllerUI.Show();
            trackpadDescText.text = ResourcesManager.Get(Strings.SHOW_MENU);
            slider.OnValueChanged -= OnSpeedSliderChanged;
            slider.OnValueChanged -= OnTimeSliderChanged;
        }
        #endregion
    }
}