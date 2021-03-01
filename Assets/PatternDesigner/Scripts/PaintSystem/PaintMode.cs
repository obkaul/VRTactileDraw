using Assets.PatternDesigner.Scripts.HapticHead;
using Assets.PatternDesigner.Scripts.misc;
using Assets.PatternDesigner.Scripts.PaintSystem.Evaluator;
using Assets.PatternDesigner.Scripts.PaintSystem.Player;
using Assets.PatternDesigner.Scripts.Pattern;
using Assets.PatternDesigner.Scripts.UI.HeadmountedDisplay;
using Assets.PatternDesigner.Scripts.UI.MainMenu;
using Assets.PatternDesigner.Scripts.Util;
using Assets.PatternDesigner.Scripts.Values;
using HTC.UnityPlugin.Vive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Valve.VR;

namespace Assets.PatternDesigner.Scripts.PaintSystem
{
    /// <summary>
    ///     This class controls the behaviour while using the paint mode.
    /// </summary>
    [DisallowMultipleComponent]
    public class PaintMode : PatternEditMode
    {

        private static readonly float paintThreshold = 0.05f;

        private readonly Dictionary<Stroke.Stroke, LineRenderer> lineRenderers =
            new Dictionary<Stroke.Stroke, LineRenderer>();

        private SteamInputHandler anyInputHandler;

        private PaintingTool currentPaintingTool;
        private PaintingTool.DefaultPainting defaultPaintingTool;
        private PaintingTool.EraserMode eraserModeTool;
        private bool erasing;
        // STROKING AND SELECTING //
        private SteamInputHandler leftInputHandler;

        [SerializeField] private MainUI mainUI;
        private float nextKeyTime;
        // SERIALIZE FIELDS //
        [SerializeField] private LineRenderer prefab;

        private Stroke.Stroke reflectedStroke;
        private PaintingTool.ReflectionPainting reflectionPaintingTool;
        private bool stroking;
        private float strokingValue;

        [SerializeField] public SteamVR_Action_Boolean changePaintingToolAction;
        [SerializeField] public Transform head;

        public GameObject leftControllerGO;
        public GameObject rightControllerGO;

        [SerializeField] public SteamVR_Action_Boolean selectStrokeAction;

        public float strokeTime;
        public Timeline timeline;

        public Stroke.Stroke currentStroke { get; set; }

        // COMPONENTS //
        public HeadmountedDisplay hmdDisplay { private set; get; }

        // PATTERN //

        public PaintPattern pattern { get; private set; }

        private void LateUpdate()
        {
            if (currentStroke != null)
                Paint(currentStroke);
            if (reflectedStroke != null)
                Paint(reflectedStroke);

            ShowActiveSegments();
        }

        /// <summary>
        ///     Will switch between painting modes.
        /// </summary>
        private void OnChangePaintingToolClicked(SteamVR_Action_In actionIn)
        {
            if (changePaintingToolAction.GetStateDown(SteamVR_Input_Sources.Any))
                if (currentPaintingTool is PaintingTool.DefaultPainting)
                {
                    currentPaintingTool = reflectionPaintingTool;
                }
                else if (currentPaintingTool is PaintingTool.ReflectionPainting)
                {
                    currentPaintingTool = eraserModeTool;
                    erasing = true;
                }
                else
                {
                    currentPaintingTool = defaultPaintingTool;
                    erasing = false;
                }

            hmdDisplay.SetPaintingToolName(currentPaintingTool.name);
        }

        private void OnDestroy()
        {
            leftInputHandler.UnregisterActions();
            anyInputHandler.UnregisterActions();
        }

        private void OnPatternRemoved(Pattern.Pattern pattern)
        {
        }

        private void OnStrokeAdded(Stroke.Stroke stroke)
        {
            var lineRenderer = Instantiate(prefab, transform);
            lineRenderer.enabled = true;

            lineRenderers.Add(stroke, lineRenderer);

            selectStroke(stroke);

            Paint(stroke);
        }

