using Assets.PatternDesigner.Scripts.Pattern;
using UnityEngine;

namespace Assets.PatternDesigner.Scripts
{
    /// <summary>
    ///     Main entry point.
    /// </summary>
    [DisallowMultipleComponent]
    public class App : MonoBehaviour
    {
        private void Start()
        {
            PatternManager.LoadAll();
        }

        private void OnApplicationQuit()
        {
            PatternManager.SaveAll();
        }
    }
}