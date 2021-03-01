//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Debug UI shown for the player
//
//=============================================================================

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	public class DebugUI : MonoBehaviour
	{
		private Player player;

		//-------------------------------------------------
		private static DebugUI _instance;
		public static DebugUI instance
		{
			get
			{
				if ( _instance == null )
				{
					_instance = GameObject.FindObjectOfType<DebugUI>();
				}
				return _instance;
			}
		}


		//-------------------------------------------------
		private void Start()
		{
			player = Player.instance;
		}


		//-------------------------------------------------
		private void OnGUI()
		{
            if (Debug.isDebugBuild)
            {
#if !HIDE_DEBUG_UI
                player.Draw2DDebug();
#endif
            }
		}
	}
}