        private void OnStrokeEnd()
        {
            if (hmdDisplay != null)
                hmdDisplay.SetStrokingInfo(0f);
            reflectedStroke = null;

            currentPaintingTool.finishPainting();
        }

        private void OnStrokeRemoved(Stroke.Stroke stroke)
        {
            player.Stop();

            SelectStrokeByCurrentWithOffset(-1);

            if (currentStroke == stroke)
                currentStroke = null;

            var lineRenderer = lineRenderers[stroke];

            lineRenderers.Remove(stroke);
            stroke.DestroyStroke();

            Destroy(lineRenderer.gameObject);
            player.SetTime(Math.Min(timeline.time, pattern.getDuration()));
            timeline.SetDuration(pattern.getDuration());
        }

        /// <summary>
        ///     Will search for the next key by strokes. If a key is closer than 0.025 cm it will be selected.
        /// </summary>
        /// <param name="actionIn"></param>
        private void OnStrokeSelectedClicked(SteamVR_Action_In actionIn)
        {
            var start = Environment.TickCount;
            if (!selectStrokeAction.GetStateDown(SteamVR_Input_Sources.LeftHand)) return;
            var probe = new Ray(leftControllerGO.transform.position,
                leftControllerGO.transform.rotation * Vector3.forward);
            if (!Physics.Raycast(probe, out var hit, 50f, LayerMask.GetMask("Interface/Model"))) return;
            foreach (var stroke in from stroke in lineRenderers.Keys
                                   from key in stroke.keys
                                   let dist = Vector3.Distance(key.position, hit.point)
                                   where dist < 0.025
                                   select stroke)
            {
                currentStroke?.myMarker.Select(false);
                selectStroke(stroke);
                currentStroke?.myMarker.Select(true);
                return;
            }
        }

        private void OnStrokeStart()
        {
            player.Stop();

            var currentPatternColorIndex = pattern.length;
            if (currentPatternColorIndex > 0)
                currentPatternColorIndex = erasing ? -2 : pattern.getNextUnusedColorIndex(pattern.strokes);

            if (currentPaintingTool is PaintingTool.ReflectionPainting painting)
            {
                var reflected = new Stroke.Stroke(currentPatternColorIndex);

                pattern.AddStroke(reflected);
                reflectedStroke = reflected;
                painting.reflectedStroke = reflectedStroke;
            }
            else
            {
                reflectedStroke = null;
            }

            var stroke = new Stroke.Stroke(currentPatternColorIndex);

            if (reflectedStroke != null)
            {
            }

            pattern.AddStroke(stroke);

            strokeTime = timeline.time;
            selectStroke(stroke);
        }

        private void Paint(Stroke.Stroke stroke)
        {
            if (!lineRenderers.ContainsKey(stroke))
                return;
            var keys = stroke.keys;
            var lineRenderer = lineRenderers[stroke];

            var gradient = new Gradient();
            gradient.SetKeys(
                new[] { new GradientColorKey(stroke.color, 0) },
                new[] { new GradientAlphaKey(1, 0) }
            );

            lineRenderer.colorGradient = gradient;
            lineRenderer.positionCount = keys.Count;

            if (keys.Count <= 0) return;
            var startTime = keys[0].time;
            var endTime = keys[keys.Count - 1].time;
            var duration = endTime - startTime;
            var widthCurve = new AnimationCurve();

            for (var i = 0; i < keys.Count; i++)
            {
                lineRenderer.SetPosition(i, keys[i].position * 1.05f);
                widthCurve.AddKey((keys[i].time - startTime) / duration, keys[i].intensity);
            }

            lineRenderer.widthCurve = widthCurve;
            lineRenderer.widthMultiplier = 0.01f;
        }

        private void selectStroke(Stroke.Stroke stroke)
        {
            // unselect all others
            foreach (var myStroke in pattern.strokes.Where(myStroke => myStroke.myMarker != null))
            {
                myStroke.myMarker.Select(false);
                setGradientAlpha(myStroke, 0.4f);
            }

            currentStroke = stroke;
            if (currentStroke.myMarker != null) currentStroke.myMarker.Select(true);
        }

