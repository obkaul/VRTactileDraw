using Assets.PatternDesigner.Scripts.Attributes;

namespace Assets.PatternDesigner.Scripts.Values.Resources
{
    public enum Strings
    {
        #region General strings

        [StringValue("Unknown String")] DEFAULT,

        [StringValue("Play\nPattern")] PLAY,

        [StringValue("Stop\nPattern")] STOP,

        [StringValue("Enable\nLoop")] ENABLE_LOOP,

        [StringValue("Disable\nLoop")] DISABLE_LOOP,

        [StringValue("Touch for\nmenu")] SHOW_MENU,

        [StringValue("Click for\naction")] CLICK_ACTION,

        [StringValue("Current time")] SLIDER_TIME_DESC,

        [StringValue("Speed: {0:0.0}x")] SLIDER_SPEED,

        [StringValue("{0:0.0}x")] SPEED,

        [StringValue("{0:0.0}s")] SECONDS,

        [StringValue("No task found.")] NO_TASK,

        [StringValue("Change\nSpeed")] CHANGE_SPEED,

        [StringValue("Record\nMode")] MAIN_MENU,

        [StringValue("Set Player Time")] CHANGE_TIME,

        [StringValue("Prev\nStroke")] PREV_STROKE,

        [StringValue("Next\nStroke")] NEXT_STROKE,

        [StringValue("Delete\nStroke")] DELETE_STROKE,

        [StringValue("Set Stroke Time")] CHANGE_START_TIME,

        [StringValue("{0:0.00}s")] SECONDS_FINE,

        [StringValue("Stroke Intensity: {0:0}%")]
        STROKING,

        [StringValue("Set Stroke Duration")] CHANGE_STROKE_DURATION,

        [StringValue("Painting Tool: {0}")] PAINTING_TOOL,

        #endregion General strings

        #region Text Strings

        [StringValue(
            "This tutorial will give you a short description of the inputs. The description will be displayed here.")]
        DEF_TXT,

        [StringValue("The menu button will take you back to the main UI.")]
        MEN_BUT_TXT,

        [StringValue(
            "Use the trackpad to use the paint utility. The tools of the left and right hand are different. Touch the trackpad to display them.")]
        TRACKP_TXT,

        [StringValue("Takes you back to the VR-Dashboard where you can change controller bindings.")]
        STM_BUT_TXT,

        [StringValue("The grip button is used to switch between different paint modes.")]
        GRIP_BUT_TXT,

        [StringValue(
            "Use the trigger to draw on the model or to select strokes. The intensity of the stroke varies depending on how hard you press the trigger.")]
        TRIG_BUT_TXT,

        #endregion Text Strings

        #region Text headers

        [StringValue("About")] DEF_HEAD,
        [StringValue("Menu Button")] MEN_BUT_HEAD,
        [StringValue("Trackpad")] TRACKP_HEAD,
        [StringValue("Steam Menu Button")] STM_BUT_HEAD,
        [StringValue("Grip Button")] GRIP_BUT_HEAD,
        [StringValue("Trigger")] TRIG_BUT_HEAD

        #endregion Text headers
    }
}