using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;

public class RoomFactory : RoomScript<RoomFactory>
{
	bool m_seenBuckets = false;
	bool m_seenSack = false;
	
	enum eState { Start, SawMax, Jumped, Down }
	eState m_state = eState.Start;
	
	
	bool m_updateColls = false;
	public void OnEnterRoom()
	{
		E.FadeOutBG(0);
		C.Prince.AnimIdle = "Sack";
		C.Wizard.AnimIdle = "Sack";
		C.Prince.Position = Point("Prince");
		C.Wizard.Position = Point("Wizard");
		C.Prince.Facing = eFace.Right;
		C.Wizard.Facing = eFace.Right;
		
		C.Prince.Moveable = false;
		C.Wizard.Moveable = false;
		
		C.Prince.Room = R.Current;
		C.Wizard.Room = R.Current;
		
		GlobalScript.Script.m_inSack = true;
		GlobalScript.Script.FollowOff();
		GlobalScript.Script.FaceOff();
		
		
		if ( R.EnteredFromEditor )
		{ 
			// first time entering
			GlobalScript.Script.m_swapEnabled = true;
			E.Player = C.Wizard;
			Camera.Snap();	
			
			C.Wizard.AddInventory(I.Hyperdrive);
			C.Wizard.AddInventory(I.Steal);
			C.Wizard.AddInventory(I.Eat);
			C.Wizard.AddInventory(I.Shoot);
			C.Wizard.AddInventory(I.Mindtrick);
			
			C.Prince.AddInventory(I.Charm);
			C.Prince.AddInventory(I.Lift);
			C.Prince.AddInventory(I.Punch);
			C.Prince.AddInventory(I.Levitate);	
		}
		
		GlobalScript.Script.m_goal = eGoal.EscapeSack;
		
		Camera.SetPositionOverride(-50);
		
		GlobalScript.Script.FollowOff();
		GlobalScript.Script.ShadowPrince = false;
		GlobalScript.Script.ShadowWizard = false;
		GlobalScript.Script.Hover = false;
		
	}

	public IEnumerator OnEnterRoomAfterFade()
	{
		
		// TODO: fade-in and stuff
		
		yield return E.WaitSkip(1.0f);
		yield return C.Display("What have our heroes gotten themselves into now?", 10);
		yield return E.WaitSkip();
		yield return C.Display("Will they find the missing Labyrinthians? ", 11);
		yield return E.WaitSkip();
		yield return C.Display("Just what makes \"Space Chicken\" so delicious? ", 12);
		yield return E.WaitSkip();
		yield return C.Display("Find out... now!", 13);
		yield return E.WaitSkip();
		
		Audio.PlayMusic("MusicFactory",4);
		Audio.PlayAmbientSound("SoundAmbient-Meat",3,2);
		E.FadeInBG(2);
		
		yield return E.WaitSkip(1.0f);
		yield return C.Wizard.Say("Huuruuurrrraacccck!", 152);
		yield return E.WaitSkip(1.0f);
		yield return C.Prince.FaceLeft();
		yield return C.Prince.Say("Maldrek!", 151);
		yield return E.WaitSkip();
		yield return C.Wizard.Say("Well, this is a GREAT development", 153);
		
		E.UpdateClickableCollider( C.Prince );
		E.UpdateClickableCollider( C.Wizard );
		
		yield return E.Break;
	}

	public IEnumerator PrinceClickHotspotSack( IHotspot hotspot )
	{
		
		yield return C.Player.FaceRight();
		if ( C.Prince.IsPlayer )
		{
			yield return C.Prince.Say("Hey, that looks like...", 152);
		}
		else 
		{
			yield return C.Wizard.Say("Hey, that looks like...", 154);
		}
		
		yield return C.Prince.FaceRight();
		yield return C.Prince.Say("Doorian! ", 153);
		yield return C.Prince.Say("Hey, Dorian? Hello?", 154);
		yield return C.Wizard.Say("Forget it, she's passed the final threshold", 155);
		yield return E.WaitSkip();
		yield return C.Prince.FaceLeft();
		yield return C.Wizard.Say("Closed the final door", 156);
		yield return E.WaitSkip();
		yield return C.Wizard.Say("Her bolt has been drawn", 157);
		yield return E.WaitSkip();
		yield return C.Wizard.Say("Her lock clicked shut", 158);
		yield return E.WaitSkip(1.0f);
		yield return C.Wizard.Say("By gloop she was annoying", 159);
		yield return C.Prince.FaceRight();
		yield return C.Prince.Say("What's that thing she's in?", 155);
		yield return C.Wizard.Say("It's the ones we're in I'm worried about", 160);
		yield return C.Prince.FaceLeft();
		yield return C.Wizard.Say("Your classic flesh sack for digesting victims into a pulpy goo", 161);
		
		m_seenSack = true;
		
		yield return E.WaitFor(MaxillaAppear);
		Hotspot("Sack").Cursor = "None";
		
		
		yield return E.Break;
	}

