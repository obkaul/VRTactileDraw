using Assets.PatternDesigner.Scripts.Pattern;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PatternDesigner.Scripts.UI.MainMenu
{
    /// <summary>
    ///     Represents the pattern list.
    /// </summary>
    [DisallowMultipleComponent]
    public class PatternList : MonoBehaviour
    {
        private readonly SortedDictionary<Pattern.Pattern, PatternListItem> items =
            new SortedDictionary<Pattern.Pattern, PatternListItem>(new PatternNameComparer());

        private MainUI mainUI;

        [SerializeField] private PatternListItem prefab;

        private void Start()
        {
            mainUI = GetComponentInParent<MainUI>();
            PatternManager.Added += OnPatternAdded;
            PatternManager.Removed += OnPatternRemoved;
        }

        private void OnDestroy()
        {
            PatternManager.Added -= OnPatternAdded;
            PatternManager.Removed -= OnPatternRemoved;
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Adds pattern to pattern list
        /// </summary>
        /// <param name="pattern">Pattern that's added</param>
        private void OnPatternAdded(Pattern.Pattern pattern)
        {
            var item = Instantiate(prefab);
            item.Init(pattern);

            var gameObject = item.gameObject;
            var transform = item.transform as RectTransform;
            gameObject.SetActive(true);

            transform?.SetParent(prefab.transform.parent, false);
            transform?.SetAsFirstSibling();

            item.Selected += OnSelected;

            items.Add(pattern, item);

            ApplyOrderToTransforms();
        }

        /// <summary>
        /// Removes pattern from pattern list
        /// </summary>
        /// <param name="pattern">pattern to remove</param>
        private void OnPatternRemoved(Pattern.Pattern pattern)
        {
            var item = items[pattern];
            var gameObject = item.gameObject;

            item.Selected -= OnSelected;

            items.Remove(pattern);

            Destroy(gameObject);

            ApplyOrderToTransforms();
        }

        private void ApplyOrderToTransforms()
        {
            var i = 0;

            foreach (var pair in items)
                pair.Value.transform.SetSiblingIndex(i++);
        }

        private void OnSelected(Pattern.Pattern pattern)
        {
            mainUI.OpenPaintMode(pattern);
        }
    }
}