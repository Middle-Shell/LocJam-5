using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;

public class RoomLabyrinth : RoomScript<RoomLabyrinth>
{
	
	//bool m_steppedDoor = false;
	public enum eFlat { Start, Stepped, Talked, Talked2, Up, NeedPotion, Dead, Done }
	public eFlat m_flat = eFlat.Start;
	
	public enum eDoorLady{ Start, Talked, Opened, AllOpen, Stolen, Done }
	public eDoorLady m_doorLady = eDoorLady.Start;
	
	bool m_planetRise = false;
	float m_planetHeight = -89;
	
	public enum eChicken { Start, Need, Have, Eaten, Hyperdrived }
	public eChicken m_chicken = eChicken.Start;
	
	public void OnEnterRoom()
	{
		if ( R.EnteredFromEditor )
		{
			E.FadeOutBG(0);
		}
		
		if ( R.Current.FirstTimeVisited && R.Skull.Visited == false )
		{	
			// Remove items you ahve on first screen
			C.Prince.RemoveInventory(I.Spellbook);
			C.Prince.RemoveInventory(I.Map);
			C.Wizard.AddInventory(I.Hyperdrive);
			C.Wizard.AddInventory(I.Steal);
			C.Wizard.AddInventory(I.Eat);
			C.Wizard.AddInventory(I.Shoot);
			C.Wizard.AddInventory(I.Mindtrick);	 
			
			C.Prince.Position = Point("StartPrince");		
			C.Wizard.Position = Point("StartWizard");
			C.Wizard.AnimIdle = "Sit";
			C.Wizard.Facing = eFace.Left;
			C.Prince.Facing = eFace.Right;
			C.Wizard.Clickable = true;
			C.Prince.AnimIdle = "Idle";
		
			C.Flat.Position = Point("FlatGuy");
			C.Flat.AnimIdle = "Chill";
			C.Flat.Show();
				
			Prop("DoorDown").Visible = false;
			Prop("Book").Visible = false;
			Prop("House").Animation = "House";
			Prop("WbA").Visible = false;
			
			
			Camera.SetPositionOverride(140);	
			
			C.Prince.Visible = false;
			C.Wizard.Visible = false;
			
		
			
			/*
			C.Prince.Position = Point("StartPrince");		
			C.Wizard.Position = Point("StartWizard");
			C.Wizard.AnimIdle = "Sit";
			C.Wizard.Facing = eFace.Left;
			C.Prince.Facing = eFace.Right;
			C.Wizard.Show();
			
			C.Flat.Position = Point("FlatGuyVoice");
			C.Flat.Hide();
			
			E.FadeIn(1f);
			Camera.Shake(1,0.5f,2);
			*/
			   
		}
		else 
		{
			Audio.PlayMusic("MusicLabyrinth",0.8f,0.4f);
			// came from skull planet
			Camera.Shake(0.5f,0.2f,1.0f);
			
			C.Prince.Position = Point("PrinceFromSkull");
			C.Wizard.Position = Point("WizardFromSkull");
			C.Wizard.Facing = eFace.Right;
			C.Prince.Facing = eFace.Right;
			C.Prince.Show();
			C.Wizard.Visible = true;
			C.Prince.AnimIdle = "Idle";
			C.Wizard.AnimIdle = "Idle";
		}
		Audio.PlayAmbientSound("SoundAmbient-Village",0.8f,0.4f);
		C.Prince.FootstepSound = "Footstep";
	}


	public IEnumerator OnExitRoom( IRoom oldRoom, IRoom newRoom )
	{
		E.StopCustomBackgroundSequence();
		yield return E.Break;
	}

	public IEnumerator OnEnterRoomAfterFade()
	{
		if ( R.Current.FirstTimeVisited && R.Skull.Visited == false )
		{
			// Camera pan from right
			// Slow fade to white
			
			E.StartCutscene();
			
			Camera.SetPositionOverride(-140,0,0);
			E.FadeInBG(1f);
			yield return E.WaitSkip(1.0f);
			Camera.SetPositionOverride(140,0,6);
			yield return E.Wait(5);
			E.FadeColor = E.ColorFlashWhite;
			
			AudioHandle h = Audio.Play("CrashFall");
			h.panStereo = -0.8f;
			h.FadeIn(1);
			
			yield return E.Wait(0.5f);
			yield return E.FadeOut(2);
			Audio.Play("Crash").panStereo = -0.7f;	
			
			
			
			C.Wizard.Show();	
			C.Prince.Visible = true;
			C.Wizard.Visible = true;
			
			GlobalScript.Script.ShadowPrince = true;
			
			Prop("DoorDown").Visible = true;
			Prop("Book").Show();
			Prop("House").Animation = "HouseSmash";
			Prop("WbA").Visible = true;	
			C.Flat.Position = Point("FlatGuyVoice");
			C.Flat.AnimIdle = "Idle";
			C.Flat.Hide();
			
			Camera.Shake(1,0.5f,2);
			yield return E.FadeIn(1.5f);
			
			yield return E.WaitSkip(1.0f);
			Camera.SetPositionOverride(-140,0,4);
			yield return E.WaitSkip(4); 
			Audio.PlayMusic("MusicLabyrinth",2.0f);
			yield return E.WaitSkip(1.0f);
			/*
			E.FadeInBG(1f);
			Camera.SetPositionOverride(-140,0,10);
			yield return E.Wait(6);
			E.FadeColor = E.ColorFlashWhite;
			Audio.Play("CrashFall").panStereo = -0.8f;
			yield return E.Wait(0.5f);
			yield return E.FadeOut(2);
			Audio.Play("Crash").panStereo = -0.8f;	
			
			C.Wizard.Show();	
			C.Prince.Visible = true;
			C.Wizard.Visible = true;
			
			Prop("DoorDown").Visible = true;
			Prop("Book").Show();
			Prop("House").Animation = "HouseSmash";
			Prop("WbA").Visible = true;
			
			yield return E.WaitSkip(0.25f);
			Camera.Shake(1,0.5f,2);
			yield return E.FadeIn(1.5f);
			
			*/
		
			E.FadeColorRestore();
			Camera.ResetPositionOverride(2);
			E.EndCutscene();
		}
		else 
		{
			GlobalScript.Script.FollowOn();
			GlobalScript.Script.Hover = true;
			GlobalScript.Script.ShadowPrince = true;
			GlobalScript.Script.ShadowWizard = true;
		}
		yield return E.Break;
	}

	public IEnumerator UpdateBlocking()
	{
		if ( Input.GetKey(KeyCode.Tab)&& Input.GetKey(KeyCode.F) )
		{
			yield return C.Display("Debug: flat guy dead", 4);
			m_flat = eFlat.Dead;
			C.Flat.Show();
			C.Flat.Position = Point("FlatGuy");
			Prop("DoorDown").Hide();
			Prop("DoorUp").Show(false);
			C.Flat.Cursor = "Look";
			C.Flat.AnimIdle = "Dead";
			C.Flat.Description = "&10 Our friend here has somehow developed an unsightly hole";
		}
		if ( Input.GetKey(KeyCode.Tab)&& Input.GetKey(KeyCode.P) )
		{
			yield return C.Display("Debug: Got hyperdrive", 5);
			m_flat = eFlat.Dead;
			C.Flat.Show();			
			C.Flat.Position = Point("FlatGuy");
			Prop("DoorDown").Hide();
			Prop("DoorUp").Show(false);
			C.Flat.Cursor = "Look";
			C.Flat.AnimIdle = "Dead";
			C.Flat.Description = "&11 Our friend here has somehow developed an unsightly hole";
			
			// STart planet moving	
			C.Wizard.AnimIdle = "Idle";
			C.Wizard.Baseline = 0;
			
			GlobalScript.Globals.FollowOn();

			yield return E.WaitSkip();
			yield return C.Wizard.WalkTo(-118, -120);
			yield return E.WaitSkip();	
			yield return C.Prince.Face(C.Wizard);
			m_planetRise = true;
			
			m_flat = eFlat.Done;
			C.Flat.Cursor = "Look";
		
			// Enable swap
			GlobalScript.Script.m_swapEnabled = true;
			G.Inventory.Instance.GetComponent<GuiPlrChange>().FlashOn();
			
			// Door person appears
			C.Doorian.Show();
			
			m_chicken = eChicken.Eaten;
			
		}
		yield return E.Break;
	}