        private void setGradientAlpha(Stroke.Stroke myStroke, float alpha)
        {
            if (myStroke == null || !lineRenderers.ContainsKey(myStroke)) return;
            var lineRenderer = lineRenderers[myStroke];
            if (lineRenderer == null) return;
            var gradient = lineRenderer.colorGradient;
            gradient.SetKeys(gradient.colorKeys, new[] { new GradientAlphaKey(alpha, 0) });

            lineRenderer.colorGradient = gradient;
        }

        private void ShowActiveSegments()
        {
            foreach (var stroke in lineRenderers.Keys)
            {
                var lineRenderer = lineRenderers[stroke];
                var gradient = lineRenderer.colorGradient;
                var time = timeline.time;

                if (time < stroke.startTime)
                    continue;
                if (time > stroke.endTime)
                    continue;

                var point = (time - stroke.startTime) / stroke.duration;
                const float range = 0.1f;

                gradient.SetKeys(
                    new[]
                    {
                        new GradientColorKey(stroke.color, 0),
                        new GradientColorKey(stroke.color, point - range),
                        new GradientColorKey(Color.white, point),
                        new GradientColorKey(stroke.color, point + range),
                        new GradientColorKey(stroke.color, 1)
                    },
                    new[] { gradient.alphaKeys[0] }
                );

                lineRenderer.colorGradient = gradient;
            }
        }

        private void Start()
        {
            leftInputHandler.RegisterActions();
            anyInputHandler.RegisterActions();
            GetComponentInChildren<VibratorMesh>().snapVibratorsToMesh();
        }

        protected override void Awake()
        {
            base.Awake();
            player = GetComponentInParent<TactilePlayer>();
            timeline = player.timeline;
            hmdDisplay = GetComponentInChildren<HeadmountedDisplay>();

            leftInputHandler = new SteamInputHandler(
                SteamVR_Input_Sources.LeftHand,
                SteamInputHandler.newKeyValuePair(selectStrokeAction, OnStrokeSelectedClicked)
            );

            anyInputHandler = new SteamInputHandler(
                SteamVR_Input_Sources.Any,
                SteamInputHandler.newKeyValuePair(changePaintingToolAction, OnChangePaintingToolClicked)
            );
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            pattern.StrokeAdded -= OnStrokeAdded;
            pattern.StrokeRemoved -= OnStrokeRemoved;
            PatternManager.Removed -= OnPatternRemoved;
            timeline.onTimeChange.RemoveListener(OnTimeChange);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            pattern.StrokeAdded += OnStrokeAdded;
            pattern.StrokeRemoved += OnStrokeRemoved;
            PatternManager.Removed += OnPatternRemoved;
            timeline.onTimeChange.AddListener(OnTimeChange);
        }

        protected override void Update()
        {
            hmdDisplay.recordingModeActiveIndicator(player.isRecording);

            if (ViveInput.GetPressDown(HandRole.LeftHand, ControllerButton.Menu) ||
                ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Menu))
            {
                Close();
                mainUI.Open();
            }

            base.Update();

            strokingValue = ViveInput.GetAxisEx(HandRole.RightHand, ControllerAxis.Trigger);

            // abort condition
            var endStrokeNow = stroking && strokingValue < paintThreshold;

            // start a new stroke if not stroking right now and over threshold
            if (strokingValue >= paintThreshold && !stroking)
            {
                OnStrokeStart();
                stroking = true;
            }

