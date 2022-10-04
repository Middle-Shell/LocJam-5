using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PowerTools.Quest;

namespace PowerScript
{	
	// Shortcut access to SystemAudio.Get
	public class Audio : SystemAudio
	{
	}

	public static partial class C
	{
		// Access to specific characters (Auto-generated)
		public static ICharacter Prince		{ get{return PowerQuest.Get.GetCharacter("Prince"); } }
		public static ICharacter Wizard		{ get{return PowerQuest.Get.GetCharacter("Wizard"); } }
		public static ICharacter Flat		{ get{return PowerQuest.Get.GetCharacter("Flat"); } }
		public static ICharacter Merchant		{ get{return PowerQuest.Get.GetCharacter("Merchant"); } }
		public static ICharacter Dog		{ get{return PowerQuest.Get.GetCharacter("Dog"); } }
		public static ICharacter Doorian		{ get{return PowerQuest.Get.GetCharacter("Doorian"); } }
		public static ICharacter Maxilla		{ get{return PowerQuest.Get.GetCharacter("Maxilla"); } }
		public static ICharacter Maria		{ get{return PowerQuest.Get.GetCharacter("Maria"); } }
		// #CHARS# - Do not edit this line, it's used by the system to insert characters
	}

	public static partial class I
	{		
		// Access to specific Inventory (Auto-generated)
		public static IInventory Map		{ get{return PowerQuest.Get.GetInventory("Map"); } }
		public static IInventory Spellbook		{ get{return PowerQuest.Get.GetInventory("Spellbook"); } }
		public static IInventory Coins		{ get{return PowerQuest.Get.GetInventory("Coins"); } }
		public static IInventory Chicken		{ get{return PowerQuest.Get.GetInventory("Chicken"); } }
		public static IInventory Charm		{ get{return PowerQuest.Get.GetInventory("Charm"); } }
		public static IInventory Lift		{ get{return PowerQuest.Get.GetInventory("Lift"); } }
		public static IInventory Punch		{ get{return PowerQuest.Get.GetInventory("Punch"); } }
		public static IInventory Open		{ get{return PowerQuest.Get.GetInventory("Open"); } }
		public static IInventory Hyperdrive		{ get{return PowerQuest.Get.GetInventory("Hyperdrive"); } }
		public static IInventory Shoot		{ get{return PowerQuest.Get.GetInventory("Shoot"); } }
		public static IInventory Steal		{ get{return PowerQuest.Get.GetInventory("Steal"); } }
		public static IInventory Eat		{ get{return PowerQuest.Get.GetInventory("Eat"); } }
		public static IInventory Vomit		{ get{return PowerQuest.Get.GetInventory("Vomit"); } }
		public static IInventory Mindtrick		{ get{return PowerQuest.Get.GetInventory("Mindtrick"); } }
		public static IInventory Levitate		{ get{return PowerQuest.Get.GetInventory("Levitate"); } }
		// #INVENTORY# - Do not edit this line, it's used by the system to insert rooms for easy access
	}

	public static partial class G
	{
		// Access to specific gui (Auto-generated)
		public static IGui DisplayBox		{ get{return PowerQuest.Get.GetGui("DisplayBox"); } }
		public static IGui InfoBar		{ get{return PowerQuest.Get.GetGui("InfoBar"); } }
		public static IGui Toolbar		{ get{return PowerQuest.Get.GetGui("Toolbar"); } }
		public static IGui Inventory		{ get{return PowerQuest.Get.GetGui("Inventory"); } }
		public static IGui DialogTree		{ get{return PowerQuest.Get.GetGui("DialogTree"); } }
		public static IGui SpeechBox		{ get{return PowerQuest.Get.GetGui("SpeechBox"); } }
		public static IGui Prompt         { get { return PowerQuest.Get.GetGui("Prompt"); } }
		// #GUI# - Do not edit this line, it's used by the system to insert rooms for easy access
	}

	public static partial class R
	{
		// Access to specific room (Auto-generated)
		public static IRoom Space		{ get{return PowerQuest.Get.GetRoom("Space"); } }
		public static IRoom Labyrinth		{ get{return PowerQuest.Get.GetRoom("Labyrinth"); } }
		public static IRoom Skull		{ get{return PowerQuest.Get.GetRoom("Skull"); } }
		public static IRoom Factory		{ get{return PowerQuest.Get.GetRoom("Factory"); } }
		public static IRoom Darkness		{ get{return PowerQuest.Get.GetRoom("Darkness"); } }
		// #ROOM# - Do not edit this line, it's used by the system to insert rooms for easy access
	}

	// Dialog
	public static partial class D
	{
		// Access to specific dialog trees (Auto-generated)
		// #DIALOG# - Do not edit this line, it's used by the system to insert rooms for easy access	    	    
	}


}