	public IEnumerator PrinceClickHotspotBuckets( IHotspot hotspot )
	{
		if ( C.Prince.IsPlayer )
		{
			yield return C.Prince.Say("Those look like \"Space Chicken\" buckets", 156);
		}
		else 
		{
			yield return C.Wizard.Say("Those are \"Space Chicken\" buckets", 162);
		}
		
		yield return C.Prince.Say("But what are they doing here?", 157);
		
		Hotspot("Buckets").Cursor = "None";
		m_seenBuckets = true;
		
		yield return E.WaitFor(MaxillaAppear);
		yield return E.Break;
	}

	public IEnumerator WizardClickHotspotBuckets( IHotspot hotspot )
	{
		yield return E.HandleInteract(hotspot);
		yield return E.Break;
	}

	public IEnumerator WizardClickHotspotSack( IHotspot hotspot )
	{
		yield return E.HandleInteract(hotspot);
		yield return E.Break;
	}


	public IEnumerator WizardClickCharacterMaxilla( ICharacter character )
	{
		if ( m_state < eState.Down )
		{
			C.Section("Wizard Look at maxilla when up on hook");
			yield return C.Wizard.Say("She's the spongiosum behind this, we've gotta stop her Xandar", 163);
		}
		else 
		{
			C.Section("Wizard Look at maxilla when on ground (end sequence)");
			yield return C.Wizard.Say("Now's our chance", 164);
		}
		yield return E.Break;
	}

	public IEnumerator PrinceClickCharacterMaxilla( ICharacter character )
	{
		if ( m_state < eState.Down )
		{
			C.Section("Prince Look at maxilla when up on hook");
			yield return C.Prince.Say("That fiend, that devil!", 158);
		}
		else 
		{
			C.Section("Prince Look at maxilla when on ground (end sequence)");
			yield return C.Wizard.Say("This ends now!", 165);
		}
		yield return E.Break;
	}

	public IEnumerator PrinceClickCharacterWizard( ICharacter character )
	{
		if ( m_outroTime )
		{
			m_outroTime = false;
			yield return E.WaitFor( Outro2 );
		}
		else if ( E.FirstOccurance("getout") )
		{
			yield return C.Prince.FaceLeft();
			yield return C.Prince.Say("How are we going to get out of this one, Maldrek?", 159);
			yield return C.Wizard.Say("It ain't looking good, that's for sure", 166);
		}
		else
		{
			yield return C.Prince.FaceLeft();
			yield return C.Prince.Say("There must be something you can do", 160);
			yield return C.Wizard.Say("It's hopeless", 167);
		}
		yield return E.Break;
	}

	public IEnumerator WizardClickCharacterWizard( ICharacter character )
	{

		yield return E.Break;
	}

	public IEnumerator WizardClickCharacterPrince( ICharacter character )
	{
		if ( E.FirstOccurance("endtlkchrpince") )
		{
			yield return C.Wizard.Say("Some state we got ourselves into this time old friend", 168);
			yield return C.Prince.Say("Don't despair, I have a few spells up my sleeve yet!", 161);
			yield return C.Wizard.Say("I won't hold my breath", 169);
		}
		else
		{
			yield return C.Wizard.Say("You know, it's not such a bad way to die", 170);
			yield return C.Prince.Say("Hold on, Maldrek!", 162);
		}
		
		yield return E.Break;
	}

