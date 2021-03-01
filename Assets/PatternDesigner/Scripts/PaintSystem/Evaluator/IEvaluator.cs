using UnityEngine;

namespace Assets.PatternDesigner.Scripts.PaintSystem.Evaluator
{
    /// <summary>
    ///     Interface for intensity evaluators
    /// </summary>
    public interface IEvaluator
    {
        Vector3 Evaluate(float time, float[] intensities);
    }
}