	public void Update()
	{
		if ( m_planetRise && E.Paused == false )
		{
			//float planetY = Prop("Planet").Position.y;
			m_planetHeight = Mathf.Lerp(m_planetHeight,0,Time.deltaTime / 30.0f );
			
			Prop("Planet").Position = Prop("Planet").Position.WithY(Utils.Snap(m_planetHeight));
		
			if ( m_planetHeight > -60 && Prop("Planet").Clickable == false )
				Prop("Planet").Clickable = true;
		}
	}

	public IEnumerator OnEnterRegionDoor( IRegion region, ICharacter character )
	{
		if ( character.IsPlayer && Prop("DoorDown").Visible )
		{
			Prop("DoorDown").Animation = "DoorDownSquash";   
			if ( m_flat == eFlat.Start )
				m_flat = eFlat.Stepped;
			Prop("DoorDown").Clickable = true;
			switch ( Random.Range(0,3) )
			{	
			case 0:
				C.Flat.SayBG("Oof", 0);
				break;
			case 1:
				C.Flat.SayBG("Agh", 1);
				break;
			case 2:
				C.Flat.SayBG("Ugh", 2);
				break;
			}
		}
		yield return E.Break;
	}

	public IEnumerator OnExitRegionDoor( IRegion region, ICharacter character )
	{
		if ( character.IsPlayer && Prop("DoorDown").Visible )
		{
			Prop("DoorDown").Animation = "DoorDown";   
			if ( m_flat == eFlat.Stepped )
			{
				Prop("DoorDown").Clickable = true;
				Prop("DoorDown").Cursor = "Look";
			}
		}
		yield return E.Break;
	}

	public IEnumerator PrinceClickPropDoorDown( IProp prop )
	{
		
		if ( m_flat == eFlat.Stepped )
		{
			C.Section(" Talk to flattened man first time");
			yield return C.Plr.WalkTo(Point("StartPrince"));
			yield return C.FaceClicked();	
			yield return E.WaitSkip();
			yield return C.Prince.FaceRight();
			yield return C.Prince.Say("The floor's making noises Maldrek", 27);
			yield return C.Flat.Say("I'm stuck under here, eh?", 3);
			Prop("DoorDown").Cursor = "Talk";
			m_flat = eFlat.Talked;
		}
		else if ( m_flat == eFlat.Talked )
		{
			C.Section("Talk to flattened man again");
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			yield return C.Prince.Say("Ah, hello?", 28);
			yield return C.Flat.Say("Hi, ah, sorry to bother you, but I seem to have been crushed under this door, eh?", 4);
			yield return C.Prince.Say("Ah ha, this calls for some magic", 29);
			m_flat = eFlat.Talked2;
		}
		else 
		{
			C.Section("Talk to flattened man again 2");
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			yield return C.Prince.Say("Hold tight my friend, I'll have that off you in a jiffy", 30);
			yield return C.Flat.Say("She'll be right bro, kinda cosy under here, eh?", 5);
			yield return C.Prince.Say("This calls for some magic", 31);
			//m_flat = eFlat.Talked2;
		}
		
		
		
		yield return E.Break;
	}

	public IEnumerator PrinceClickPropBook( IProp prop )
	{
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		yield return C.Prince.Say("Ooh, my ah, spell book", 32);
		yield return C.Prince.Say("I'll need that to work my magic", 33);
		C.Prince.AnimIdle = "Crouch";
		GlobalScript.Script.FaceOn();
		yield return C.Wizard.Say("Heh, your \"magic\"", 15);
		
		prop.Hide();
		I.Charm.Add();
		I.Lift.Add();
		I.Punch.Add();
		I.Levitate.Add();
		yield return E.WaitSkip();
		C.Prince.AnimIdle = "Idle";
		C.Prince.WalkToBG(C.Wizard,false, eFace.Right);
		yield return E.WaitSkip();
		yield return C.Wizard.Say("My epileptic cat Jasper has more magic in her left foot", 16);
		yield return C.Prince.Face(C.Wizard);
		yield return C.Wizard.Say("That's the one we had to cut off", 17);
		yield return E.WaitSkip();
		yield return C.Wizard.Say("Gangrene, you know", 18);
		GlobalScript.Script.FaceOff();
		//Wizard: I have more magic in my vestigial tail than you have in your entire meaty body
		
		yield return E.Break;
	}

	public IEnumerator OnUseInvPropDoorDown( IProp prop, IInventory item )
	{
		if ( item == I.Lift )
		{
			yield return C.Plr.WalkTo(-184, -115);
			yield return C.Plr.FaceUpLeft();
			C.Section("Lift door of flattened guy	");
		
			yield return E.WaitSkip();
			C.Prince.AnimIdle = "Crouch";
			yield return C.Prince.Say("TELEKINESIS!", 34);
			yield return E.WaitSkip(0.25f);
			
			C.Flat.Show();
			C.Flat.Clickable=true;
			C.Flat.Position = Point("FlatGuy");	
			C.Prince.AnimIdle = "Idle";
			prop.Hide();
			Prop("DoorUp").Visible = true;
			Audio.Play("DoorLift");
			Camera.Shake();
			I.Active = null;
			
			m_flat = eFlat.Up;
			yield return E.WaitSkip(1.0f);
			yield return C.Flat.Say("Sweet as, thanks cuz", 6);
			yield return C.Prince.Say("Sorry about, er, landing on your hovel", 35);
			yield return C.Flat.Say("Oh, no worries bro, that's what insurance is for, eh?", 7);
			yield return C.Flat.Say("Just grab your deets if that's cool? ", 8);
			yield return C.Prince.Say("What do you need?", 36);
			yield return C.Flat.Say("Name, where you hail from, wizard rego", 9);
			yield return C.Prince.Say("Oh, yes, it's...", 37);
			yield return E.WaitSkip();
			yield return C.Prince.Say("Prince Xandar...", 38);
			yield return C.Prince.Say("Ah, X.A.N.D.A.R.", 39);
			yield return C.Flat.Say("Mmmm	", 10);
			yield return C.Prince.Say("Yep, ah, Henderson", 40);
			yield return E.WaitSkip();
			yield return C.Prince.Face(C.Wizard);
			yield return C.Prince.Say("Maldrek, what's your rego?", 41);
			yield return C.Wizard.Say("E 4 T - 5 H 1 T", 19);
			yield return C.Prince.Face(C.Flat);
			yield return C.Flat.Say("Planet?", 11);
			yield return C.Prince.Say("Oh, ah, Spacekeep Mana-1", 42);
			yield return E.WaitSkip();
			yield return C.Flat.Say("Too easy", 12);
		}
		yield return E.Break;
	}

