using Assets.PatternDesigner.Scripts.HapticHead;
using Assets.PatternDesigner.Scripts.misc;
using Assets.PatternDesigner.Scripts.PaintSystem.Evaluator;
using Assets.PatternDesigner.Scripts.UI.HeadmountedDisplay;
using Assets.PatternDesigner.Scripts.Values;
using System;
using UnityEngine;

namespace Assets.PatternDesigner.Scripts.PaintSystem.Player
{
    /// <summary>
    ///     Takes and IEvalutor and displays and send the intensities.
    /// </summary>
    [DisallowMultipleComponent]
    public class TactilePlayer : MonoBehaviour, ITactilePlayer
    {
        // Pattern and evaluator
        private Pattern.Pattern _pattern;

        public float[] activeTimes;

        [SerializeField] public GameObject debugSphere;

        // HH settings
        private float[] intensities;

        private IntensityDisplay intensityDisplay;

        private bool isInit;
        public bool isRecording = false;

        private float nextSampleTime;

        private bool playing;

        private float time;

        // Components
        [SerializeField] public Timeline timeline;

        public Pattern.Pattern pattern
        {
            get => _pattern;
            set
            {
                OnPatternChanged?.Invoke(pattern, value);

                _pattern = value;
            }
        }

        // SETTER / GETTER //

        public void SetTime(float time)
        {
            this.time = time;
            timeline.SetTime(time);
        }
        public void SetLooping(bool looping)
        {
            if (pattern != null)
                pattern.looping = looping;
        }

        public float GetSpeed()
        {
            return pattern?.speed ?? Constants.PLAYER_DEFAULT_SPEED;
        }

        public void SetSpeed(float speed)
        {
            if (pattern != null)
                pattern.speed = speed;
        }
        public IEvaluator evaluator { get; set; }

        // PLAYBACK CONTROL //

        public void Play()
        {
            if (time >= GetDuration())
                SetTime(0);
            playing = true;

            PlayStarted?.Invoke();
        }

        public void Stop()
        {
            playing = false;
            
            PlayStopped?.Invoke();
        }

        public bool IsLooping()
        {
            return pattern != null && pattern.looping;
        }


        public float GetTime()
        {
            return time;
        }

        public bool IsPlaying()
        {
            return playing;
        }

        public float GetDuration()
        {
            return pattern?.getDuration() ?? 0f;
        }

        // Actions
        public event Action PlayStarted;

        public event Action PlayStopped;

        public event Action<Pattern.Pattern, Pattern.Pattern> OnPatternChanged;

        private void OnEnable()
        {
            if (timeline != null)
                timeline.onTimeChange.AddListener(OnTimeChange);

            intensityDisplay = GetComponentInChildren<IntensityDisplay>(false);
        }

        private void OnDisable()
        {
            Stop();

            if (timeline != null)
                timeline.onTimeChange.RemoveListener(OnTimeChange);

            if (VibratorMesh.VIBRATOR_COUNT > 0)
                intensityDisplay.Set(new float[VibratorMesh.VIBRATOR_COUNT]);
        }

        private void Update()
        {
            if (!isInit)
            {
                if (VibratorMesh.VIBRATOR_COUNT > 0)
                {
                    intensities = new float[VibratorMesh.VIBRATOR_COUNT];
                    activeTimes = new float[VibratorMesh.VIBRATOR_COUNT];

                    for (var i = 0; i < activeTimes.Length; i++)
                        activeTimes[i] = 0.05f;

                    isInit = true;
                }
                else
                {
                    return;
                }
            }

            if (pattern == null || !playing)
                return;

            if (nextSampleTime <= Time.unscaledTime)
            {
                nextSampleTime = Time.unscaledTime + 1f / Constants.SAMPLE_RATE;

                RaspberryCommandSender.Instance.updateActuators(intensities, activeTimes);
            }

            time += Time.deltaTime * GetSpeed();

            if (IsLooping())
                time %= pattern.getDuration();
            else if (time >= pattern.getDuration()) Stop();

            timeline.SetTime(time);
        }

        // EVENT HANDLING //

        private void OnTimeChange(float time)
        {
            var lastPosition = evaluator.Evaluate(time, intensities);
            if (debugSphere != null) debugSphere.transform.position = lastPosition;

            if (intensities != null && intensities.Length > 0)
                intensityDisplay.Set(intensities);
        }

        private void OnPlayButtonClick()
        {
            if (playing)
                Stop();
            else
                Play();
        }
    }
}