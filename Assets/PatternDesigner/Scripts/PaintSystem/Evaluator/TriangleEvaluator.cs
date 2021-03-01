using Assets.PatternDesigner.Scripts.HapticHead;
using System.Linq;
using UnityEngine;

namespace Assets.PatternDesigner.Scripts.PaintSystem.Evaluator
{
    /// <summary>
    ///     Raycasts against the VibratorMesh collider to determine the intensitiy using barycentric coordinates of the hit on
    ///     the triangle.
    /// </summary>
    public class TriangleEvaluator : PaintEvaluator
    {
        public TriangleEvaluator(VibratorMesh vibratorMesh, PaintPattern pattern) : base(vibratorMesh, pattern.strokes)
        {
        }

        public override Vector3 Evaluate(float time, float[] intensities)
        {
            var lastPosition = new Vector3();

            //set all intesities to 0
            for (var i = 0; i < intensities.Length; i++)
                intensities[i] = 0;

            //for every key in stroke
            foreach (var keys in strokes.Select(stroke => stroke.keys))
                for (var j = 0; j < keys.Count - 1; j++)
                {
                    var left = keys[j];
                    var right = keys[j + 1];
                    var strokeIntensityAtPoint = (keys[j].intensity + keys[j + 1].intensity) / 2f;

                    if (!(time >= left.time) || !(time <= right.time)) continue;
                    var position = Vector3.Lerp(left.position, right.position, time - left.time);
                    lastPosition = position;

                    var direction = (-position).normalized;
                    var start = position - direction;

                    Physics.Raycast(start, direction, out var hit, Mathf.Infinity,
                        LayerMask.GetMask("Interface/VibratorMesh"));

                    var triangle = vibratorMesh.GetIdTriangle(hit.triangleIndex);
                    var coordinate = hit.barycentricCoordinate;

                    var x = triangle[0];
                    var y = triangle[1];
                    var z = triangle[2];
                    if (x >= 0) intensities[x] = Mathf.Clamp01(intensities[x] + strokeIntensityAtPoint * coordinate.x);
                    if (y >= 0) intensities[y] = Mathf.Clamp01(intensities[y] + strokeIntensityAtPoint * coordinate.y);
                    if (z >= 0) intensities[z] = Mathf.Clamp01(intensities[z] + strokeIntensityAtPoint * coordinate.z);
                }

            return lastPosition;
        }
    }
}