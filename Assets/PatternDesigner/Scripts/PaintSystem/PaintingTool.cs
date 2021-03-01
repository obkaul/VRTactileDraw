using Assets.PatternDesigner.Scripts.HapticHead;
using Assets.PatternDesigner.Scripts.PaintSystem.Evaluator;
using Assets.PatternDesigner.Scripts.PaintSystem.Stroke;
using UnityEngine;

namespace Assets.PatternDesigner.Scripts.PaintSystem
{
    /// <summary>
    ///     Represents a painting tool. Create new painting tool by inherit that class.
    /// </summary>
    public abstract class PaintingTool
    {
        private readonly float[] intensities = new float[VibratorMesh.VIBRATOR_COUNT];

        public string name;

        protected PaintingTool(string name)
        {
            this.name = name;
        }

        /// <summary>
        ///     Call this on stroking
        /// </summary>
        /// <param name="originToTarget">The origin ray</param>
        /// <param name="currentStroke">Current drawing stroke</param>
        /// <param name="strokeTime">Current player time for next key</param>
        /// <param name="strokingValue">The intensitiy</param>
        /// <returns>Returns intensities for controlling HapticHead</returns>
        public abstract float[] OnStroke(RaycastHit hit, Stroke.Stroke currentStroke, float strokeTime,
            float strokingValue);

        /// <summary>
        ///     Call this after finished one stroke
        /// </summary>
        public abstract void finishPainting();

        /// <summary>
        ///     Default painting tool for drawing at the pointer position.
        /// </summary>
        public class DefaultPainting : PaintingTool
        {
            private readonly ClosestEvaluator evaluator;

            public DefaultPainting(ClosestEvaluator evaluator) : base("Single stroke")
            {
                this.evaluator = evaluator;
            }

            public override float[] OnStroke(RaycastHit hit, Stroke.Stroke currentStroke, float strokeTime,
                float strokingValue)
            {
                var key = new Key
                {
                    time = strokeTime,
                    position = hit.point,
                    intensity = strokingValue * 2,
                    deleted = false
                };

                // set all intensities to 0
                for (var i = 0; i < intensities.Length; i++) intensities[i] = 0;

                evaluator.CalculateIntensitiesForKey(key, intensities);

                currentStroke?.AddKey(key);

                return intensities;
            }

            public override void finishPainting()
            {
            }
        }

        /// <summary>
        ///     Creates the default stroke and another stroke by copying the location on the other head side.
        /// </summary>
        public class ReflectionPainting : PaintingTool
        {
            private readonly ClosestEvaluator evaluator;

            public Stroke.Stroke reflectedStroke;

            public ReflectionPainting(ClosestEvaluator evaluator) : base("Symmetric mirror strokes")
            {
                this.evaluator = evaluator;
            }

            public override float[] OnStroke(RaycastHit hit, Stroke.Stroke currentStroke, float strokeTime,
                float strokingValue)
            {
                if (reflectedStroke == null)
                    Debug.Log("ERROR in PaintingTool:ReflectionPainting - reflectedStroke is still null!");

                var keyOrigin = new Key
                {
                    time = strokeTime,
                    position = hit.point,
                    intensity = strokingValue * 2
                };

                var reflectedPosition = hit.point;
                reflectedPosition.x *= -1;

                var keyReflected = new Key
                {
                    time = strokeTime,
                    position = reflectedPosition,
                    intensity = strokingValue * 2,
                    deleted = false
                };

                // set all intensities to 0
                for (var i = 0; i < intensities.Length; i++) intensities[i] = 0;
                evaluator.CalculateIntensitiesForKey(keyOrigin, intensities);
                evaluator.CalculateIntensitiesForKey(keyReflected, intensities);

                if (currentStroke == null) return intensities;
                currentStroke.AddKey(keyOrigin);
                reflectedStroke.AddKey(keyReflected);

                return intensities;
            }

            public override void finishPainting()
            {
                reflectedStroke = null;
            }
        }

        public class EraserMode : PaintingTool
        {
            private readonly ClosestEvaluator evaluator;
            public EraserMode(ClosestEvaluator evaluator) : base("Erasing Strokes")
            {
                this.evaluator = evaluator;
            }

            public override float[] OnStroke(RaycastHit hit, Stroke.Stroke currentStroke, float strokeTime,
                float strokingValue)
            {
                var key = new Key
                {
                    time = strokeTime,
                    position = hit.point,
                    intensity = strokingValue * 2,
                    deleted = true
                };

                // set all intensities to 0
                for (var i = 0; i < intensities.Length; i++) intensities[i] = 0;

                evaluator.CalculateIntensitiesForKey(key, intensities);

                currentStroke?.AddKey(key);

                return intensities;
            }

            public override void finishPainting()
            {
            }
        }
    }
}