	public IEnumerator OnUseInvPropPlanet( IProp prop, IInventory item )
	{
		if ( item == I.Hyperdrive )
		{
			if ( m_chicken == eChicken.Start )
			{
				C.Section("Use hyperdrive on planet first time");
				yield return C.Wizard.FaceUpLeft();
				yield return C.Prince.Face(C.Wizard);
				yield return C.Wizard.Say("Lets go to that there planet			", 20);
				yield return C.Wizard.WalkTo(Point("WizBlastOff"));
				yield return C.Wizard.FaceUpLeft();
				yield return E.WaitSkip();
				yield return C.Wizard.Say("Oh, Alcock's Canal!", 21);
				yield return C.Wizard.Say("My MP's too phleppin' low to cast Hyperdrive", 22);
				C.Wizard.FaceBG(C.Prince);
				yield return C.Prince.Say("Maybe a protein shake?		", 43);
				yield return C.Wizard.Say("Sod that. A good bucket of \"Space Chicken\"'s what I need", 23);
				
				// TODO: udpate hyperdrive description
				I.Hyperdrive.Description = "&12 Spell: Hyperdrive - Interstellar travel requiring tremendous energy. Currently depleted";
				
				m_chicken = eChicken.Need;
			}
			else if ( m_chicken == eChicken.Need )
			{
				C.Section("Use hyperdrive on planet 2nd time");
				yield return C.Wizard.FaceUpLeft();
				yield return C.Wizard.Say("Can't hyperdrive without my \"Space Chicken\"", 24);
			}
			else if ( m_chicken == eChicken.Have )
			{
				C.Section("Use hyperdrive without having eaten  chicken");
				yield return C.FaceClicked();
				yield return C.Wizard.Say("Gotta absorb that \"Space Chicken\" power!", 25);
				yield return C.Wizard.Say("I have just the spell too", 26);
			}
			else if ( m_chicken == eChicken.Eaten )
			{
				C.Section("Use hyperdrive first time");
				yield return C.Wizard.Face(C.Prince);
				yield return C.Wizard.Say("Hey, Xandar, time to hightail it to Lobos", 27);
				yield return C.Wizard.WalkTo(Point("WizBlastOff"));
				yield return C.Wizard.FaceUpLeft();
				C.Prince.WalkToBG(Point("WizBlastOff"));
				I.Active = null;
				E.ChangeRoomBG(R.Space);
			}
			else if ( m_chicken == eChicken.Hyperdrived )
			{
				C.Section("Use hyperdrive again");
				switch ( E.Occurrance("golobos")%5 )
				{
					case 0:
					yield return C.Wizard.Say("I'm sick of this wreck of a planet, let's go", 28);
					break; case 1:
					yield return C.Wizard.Say("Hey, let's blow this stinkin' dump", 29);
					break; case 2:
					yield return C.Wizard.Say("Let's check out Lobos again", 30);
					break; case 3:
					yield return C.Wizard.Say("We're going back Xandar", 31);
					break; case 4:
					yield return C.Wizard.Say("I'm goin' back if you wanna lift", 32);
					break;
				}
				C.Wizard.WalkToBG(prop, false, eFace.UpLeft );
				yield return E.WaitSkip();
				switch ( E.Occurrance("golobos")%3 )
				{
					case 0:
					yield return C.Prince.Say("Coming", 44);
					break; case 1:
					yield return C.Prince.Say("Okay, okay", 45);
					break; case 2:
					yield return C.Wizard.Say("Sounds good", 33);
					break;
				}
				C.Prince.WalkToBG(prop);
				I.Active = null;
				E.ChangeRoomBG(R.Space);
			}
		}
		else if ( item == I.Chicken )
		{
			yield return C.FaceClicked();
			yield return C.Wizard.Say("The planet doesn't need the chicken, I do", 34);
		}
		yield return E.Break;
	}

	public IEnumerator PrinceClickCharacterFlat( ICharacter character )
	{
		
		if ( m_flat == eFlat.Up )
		{
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			yield return C.Prince.Say("Anyway, we're here on bit of a quest, you know", 46);
			yield return C.Wizard.Say("Yeah, what's this distress call about?", 35);
			yield return C.Flat.Say("Oh, distress call? Dunno eh?", 13);
			yield return E.WaitSkip();
			yield return C.Flat.Say("Heard a pretty juicy rumor, though", 14);
			yield return C.Prince.Say("Oh yeah?", 47);
			yield return C.Flat.Say("Yeah, the planetary council's not happy about how dear the old space-waka's getting", 15);
			yield return C.Flat.Say("Honest to G, I heard they sent some hatchet-fella to overthrow the-", 16);
			yield return C.Wizard.Say("A-HEEEM", 36);
			yield return C.Prince.Face(C.Wizard);
			yield return E.WaitSkip();
			yield return C.Wizard.Say("Poor idiot's clearly delusional, probably loss of blood", 37);
			yield return C.Prince.Face(C.Flat);
			yield return C.Flat.Say("I do seem to be dying of blood loss, eh?", 17);
			yield return C.Prince.Say("Tell you what, I'll go fetch something to sort you out", 48);
			yield return C.Flat.Say("Minor healing potion should do it, guy over there sells 'em", 18);
			yield return C.Prince.Say("Then you can tell me the rest", 49);
			yield return C.Flat.Say("Sure, I have this guy's name and everything, eh?", 19);
			yield return C.Wizard.Say("That can wait 'til you're all better", 38);
				
			m_flat = eFlat.NeedPotion;
		}
		else if ( m_flat == eFlat.NeedPotion )
		{
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			yield return C.Prince.Say("I'll get you that potion", 50);
			yield return C.Flat.Say("No wuckas bro", 20);
		}
		else if ( m_flat == eFlat.Dead )
		{
			yield return C.WalkToClicked();
			yield return C.Prince.Face(C.Flat);
			C.Section("Look at flat guy when he's dead");
			yield return E.WaitSkip(1.5f);
			yield return C.Prince.Face(C.Wizard);
			yield return E.WaitSkip();
			yield return C.Prince.Say("What happened?", 51);
			yield return E.WaitSkip(0.25f);
			yield return C.Wizard.Say("Eh. Who could say?", 39);
			yield return C.Prince.Face(C.Flat);
			yield return C.Prince.Say("Pity, seems like he knew something", 52);
			yield return C.Wizard.Say("Yeah, a real tragedy", 40);
			
			// Wizard stands
			
			C.Wizard.SayBG("We goin'?", 41);
			//Wizard(41): We goin'?
			C.Wizard.Facing = eFace.Left;
			C.Wizard.AnimIdle = "Idle";
			C.Wizard.Baseline = 0;
			GlobalScript.Script.Hover = true;
			GlobalScript.Script.ShadowWizard = true;
			yield return E.WaitSkip();
			yield return C.Wizard.WalkTo(-118, -120);
			C.Prince.FaceBG(C.Wizard);	
			yield return E.WaitSkip();
			yield return C.Prince.Say("Well, what about our quest, this, ah, distress call?", 53);
			yield return C.Wizard.Say("Herrrrrrrrrrr... ", 42);
			yield return C.Wizard.Say("Fine. I'll poke around", 43);
			yield return E.WaitSkip();
			// STart planet moving
			
			m_planetRise = true;
			
			m_flat = eFlat.Done;
			C.Flat.Cursor = "Look";
		
			// Enable swap
			GlobalScript.Script.m_swapEnabled = true;			
			GlobalScript.Script.FollowOn();
			G.Inventory.Instance.GetComponent<GuiPlrChange>().FlashOn();
			yield return E.WaitSkip(1.0f);
			// Door person appears
			C.Doorian.Show();
			
			GlobalScript.Script.m_goal = eGoal.Investigate;
			GlobalScript.Script.FollowOn();
		}
		else
		{
		
			C.Section("prince look at dead guy again");
			yield return C.FaceClicked();		
			yield return C.Prince.Say("The mystery deepens", 54);
		}
		yield return E.Break;
	}