            if (stroking)
            {
                var probe = new Ray(rightControllerGO.transform.position,
                    rightControllerGO.transform.rotation * Vector3.forward);
                if (Physics.Raycast(probe, out var hit, 10f, LayerMask.GetMask("Interface/Model")))
                {
                    var distanceToPrev = float.MaxValue;
                    if (currentStroke != null && currentStroke.keys.Count > 0)
                        distanceToPrev = Vector3.Distance(currentStroke.keys.Last().position, hit.point);

                    // only add a key to our stroke in case a min time has passed and there is a min distance to the previous key
                    if (nextKeyTime <= Time.time && distanceToPrev >= Constants.MIN_DISTANCE_BETWEEN_KEYS ||
                        endStrokeNow)
                    {
                        nextKeyTime = Time.time + 1f / Constants.SAMPLE_RATE;
                        var currentIntensities =
                            currentPaintingTool.OnStroke(hit, currentStroke, strokeTime, strokingValue);
                        
                        if (hmdDisplay != null)
                            hmdDisplay.SetStrokingInfo(strokingValue);
                        
                        RaspberryCommandSender.Instance.updateActuators(currentIntensities, player.activeTimes);

                            if (erasing)
                            {

                            }
                            strokeTime += Time.deltaTime;
                            player.SetTime(strokeTime);


                            if (endStrokeNow)
                            {
                                if (erasing)
                                {

                                }
                            stroking = false;
                            OnStrokeEnd();
                        }
                    }

                    if (player.isRecording && !stroking && currentStroke != null && currentStroke.duration > 0)
                    {
                        strokeTime += Time.deltaTime;
                        player.SetTime(strokeTime);
                    }
                }
            }
            timeline.ReDrawAllMarkers();
        }

        /// <summary>
        ///     Closes the paint mode.
        /// </summary>
        public override void Close()
        {
            if (!gameObject.activeSelf)
                return;

            base.Close();

            foreach (var stroke in pattern.strokes)
            {
                player.Stop();

                SelectStrokeByCurrentWithOffset(-1);

                if (currentStroke == stroke)
                    currentStroke = null;

                var lineRenderer = lineRenderers[stroke];

                lineRenderers.Remove(stroke);
                stroke.HideStroke();

                Destroy(lineRenderer.gameObject);
                player.SetTime(Math.Min(timeline.time, pattern.getDuration()));
                timeline.SetDuration(pattern.getDuration());
            }
        }

        public override void OnDurationUpdate()
        {
            timeline.SetDuration(pattern.getDuration());
        }

        public void OnTimeChange(float time)
        {
            var duration = Math.Max(time, pattern.getDuration());
            timeline.SetDuration(duration);
        }

        /// <summary>
        ///     Starts the PaintMode. Call this method with the pattern.
        /// </summary>
        /// <param name="pattern">Pattern to edit.</param>
        /// <returns>Returns true for success, false otherwise</returns>
        public bool Open(Pattern.Pattern pattern)
        {
            if (gameObject.activeSelf)
                return false;

            if (!(pattern is PaintPattern)) return false;
            var paintPattern = (PaintPattern)pattern;

            this.pattern = paintPattern;

            gameObject.SetActive(true);

            player.pattern = pattern;
            player.evaluator = new ClosestEvaluator(GetComponentInChildren<VibratorMesh>(), paintPattern);

            defaultPaintingTool = new PaintingTool.DefaultPainting(player.evaluator as ClosestEvaluator);
            currentPaintingTool = defaultPaintingTool;
            hmdDisplay.SetPaintingToolName(currentPaintingTool.name);

            reflectionPaintingTool = new PaintingTool.ReflectionPainting(player.evaluator as ClosestEvaluator);

            eraserModeTool = new PaintingTool.EraserMode(player.evaluator as ClosestEvaluator);

            foreach (var stroke in paintPattern.strokes)
                OnStrokeAdded(stroke);

            player.SetTime(0);

            if (this.pattern.strokes.Count > 0) selectStroke(this.pattern.strokes[0]);

            if (!int.TryParse(Regex.Match(pattern.name, @"\d+").Value, out _))
            {
            }

            return true;
        }

        public void SelectStrokeByCurrentWithOffset(int offset)
        {
            if (pattern.strokes.Count == 0)
                return;
            var index = pattern.strokes.IndexOf(currentStroke) + offset;

            if (index >= pattern.strokes.Count)
                index = 0;
            else if (index < 0)
                index = pattern.strokes.Count - 1;
            selectStroke(pattern.strokes[index]);
        }

    }
}