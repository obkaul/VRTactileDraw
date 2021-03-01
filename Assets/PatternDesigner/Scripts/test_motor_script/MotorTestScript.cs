using Assets.PatternDesigner.Scripts.HapticHead;
using UnityEngine;

namespace Assets.PatternDesigner.Scripts.misc
{
    public class MotorTestScript : MonoBehaviour
    {
        public TextMesh infoTextMesh;

        public float motorTestIntensity = 1f;
        private float[] vibratorActiveTimes;

        private int vibratorId;
        private float[] vibratorIntensities;

        // Use this for initialization
        private void Start()
        {
            infoTextMesh.text = "Motor 0 deactivated, press Space to activate";

            vibratorIntensities = new float[VibratorMesh.VIBRATOR_COUNT];
            vibratorActiveTimes = new float[VibratorMesh.VIBRATOR_COUNT];

            print("Detected " + VibratorMesh.VIBRATOR_COUNT + " total actuators.");

            for (var i = 0; i < vibratorActiveTimes.Length; i++) vibratorActiveTimes[i] = 5f;
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                if (vibratorId > 0)
                {
                    vibratorId -= 1;
                    infoTextMesh.text = "Motor " + vibratorId;
                }

            if (Input.GetKeyDown(KeyCode.RightArrow))
                if (vibratorId + 1 < VibratorMesh.VIBRATOR_COUNT)
                {
                    vibratorId += 1;
                    infoTextMesh.text = "Motor " + vibratorId;
                }

            if (Input.GetKey(KeyCode.Space))
            {
                // VibratorLocationManager.Instance.getCommandSender().deactivateFeedback();
                for (var i = 0; i < vibratorIntensities.Length; i++)
                    if (i == vibratorId)
                        vibratorIntensities[i] = motorTestIntensity;
                    else
                        vibratorIntensities[i] = 0;
                infoTextMesh.text = "Motor " + vibratorId + " activated";
                RaspberryCommandSender.Instance.updateActuators(vibratorIntensities, vibratorActiveTimes);
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                // VibratorLocationManager.Instance.getCommandSender().deactivateFeedback();
                for (var i = 0; i < vibratorIntensities.Length; i++) vibratorIntensities[i] = motorTestIntensity;
                infoTextMesh.text = "All motors activated!";
                RaspberryCommandSender.Instance.updateActuators(vibratorIntensities, vibratorActiveTimes);
                //VibratorLocationManager.Instance.updateColor(vibratorId, activatedColor);
            }
            else
            {
                infoTextMesh.text = "Motor " + vibratorId + " deactivated";
                RaspberryCommandSender.Instance.deactivateFeedback();
            }
        }
    }
}