	public IEnumerator PrinceClickCharacterMerchant( ICharacter character )
	{
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		
		bool talked = false;
		if ( E.FirstOccurance("tlkPrince") )
		{
			C.Section(" Talk to merchant first time");
		
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			yield return C.Prince.Say("Hello there", 55);
			yield return C.Merchant.Say("Ah, a wizard if I'm not mistaken", 0);
			yield return C.Merchant.Say("After some \"Space Chicken\" then, are you?", 1);
			yield return C.Prince.Say("Ooh, er, no don't touch the stuff myself", 56);
			yield return C.Prince.Say("Processed food does terrible things to your gut biome, you know", 57);
			yield return C.Merchant.Say("Heh, suit yourself", 2);
			talked = true;
		}
		if ( m_flat == eFlat.NeedPotion )
		{
			C.Section(" Talk to merchant Ask for potion");
			
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			yield return C.Prince.Say("I'm after a minor heal potion, I was hoping-", 58);
			yield return C.Merchant.Say("That'll be two gronks", 3);
			
			// SEt flat guy to dead		
			C.Flat.Cursor = "Look";
			C.Flat.AnimIdle = "Dead";
			C.Flat.Description = "&13 Our friend here has somehow developed an unsightly hole";
			m_flat = eFlat.Dead;
			talked = true;
			C.Wizard.SayBG("FORCE BOLT!", 44);
			yield return E.WaitSkip();
			Audio.Play("GunshotKill2").panStereo = -0.6f;
			yield return E.WaitSkip(0.25f);
			yield return C.Prince.Say("Oh, right, ah, see, the guy over there? He's gonna bleed to death if I don't-", 59);
			yield return C.Merchant.Say("Got private health?", 4);
			yield return C.Prince.Say("Well, I don't have my card on me", 60);
			yield return C.Merchant.Say("Two cronks", 5);
			yield return C.Prince.Say("I thought you said gronks", 61);
			yield return C.Merchant.Say("Gronks, cronks... doesn't matter to me. All I know is, minor heal, that's 2 of em", 6);
			yield return C.Prince.Say("I only have imperial kothags", 62);
			yield return E.WaitSkip(1.0f);
			yield return C.Prince.FaceLeft();
			yield return C.Prince.Say("Hey Maldrek!", 63);
			talked = true;	
		}
		
		// tODO: add stuff about getting chicken in here
		if ( m_chicken == eChicken.Need && E.FirstOccurance("tlkMechChickPrin") )
		{
			C.Section("Prince talks to merchant about wizard needing chicken");
		
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			yield return C.Prince.Say("So, as it turns out, I do need some of that \"Space Chicken\", for my friend", 64);
			yield return E.WaitSkip();
			yield return C.Merchant.Say("Of course, yes, for your friend", 7);
			yield return E.WaitSkip(1.0f);
			yield return C.Merchant.Say("50 zonbecks ", 8);
			talked = true;	
		}
		else if ( m_chicken >= eChicken.Need && E.FirstOccurance("tlkMechChickPrin2") )
		{
			C.Section("Talk to merchant about chicken again");
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			yield return C.Prince.Say("They say he who controls the \"Space Chicken\" controls the universe", 65);
			C.Prince.SayBG("That must mean you control the universe", 66);
			yield return E.Wait(1);
			yield return C.Merchant.Say("...control the universe", 9);
			yield return E.WaitSkip(1.0f);
			yield return C.Prince.Say("I suppose you get that one a lot", 67);
			yield return E.WaitSkip(1.0f);
			yield return C.Merchant.Say("Errr-huh", 10);
			talked = true;
		}
		else if ( talked == false )
		{
			C.Section("Talk to merchant about random stuff");
			int occ = E.Occurrance("tlkmerch");
			
			if ( occ == 0 )
			{
				yield return C.WalkToClicked();
				yield return C.FaceClicked();
				C.Section("Talk to merchant again");
				yield return C.Prince.Say("Do you know why a distress call would have been sent from this planet?", 68);
				yield return C.Merchant.Say("Can't help you there, I'm just a humble merchant", 11);
			}
			else if ( occ == 1 )
			{
			C.Section("Talk to merchant about random 2");
				yield return C.Prince.Say("So what else do you sell?", 69);
				yield return C.WalkToClicked();
				yield return C.FaceClicked();
				yield return C.Merchant.Say("We have blood sausage", 12);
				yield return E.WaitSkip();
				yield return C.Merchant.Say("Blood pies", 13);
				yield return E.WaitSkip();
				yield return C.Merchant.Say("Blood", 14);
				yield return E.WaitSkip();
				yield return C.Merchant.Say("Chips with blood", 15);
				yield return E.WaitSkip();
				yield return C.Merchant.Say("Blood soup", 16);
				yield return E.WaitSkip();
				yield return C.Merchant.Say("That one's popular", 17);
				yield return E.WaitSkip(1.0f);
				yield return C.Merchant.Say("Minor heal potions", 18);
				yield return E.WaitSkip();
				yield return C.Merchant.Say("Made with real blood", 19);
				yield return E.WaitSkip();
				yield return C.Merchant.Say("Huuuuuuuuurrrr", 20);
				yield return E.WaitSkip(1.0f);	
				yield return C.Merchant.Say("And yer \"Space Chicken\"", 21);
				yield return E.WaitSkip();
				yield return C.Prince.Say("Right... I'm trying to eat less meat, you see, and-", 70);
				yield return C.Merchant.Say("Blood's not meat", 22);
				yield return E.WaitSkip();
				yield return C.Prince.Say("Right, but it's... kind of the same thing, you see-", 71);
				yield return C.Merchant.Say("I don't", 23);
				yield return E.WaitSkip();
				yield return C.Prince.Say("Ah, well. Actually, I'll just keep browsing for now", 72);
			}
			else if ( occ == 2)
			{
				yield return C.WalkToClicked();
				yield return C.FaceClicked();
				C.Section("Talk to merchant about random 3 ");
				yield return C.Prince.Say("This blood. It's not Labyrinthian blood is it?", 73);
				yield return C.Merchant.Say("Cat blood", 24);
				yield return C.Prince.Say("Okay. That's good... I guess", 74);
			}
			else
			{
				C.Section("Talk to him again 2");
				yield return C.FaceClicked();
				yield return C.Prince.Say("I might just leave him be", 75);
				if ( occ == 3 )   
					yield return C.Prince.Say("I, ah, find him a little off-putting", 76);
			}
		}
		
		yield return E.Break;
	}

	public IEnumerator OnEnterRegionLeftHalf( IRegion region, ICharacter character )
	{
		/*if ( character.IsPlayer && m_flat == eFlat.Dead )
		{
			yield return C.Plr.WalkTo(-47, -121);
			Camera.SetPositionOverride(-480,0,1);
			
		}
		*/
		
		yield return E.Break;
	}

