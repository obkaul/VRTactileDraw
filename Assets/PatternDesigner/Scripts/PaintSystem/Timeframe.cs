using UnityEngine;

namespace Assets.PatternDesigner.Scripts.PaintSystem
{
    /// <summary>
    ///     Calculates an appropriate timestep given a beign and end time.
    /// </summary>
    public class Timeframe
    {
        public readonly float beginTime;

        public readonly float endTime;

        public readonly float timestep;

        public Timeframe(float beginTime, float endTime)
        {
            this.beginTime = beginTime;
            this.endTime = endTime;

            var numberOfDigits = Mathf.Floor(Mathf.Log10(length) + 1);
            var power10 = Mathf.Pow(10, numberOfDigits);

            var rounded = Mathf.Round(length / power10 * 10);

            if (rounded >= 6)
                rounded = 10;
            else if (rounded >= 3)
                rounded = 5;
            else
                rounded = 2.5f;

            timestep = rounded * power10 / 100;
        }

        public float length => endTime - beginTime;
    }
}