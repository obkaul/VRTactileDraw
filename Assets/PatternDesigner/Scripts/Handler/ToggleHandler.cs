using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.PatternDesigner.Scripts.Util
{
    /// <summary>
    /// Manages actions on toggle buttons
    /// </summary>
    public class ToggleHandler : EventHandler<Toggle, UnityAction<bool>>
    {
        public ToggleHandler(params KeyValuePair<Toggle, UnityAction<bool>>[] actions)
            : base(
                (toggle, action) => toggle.onValueChanged.AddListener(action),
                (toggle, action) => toggle.onValueChanged.RemoveListener(action),
                actions
            )
        {
        }
    }
}