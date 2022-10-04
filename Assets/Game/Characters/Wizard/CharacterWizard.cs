using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;

public class CharacterWizard : CharacterScript<CharacterWizard>
{


	public IEnumerator PrinceClick()
	{
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		C.Wizard.FaceBG(C.Prince);
		yield return C.Prince.Say("Any idea what we should do, Maldrek?", 176);
		
		switch ( GlobalScript.Script.m_goal )
		{
			case eGoal.HealGuy:
			case eGoal.Investigate:
			{
				C.Section("Ask for help healing guy & talking to doorian");
				yield return C.Wizard.Say("Leave me alone and bug the natives, find out what they want", 187);
			} break;
			case eGoal.Hypderdrive:
			{
				C.Section("Ask for help using to hyperdrive");
				yield return C.Wizard.Say("If I had my hyperdrive charged we could visit Lobos", 188);
				yield return C.Wizard.Say("I've probably got some spells that can help out", 189);
			} break;
			case eGoal.Doorbell:
			{
				C.Section("Ask for help getting & using doorbell on gate");
				yield return C.Wizard.Say("Well, getting into the monstrous skull castle might be one idea", 190);
			} break;
			case eGoal.Spew:
			{
				C.Section("Ask for help using doorbell on doorian, and with spew puzzle		");
				yield return C.Wizard.Say("I reckon i can get this Maria broad to spill her guts", 191);
				yield return C.Wizard.Say("Literally, that is", 192);
				yield return E.WaitSkip();
				yield return C.Wizard.Say("Then we make her let us in the fortress", 193);
			} break;
		}
		
		yield return E.Break;
	}

	public IEnumerator OnUseInv( IInventory item )
	{

		yield return E.Break;
	}

	public IEnumerator WizardClick()
	{

		yield return E.Break;
	}
}