	public IEnumerator OnUseInvHotspotSack( IHotspot hotspot, IInventory item )
	{

		yield return E.Break;
	}

	public IEnumerator OnUseInvHotspotBuckets( IHotspot hotspot, IInventory item )
	{

		yield return E.Break;
	}

	public IEnumerator OnAnyClick()
	{
		
		
		if ( m_outroTime )
		{
			// E.WaitFor(Outro2);
		}
		else if ( m_state == eState.Down )
		{
			yield return E.WaitFor(Outro);
		}
		else if ( I.Levitate.Active )
		{
			yield return E.WaitFor(Jump);
		}
		yield return E.Break;
	}


	IEnumerator MaxillaAppear()
	{
		if (m_seenBuckets == false || m_seenSack == false || m_state >= eState.Down || E.FirstOccurance("maxillaAppear") == false )
			yield break;
			
		yield return C.Prince.Say("So... \"Space Chicken\"... is made out of people...", 163);
		yield return E.WaitSkip();
		yield return C.Wizard.Say("Well, I mean, they're barely people really. More weird alien things...", 171);
		yield return E.WaitSkip();
		yield return C.Prince.FaceRight();
		yield return C.Prince.Say("\"Space Chicken\" is people!!", 164);
		yield return E.WaitSkip();
		yield return C.Wizard.Say("Oh... ah. By Bartholin's Glands! I had no idea! How, err, awful!", 172);
		
		yield return E.WaitSkip();
		AudioHandle handle = Audio.Play("PhoneRing");
		handle.panStereo = 0.6f;
		yield return E.WaitSkip(1.0f);
		C.Maxilla.AnimIdle = "Phone";
		handle.Stop();
		Audio.Play("PhonePickup");
		yield return E.WaitSkip(0.25f);
		C.Maxilla.SayBG( "Space Chicken LLC, The meat must flow!", 0);
		
		// Fde in lady
		Camera.SetPositionOverride(10,0,1);
		float tint = 1.0f;
		while ( tint > 0.0f )
		{
			tint -= Time.deltaTime * 1.0f;
			Region("MaxillaTint").Tint = Region("MaxillaTint").Tint.WithAlpha(tint);
			yield return null;
		}
		Region("MaxillaTint").Tint = Region("MaxillaTint").Tint.WithAlpha(0);
		
		yield return E.WaitForDialog();
		yield return E.WaitSkip();
		
		C.Maxilla.SayBG("Yes, this is Dread-Queen Maxilla", 1);
		yield return E.WaitForDialogSkip();
		yield return C.Maxilla.FaceDown();
		yield return C.Maxilla.Say("Yes, yes, I've got two more Labyrinthians now, be processed by four", 2);
		yield return E.WaitSkip();
		yield return C.Maxilla.FaceRight();
		yield return C.Maxilla.Say("No! I don't want- Just- Get distribution onto it", 3);
		yield return E.WaitSkip();
		yield return C.Maxilla.FaceLeft();
		yield return C.Maxilla.Say("There's plenty of palettes in the warehouse, Sandra, we're-", 4);
		yield return C.Maxilla.FaceRight();
		yield return C.Maxilla.Say("I don't care if it takes you all day, I'm up to my mandible in back-orders!", 5);
		C.Maxilla.AnimIdle = "Idle";
		Audio.Play("PhoneHangup");
		yield return E.WaitSkip();
		yield return C.Maxilla.Say("Bah! Useless!", 6);
		
		Camera.ResetPositionOverride(2.0f);
		tint = 0.0f;
		while ( tint < 1.0f )
		{
			tint += Time.deltaTime / 2.0f;
			Region("MaxillaTint").Tint = Region("MaxillaTint").Tint.WithAlpha(tint);
			yield return null;
		}
		Region("MaxillaTint").Tint = Region("MaxillaTint").Tint.WithAlpha(1);
		
		C.Maxilla.Clickable = true;
		
		m_state = eState.SawMax;
		C.Maxilla.AnimIdle= "Phone";
		
		yield return E.Break;
	}
	
