using UnityEngine;

namespace Assets.PatternDesigner.Scripts.HapticHead
{
    /// <summary>
    ///     Represents a vibrator around the head.
    /// </summary>
    [DisallowMultipleComponent]
    public class Vibrator : MonoBehaviour
    {
        //IDs must be unique, set them in the unity object viewer
        [field: SerializeField] public int id;

        public override string ToString()
        {
            return $"Vibrator(id=+{id}, position={transform.position})";
        }
    }
}