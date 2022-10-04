using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;

public class CharacterFlat : CharacterScript<CharacterFlat>
{


	public IEnumerator OnUseInv( IInventory item )
	{

		yield return E.Break;
	}
}