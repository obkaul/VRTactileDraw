using Assets.PatternDesigner.Scripts.PaintSystem.Player;
using Assets.PatternDesigner.Scripts.UI.RadialMenu;
using Assets.PatternDesigner.Scripts.Values;
using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace Assets.PatternDesigner.Scripts.Pattern
{
    public abstract class PatternEditMode : MonoBehaviour
    {
        private int enabledAt = int.MaxValue;

        [SerializeField]
        [SteamVR_DefaultAction("Haptic")]
        public SteamVR_Action_Vibration hapticAction;

        [SerializeField] private List<GameObject> involvedUI;

        [SerializeField] private List<RadialMenu> radialMenus;

        public TactilePlayer player { get; protected set; }

        protected virtual void OnEnable()
        {
            involvedUI.ForEach(go => go.SetActive(true));
            enabledAt = Environment.TickCount;
            if (radialMenus != null)
                foreach (var menu in radialMenus)
                    menu.OnClick += OnMenuClicked;
        }

        protected virtual void Update()
        {
            if (Environment.TickCount - enabledAt <= Constants.TIME_UNTIL_HINT) return;
            hapticAction.Execute(0, 1, 150, 75, SteamVR_Input_Sources.LeftHand);

            enabledAt = int.MaxValue;
        }

        protected virtual void OnDisable()
        {
            involvedUI.ForEach(go =>
            {
                if (go != null) go.SetActive(false);
            });
            if (radialMenus == null) return;
            foreach (var menu in radialMenus) menu.OnClick -= OnMenuClicked;
        }

        protected virtual void Awake()
        {
        }

        public virtual void Close()
        {
            if (!gameObject.activeSelf)
                return;
            player.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        public virtual void OnDurationUpdate()
        {
        }

        private void OnMenuClicked()
        {
            enabledAt = int.MaxValue;
        }
    }
}