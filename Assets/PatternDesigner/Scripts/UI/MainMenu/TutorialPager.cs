using Assets.PatternDesigner.Scripts.Values.Resources;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PatternDesigner.Scripts.UI.MainMenu
{
    /// <summary>
    /// Manages the "Tutorial" window in the scene
    /// </summary>
    /// <remarks>
    /// Needs some work as it was quickly put together to have at least a short demonstration of the available tools. Maybe videos/gifs?
    /// </remarks>
    public class TutorialPager : MonoBehaviour
    {
        private static int counter;
        public Text head;
        public Text text;

        public void getNextPage()
        {
            switch (counter++)
            {
                case 0:
                    text.text = ResourcesManager.Get(Strings.DEF_TXT);
                    head.text = ResourcesManager.Get(Strings.DEF_HEAD);
                    break;

                case 1:
                    text.text = ResourcesManager.Get(Strings.MEN_BUT_TXT);
                    head.text = ResourcesManager.Get(Strings.MEN_BUT_HEAD);
                    break;

                case 2:
                    text.text = ResourcesManager.Get(Strings.TRACKP_TXT);
                    head.text = ResourcesManager.Get(Strings.TRACKP_HEAD);
                    break;

                case 3:
                    text.text = ResourcesManager.Get(Strings.STM_BUT_TXT);
                    head.text = ResourcesManager.Get(Strings.STM_BUT_HEAD);
                    break;

                case 4:
                    text.text = ResourcesManager.Get(Strings.GRIP_BUT_TXT);
                    head.text = ResourcesManager.Get(Strings.GRIP_BUT_HEAD);
                    break;

                case 5:
                    text.text = ResourcesManager.Get(Strings.TRIG_BUT_TXT);
                    head.text = ResourcesManager.Get(Strings.TRIG_BUT_HEAD);
                    counter = 1;
                    break;
            }
        }

        public void getPrevPage()
        {
            switch (counter--)
            {
                case 0:
                    counter = 5;
                    getPrevPage();
                    break;

                case 1:
                    text.text = ResourcesManager.Get(Strings.MEN_BUT_TXT);
                    head.text = ResourcesManager.Get(Strings.MEN_BUT_HEAD);
                    break;

                case 2:
                    text.text = ResourcesManager.Get(Strings.TRACKP_TXT);
                    head.text = ResourcesManager.Get(Strings.TRACKP_HEAD);
                    break;

                case 3:
                    text.text = ResourcesManager.Get(Strings.STM_BUT_TXT);
                    head.text = ResourcesManager.Get(Strings.STM_BUT_HEAD);
                    break;

                case 4:
                    text.text = ResourcesManager.Get(Strings.GRIP_BUT_TXT);
                    head.text = ResourcesManager.Get(Strings.GRIP_BUT_HEAD);
                    break;

                case 5:
                    text.text = ResourcesManager.Get(Strings.TRIG_BUT_TXT);
                    head.text = ResourcesManager.Get(Strings.TRIG_BUT_HEAD);
                    break;
            }
        }
    }
}