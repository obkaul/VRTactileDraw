// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.1433
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Valve.VR
{
    public partial class SteamVR_Input
    {
        
        public static Valve.VR.SteamVR_Input_ActionSet_default _default;
        
        public static Valve.VR.SteamVR_Input_ActionSet_platformer platformer;
        
        public static Valve.VR.SteamVR_Input_ActionSet_buggy buggy;
        
        public static Valve.VR.SteamVR_Input_ActionSet_htc_viu htc_viu;
        
        public static Valve.VR.SteamVR_Input_ActionSet_ui_input ui_input;
        
        public static void Dynamic_InitializeActionSets()
        {
            SteamVR_Input._default.Initialize();
            SteamVR_Input.platformer.Initialize();
            SteamVR_Input.buggy.Initialize();
            SteamVR_Input.htc_viu.Initialize();
            SteamVR_Input.ui_input.Initialize();
        }
        
        public static void Dynamic_InitializeInstanceActionSets()
        {
            Valve.VR.SteamVR_Input._default = ((SteamVR_Input_ActionSet_default)(SteamVR_Input_References.GetActionSet("_default")));
            Valve.VR.SteamVR_Input.platformer = ((SteamVR_Input_ActionSet_platformer)(SteamVR_Input_References.GetActionSet("platformer")));
            Valve.VR.SteamVR_Input.buggy = ((SteamVR_Input_ActionSet_buggy)(SteamVR_Input_References.GetActionSet("buggy")));
            Valve.VR.SteamVR_Input.htc_viu = ((SteamVR_Input_ActionSet_htc_viu)(SteamVR_Input_References.GetActionSet("htc_viu")));
            Valve.VR.SteamVR_Input.ui_input = ((SteamVR_Input_ActionSet_ui_input)(SteamVR_Input_References.GetActionSet("ui_input")));
            Valve.VR.SteamVR_Input.actionSets = new Valve.VR.SteamVR_ActionSet[]
            {
                    Valve.VR.SteamVR_Input._default,
                    Valve.VR.SteamVR_Input.platformer,
                    Valve.VR.SteamVR_Input.buggy,
                    Valve.VR.SteamVR_Input.htc_viu,
                    Valve.VR.SteamVR_Input.ui_input};
        }
    }
}