	public IEnumerator PrinceClickCharacterWizard( ICharacter character )
	{
		
		if ( E.FirstOccurance("tlkMeldr") && m_flat != eFlat.Dead )
		{
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			C.Section("Talk to maldrek first time");
			yield return C.Wizard.Face(C.Plr);
			yield return C.Prince.Say("Quite the landing there, you okay?", 77);
			yield return C.Wizard.Say("Of course I'm not you great idiot!", 45);
			yield return E.WaitSkip();
			yield return C.Prince.Say("Maldrek...", 78);
			yield return E.WaitSkip();
			yield return C.Wizard.Say("Ah... sorry", 46);
			yield return C.Wizard.Say("Blood sugar gets a little low sometimes, I get cranky", 47);
			yield return C.Prince.Say("You should try papaya, there's a man in Brazil, ate nothing but papaya for a year", 79);
			yield return C.Prince.Say("CURED his diabetes", 80);
			yield return C.Wizard.Say("I'll stick to my hourly rat pancreas thanks", 48);
			yield return C.Prince.Say("Suit yourself", 81);
			yield return C.Wizard.Say("Just give me a minute, I'll be fine", 49);
		
		}
		else if ( m_flat == eFlat.Dead )
		{
			yield return E.HandleInteract(C.Flat);
		}
		else 
		{
			int occ = E.Occurrance("TlkMeldrekRand");
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			
			if ( occ == 0 )
			{
				C.Section("Talk to maldrek again");
				yield return C.Prince.Say("So where do you think we are?", 82);
				yield return C.Wizard.Face(C.Plr);
				yield return C.Wizard.Say("Well, based on the fact we were heading to Labyrinthia, the labyrinth planet", 50);
				yield return C.Wizard.Say("And factoring in the endless labyrinth surrounding us...", 51);
				yield return E.WaitSkip();
				yield return C.Prince.FaceAway();
				yield return E.WaitSkip();
				yield return C.Prince.Say("You're right, we should ask someone", 83);
			}
			else if ( occ == 1 )
			{
				C.Section("Talk to maldrek about mission");
				yield return C.Prince.Say("So, this mission, ah...", 84);
				yield return C.Wizard.Face(C.Plr);
				yield return C.Wizard.Say("We're here investigating a distress call", 52);
				yield return C.Prince.Say("Right, that's right", 85);
				yield return C.Wizard.Say("Go ask around or something		", 53);
			}
			else 
			{
				C.Section("Talk to maldrek again");
				yield return C.Prince.Say("Whatcha wanna do next?", 86);
				yield return C.Wizard.Face(C.Plr);
				yield return C.Wizard.Say("I don't know, talk to some of these repulsive natives", 54);
				
				if ( occ == 2 )
				{
					yield return C.Wizard.Say("Do whatever pathetic task they have for us", 55);
					yield return C.Wizard.Say("Then leg it back before my toes itch too bad", 56);
					yield return C.Prince.Say("Oh, Maldrek, you didn't forget your fungus powder?", 87);
				}
			}
		}
		
		yield return E.Break;
	}

	public IEnumerator OnUseInvCharacterWizard( ICharacter character, IInventory item )
	{

		yield return E.Break;
	}

	public IEnumerator PrinceClickCharacterDoorian( ICharacter character )
	{
		E.StopCustomBackgroundSequence();
		if ( m_doorLady == eDoorLady.Start )
		{
			if ( C.Wizard.IsPlayer )
			{			
				yield return C.Wizard.WalkTo(Point("WizTalkDoor"));
				yield return C.FaceClicked();  
				C.Prince.WalkToBG(C.Doorian,false, eFace.UpLeft);
				yield return C.Wizard.Say("Space-christ, what's wrong with this one?", 57);
				yield return C.Doorian.Face(C.Wizard);
				yield return E.WaitUntil( ()=>C.Prince.Walking == false );
				yield return C.Prince.Face(eFace.UpLeft);
				yield return C.Prince.Say("Pardon my friend here", 88);
			}
			else 
			{
				yield return C.WalkToClicked();
				yield return C.FaceClicked();
			}
			C.Doorian.FaceBG(C.Prince);
			yield return C.Prince.Say("I'm Prince Xandar, and this is Maldrek", 89);
			yield return C.Wizard.WalkTo(Point("WizTalkDoor"));
			yield return C.Wizard.Face(C.Prince);
			yield return C.Wizard.Say("Oh, for- Don't TALK to it!	", 58);
			C.Doorian.AnimIdle = "Idle"+Random.Range(1,4);
			yield return C.Doorian.Say("Hi!! Oh my hinges, it's great to meet you! ", 0);
			C.Doorian.AnimIdle = "Idle"+Random.Range(1,4);
			yield return C.Doorian.Say("I'm Doorian, Doorian Knobb!", 1);
			C.Doorian.AnimIdle = "Idle"+Random.Range(1,4);
			yield return C.Prince.Say("What, ah, what's happening?", 90);
			C.Doorian.AnimIdle = "Idle"+Random.Range(1,4);
			yield return C.Doorian.Say("Oh, you know, just thinking about DOORS!", 2);
			C.Doorian.AnimIdle = "Idle"+Random.Range(1,4);
			yield return C.Doorian.Say("Did you know, there are over eight thousand types of doors in the labyrinth!", 3);
			C.Doorian.AnimIdle = "Idle"+Random.Range(1,4);
			yield return C.Doorian.Say("I have a spreadsheet of all of them, I can show you!", 4);
			yield return C.Wizard.Say("Oh, here we go", 59);
			C.Doorian.AnimIdle = "Idle"+Random.Range(1,4);
			yield return C.Doorian.Say("Oh, I forgot my little intro spiel!", 5);
			yield return C.Prince.Say("Your spiel?", 91);
			C.Doorian.AnimIdle = "Idle"+Random.Range(1,4);
			yield return C.Doorian.Say("Here goes- ahem", 6);
			C.Doorian.AnimIdle = "Idle"+Random.Range(1,4);
			yield return E.WaitSkip();
			C.Doorian.AnimIdle = "Idle"+Random.Range(1,4);
			yield return C.Doorian.Say("Choose a door! Any door!", 7);
			yield return C.Wizard.FaceAway();
			yield return C.Wizard.Say("Bleedin' Labyrinthians", 60);
			C.Doorian.AnimIdle = "Idle"+Random.Range(1,4);
			yield return C.Doorian.Say("Choose wisely and you may enter a fantastical enchanted kingdom", 8);
			yield return C.Doorian.Say("Where a treasure lies hidden", 9);
			yield return C.Prince.Say("And if I choose poorly?", 92);
			C.Doorian.AnimIdle = "Idle"+Random.Range(1,4);
			yield return C.Doorian.Say("You get to stay here with me!", 10);
			yield return C.Wizard.Face(C.Doorian);
			yield return C.Wizard.Say("Do NOT choose a door", 61);
			
			m_doorLady = eDoorLady.Talked;
			character.Cursor = "Use";	
		}
		else if ( m_doorLady == eDoorLady.Talked )
		{
			C.Section("Prince chooses a door on door lady");
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			
			C.Wizard.WalkToBG(Point("WizTalkDoor"), false, eFace.Right);
			yield return C.Prince.Say("What's this treasure anyway?", 93);
			C.Doorian.AnimIdle = "Idle"+Random.Range(1,4);
			yield return C.Doorian.Say("Well, some say it's a powerful Potion of Expectorium!", 11);
			yield return E.WaitSkip();
			yield return C.Wizard.Say("Vomit potion?", 62);
			yield return C.Wizard.FaceAway();
			yield return C.Wizard.Say("I can make my own in my bath at home", 63);
			yield return E.WaitSkip();
			yield return C.Prince.Say("Okay, well, I choose...", 94);
			yield return C.Wizard.Face(C.Prince);
			yield return C.Wizard.Say("Don't do it...", 64);
			yield return C.Prince.Say("This one", 95);
			yield return E.WaitSkip(0.25f);
			Audio.Play("DoorOpen");
			C.Doorian.AnimIdle = "OpenOne";
			yield return E.WaitSkip();
			yield return C.Doorian.Say("Ah, bad luck, that's not it", 12);
			yield return C.Doorian.Say("But don't worry! We can hang out!", 13);
			yield return C.Prince.Say("That'd be okay I guess", 96);
			yield return C.Doorian.Say("Great! I'm actually moving flat on Saturday, you'll help out, wontcha? It'll be fun!", 14);
			yield return C.Doorian.Say("Better start early, what with all the stairs. Lets say 5 am!  ", 15);
			yield return C.Doorian.Say("Loooots of wardrobes to move", 16);
			yield return C.Doorian.Say("They're kinda my thing, ya know? 'Cause of the doors!", 17);
			yield return C.Prince.Say("Ah... I'm pretty busy...", 97);
			yield return C.Doorian.Say("Oh, talking about doors, there's this museum, they have this amazing transom exhibition, you just have to see it", 18);
			yield return C.Prince.FaceAway();
			yield return C.Prince.Say("I... need to go over here now", 98);
			yield return E.WaitSkip();
			yield return C.Wizard.Face(C.Prince);
			yield return C.Wizard.Say("Told you. I can pick 'em a mile away", 65);
		
			m_doorLady = eDoorLady.Opened;
			C.Doorian.Cursor = "Talk";
		}
		else if (  m_doorLady == eDoorLady.AllOpen && E.FirstOccurance("prinTlkDoorOpen") )
		{
			C.Section("Prince asked about opened doors");
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			C.Doorian.FaceBG(C.Prince);
			yield return C.Prince.Say("Oh, hey, ah, looks like your doors have popped open", 99);
			yield return C.Doorian.Say("I KNOW! This has never HAPPENED before!", 19);
			yield return C.Prince.Say("I could visit that magic kingdom now, yes?", 100);
			yield return C.Doorian.Say("Of COURSE!", 20);
			yield return C.Doorian.Say("While you're inside, I'll take you to meet my cousin Theodoor", 21);
			yield return C.Doorian.Say("You think I like doors, wait until you meet him!", 22);
			E.StartCustomBackgroundSequence(DoorLadyGoOn);
			yield return E.WaitSkip(1.0f);
			yield return C.Prince.FaceAway();
			yield return C.Prince.Say("On second thought, ah, I'll pass", 101);
		}
		else if ( E.FirstOccurance("tlkDoorDisappr") )
		{
			C.Section("prince talks more to door lady about quest");
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			C.Doorian.FaceBG(C.Prince);
			yield return C.Prince.Say("So, do you know why there'd be a distress call from here?", 102);
			yield return C.Doorian.Say("Well, probably because of all the disappearances! ", 23);
		
			yield return C.Prince.Say("Oh yeah?", 103);
			yield return C.Doorian.Say("It's been mostly people who no one really likes though", 24);
			yield return C.Doorian.Say("One guy, Rooperp, he had this strange laugh, kinda like a-ha ha ha ha ha ha ha. Very grating, very disappearance worthy that laugh ", 25);
			yield return E.WaitSkip();
			yield return C.Doorian.Say("Y'know, now that I think about it, this disappearing business has actually been quite lovely", 26);
			yield return C.Doorian.Say("Less people around to wear out all the doors!", 27);
			if ( GlobalScript.Script.m_goal < eGoal.Hypderdrive )
				GlobalScript.Script.m_goal = eGoal.Hypderdrive;
		}
		else 
		{
			C.Section("prince talks more to door lady about quest");
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			C.Doorian.FaceBG(C.Prince);
			yield return C.Prince.Say(" So, people disappearing. Got any more info?", 104);
			yield return C.Doorian.Say("I mean, not really", 28);
			if ( E.FirstOccurance("askdisapp2") )
			{
				yield return E.WaitSkip();
				yield return C.Doorian.Say("It's not like DOORS are disappearing", 29);
				yield return E.WaitSkip();
				yield return C.Doorian.Say("Ooooh, imagine that! Doors! Gone! What would we do? ", 30);
				yield return C.Prince.Say("Just walk through the gap?", 105);
				yield return C.Doorian.Say("Horrifying", 31);
			}
		}
		
		yield return E.Break;
	}

