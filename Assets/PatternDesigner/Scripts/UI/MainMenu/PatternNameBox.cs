    using Assets.PatternDesigner.Scripts.Pattern;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PatternDesigner.Scripts.UI.MainMenu
{
    /// <summary>
    ///     A rename pattern prompt.
    /// </summary>
    [DisallowMultipleComponent]
    public class PatternNameBox : MonoBehaviour
    {
        private const string MAIN_UI_PATH = "MainUI";
        private const string PATH = "UI/PatternNameBox";

        private Action<string> callback;

        [SerializeField] private InputField nameField;

        [SerializeField] private Button okayButton;

        public static void Show(string name, Action<string> callback)
        {
            var canvas = GameObject.Find(MAIN_UI_PATH).GetComponent<Canvas>();

            var box = Instantiate(Resources.Load<PatternNameBox>(PATH), canvas.transform);
            var boxTransform = box.transform;
            boxTransform.position = boxTransform.position + boxTransform.forward * -0.3f;

            box.nameField.text = name;
            box.callback = callback;

            box.OnNameChanged(name);
        }

        private void OnEnable()
        {
            nameField.onValueChanged.AddListener(OnNameChanged);
        }

        private void OnDisable()
        {
            nameField.onValueChanged.RemoveListener(OnNameChanged);
        }

        private void OnNameChanged(string name)
        {
            okayButton.interactable = PatternManager.IsValidName(name) && !PatternManager.DoesNameExist(name);
        }

        public void OnAnswerChosen(bool answer)
        {
            callback(answer ? nameField.text : null);

            Destroy(gameObject);
        }
    }
}