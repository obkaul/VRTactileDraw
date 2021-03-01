using Assets.PatternDesigner.Scripts.UI.RadialMenu;
using Assets.PatternDesigner.Scripts.Util;
using Assets.PatternDesigner.Scripts.Values;
using Assets.PatternDesigner.Scripts.Values.Resources;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PatternDesigner.Scripts.PaintSystem.UI
{
    /// <summary>
    ///     Represents the primary radial menu in paint mode.
    /// </summary>
    public class PaintModeRadialMenu : PatternRadialMenu
    {
        [SerializeField]
        private Button btnDeleteStroke, btnPrevStroke, btnNextStroke, btnSetStrokeTime, btnSetStrokeDuration;

        private ButtonHandler buttonHandler;

        [SerializeField] private ControllerUI controllerUI;

        private float fallbackStartTime = -1f, fallbackDuration = -1f;

        [SerializeField] private VRSlider slider;

        [SerializeField] public TextMesh trackpadDescText;

        protected override void Awake()
        {
            base.Awake();

            buttonHandler = new ButtonHandler(
                ButtonHandler.newKeyValuePair(btnDeleteStroke, OnDeleteStrokeClicked)
            );
        }

        // Use this for initialization
        protected override void Start()
        {
            base.Start();

            slider.OnSliderExit += OnSliderExit;

            buttonHandler.RegisterActions();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            slider.OnSliderExit -= OnSliderExit;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EnableButtons(paintMode.pattern.strokes.Count);
            DisableButtons(paintMode.pattern.strokes.Count);
        }

        protected override void OnStrokeAdded(Stroke.Stroke stroke)
        {
            EnableButtons(paintMode.pattern.strokes.Count);
        }

        protected override void OnStrokeRemoved(Stroke.Stroke stroke)
        {
            DisableButtons(paintMode.pattern.strokes.Count);
        }

        private void EnableButtons(int strokeAmount)
        {
            if (strokeAmount > 0)
            {
                EnableSelectable(btnDeleteStroke, btnSetStrokeTime, btnSetStrokeDuration);
                if (strokeAmount > 1)
                    EnableSelectable(btnPrevStroke, btnNextStroke);
            }
        }

        private void DisableButtons(int strokeAmount)
        {
            if (strokeAmount <= 1)
            {
                DisableSelectable(btnPrevStroke, btnNextStroke);

                if (strokeAmount <= 0)
                    DisableSelectable(btnDeleteStroke, btnSetStrokeTime, btnSetStrokeDuration);
            }
        }

        /// <summary>
        /// Deletes the stroke and removes it from the scene
        /// </summary>
        private void OnDeleteStrokeClicked()
        {
            var stroke = paintMode.currentStroke;
            if (stroke == null)
                return;

            paintMode.pattern.RemoveStroke(stroke);
            stroke.DestroyStroke();
            
            //patterns consist of strokes, so basically a null check if there even are strokes
            if (((PaintPattern)paintMode.player.pattern).length <= 0)
            {
                paintMode.player.Stop();
                paintMode.player.SetTime(0);
                paintMode.strokeTime = 0;
            }
            //make sure that player time is not larger than pattern time, if so set the player time to the pattern time
            else if (paintMode.player.GetTime() > ((PaintPattern)paintMode.player.pattern).getDuration())
            {
                var currentTime = ((PaintPattern)paintMode.player.pattern).getDuration();

                paintMode.player.SetTime(currentTime);
            }
        }

        public void OnSetDurationClicked()
        {
            var currentStroke = paintMode.currentStroke;
            if (currentStroke == null)
                return;

            var patternDuration = paintMode.player.GetDuration();
            fallbackDuration = currentStroke.duration;

            slider.OnValueChanged += OnDurationSliderChanged;

            paintMode.player.Stop();

            var value = 0.5f;

            if (patternDuration > 0 && fallbackDuration > 0)
                value = calcDurationSliderValue(fallbackDuration, patternDuration - currentStroke.startTime + 1);

            ShowSlider(value);
        }

        private float calcDurationSliderValue(float duration, float maxDuration)
        {
            return (duration - Constants.STROKE_MIN_DURATION) / (maxDuration - Constants.STROKE_MIN_DURATION);
        }

        public void OnSetStartTimeClicked()
        {
            var currentStroke = paintMode.currentStroke;
            if (currentStroke == null)
                return;

            var duration = paintMode.player.GetDuration();
            fallbackStartTime = currentStroke.startTime;

            slider.OnValueChanged += OnStartTimeSliderChanged;

            paintMode.player.Stop();

            var value = 0f;

            if (duration > 0 && fallbackStartTime >= 0)
                value = fallbackStartTime / duration;

            ShowSlider(value);
        }

        private void ShowSlider(float value)
        {
            inputHandler.UnregisterActions();
            controllerUI.inputHandler.UnregisterActions();
            slider.Show(value);
        }

        private void OnStartTimeSliderChanged(float value)
        {
            var time = paintMode.player.GetDuration() * value;
            paintMode.currentStroke.ChangeStartTime(time);
            trackpadDescText.text = ResourcesManager.GetSecondsText(time);
            paintMode.OnDurationUpdate();
        }

        private void OnDurationSliderChanged(float value)
        {
            var duration =
                (paintMode.player.GetDuration() - paintMode.currentStroke.startTime + 1 -
                 Constants.STROKE_MIN_DURATION) * value + Constants.STROKE_MIN_DURATION;
            paintMode.currentStroke.duration = duration;
            paintMode.OnTimeChange(duration);

            trackpadDescText.text = ResourcesManager.GetSecondsText(duration);
        }

        private void OnSliderExit(float value, bool shouldSave)
        {
            var timeChange = fallbackStartTime >= 0;

            if (!shouldSave)
            {
                if (timeChange)
                    paintMode.currentStroke.ChangeStartTime(fallbackStartTime);
                else
                    paintMode.currentStroke.duration = fallbackDuration;
            }

            paintMode.pattern.strokes.Sort();

            fallbackStartTime = -1f;
            fallbackDuration = -1f;

            controllerUI.inputHandler.RegisterActions();

            if (shouldSave)
                controllerUI.Show();

            trackpadDescText.text = ResourcesManager.Get(Strings.SHOW_MENU);
            if (timeChange)
                slider.OnValueChanged -= OnStartTimeSliderChanged;
            else
                slider.OnValueChanged -= OnDurationSliderChanged;
        }
    }
}