	public IEnumerator DoorLadyGoOn()
	{
		C.Doorian.SayBG("He's into bi-fold doors at the moment, I told him, as long as it's not in-swung to a small room, I'm not fussed", 32);
		yield return E.WaitWhile(()=>C.Doorian.Talking);
		yield return E.Wait(0.5f);
		C.Doorian.SayBG("And you know what he said? Doorian, he said... well, I can't remember what he said, but it was about doors!", 33);
		yield return E.WaitWhile(()=>C.Doorian.Talking);
		yield return E.Wait(2);
		C.Doorian.SayBG("Oh! And he has a great collection of louvers. And you should see his list of architrave profies!", 34);
		yield return E.WaitWhile(()=>C.Doorian.Talking);
		yield return E.Wait(0.5f);
		C.Doorian.SayBG("Chamfered, beveled, ovolo, half scoop....", 35);
	}

	public IEnumerator PrinceClickHotspotStatue( IHotspot hotspot )
	{
		C.Section("Prince looks at statue");
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		if ( E.GetOccuranceCount("doortalkStat") == 0 )
		{
			yield return C.Prince.Say("Who's this then?", 106);
			yield return E.WaitFor(DoorianTalkStatue);
		}
		else
		{
			yield return C.Prince.Say("Dread-Queen Maxilla. I bet she knows what's been happening around here", 107);
		}
		 
		yield return E.Break;
	}

	IEnumerator DoorianTalkStatue()
	{
		if ( C.Doorian.VisibleInRoom && E.FirstOccurance("doortalkStat") )
		{
			yield return C.Doorian.Face(C.Plr);
			yield return C.Doorian.Say("Oh, that's Maxilla, Dread-Queen of Planet Lobos", 36);
			yield return C.Prince.Face(C.Doorian);
			yield return C.Prince.Say("What's her deal then?", 108);
			yield return C.Doorian.Say("Well... I heard she's got a thing for sliding doors", 37);
			yield return C.Doorian.Say("NOT my preference", 38);
			yield return C.Doorian.Say("Interestingly, she appeared around the same time these disappearances started", 39);
			C.Doorian.FaceRightBG();			
			Prop("Planet").Description = "&14 The dread planet Lobos rises in the east";
			if ( GlobalScript.Script.m_goal < eGoal.Hypderdrive )
				GlobalScript.Script.m_goal = eGoal.Hypderdrive;
		}
	}

	public IEnumerator OnUseInvCharacterMerchant( ICharacter character, IInventory item )
	{
		if ( item == I.Coins )
		{
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			yield return C.Wizard.Say("Alright, here's your money", 66);
			yield return C.Merchant.Say("Pleasure doin' business with yer", 25);
			yield return C.Wizard.Say("The pleasure's all mine", 67);
			I.Coins.Remove();
			I.Chicken.AddAsActive();
			m_chicken = eChicken.Have;
		}
		else if ( item == I.Steal )
		{
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			yield return C.Wizard.Say("DISPOSSESS!", 68);
			yield return E.WaitSkip();	
			yield return C.Merchant.Say("Try it and I'll gut ya", 26);
		}
		
		yield return E.Break;
	}

	public IEnumerator OnUseInvCharacterFlat( ICharacter character, IInventory item )
	{
		if ( I.Active == I.Steal )
		{
			yield return C.Plr.WalkTo(-235, -116);
			yield return C.Plr.Face(eFace.UpLeft);
		
			yield return E.WaitSkip();
			yield return C.Wizard.Say("DISPOSSESS!", 69);
			yield return E.WaitSkip(); 
			GlobalScript.Script.Hover = false;
			C.Wizard.AnimIdle = "CrouchSteal";	 
			yield return E.WaitSkip(1.0f);
			GlobalScript.Script.Hover = true;
			C.Wizard.AnimIdle = "Idle";	
			yield return E.WaitSkip();
			yield return C.Wizard.Say("He had a tidy little sum tucked away", 70);
			yield return E.WaitSkip();
			
			C.Flat.Cursor = "None";
			I.Coins.AddAsActive();
			
		}
		yield return E.Break;
	}

	public IEnumerator PrinceClickHotspotExit( IHotspot hotspot )
	{
		
		C.Section("Prince Uses exit");
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		yield return C.Prince.Say("Maybe I'll go explore", 109);
		yield return C.Merchant.Say("Yes, yes, explore by all means", 27);
		yield return C.Prince.FaceRight();
		Camera.SetPositionOverride(140,0,0.4f);
		yield return E.WaitSkip();
		yield return C.Merchant.Say("Explore the eternal labyrinth of the damned!	", 28);
		yield return C.FaceClicked();
		yield return C.Prince.Say("I kinda like mazes", 110);
		yield return E.WaitSkip();
		yield return C.Merchant.Say("No one has ever entered the labyrinth and left", 29);
		yield return C.Prince.FaceRight();
		yield return E.WaitSkip();
		yield return C.Merchant.Say("At least, not with their sanity", 30);
		yield return E.WaitSkip(); 
		yield return C.Prince.FaceLeft();
		yield return C.Prince.Say("Well, maybe I'll just hang around here then", 111);
		Hotspot("Exit").Hide();
		//Hotspot("Exit2").Hide();
		Camera.ResetPositionOverride(0.4f);
		
		yield return E.Break;
	}

