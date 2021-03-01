using Assets.PatternDesigner.Scripts.Pattern;
using Assets.PatternDesigner.Scripts.Util;
using Assets.PatternDesigner.Scripts.Values.Resources;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PatternDesigner.Scripts.UI.MainMenu
{
    /// <summary>
    ///     Represents one item in the PatternList.
    /// </summary>
    [DisallowMultipleComponent]
    public class PatternListItem : MonoBehaviour
    {
        private ButtonHandler buttonHandler;

        [SerializeField] private Text nameText, lastWriteText, durationText, creationText;

        [SerializeField] private Button selectButton, removeButton, confirmPosButton, confirmNegButton;

        public Pattern.Pattern pattern { get; private set; }

        public event Action<Pattern.Pattern> Selected;

        private void Awake()
        {
            buttonHandler = new ButtonHandler(
                ButtonHandler.newKeyValuePair(selectButton, Select),
                ButtonHandler.newKeyValuePair(removeButton, Remove),
                ButtonHandler.newKeyValuePair(confirmPosButton, RemovePositive),
                ButtonHandler.newKeyValuePair(confirmNegButton, RemoveNegative)
            );
        }

        private void OnEnable()
        {
            durationText.text = ResourcesManager.GetSecondsText(pattern.getDuration());
        }

        private void Start()
        {
            buttonHandler.RegisterActions();

            pattern.NameChanged += OnNameChanged;
            pattern.PatternUpdated += OnPatternUpdated;

            OnNameChanged(pattern);
            OnPatternUpdated(pattern);
        }

        private void OnDestroy()
        {
            buttonHandler.UnregisterActions();

            pattern.NameChanged -= OnNameChanged;
            pattern.PatternUpdated -= OnPatternUpdated;
        }

        public void Init(Pattern.Pattern pattern)
        {
            this.pattern = pattern;
        }

        public void Select()
        {
            Selected?.Invoke(pattern);
        }

        private void Remove()
        {
            removeButton.gameObject.SetActive(false);
            confirmPosButton.gameObject.SetActive(true);
            confirmNegButton.gameObject.SetActive(true);
        }

        private void RemovePositive()
        {
            PatternManager.Delete(pattern);
        }

        private void RemoveNegative()
        {
            removeButton.gameObject.SetActive(true);
            confirmPosButton.gameObject.SetActive(false);
            confirmNegButton.gameObject.SetActive(false);
        }

        private void OnNameChanged(Pattern.Pattern pattern)
        {
            if (pattern == null)
            {
                nameText.text = "Could not load pattern correctly";
                return;
            }

            nameText.text = pattern.name;
        }

        private void OnPatternUpdated(Pattern.Pattern pattern)
        {
            var format = "dd.MM.yyyy - HH:mm";
            lastWriteText.text = "Edited on " + pattern.lastWriteTime.ToString(format);
            creationText.text = "Created on " + pattern.creationTime.ToString(format);
        }
    }
}