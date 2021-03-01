using Assets.PatternDesigner.Scripts.HapticHead;
using Assets.PatternDesigner.Scripts.PaintSystem.Stroke;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.PatternDesigner.Scripts.PaintSystem.Evaluator
{
    /// <summary>
    ///     Evaluates the intensity using the x closest vibrators.
    /// </summary>
    public class ClosestEvaluator : PaintEvaluator
    {
        // How many actuators should be actuated?
        private const int COUNT = 3;

        public ClosestEvaluator(VibratorMesh vibratorMesh, PaintPattern pattern) : base(vibratorMesh, pattern.strokes)
        {
        }

        /// <summary>
        ///     Evaluates the intensities array for current time in pattern
        /// </summary>
        /// <param name="time">The current time</param>
        /// <param name="intensities">The intensities array</param>
        public override Vector3 Evaluate(float time, float[] intensities)
        {
            var lastPosition = new Vector3();

            if (intensities == null)
            {
                Debug.Log("ClosestEvaluator got null intensities array!");
                return lastPosition;
            }

            // set all intensities to 0
            for (var i = 0; i < intensities.Length; i++)
                intensities[i] = 0;

            // Get every stroke in pattern
            foreach (var stroke in strokes)
            {
                var keys = stroke.keys;

                // Get every key in stroke
                for (var j = 0; j < keys.Count - 1; j++)
                {
                    var left = keys[j]; // current stroke
                    var right = keys[j + 1]; // next stroke

                    // if current time is between both keys, calculate nearest vibrators, otherwise don't need this
                    if (!(time >= left.time) || !(time <= right.time)) continue;
                    var strokeIntensityAtPoint = (left.intensity + right.intensity) / 2f; // average intensity

                    var position =
                        Vector3.Lerp(left.position, right.position,
                            time - left.time); // interpolate current position with both keys

                    CalculateIntensitiesForPosition(position, strokeIntensityAtPoint, intensities);

                    lastPosition = position;
                }
            }

            return lastPosition;
        }

        public void CalculateIntensitiesForKey(Key key, float[] intensities)
        {
            // set all intensities to 0
            //for (var i = 0; i < intensities.Length; i++) intensities[i] = 0;

            CalculateIntensitiesForPosition(key.position, key.intensity, intensities);
        }

        private void CalculateIntensitiesForPosition(Vector3 position, float strokeIntensityAtPoint,
            IList<float> intensities)
        {
            var closestVibrators =
                vibratorMesh.GetClosestVibrators(position, COUNT); // get COUNT closest vibrators at this position

            // sum the distances from position to COUNT vibrators
            var distanceSum = closestVibrators.Sum(closest => 1 / closest.Key);

            foreach (var closest in closestVibrators)
            {
                var id = closest.Value.id; // current vibrator id

                // calculate intensity for that vibrator and add to array
                var percentage = 1 / closest.Key / distanceSum;
                if (intensities.Count > id && id >= 0)
                {
                    intensities[id] = Mathf.Clamp01(intensities[id] + strokeIntensityAtPoint * percentage);
                    //Debug.Log("Actuating vibrator " + id + " with intensity " + intensities[id]);
                }
            }
        }
    }
}