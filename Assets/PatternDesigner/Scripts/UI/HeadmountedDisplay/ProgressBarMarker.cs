using Assets.PatternDesigner.Scripts.PaintSystem.Stroke;
using Assets.PatternDesigner.Scripts.Values.Resources;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PatternDesigner.Scripts.UI.HeadmountedDisplay
{
    public class ProgressBarMarker : MonoBehaviour
    {
        private Image background;

        private int defaultTimeFontSize;

        [SerializeField] private Text startTime, endTime;

        public Stroke stroke;

        private Timeline timeline;
        
        /// <summary>
        /// Highlights currently selected stroke by adjusting alpha to 0.93
        /// </summary>
        private Color colorSelected
        {
            get
            {
                var c = stroke?.color ?? new Color();
                c.a = 0.93f;
                return c;
            }
        }

        /// <summary>
        /// The unselected strokes get set to 0.7 alpha to highlight selected stroke
        /// </summary>
        private Color colorDeselected
        {
            get
            {
                var c = stroke?.color ?? new Color();
                c.a = 0.7f;
                return c;
            }
        }

        public bool isSelected { get; private set; }

        public bool liteMode { private set; get; }

        public bool showTimeLabel { private set; get; }

        public event Action OnMarkerAutoRefreshed;

        private void Awake()
        {
            defaultTimeFontSize = startTime.fontSize;
        }
        
        public void Init(Timeline timeline, Stroke stroke, bool showTimeLabel)
        {
            isSelected = false;
            if (background == null || stroke == null)
            {
                Debug.Log("Background or stroke is null.");
                return;
            }

            this.stroke = stroke;

            this.timeline = timeline;
            timeline.onTimeChange.AddListener(TimelineTimeChanged);
            stroke.KeysChanged += KeysChanged;
            Refresh(showTimeLabel);
        }

        private void TimelineTimeChanged(float time)
        {
            if (!timeline.IsPlaying()) return;
            if (time >= stroke.startTime && time <= stroke.endTime)
            {
                if (isSelected)
                    return;
                Select();
                transform.SetAsLastSibling();
                transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
            }
            else if (isSelected)
            {
                Deselect();
            }
        }

        /// <summary>
        /// When play is pressed, this function is responsible for highlighting the currently playing stroke
        /// </summary>
        /// <param name="showTimeLabel"></param>
        public void Refresh(bool showTimeLabel)
        {
            background.color = isSelected ? colorSelected : colorDeselected;

            startTime.color = stroke.color;
            startTime.text = ResourcesManager.GetSecondsText(stroke.startTime);

            endTime.color = stroke.color;
            endTime.text = ResourcesManager.GetSecondsText(stroke.endTime);

            this.showTimeLabel = showTimeLabel;

            if (showTimeLabel)
                ShowTimeLabels();
            else
                HideTimeLabels();

            ActivateLiteMode(false);

            transform.SetAsLastSibling();
            transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
            timeline.HighlightSection(this);
        }

        public void DestroyMarker()
        {
            if (stroke != null)
                stroke.KeysChanged -= KeysChanged;
            if (timeline != null)
                timeline.onTimeChange.RemoveListener(TimelineTimeChanged);
            OnMarkerAutoRefreshed = null;
            Destroy(gameObject);
        }

        private void KeysChanged(Stroke stroke)
        {
            if (stroke.keys.Count == 0)
            {
                DestroyMarker();
                return;
            }

            Refresh(showTimeLabel);
            OnMarkerAutoRefreshed?.Invoke();
        }

        public void HideTimeLabels()
        {
            showTimeLabel = false;
            ActivateStartTime(showTimeLabel);
            ActivateEndTime(showTimeLabel);
        }

        public void ShowTimeLabels()
        {
            showTimeLabel = true;
            ActivateStartTime(showTimeLabel);
            ActivateEndTime(showTimeLabel);
        }

        private void OnEnable()
        {
            background = GetComponent<Image>();
        }

        public void ActivateStartTime(bool active)
        {
            startTime.gameObject.SetActive(active);
        }

        public void ActivateEndTime(bool active)
        {
            endTime.gameObject.SetActive(active);
        }

        public void HighlightStartTime(bool highlight)
        {
            if (highlight)
                startTime.fontSize = defaultTimeFontSize + 5;
            else
                startTime.fontSize = defaultTimeFontSize - 5;
        }

        public void HighlightEndTime(bool highlight)
        {
            if (highlight)
                endTime.fontSize = defaultTimeFontSize + 5;
            else
                endTime.fontSize = defaultTimeFontSize - 5;
        }

        public void ActivateLiteMode(bool activate)
        {
            liteMode = activate;
            if (showTimeLabel)
                ActivateEndTime(!liteMode);
        }

        public float GetStartTime()
        {
            return stroke.startTime;
        }

        public float GetEndTime()
        {
            return stroke.endTime;
        }

        public void Select()
        {
            Select(true);
        }

        public void Deselect()
        {
            Select(false);
        }

        public void Select(bool select)
        {
            if (background == null)
                return;
            isSelected = select;
            background.color = select ? colorSelected : colorDeselected;
        }
    }
}