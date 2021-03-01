using Assets.PatternDesigner.Scripts.HapticHead;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PatternDesigner.Scripts.PaintSystem.Evaluator
{
    /// <summary>
    ///     Base Evaluator class for PaintPatterns
    /// </summary>
    public abstract class PaintEvaluator : IEvaluator
    {
        protected readonly List<Stroke.Stroke> strokes;
        protected readonly VibratorMesh vibratorMesh;

        protected PaintEvaluator(VibratorMesh vibratorMesh, List<Stroke.Stroke> strokes)
        {
            this.vibratorMesh = vibratorMesh;
            this.strokes = strokes;
        }

        public abstract Vector3 Evaluate(float time, float[] intensities);
    }
}