using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;

public class InventoryChicken : InventoryScript<InventoryChicken>
{


	public IEnumerator OnUseInvInventory( IInventory thisItem, IInventory item )
	{
		if ( item == I.Eat )
		{
			yield return C.Wizard.FaceUp();
			yield return E.WaitSkip(0.25f);	
			Audio.Play("EatLoop").FadeIn(0.15f);
			yield return C.Wizard.Say("Schrlp cmncgh Hrmmmmm", 194);
			yield return E.WaitSkip();
			yield return C.Wizard.Say("OOooh, that's some goooood \"Chicken\"", 195);
			yield return E.WaitSkip();
			Audio.Stop("EatLoop",0.5f);
			yield return C.Wizard.Say("Grllp schlppp mwaah!", 196);
			
			yield return E.WaitSkip();
			yield return C.Wizard.FaceDown();
			yield return C.Wizard.Say("Aaaaah!", 197);
			yield return C.Prince.Say("What's in that stuff anyway", 177);
			yield return C.Wizard.Say("Eh, who could say?", 198);
			yield return C.Wizard.Say("But it charges up my hyperdrive like nothin' else", 199);
			
			I.Chicken.Remove();
			RoomLabyrinth.Script.m_chicken = RoomLabyrinth.eChicken.Eaten;
			
			I.Hyperdrive.Description = "&48 Spell: Hyperdrive - Interstellar travel, now fully charged";
			
		}
		yield return E.Break;
	}
}