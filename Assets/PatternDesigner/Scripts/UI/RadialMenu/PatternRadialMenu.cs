using Assets.PatternDesigner.Scripts.PaintSystem;
using Assets.PatternDesigner.Scripts.PaintSystem.Stroke;
using Assets.PatternDesigner.Scripts.UI.MainMenu;
using UnityEngine;

namespace Assets.PatternDesigner.Scripts.UI.RadialMenu
{
    /// <summary>
    ///     Represents a radial menu for paint mode.
    /// </summary>
    public class PatternRadialMenu : RadialMenu
    {
        [SerializeField] protected MainUI mainUI;

        protected PaintMode paintMode;

        protected override void Awake()
        {
            base.Awake();

            paintMode = mainUI.paintMode;
        }

        protected override void Start()
        {
            base.Start();
            paintMode.pattern.StrokeAdded += OnStrokeAdded;
            paintMode.pattern.StrokeRemoved += OnStrokeRemoved;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (paintMode == null || paintMode.pattern == null)
                return;
            paintMode.pattern.StrokeAdded -= OnStrokeAdded;
            paintMode.pattern.StrokeRemoved -= OnStrokeRemoved;
        }

        protected virtual void OnStrokeAdded(Stroke stroke)
        {
        }

        protected virtual void OnStrokeRemoved(Stroke stroke)
        {
        }
    }
}