using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;

public class RoomDarkness : RoomScript<RoomDarkness>
{


	public void OnEnterRoom()
	{
		
		
		
		Audio.PlayAmbientSound("SoundAmbient-Darkness",0.8f,0.4f);
	}
}