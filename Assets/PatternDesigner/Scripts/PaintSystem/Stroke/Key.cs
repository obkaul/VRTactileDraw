using System;
using UnityEngine;

namespace Assets.PatternDesigner.Scripts.PaintSystem.Stroke
{
    /// <summary>
    ///     A Key for a Stroke
    /// </summary>
    [Serializable]
    public struct Key
    {
        public float time;

        public Vector3 position;

        public float intensity;

        public bool deleted;

        public override string ToString()
        {
            return $"Key(Time={time}, position={position}, intensity={intensity})";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var k = (Key)obj;
            return time == k.time && position.Equals(k.position) && intensity == k.intensity;
        }

        public override int GetHashCode()
        {
            return (int)(time + intensity + position.GetHashCode());
        }
    }
}