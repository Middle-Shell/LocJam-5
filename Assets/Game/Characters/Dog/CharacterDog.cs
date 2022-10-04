using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;

public class CharacterDog : CharacterScript<CharacterDog>
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