using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;

public class CharacterDoorian : CharacterScript<CharacterDoorian>
{


	public IEnumerator WizardClick()
	{

		yield return E.Break;
	}

	public IEnumerator OnUseInv( IInventory item )
	{

		yield return E.Break;
	}
}