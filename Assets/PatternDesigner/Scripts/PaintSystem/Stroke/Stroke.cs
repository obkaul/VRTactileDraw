using Assets.PatternDesigner.Scripts.UI.HeadmountedDisplay;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.PatternDesigner.Scripts.PaintSystem.Stroke
{
    /// <summary>
    ///     Represents an intensity stroke on the head.
    /// </summary>
    [Serializable]
    public class Stroke : IComparable<Stroke>
    {
        #region Colors
        //all available stroke colors
        private static List<Color> armytage_colors = new List<Color>(new Color[]
        {
            new Color32(0, 177, 220, 255),
            new Color32(153, 63, 0, 255),
            new Color32(76, 0, 92, 255),
            new Color32(25, 25, 25, 255),
            new Color32(0, 92, 49, 255),
            new Color32(43, 206, 72, 255),
            new Color32(255, 204, 153, 255),
            new Color32(128, 128, 128, 255),
            new Color32(148, 255, 181, 255),
            new Color32(143, 124, 0, 255),
            new Color32(157, 204, 0, 255),
            new Color32(194, 0, 136, 255),
            new Color32(0, 51, 128, 255),
            new Color32(255, 164, 5, 255),
            new Color32(255, 168, 187, 255),
            new Color32(66, 102, 0, 255),
            new Color32(255, 0, 16, 255),
            new Color32(94, 241, 242, 255),
            new Color32(0, 153, 143, 255),
            new Color32(240, 163, 255, 255),
            new Color32(224, 255, 102, 255),
            new Color32(116, 10, 255, 255),
            new Color32(153, 0, 0, 255),
            new Color32(255, 255, 128, 255),
            new Color32(255, 255, 0, 255),
            new Color32(255, 80, 5, 255)
        });
        #endregion


        private static bool armytage_colors_cleaned;
        
        public Color color;
        public int colorIndex = -1;

        public List<Key> keys = new List<Key>();

        public ProgressBarMarker myMarker;

        public Stroke()
        {
            if (colorIndex >= 0) setColorIndex(colorIndex);
        }

        public Stroke(int colorIndex)
        {
            setColorIndex(colorIndex);
        }

        public float startTime => keys.Count > 0 ? keys[0].time : 0;

        public float endTime => keys.Count > 0 ? keys[keys.Count - 1].time : 0;

        public float duration
        {
            get => endTime - startTime;
            set
            {
                if (value <= 0)
                    return;
                var delta = value / duration;
                ChangeDuration(delta);
            }
        }

        public int CompareTo(Stroke other)
        {
            var a = -other.startTime.CompareTo(startTime);
            if (a == 0)
                return -other.endTime.CompareTo(endTime);
            return a;
        }

        private static float brightnessOfColor(Color c)
        {
            return (c.r + c.g + c.b) / 3;
        }

        public event Action<Stroke> KeysChanged;

        internal void setColorIndex(int colorIndex)
        {
            if (colorIndex == -2)
            {
                this.colorIndex = colorIndex;
                color = Color.black;
                return;
            }
            cleanArmytageColorsIfNecessary();
            this.colorIndex = colorIndex;
            color = armytage_colors[colorIndex % armytage_colors.Count];
        }

        private void cleanArmytageColorsIfNecessary()
        {
            if (!armytage_colors_cleaned)
            {
                for (var i = 0; i < armytage_colors.Count; i++)
                    if (brightnessOfColor(armytage_colors[i]) < 0.2 || brightnessOfColor(armytage_colors[i]) > 0.9)
                    {
                        Debug.Log(
                            $"Brightness of color {armytage_colors[i]} is: {brightnessOfColor(armytage_colors[i])}");
                        armytage_colors.RemoveAt(i);
                        i -= 1;
                    }

                Debug.Log("Cleaned armytage colors. Colors left: " + armytage_colors.Count);
                armytage_colors_cleaned = true;
            }
        }

        public void removeErased(Stroke erasedStroke)
        {
            keys.Clear();
            keys = erasedStroke.keys.Where(key => key.deleted == false).ToList();
        }
        public void createMarkerIfNoneAvailable(Timeline tl)
        {
            if (myMarker == null) myMarker = tl.CreateStrokeMarker(this, false);
        }

        public void AddKey(Key key)
        {
            keys.Add(key);
            KeysChanged?.Invoke(this);
        }


        public void DestroyStroke()
        {
            Clear();
            if (myMarker != null) myMarker.DestroyMarker();
        }

        public void HideStroke()
        {
            if (myMarker != null) myMarker.DestroyMarker();
        }

        public void Clear()
        {
            keys.Clear();
            KeysChanged?.Invoke(this);
        }

        public void SetKeys(List<Key> keys)
        {
            this.keys = keys;
            KeysChanged?.Invoke(this);
        }

        public void ChangeStartTime(float time)
        {
            var keyArray = keys.ToArray();
            if (keyArray.Length <= 0) return;
            var delta = time - keyArray[0].time;
            for (var i = 0; i < keyArray.Length; ++i) keyArray[i].time = keyArray[i].time + delta;
            SetKeys(new List<Key>(keyArray));
        }

        private void ChangeDuration(float delta)
        {
            var keyArray = keys.ToArray();
            if (keyArray.Length > 1)
            {
                for (var i = 1; i < keyArray.Length; ++i)
                    keyArray[i].time = keyArray[i - 1].time + (keys[i].time - keys[i - 1].time) * delta;
                SetKeys(new List<Key>(keyArray));
            }
        }
    }
}