	public IEnumerator PrinceClickHotspotExit2( IHotspot hotspot )
	{
		yield return E.HandleInteract(Hotspot("Exit"));
		yield return E.Break;
	}

	public IEnumerator WizardClickHotspotStatue( IHotspot hotspot )
	{
		C.Section("Wizard looks at statue");
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		yield return C.Wizard.Say("What an eyesore, my pet marmoset could sculpt better", 71);
		if ( E.FirstOccurance("rottdout") )
		{
			yield return C.Wizard.Say("And her eyes have rotted out", 72);
			yield return E.WaitSkip();
			yield return C.Wizard.Say("Looks strangely familiar though", 73);
		}
			
		yield return E.WaitFor(DoorianTalkStatue);
			
		yield return E.Break;
	}

	public IEnumerator WizardClickHotspotExit( IHotspot hotspot )
	{
		yield return C.FaceClicked();
		yield return C.Wizard.Say("Not on your life am I goin' in there", 74);
		yield return E.Break;
	}

	public IEnumerator WizardClickHotspotExit2( IHotspot hotspot )
	{
		yield return E.HandleLookAt( Hotspot("Exit"));
		yield return E.Break;
	}

	public IEnumerator WizardClickHotspotLabyrinth( IHotspot hotspot )
	{

		yield return E.Break;
	}

	public IEnumerator PrinceClickHotspotLabyrinth( IHotspot hotspot )
	{

		yield return E.Break;
	}

	public IEnumerator WizardClickHotspotHovel( IHotspot hotspot )
	{

		yield return E.Break;
	}

	public IEnumerator PrinceClickPropPlanet( IProp prop )
	{
		yield return C.Plr.FaceUpLeft();
		
		if ( prop.FirstUse )
		{
			C.Section("First time prince looks at planet");
			yield return C.Prince.Say("That planet there, I think we should investigate it", 112);
			yield return C.Wizard.Say("Well, I'll crank out the ol' Hyperdrive spell if you want", 75);
		}
		else
		{
			if ( R.Skull.Visited == false )
			{
				C.Section("Prince looks at planet again");
				yield return C.Prince.Say("What wonders does that planet hold?", 113);
			}
			else 
			{
				C.Section("Prince looks at planet after visiting it");
				yield return C.Prince.Say("Lobos... There's more adventures to be had there, I'll bet", 114);
			}
		}
		
		yield return E.Break;
	}
	
	public IEnumerator WizardClickPropPlanet( IProp prop )
	{
		yield return C.Plr.FaceUpLeft();
		if ( R.Skull.Visited == false )
		{
			yield return C.Wizard.Say("Nice lookin' rock. I think that's Lobos			", 76);
			Prop("Planet").Description = "&15 The dread planet Lobos rises in the east";
		}
		else 
		{
			yield return C.Wizard.Say("Well, it's less of a stink hole than this place, but not by much", 77);
		}
		yield return E.Break;
	}

	public IEnumerator PrinceClickCharacterPrince( ICharacter character )
	{

		yield return E.Break;
	}

	public IEnumerator WizardClickCharacterPrince( ICharacter character )
	{

		yield return E.Break;
	}

	public IEnumerator WizardClickCharacterMerchant( ICharacter character )
	{
		
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		bool talked = false;
		
		if ( m_chicken == eChicken.Need )
		{
			if ( E.FirstOccurance("tlkMechChickw") )
			{
				C.Section("Wizard talks to merchant about needing chicken");
				yield return C.WalkToClicked();
				yield return C.FaceClicked();
				yield return C.Wizard.Say("You got \"Chicken\"?", 78);
				yield return C.Merchant.Say("50 zonbecks", 31);
				yield return C.Wizard.Say("I'll do you 20", 79);
				yield return E.WaitSkip(0.25f);
				yield return C.Merchant.Say("50", 32);
				yield return C.Wizard.Say("30", 80);
				yield return E.WaitSkip(0.25f);
				yield return C.Merchant.Say("50", 33);
				yield return E.WaitSkip();
				yield return C.Wizard.Say("10", 81);
				yield return E.WaitSkip(1.5f);
				yield return C.Wizard.FaceAway();
				yield return E.WaitSkip(0.25f);
				yield return C.Wizard.Say("Ah, worth a shot", 82);
			}
			else if ( I.Coins.Owned )
			{
				yield return E.HandleInventory( C.Merchant, I.Coins );
			}	
			else 
			{
				C.Section("Wizard asks again for chicken");
				yield return C.FaceClicked();
				yield return C.Wizard.Say("I need some \"Space Chicken\"", 83);
				yield return E.WaitSkip();
				yield return C.Wizard.Say("I NEED it!", 84);
				yield return C.Wizard.FaceLeft();
				yield return C.Wizard.Say("Someone around here's gotta have some dosh", 85);
			}
			talked = true;
		}
		
		if ( talked == false )
		{
			C.Section("Talk to merchant about random stuff");
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			if ( E.FirstOccurance("tlkdogy") )
				yield return C.Wizard.Say("Hey, know anywhere a wizard could get some skirt around here?", 86);
			else
				yield return C.Wizard.Say("Give me directions to that place again?", 87);
			
			yield return C.Merchant.Say("Sure. Just go down there...", 34);
			yield return E.WaitSkip();
			ShuffledIndex ids = new ShuffledIndex(12);
			bool skip = false;
			while (skip == false)
			{
				
				switch( ids.Next() )
				{
					case 0: C.Merchant.SayBG("First left...", 35); break;			
					case 1: C.Merchant.SayBG("Second left...", 36); break;
					case 2: C.Merchant.SayBG("Then fourth left...", 37); break;
					case 3: C.Merchant.SayBG("First right...", 38); break;
					case 4: C.Merchant.SayBG("Then second right...", 39); break;
					case 5: C.Merchant.SayBG("Third right...", 40); break;
					case 6: C.Merchant.SayBG("Straight a while...", 41); break;
					case 7: C.Merchant.SayBG("Straight...", 42); break;			
					case 8: C.Merchant.SayBG("Up the stairs...", 43); break;
					case 9: C.Merchant.SayBG("Down the trapdoor...", 44); break;
					case 10: C.Merchant.SayBG("Keep left...", 45); break;
					case 11: C.Merchant.SayBG("Pull the lever...", 46); break;
				}   
				
				while ( C.Merchant.Talking && skip == false  )
				{
					if ( Input.GetMouseButton(0) )
						skip = true;
					yield return E.Wait(0); 
				}	 
			}
			yield return C.Wizard.Say("Forget it", 88);
		}
		yield return E.Break;
	}

	public IEnumerator OnUseInvHotspotExit2( IHotspot hotspot, IInventory item )
	{

		yield return E.Break;
	}

	public IEnumerator WizardClickCharacterFlat( ICharacter character )
	{
		
		C.Section("prince look at dead guy again");
		yield return C.FaceClicked();
		yield return C.Wizard.Say("Bet he has some neat stuff to possess", 89);
		yield return E.Break;
	}

