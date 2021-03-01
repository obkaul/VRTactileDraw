using Assets.PatternDesigner.Scripts.PaintSystem.Stroke;
using System;
using System.Collections.Generic;

namespace Assets.PatternDesigner.Scripts.Pattern
{
    /// <summary>
    ///     Base class for Patterns
    /// </summary>
    public abstract class Pattern
    {
        private bool _looping;

        private string _name;

        private float _speed;

        private float duration;

        protected Pattern(string name) : this(name, DateTime.Now, DateTime.Now)
        {
        }

        protected Pattern(string name, DateTime creationTime, DateTime lastWriteTime)
        {
            this.name = name;
            this.creationTime = creationTime;
            this.lastWriteTime = lastWriteTime;
            looping = false;
            speed = 1.0f;
        }

        public float speed
        {
            set
            {
                _speed = value;
                OnSpeedChanged?.Invoke(value);
            }
            get => _speed;
        }

        public bool looping
        {
            set
            {
                _looping = value;
                OnLoopChanged?.Invoke(value);
            }
            get => _looping;
        }

        public abstract int type { get; }

        public string name
        {
            get => _name;
            set
            {
                _name = value;

                NameChanged?.Invoke(this);
            }
        }

        public DateTime creationTime { get; }

        public DateTime lastWriteTime { get; private set; }

        public event Action<Pattern> NameChanged;

        public event Action<Pattern> PatternUpdated;

        public event Action<bool> OnLoopChanged;

        public event Action<float> OnSpeedChanged;

        public abstract float getDuration();

        public void UpdateLastWriteTime()
        {
            lastWriteTime = DateTime.Now;
            PatternUpdated?.Invoke(this);
        }

        public abstract string ToJson();

        public abstract void FromJson(string json);

        protected void LoadBaseData(BaseData data)
        {
            looping = data.looping;
            speed = data.speed;
        }

        public int getNextUnusedColorIndex(List<Stroke> strokes)
        {
            return getNextUnusedColorIndex(strokes, 0);
        }

        public int getNextUnusedColorIndex(List<Stroke> strokes, int index)
        {
            foreach (var stroke in strokes)
                if (stroke.colorIndex == index)
                    return getNextUnusedColorIndex(strokes, index + 1);
            return index;
        }

        [Serializable]
        public class BaseData
        {
            public bool looping;
            public float speed;
        }
    }
}