	public IEnumerator Jump()
	{
		if ( m_state < eState.Jumped )
		{
			C.Section("Try jumping off hook");
			// TODO: jump on hook first attempt anim
					 
			C.Prince.SayBG("LEVITATE!", 182);				
			yield return E.WaitForDialog();
			yield return E.WaitSkip();
			Audio.Play("MeatJumpA");
			Camera.Shake(1,0.1f,0.4f);			
			yield return E.WaitSkip(1);
			yield return C.Wizard.Say("Hey, you're on to something, do that again", 173);
			m_state = eState.Jumped;
		}
		else 
		{
			C.Section("Jump off hook	");
			// jump again ,get off
			// TODO: jump off hook anim
			
			C.Prince.SayBG("LEVITATE!", 182);				
			yield return E.WaitForDialog();
			yield return E.WaitSkip();
			Audio.Play("MeatJumpA");
			Camera.Shake(1,0.1f,0.4f);   
			yield return E.WaitSkip();
			Audio.Play("MeatJumpB");
			Camera.Shake(2,0.1f,0.4f);  
			yield return E.WaitSkip(0.25f);	
			E.ScreenFlash(E.ColorFlashRed);
			yield return E.WaitSkip(1.0f);
			Audio.Play("MeatJumpC");
			Camera.Shake(3,0.4f,0.4f);  
			yield return E.WaitSkip();
			E.ScreenFlash(E.ColorFlashRed,2.3f,1.0f);
			
			C.Prince.AnimIdle = "Idle";
			C.Wizard.AnimIdle = "Idle";
			C.Prince.Position = Point("PrinceDown");
			C.Wizard.Position = Point("WizardDown");	
			C.Wizard.Facing = eFace.DownRight;
			C.Prince.Facing = eFace.UpLeft;
			Prop("SacksDown").Visible = true;
			Prop("SackPrince").Hide();
			Prop("SackWizard").Hide();
			
			//Region("MaxillaTint").Tint = Region("MaxillaTint").Tint.WithAlpha(0);
			
			E.DisableAllClickablesExcept("Maxilla");
				
			m_state = eState.Down;
			GlobalScript.Script.m_inSack = false;
			C.Maxilla.Clickable = true;
			C.Maxilla.Cursor = "Talk";
			GlobalScript.Script.Hover = true;
			GlobalScript.Script.ShadowPrince = true;
			GlobalScript.Script.ShadowWizard = true;
			
			E.UpdateClickableCollider( C.Prince );
			E.UpdateClickableCollider( C.Wizard );
			I.Active = null;
			
			//Camera.SetPositionOverride(30);
			GlobalScript.Script.m_swapEnabled = false;
			yield return E.WaitSkip(1.0f);
			Audio.Play("MeatJumpFree");
			yield return E.WaitSkip(1.0f);
			yield return C.Prince.FaceRight();
			
		}
		yield return E.Break;
	}


