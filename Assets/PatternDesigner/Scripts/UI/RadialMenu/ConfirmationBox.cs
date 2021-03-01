using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PatternDesigner.Scripts.UI.RadialMenu
{
    /// <summary>
    ///     A box that can be shown to the user to request a confirmation.
    /// </summary>
    [DisallowMultipleComponent]
    public class ConfirmationBox : MonoBehaviour
    {
        private const string PATH = "UI/ConfirmationBox";

        private Action<bool> callback;

        [SerializeField] private Text message;

        public static void Show(string text, Action<bool> callback)
        {
            var canvas = FindObjectOfType<Canvas>();
            var box = Instantiate(Resources.Load<ConfirmationBox>(PATH), canvas.transform);

            box.message.text = text;
            box.callback = callback;
        }

        public void OnAnswerChosen(bool confirmation)
        {
            callback(confirmation);
            Destroy(gameObject);
        }
    }
}