using System;
using UnityEngine;

namespace Assets.PatternDesigner.Scripts.UI.RadialMenu
{
    /// <summary>
    ///     Represents any progress bar on controllers.
    /// </summary>
    public class VRProgressBar : MonoBehaviour
    {
        public float lastValue = -1f;

        private Material material;

        public event Action<float> OnValueChanged;

        protected virtual void OnEnable()
        {
            material = GetComponent<Renderer>().material;
        }

        /// <summary>
        ///     Set progress value.
        /// </summary>
        /// <param name="value">Percentage value</param>
        public virtual void setValue(float value)
        {
            lastValue = value;
            material.SetFloat("_Cutoff", 1.2f - (value * 0.8f + 0.2f)); //calculates the progress

            OnValueChanged?.Invoke(value);
        }
    }
}