	public IEnumerator Outro()
	{
		
		// Called on any click
		yield return E.WaitSkip();
		// prince steps forward no matter what you've clicked (i think)
		
		Globals.FollowOff();
		Globals.FaceOff();
		yield return C.Wizard.Face(C.Prince);
		yield return C.Wizard.Say("Okay, so-", 174);
		C.Wizard.Moveable = true;
		C.Prince.Moveable = true;
		
		C.Prince.SayBG("Dread-Queen Elsbeth Maxilla!", 165);
		yield return C.Prince.WalkTo(Point("PrinceEnd"));
		
		//Prince(165): Dread-Queen Elsbeth Maxilla!
		
		// Fde in lady
		Camera.SetPositionOverride(10,0,1);
		float tint = 1.0f;
		while ( tint > 0.0f )
		{
			tint -= Time.deltaTime * 1.0f;
			Region("MaxillaTint").Tint = Region("MaxillaTint").Tint.WithAlpha(tint);
			yield return null;
		}
		Region("MaxillaTint").Tint = Region("MaxillaTint").Tint.WithAlpha(0);
		
		yield return E.WaitWhile( ()=> C.Prince.Talking );
		yield return C.Wizard.Face(C.Prince);
		yield return C.Wizard.Say("Hold on, you idiot", 175);
		yield return C.Wizard.FaceRight();
		yield return C.Prince.FaceRight();
		C.Maxilla.FaceLeftBG();
		//C.Wizard.WalkToBG(Point("WizardEnd"));
		yield return E.WaitSkip();
		C.Maria.Pose = "Idle";
		yield return C.Maxilla.Say("Oh, hello", 7);
		C.Prince.SayBG("As a representative of-", 166);
		yield return E.WaitSkip(1.0f);
		
		//AudioHandle handle = Audio.Play("PhoneRing");
		
		C.Maxilla.AnimIdle = "Wait";
		yield return C.Maxilla.Say("One tick", 8);
		yield return E.WaitSkip(0.25f);
		//handle.Stop();
		//Audio.Play("PhonePickup");
		
		// C.Maxilla.FaceRight();
		yield return C.Maxilla.Say("No no no, it's THREE thousand to the Council", 9);
		C.Maxilla.AnimIdle = "Phone";
		yield return C.Maxilla.FaceRight();
		yield return C.Maxilla.Say("I don't CARE what I said, that batch is priority for King Henderson", 10);
		yield return C.Maxilla.Say("Now, gotta run", 11);
		
		Audio.Play("PhoneHangup");
		yield return E.WaitSkip(0.25f);
		C.Maxilla.AnimIdle = "Idle";
		yield return E.WaitSkip();
		yield return C.Maxilla.FaceLeft();
		yield return C.Maxilla.Say("Maldrek, darling!", 12);
		yield return C.Wizard.Say("Ahh...", 176);
		C.Wizard.WalkToBG(Point("WizardEnd"));
		yield return C.Maxilla.Say("Now, your order isn't due until Wednesday, I could push it to this afternoon", 13);
		C.Prince.FaceBG(C.Maxilla);
		yield return C.Maxilla.Say("But, prices have gone up a smidge", 14);
		yield return C.Wizard.WalkTo(Point("WizardEnd"));
		yield return C.Wizard.FaceRight();
		yield return C.Wizard.Say("Err, ixnay on the-", 177);
		yield return C.Maxilla.Say("I know! I know, again, right?!", 15);
		yield return C.Maxilla.Say("But well, what are your options? Give up interstellar flight? Ha!", 16);
		yield return E.WaitSkip(0.25f);
		
		AudioHandle handle = Audio.Play("PhoneRing");
		yield return E.WaitSkip(0.25f);
		C.Maxilla.AnimIdle = "Wait";
		yield return C.Maxilla.Say("Hold on, babe", 17);
		handle.Stop();
		Audio.Play("PhonePickup");
		
		yield return C.Maxilla.Say("What? Yes, I KNOW freezer A is down, that's why we moved batches four through eight to unit C", 18);
		C.Maxilla.AnimIdle = "Phone";
		yield return C.Maxilla.FaceRight();
		
		C.Section("Maxilla keeps talking, distracted, but it fades into the background");
		E.StartCustomBackgroundSequence(MaxillaTalk);
		
		//E.Player = C.Prince;
		G.Inventory.Visible = false;
		G.Inventory.Clickable = false;
		Globals.m_swapEnabled = false;
		C.Wizard.Description = "&31 Does Maldrek know this person?";
		E.ActiveInventory = null;
		C.Maxilla.Cursor = "None";
		
		m_outroTime = true;
		C.Prince.Moveable = false;
		C.Wizard.Moveable = false;
		yield return C.Wizard.FaceLeft();
		
		yield return C.Wizard.Say("Ahhhh....", 179);
	}
	