	public IEnumerator WizardClickCharacterDoorian( ICharacter character )
	{
		E.StopCustomBackgroundSequence();
		
		if ( m_doorLady == eDoorLady.Start )
		{
			yield return E.HandleInteract(C.Doorian);
		}
		else if ( m_doorLady == eDoorLady.Talked )
		{
			C.Section("Wizard refuses to choose door");
			yield return C.FaceClicked();
			yield return C.Wizard.Say("Ooooh no, she's not getting me pickin' a door", 90);
			yield return C.Wizard.Say("Starts all innocent, next thing you got a bunch of half-door half-wizard freak babies, and you're paying support the rest of your life", 91);
		}
		else if (  m_doorLady == eDoorLady.AllOpen /*&& E.FirstOccurance("wizTlkDoorOpen")*/ )
		{
			C.Section("wiz talks more to door lady about quest");
			//yield return C.Wizard.WalkTo(Point("WizTalkDoor"));
			yield return C.FaceClicked();
			//C.Doorian.FaceBG(C.Wizard);
			// yield return C.Wizard.Say("Alright girly, I got your doors open, let me at that treasure!", 92);
			// yield return C.Doorian.Say("Don't 'girly' me! You're not invited, so that'd be breaking and entering", 40);
			// yield return C.Wizard.Say("I can oblige", 93);
			yield return C.Wizard.Say("I have other ways of acquiring things I want", 94);
		}
		else //if ( m_doorLady == eDoorLady.Opened )
		{
			//E.HandleInteract(C.Doorian);
			C.Section("Wizard tries to talk to door lady about quest");
			yield return C.Wizard.WalkTo(Point("WizTalkDoor"));
			yield return C.FaceClicked();
			PowerQuest.Get.StopDialog();
			yield return C.Wizard.Say("Hey you. Door... thing", 95);
			yield return C.Doorian.Face(C.Wizard);
			yield return E.WaitSkip();
			yield return C.Wizard.Say("Give me the dirt on this planet, huh?", 96);
			yield return C.Doorian.Face(C.Prince);
			yield return C.Doorian.Say("Ugh... I'd much rather talk to your friend", 41);
		}
		
		yield return E.Break;
	}

	public IEnumerator PrinceClickCharacterDog( ICharacter character )
	{

		yield return E.Break;
	}

	public IEnumerator OnUseInvCharacterDoorian( ICharacter character, IInventory item )
	{
		E.StopCustomBackgroundSequence();
		if ( I.Active == I.Open && m_doorLady < eDoorLady.AllOpen )
		{	
			C.Section("Cast 'open' on door lady");
			C.Wizard.WalkToBG(Point("WizTalkDoor"), false, eFace.Right);
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			yield return C.Prince.Say("OPEN!", 115);
			yield return E.WaitSkip(); 
			C.Plr.PlayAnimationBG("Doorbell");
			yield return C.Plr.WaitForAnimTrigger("Press");
			Audio.Play("Doorbell");
			yield return E.WaitWhile( () => C.Plr.Animating );
			yield return E.WaitSkip();
			Audio.Play("DoorOpenAll");
			yield return E.WaitSkip(0.25f);
			C.Doorian.AnimIdle = "OpenAll";	
			yield return E.WaitSkip();
			yield return C.Doorian.Say("Oh, my hinges! That tickled!", 42);
			yield return E.WaitSkip(0.25f);
			yield return C.Wizard.Say("Ah-ha! Now for the treasure inside!", 97);
			C.Doorian.Cursor = "Talk";
			m_doorLady = eDoorLady.AllOpen;
			I.Active = null;
			//C.Doorian.Instance.GetComponentInChildren<QuestText>();
		}
		else if ( I.Active == I.Steal && m_doorLady < eDoorLady.AllOpen )
		{
			C.Section("Cast dispossess on door lady, but doors are shut");
			// Can't steal her stuff
			yield return C.Wizard.WalkTo(C.Doorian.Position.WithOffset(-10,-10));	
			yield return C.FaceClicked();
			yield return E.WaitSkip();
			yield return C.Wizard.Say("DISPOSSESS!", 98);
			yield return E.WaitSkip();
			GlobalScript.Script.Hover = false;
			C.Wizard.AnimIdle = "CrouchSteal";	
			yield return E.WaitSkip(1.0f);
			GlobalScript.Script.Hover = true;
			C.Wizard.AnimIdle = "Idle";	
			yield return E.WaitSkip();
			yield return C.Wizard.FaceAway();
			yield return E.WaitSkip(0.25f);
			yield return C.Wizard.Say("Nyaaar... Can't get nothin' while her doors are shut up tight   ", 99);
			
		}
		else if ( I.Active == I.Steal && m_doorLady == eDoorLady.AllOpen )
		{
			C.Section("Cast dispossess on door lady and get 'vomit potion'");
			// Steal her stuff
			yield return C.Wizard.WalkTo(Point("WizTalkDoor"));
			yield return C.FaceClicked();
			yield return E.WaitSkip();
			
			yield return C.Wizard.Say("Ah... Look behind you... a... door of some kind", 100);
			yield return E.WaitSkip();
			yield return C.Doorian.FaceRight();
			yield return E.WaitSkip();
			yield return C.Wizard.WalkTo(C.Doorian.Position.WithOffset(-10,-10));	
			yield return C.FaceClicked();
			yield return C.Wizard.Say("DISPOSSESS!", 101);
			yield return E.WaitSkip();
			yield return C.Wizard.FaceUpRight(true);
			GlobalScript.Script.Hover = false;
			C.Wizard.AnimIdle = "CrouchSteal";	
			yield return E.WaitSkip();
			E.FadeOutBG(1.0f);
			yield return C.Display("As Maldrek stretches his wizard arm through the doorway, he's whisked away to a magical land", 6);
			yield return E.WaitSkip();
			yield return C.Display("There he encounters many wonders, and eventually stumbles upon an ancient chest", 7);
			yield return E.WaitSkip();
			yield return C.Display("What will he discover inside?  Stay tuned!", 8);
			yield return E.WaitSkip();
			yield return E.FadeIn();
			GlobalScript.Script.Hover = true;
			C.Wizard.AnimIdle = "Idle"; 
			yield return E.WaitSkip();   
			yield return C.Wizard.FaceAway();
			yield return E.WaitSkip(0.25f);
			yield return C.Wizard.WalkTo(Point("WizTalkDoor"));
			yield return C.Wizard.Say("A flask of... ", 102);
			yield return C.Wizard.Face(C.Doorian);
			yield return C.Wizard.Say("Eggnog? Hey, this ain't no treasure", 103);
			yield return C.Doorian.Face(C.Wizard);
			yield return C.Doorian.Say("The last adventurer must have left that", 43);
			yield return C.Doorian.Say("Serves you right anyway, taking advantage of an open door", 44);
			yield return C.Wizard.Say("Well, free eggnog's free eggnog", 104);
			yield return C.Doorian.Say("I wouldn't drink it, either. 'Twas years ago he passed by my threshold	", 45);
			yield return C.Wizard.Say("Well then, just the ingredient for a new spell anyway", 105);
			yield return C.Doorian.FaceAway();
			yield return C.Doorian.Say("Odd, he still hasn't called", 46);
			I.Vomit.AddAsActive();
			m_doorLady = eDoorLady.Stolen;	
		}
		else if ( I.Active == I.Steal && m_doorLady == eDoorLady.Stolen )
		{
			C.Section("Cast dispossess when alredy got vomit spell");
			yield return C.FaceClicked();
			yield return C.Wizard.Say("Nothing else in there worth having", 106);
		}
		
		yield return E.Break;
	}

	public IEnumerator OnUseInvHotspotStatue( IHotspot hotspot, IInventory item )
	{

		yield return E.Break;
	}

	public IEnumerator WizardClickCharacterWizard( ICharacter character )
	{

		yield return E.Break;
	}

	public IEnumerator WizardClickPropBook( IProp prop )
	{

		yield return E.Break;
	}

	IEnumerator OnUseInvHotspotHovel( IHotspot hotspot, IInventory item )
	{

		yield return E.Break;
	}
}
