using Assets.PatternDesigner.Scripts.PaintSystem;
using Assets.PatternDesigner.Scripts.Pattern;
using Assets.PatternDesigner.Scripts.Util;
using Assets.PatternDesigner.Scripts.Values;
using Assets.PatternDesigner.Scripts.Values.Resources;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PatternDesigner.Scripts.UI.MainMenu
{
    public class MainUI : MonoBehaviour
    {
        private const string DefaultPatternName = "New Pattern";

        private ButtonHandler buttonHandler;

        [SerializeField] private Transform cameraTransform;

        [SerializeField] private Button createPaintButton;

        private Text desc;
        private Text head;

        [SerializeField] public PaintMode paintMode;

        private ToggleHandler toggleHandler;

        public Toggle toggleShowActuators;

        private void Awake()
        {
            toggleShowActuators = GetComponentInChildren<Toggle>();
            if (toggleShowActuators != null)
                toggleShowActuators.isOn = true;
        }

        // Initialize all the components of the main UI
        private void Start()
        {
            buttonHandler = new ButtonHandler(
                ButtonHandler.newKeyValuePair(createPaintButton, OnCreatePaintClicked)
            );

            toggleHandler = new ToggleHandler(
                ToggleHandler.newKeyValuePair(toggleShowActuators, OnShowActuatorsClicked)
            );

            buttonHandler.RegisterActions();
            toggleHandler.RegisterActions();

            head = transform.Find("Tutorial").Find("Controller Button").GetComponent<Text>();
            head.text = ResourcesManager.Get(Strings.DEF_HEAD);
            desc = transform.Find("Tutorial").Find("Description").GetComponent<Text>();
            desc.text = ResourcesManager.Get(Strings.DEF_TXT);

            StartCoroutine(InitMenu());
        }

        private void Destroy()
        {
            buttonHandler.UnregisterActions();
            toggleHandler.UnregisterActions();
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }

        private IEnumerator InitMenu()
        {
            yield return new WaitForSeconds(0.5f);
        }

        //Can be added to start, changes the initial menu position to where the player is looking
        private void ResetMenu()
        {
            transform.rotation = new Quaternion(0.0f, cameraTransform.rotation.y, 0.0f, cameraTransform.rotation.w);
            var camPos = cameraTransform.position + cameraTransform.forward * 1.3f;
            camPos.y = Math.Max(camPos.y, Constants.MAIN_MENU_MIN_Y);
            camPos.y = Math.Min(camPos.y, Constants.MAIN_MENU_MAX_Y);
            transform.position = camPos;
            transform.Translate(Vector3.down * 0.1f);
        }

        /// <summary>
        /// automatically generates a new pattern name. Could be reworked to fit the needs of the user as right now it's "New Pattern 1, ..., New Pattern n"
        /// </summary>
        /// <returns></returns>
        private string GetNextPatternName()
        {
            var newPatternName = DefaultPatternName;
            var nameExists = true;

            for (var i = 1; nameExists; ++i)
            {
                nameExists = false;
                foreach (var pattern in PatternManager.patterns)
                    if (pattern.name.Equals(newPatternName))
                    {
                        if (newPatternName.EndsWith((i - 1).ToString()))
                            newPatternName = newPatternName.Substring(0,
                                newPatternName.Length - (int)Math.Floor(Math.Log10(i - 1) + 1)) + i;
                        else
                            newPatternName += " " + i;
                        nameExists = true;
                        break;
                    }
            }

            return newPatternName;
        }

        private void OnCreatePaintClicked()
        {
            PatternNameBox.Show(GetNextPatternName(), name =>
            {
                if (name != null) OpenPaintMode(PatternManager.CreatePaint(name));
            });
        }

        //Disables main UI and enables the paintmode as well as all the objects attached to it like the model
        public void OpenPaintMode(Pattern.Pattern pattern)
        {
            paintMode.Open(pattern);
            Close();
        }

        private static void OnShowActuatorsClicked(bool isOn)
        {
        }
    }
}