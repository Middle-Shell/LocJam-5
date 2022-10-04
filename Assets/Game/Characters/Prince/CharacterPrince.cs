using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;

public class CharacterPrince : CharacterScript<CharacterPrince>
{


	public IEnumerator PrinceClick()
	{

		yield return E.Break;
	}

	public IEnumerator WizardClick()
	{
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		
		C.Prince.FaceBG(C.Wizard);
		yield return C.Wizard.Say("So what's your next move, your pudendal majesty?", 184);
		
		switch ( GlobalScript.Script.m_goal )
		{
			case eGoal.HealGuy:
			{
				C.Section("Ask for help healing guy");
			} break;
			case eGoal.Investigate:
			{
				C.Section("Ask for help talking to Doorian");
				yield return C.Prince.Say("We should figure out why there was a distress call", 171);
		
			} break;
			case eGoal.Hypderdrive:
			{
				C.Section("Ask for help using to hyperdrive");
				yield return C.Prince.Say("I think we should make a visit to planet Lobos", 172);
				yield return C.Wizard.Say("And how should we do that?", 185);
				yield return C.Prince.Say("You're the one with tippy toppy spells for that kinda thing", 173);
			} break;
			case eGoal.Doorbell:
			{
				C.Section("Ask for help getting & using doorbell on gate");
				yield return C.Prince.Say("Try getting this gate open, I say", 174);
				yield return C.Wizard.Say("Time to weave a little more of the old dirty magic?", 186);
			} break;
			case eGoal.Spew:
			{
				C.Section("Ask for help using doorbell on doorian, and with spew puzzle		");
				yield return C.Prince.Say("Maybe this open spell can come in handy elsewhere", 175);
			} break;
		}
		
		yield return E.Break;
	}

	public IEnumerator OnUseInv( IInventory item )
	{

		yield return E.Break;
	}
}