	bool m_outroTime = false;
	public IEnumerator Outro2()
	{
		
		GlobalScript.Script.FollowOff();
		GlobalScript.Script.FaceOff();
		C.Wizard.Moveable = true;
		C.Prince.Moveable = true;
		
		// prince is like, wtf?
		yield return C.Prince.Say("Maldrek, she seemed to know you", 167);
		yield return C.Wizard.FaceLeft();
		yield return C.Wizard.Say("I, ahh... nooo...", 178);
		yield return C.Prince.Say("Does the council know about this? I don't understand!", 168);
		yield return C.Wizard.WalkTo(Point("WizardEnd"));
		yield return C.Wizard.Say("Ahhhh....", 179);
		
		E.StopCustomBackgroundSequence();
		
		yield return C.Wizard.FaceRight();
		yield return E.WaitSkip();
		yield return C.Wizard.FaceLeft();
		yield return E.WaitSkip();
		yield return C.Wizard.FaceRight();
		yield return C.Wizard.FaceRight();
		yield return E.WaitSkip(1.0f);
		C.Wizard.AnimIdle = "Gun";
		//Audio.Play("GunCock");
		//yield return E.WaitSkip(1.0f);
		PowerQuest.Get.StopDialog();
		yield return E.WaitSkip();
		yield return C.Wizard.Say("FORCE BOLT!		", 180);
		
		yield return E.WaitSkip(0.25f);
		C.Maxilla.CancelSay();
		Audio.Play("GunshotKill");		
		E.FadeColor = E.ColorFlashWhite;		
		yield return E.FadeOut(0.1f);
		yield return E.WaitSkip(1.0f);
		yield return E.WaitSkip(1.0f);
		E.ChangeRoomBG(R.Space);
		
		
		
		yield return E.Break;
	}

	public IEnumerator OnUseInvHotspotHooks( IHotspot hotspot, IInventory item )
	{

		yield return E.Break;
	}

	IEnumerator MaxillaTalk()
	{
		yield return E.WaitSkip(1.0f);
		C.Maxilla.SayBG("I've already SENT a crew to repair it", 19);
		yield return E.WaitWhile(()=>C.Maxilla.Talking);
		yield return E.WaitSkip(1.0f);
		yield return C.Maxilla.SayNoSkip("No! We don't need to tell the Xenonians, I don't want them on my coccyx", 20);
		yield return E.WaitWhile(()=>C.Maxilla.Talking);
		yield return E.WaitSkip(0.25f);
		yield return C.Maxilla.SayNoSkip("Have enough on my plate with the council", 21);
		yield return E.WaitWhile(()=>C.Maxilla.Talking);
		yield return E.WaitSkip();
		yield return C.Maxilla.SayNoSkip("They're planning something, probably some anti-trust litigation, real pain in my thoracic", 22);
		yield return E.WaitWhile(()=>C.Maxilla.Talking);
		yield return E.WaitSkip(1.0f);
		yield return C.Maxilla.SayNoSkip("In fact, get Michael for me", 23);
		yield return E.WaitWhile(()=>C.Maxilla.Talking);
		yield return E.WaitSkip();
		yield return C.Maxilla.SayNoSkip("No, Sandra, not my step-son, why would I want to talk to him?", 24);
		yield return E.WaitWhile(()=>C.Maxilla.Talking);
		yield return E.WaitSkip(1.0f);
		yield return C.Maxilla.SayNoSkip("Michael Simmonds. My LAWYER. That's right", 25);   
	}


	public IEnumerator WizardClickHotspotFloor( IHotspot hotspot )
	{
		yield return C.Wizard.Say("How we getting out of this one?", 181);
		yield return E.Break;
	}

	public IEnumerator PrinceClickHotspotFloor( IHotspot hotspot )
	{
		yield return C.Prince.Say("We have to get free!", 169);
		yield return E.Break;
	}

	public IEnumerator PrinceClickHotspotSacks( IHotspot hotspot )
	{
		yield return C.Prince.Say("I can't move!", 170);
		yield return E.Break;
	}

	public IEnumerator WizardClickHotspotSacks( IHotspot hotspot )
	{
		yield return C.Wizard.Say("This thing's kind of cosy really", 182);
		yield return E.Break;
	}

	public IEnumerator OnUseInvHotspotSacks( IHotspot hotspot, IInventory item )
	{
		if ( I.Eat.Active )
		{
			yield return C.Wizard.Say("Aghh, it's tougher than the perineum of a razor spiked urchin beast", 183);
		}
		yield return E.Break;
	}

	public IEnumerator OnUseInvHotspotFloor( IHotspot hotspot, IInventory item )
	{

		yield return E.Break;
	}

	public void OnPostRestore( int version )
	{
		m_updateColls = true;
	}

	public IEnumerator UpdateBlocking()
	{
		
		if ( m_updateColls )
		{
			m_updateColls = false; 
			E.UpdateClickableCollider( C.Prince );
			E.UpdateClickableCollider( C.Wizard );
		}
		yield return E.Break;
	}
}
