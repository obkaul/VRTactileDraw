/// Credit Simon (darkside) Jackson
/// Sourced from - My head

using UnityEngine;

namespace Scripts.Utilities
{
    public static class UIExtensionMethods
    {
        public static Canvas GetParentCanvas(this RectTransform rt)
        {
            var parent = rt;
            var parentCanvas = rt.GetComponent<Canvas>();

            var SearchIndex = 0;
            while (parentCanvas == null || SearchIndex > 50)
            {
                parentCanvas = rt.GetComponentInParent<Canvas>();
                if (parentCanvas == null)
                {
                    parent = parent.parent.GetComponent<RectTransform>();
                    SearchIndex++;
                }
            }
            return parentCanvas;
        }

    }
}
