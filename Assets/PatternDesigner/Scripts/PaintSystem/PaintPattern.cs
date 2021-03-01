using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.PatternDesigner.Scripts.PaintSystem
{
    /// <summary>
    ///     The PaintPattern is a collection of strokes.
    /// </summary>
    public class PaintPattern : Pattern.Pattern
    {
        private List<Stroke.Stroke> _strokes = new List<Stroke.Stroke>();

        public PaintPattern(string name) : base(name)
        {
        }

        public PaintPattern(string name, DateTime creationTime, DateTime lastWriteTime) : base(name, creationTime,
            lastWriteTime)
        {
        }

        public override int type => 1;

        public List<Stroke.Stroke> strokes
        {
            get => _strokes;
            set
            {
                _strokes = value;
                _strokes.Sort();
            }
        }

        public Stroke.Stroke this[int id] => strokes[id];

        public int length => strokes.Count;

        public event Action<Stroke.Stroke> StrokeAdded;

        public event Action<Stroke.Stroke> StrokeRemoved;
        

        /// <returns>Stroke duration</returns>
        public override float getDuration()
        {
            return strokes.Select(stroke => stroke.endTime).Concat(new float[] { 0 }).Max();
        }

        /// <summary>
        /// Adds the strokes to the stroke array and sorts it
        /// </summary>
        public void AddStroke(Stroke.Stroke stroke)
        {
            if (stroke.color.a == 0)
            {
                Debug.Log($"Stroke had no saved color: {stroke.color}");
                stroke.setColorIndex(length);
            }

            strokes.Add(stroke);

            strokes.Sort();

            StrokeAdded?.Invoke(stroke);
        }

        /// <summary>
        /// Removes the stroke from the stroke array and sorts it
        /// </summary>
        public void RemoveStroke(Stroke.Stroke stroke)
        {
            if (stroke == null) return;

            strokes.Remove(stroke);
            strokes.Sort();

            StrokeRemoved?.Invoke(stroke);
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(new Data
            {
                type = type,
                strokes = strokes,
                speed = speed,
                looping = looping
            }, true);
        }

        public override void FromJson(string json)
        {
            var data = JsonUtility.FromJson<Data>(json);

            Debug.Log($"Read data{data}");

            foreach (var stroke in data.strokes) AddStroke(stroke);

            LoadBaseData(data);
        }

        [Serializable]
        public class Data : BaseData
        {
            public List<Stroke.Stroke> strokes;

            public int type;
        }
    }
}