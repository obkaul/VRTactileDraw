using Assets.PatternDesigner.Scripts.PaintSystem;
using Assets.PatternDesigner.Scripts.PaintSystem.Stroke;
using Assets.PatternDesigner.Scripts.Values.Resources;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PatternDesigner.Scripts.UI.HeadmountedDisplay
{
    /// <summary>
    ///     Represents the timeline.
    /// </summary>
    [DisallowMultipleComponent]
    public class Timeline : MonoBehaviour //, IPointerDownHandler, IDragHandler
    {
        [SerializeField] private TimeChangeEvent _onTimeChange;

        private ProgressBarMarker currentStrokeMarker;

        private float duration;

        [SerializeField] private RectTransform indicatorRect;

        [SerializeField] private ProgressBarMarker markerPrefab;

        private PaintMode paintMode;

        [SerializeField] public Image startImage, endImage;

        [SerializeField] private Text textTime, textDuration;

        private Timeframe timeframe;

        public float time { get; private set; }

        private RectTransform rectTransform => transform as RectTransform;
        public TimeChangeEvent onTimeChange => _onTimeChange;

        private void OnEnable()
        {
            paintMode = GetComponentInParent<PaintMode>();
        }

        private void Start()
        {
            paintMode.player.PlayStarted += OnPlayerPlay;
            paintMode.player.PlayStopped += OnPlayerStop;
        }

        private void OnDestroy()
        {
            paintMode.player.PlayStarted -= OnPlayerPlay;
            paintMode.player.PlayStopped -= OnPlayerStop;
            if (currentStrokeMarker != null)
                currentStrokeMarker.DestroyMarker();
        }

        private void OnPlayerPlay()
        {
        }

        private void OnPlayerStop()
        {
        }

        /// <summary>
        ///     Changes the marked line to the new selected <paramref name="newStroke" />.
        ///     It will hide and destroy the <paramref name="old" />.
        /// </summary>
        /// <param name="old">Previous selected stroke</param>
        /// <param name="newStroke">New selected Stroke</param>
        private void StrokeChanged(Stroke old, Stroke newStroke)
        {
            if (!IsPlaying() && currentStrokeMarker != null)
                currentStrokeMarker.HideTimeLabels();
            if (newStroke == null)
                return;

            currentStrokeMarker = newStroke.myMarker;
            if (currentStrokeMarker != null) currentStrokeMarker.ShowTimeLabels();
        }

        /// <summary>
        ///     Returns true, when player script is in playing mode.
        /// </summary>
        /// <returns>True if player is playing, else false.</returns>
        public bool IsPlaying()
        {
            if (paintMode == null || paintMode.player == null)
                return false;
            return paintMode.player.IsPlaying();
        }

        public ProgressBarMarker CreateStrokeMarker(Stroke stroke, bool showTimeLabel)
        {
            var marker = Instantiate(markerPrefab.gameObject, transform).GetComponent<ProgressBarMarker>();
            marker.gameObject.SetActive(true);
            return InitMarker(marker, stroke, showTimeLabel);
        }

        private ProgressBarMarker InitMarker(ProgressBarMarker marker, Stroke stroke, bool showTimeLabel)
        {
            if (stroke == null)
            {
                Debug.Log("Stroke is null");
                return null;
            }

            marker.Init(this, stroke, showTimeLabel);
            marker.Select(showTimeLabel);
            marker.OnMarkerAutoRefreshed += MarkerRefreshed;
            return marker;
        }

        private void MarkerRefreshed()
        {
        }

        public void SetTime(float time)
        {
            this.time = time;
            paintMode.strokeTime = time;

            if (currentStrokeMarker != null && currentStrokeMarker.showTimeLabel)
            {
                if (currentStrokeMarker.liteMode)
                {
                    if (time > currentStrokeMarker.GetStartTime() && time < currentStrokeMarker.GetEndTime())
                    {
                        currentStrokeMarker.ActivateStartTime(false);
                        currentStrokeMarker.ActivateEndTime(true);
                    }
                    else
                    {
                        currentStrokeMarker.ActivateStartTime(true);
                        currentStrokeMarker.ActivateEndTime(false);
                    }
                }

                if (time > currentStrokeMarker.GetStartTime() && time <= currentStrokeMarker.GetStartTime() + 0.5)
                    currentStrokeMarker.HighlightStartTime(true);
                else
                    currentStrokeMarker.HighlightStartTime(false);
                if (time < currentStrokeMarker.GetEndTime() && time >= currentStrokeMarker.GetEndTime() - 0.5)
                    currentStrokeMarker.HighlightEndTime(true);
                else
                    currentStrokeMarker.HighlightEndTime(false);
            }

            textTime.text = ResourcesManager.GetSecondsText(time);

            onTimeChange.Invoke(time);
        }

        public void SetDuration(float duration)
        {
            this.duration = duration;
            textDuration.text = ResourcesManager.GetSecondsText(duration);
        }

        public float GetDuration()
        {
            return paintMode.pattern.strokes.Select(stroke => stroke.duration + stroke.startTime)
                .Concat(new float[] { 0 }).Max();
        }

        public void ReDrawAllMarkers()
        {
            ProgressBarMarker selectedMarker = null;
            foreach (var stroke in paintMode.pattern.strokes)
            {
                stroke.createMarkerIfNoneAvailable(this);
                if (stroke == paintMode.currentStroke)
                {
                    selectedMarker = stroke.myMarker;
                    continue;
                }

                HighlightSection(stroke.myMarker);
            }

            HighlightSection(selectedMarker);

            moveMarkerBar(startImage, endImage);
        }

        public void moveMarkerBar(Image markerBar, Image markerEnd)
        {
            var imageTransform = markerBar.GetComponent<RectTransform>();
            var currentPos = imageTransform.localPosition;

            var positionOnTimeline = time / duration;

            if (positionOnTimeline >= 0 && positionOnTimeline <= 1)
            {
                var positionOnTimelineScaled =
                    positionOnTimeline * rectTransform.rect.width - rectTransform.rect.width / 2;

                currentPos.x = positionOnTimelineScaled;


                imageTransform.localPosition = currentPos;
            }
        }

        public void HighlightSection(ProgressBarMarker marker)
        {
            if (marker == null)
                return;

            float left = 0f, right = 0f;

            if (duration > 0)
            {
                left = marker.stroke.startTime / duration * rectTransform.rect.width;
                right = (duration - marker.stroke.endTime) / duration * rectTransform.rect.width;

            }

            var markerTransform = (RectTransform)marker.transform;

            markerTransform.offsetMin = new Vector2(left, markerTransform.offsetMin.y);
            markerTransform.offsetMax = new Vector2(-right, markerTransform.offsetMax.y);

            if (markerTransform.rect.width < 200) marker.ActivateLiteMode(true);
        }
    }
}