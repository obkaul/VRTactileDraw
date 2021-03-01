using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.PatternDesigner.Scripts.Util
{
    /// <summary>
    /// Manages button clicks
    /// </summary>
    public class ButtonHandler : EventHandler<Button, UnityAction>
    {
        public ButtonHandler(params KeyValuePair<Button, UnityAction>[] actions)
            : base(
                (button, action) => button.onClick.AddListener(action),
                (button, action) => button.onClick.RemoveListener(action),
                actions
            )
        {
        }
    }
}