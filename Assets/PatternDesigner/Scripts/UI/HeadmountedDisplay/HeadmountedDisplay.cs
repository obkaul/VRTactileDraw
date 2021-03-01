using Assets.PatternDesigner.Scripts.PaintSystem.Player;
using Assets.PatternDesigner.Scripts.Values.Resources;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PatternDesigner.Scripts.UI.HeadmountedDisplay
{
    public class HeadmountedDisplay : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;

        [SerializeField] private GameObject loopIndicator, recordingIcon;

        private TactilePlayer player;

        [SerializeField] private Sprite slowSpeed, normalSpeed, fastSpeed;

        [SerializeField] private Image speedIcon;

        [SerializeField] private Text speedIndicator, strokingText, paintingToolText;

        private void OnEnable()
        {
            player = GetComponentInParent<TactilePlayer>();
            player.OnPatternChanged += OnPatternChanged;
        }

        private void OnDestroy()
        {
            player.OnPatternChanged -= OnPatternChanged;

            if (player.pattern == null) return;
            player.pattern.OnLoopChanged -= LoopChanged;
            player.pattern.OnSpeedChanged -= SpeedChanged;
        }

        /// <summary>
        /// Move the UI with the camera
        /// </summary>
        private void Update()
        {
            transform.position = cameraTransform.position;
            transform.Translate(Vector3.forward * 0.7f);
            Transform transform1;
            (transform1 = transform).Translate(Vector3.up * 0.2f);
            transform1.rotation = cameraTransform.rotation;
        }

        private void OnPatternChanged(Pattern.Pattern old, Pattern.Pattern newPattern)
        {
            if (newPattern == null)
                return;
            newPattern.OnLoopChanged += LoopChanged;
            newPattern.OnSpeedChanged += SpeedChanged;
            LoopChanged(newPattern.looping);
            SpeedChanged(newPattern.speed);
        }

        private void LoopChanged(bool looping)
        {
            loopIndicator.SetActive(looping);
        }

        public void recordingModeActiveIndicator(bool active)
        {
            recordingIcon.SetActive(active);
        }

        private void SpeedChanged(float speed)
        {
            speedIndicator.text = ResourcesManager.Get(Strings.SPEED, speed);
            if (speed < 1f)
                speedIcon.sprite = slowSpeed;
            else if (speed > 1f)
                speedIcon.sprite = fastSpeed;
            else
                speedIcon.sprite = normalSpeed;
        }

        /// <summary>
        /// Display the current stroking value on the UI
        /// </summary>
        public void SetStrokingInfo(float value)
        {
            strokingText.text = value > 0 ? ResourcesManager.Get(Strings.STROKING, value * 100f) : "";
        }

        public void SetPaintingToolName(string name)
        {
            paintingToolText.text = ResourcesManager.Get(Strings.PAINTING_TOOL, name);
        }
    }
}