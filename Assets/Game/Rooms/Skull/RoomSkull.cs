using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;

public class RoomSkull : RoomScript<RoomSkull>
{
	
	enum eGate { Start, Talked, UsedPotion, PokedSpew, Barfed, OpenedGate }
	
	eGate m_gate = eGate.Start;
	
	bool m_talkedBrothers = false;
	
	ShuffledIndex m_dogId = new ShuffledIndex(7);
	
	bool m_dogWalking = true;
	
	float m_eyeTimer = 0;
	ShuffledIndex m_eyeD = new ShuffledIndex(4); 
	
	static readonly string[] EYES = { "EyeA", "EyeB","EyeC","EyeD" };
	
	public void OnEnterRoom()
	{
		
		C.Prince.SetPosition(Point("Prince"));
		C.Wizard.SetPosition(Point("Wizard"));
		C.Wizard.Facing = eFace.Right;
		C.Prince.Facing = eFace.Right;
		C.Prince.AnimIdle = "Idle";
		C.Wizard.AnimIdle = "Idle";
		C.Wizard.Show();
		C.Prince.Show();
		
		C.Prince.FootstepSound = "FootstepGrass";
		
		C.Dog.Position = Point("Dog1");
		
		if ( R.EnteredFromEditor )
		{ 
			// first time entering
			GlobalScript.Script.m_swapEnabled = true;
			GlobalScript.Script.FollowOn();
			GlobalScript.Script.ShadowPrince = true;
			GlobalScript.Script.ShadowWizard = true;
			GlobalScript.Script.Hover = true;
			
			C.Wizard.AnimIdle = "Idle";
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
		if ( GlobalScript.Script.m_goal < eGoal.Doorbell )
			GlobalScript.Script.m_goal = eGoal.Doorbell;
		Audio.PlayAmbientSound("SoundAmbient-Mountain",0.8f,0.4f);
		Audio.PlayMusic("MusicSkull",0.8f,0.4f);
		
		GlobalScript.Script.FollowOn();
		GlobalScript.Script.Hover = true;
		GlobalScript.Script.ShadowPrince = true;
		GlobalScript.Script.ShadowWizard = true;
	}

	public IEnumerator PrinceClickPropPlanet( IProp prop )
	{
		yield return C.Prince.FaceUpLeft();
		yield return C.Prince.Say("Labyrinthia looks beautiful from here", 116);
		yield return E.Break;
	}

	public IEnumerator WizardClickPropPlanet( IProp prop )
	{
		yield return C.Wizard.FaceUp();
		yield return C.Wizard.Say("Uck, what an eyesore", 107);
		yield return E.Break;
	}

	public IEnumerator OnUseInvPropPlanet( IProp prop, IInventory item )
	{
		if ( I.Hyperdrive.Active )
		{
			C.Wizard.WalkToBG(Point("Wizard"), false, eFace.UpLeft);
			C.Prince.WalkToBG(Point("Prince"),false, eFace.UpLeft);
			yield return E.WaitSkip(1.0f);
			I.Active = null;
			E.ChangeRoomBG(R.Space);
			yield return E.Break;
		
		}
		yield return E.Break;
	}

	public IEnumerator OnEnterRoomAfterFade()
	{
		if ( R.Current.FirstTimeVisited )
		{
			yield return C.Prince.Say("Isn't space travel just the zaaz!", 117);
			yield return C.Wizard.Say("Would be if I didn't have to wash your groin sweat off my neck each time", 108);
		}
		yield return E.Break;
	}

	public IEnumerator PrinceClickPropDoorbellUp( IProp prop )
	{
		yield return C.FaceClicked();
		yield return C.Prince.Say("Looks like a button... I wonder if it would react to any of my spells", 118);
		yield return E.Break;
	}

	public IEnumerator WizardClickPropDoorbellUp( IProp prop )
	{
		yield return C.FaceClicked();
		yield return C.Wizard.Say("A button. What is this the stone age? What are we supposed to do with this?", 109);
		yield return E.Break;
	}

	public IEnumerator OnUseInvPropDoorbellUp( IProp prop, IInventory item )
	{
		if ( item == I.Punch )
		{
			GlobalScript.Script.FollowOff();
			yield return C.Plr.WalkTo(Point("DoorbellPunch"));
			// Play punch anim
			C.Wizard.WalkToBG(82, -115);
			
			yield return C.Plr.FaceUpRight();
			
			
			//
			C.Prince.SayBG("FANTASTICAL FIST!", 119);   
			yield return E.WaitSkip(0.25f);
			C.Prince.PlayAnimationBG("Punch");
			yield return C.Prince.WaitForAnimTrigger("Hit");   
			Audio.Play("PunchHit");	
		
			Camera.Shake(1);		
			yield return Prop("DoorbellUp").PlayAnimation("DoorbellFall");	   
			Prop("DoorbellUp").Hide();
			Prop("DoorbellDown").Show();
			yield return E.WaitWhile( ()=> C.Prince.Animating );
			yield return E.WaitForDialog(); 
			
			
			yield return C.Wizard.Face(Prop("DoorbellDown"));
			yield return C.Prince.WalkTo(90, -96);	
			C.Prince.FaceRightBG();
			
			yield return C.Wizard.Say("Now you've gone and broken it", 110);
			GlobalScript.Script.FollowOn();
			
		}
		else if ( item == I.Shoot )
		{
			C.Prince.WalkToBG(133, -104,false, eFace.Right);
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			C.Wizard.AnimIdle = "Gun";
			yield return E.WaitSkip(0.25f);
			Audio.Play("GunCock");
			yield return E.WaitSkip(0.25f);
			yield return C.Wizard.Say("FORCE BOLT!", 111);
			yield return E.WaitSkip(0.25f);
			//C.Wizard.WaitForAnimTrigger("Shoot");
			// TODO: gun sfx
			E.ScreenFlash( E.ColorFlashWhite);
			Audio.Play("Gunshot");
			C.Wizard.AnimIdle = "Idle";  
		 
			yield return Prop("DoorbellUp").PlayAnimation("DoorbellFall");   
			Prop("DoorbellUp").Hide();
			Prop("DoorbellDown").Show();
			yield return E.WaitSkip(0.25f);
			
			yield return C.Prince.Face(Prop("DoorbellDown"));
			yield return C.Wizard.WalkTo(102, -96);	
			yield return C.Wizard.Face(Prop("DoorbellDown"));
			
			yield return C.Wizard.Say("Take that, primitive technology", 112);
		}
		else if ( item == I.Steal )
		{
			GlobalScript.Script.FollowOff();
			C.Prince.WalkToBG(133, -104,false, eFace.UpRight);
			yield return C.Plr.WalkTo(Point("DoorbellUpClose"));
			yield return C.Plr.FaceUpRight();
			yield return C.Wizard.Say("DISPOSSESS!", 113);
			yield return E.WaitSkip();
			//C.Wizard.AnimIdle = "CrouchSteal";	
			Prop("DoorbellUp").Hide();
			yield return C.Wizard.Say("Came right off the wall, what a hunk of junk", 114);
			yield return C.Wizard.Face(C.Prince);
			yield return C.Wizard.Say("Here, you have it", 115);
			yield return E.WaitSkip();
			yield return C.Prince.Say("A new spell!", 120);
			GlobalScript.Script.FollowOn();
			C.Prince.AddInventory(I.Open);  
		}
		yield return E.Break;
	}

	public IEnumerator OnUseInvPropDoorbellDown( IProp prop, IInventory item )
	{
		if ( item == I.Steal )
		{
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			yield return C.Wizard.Say("DISPOSSESS!", 116);
			GlobalScript.Script.FollowOff();
			C.Prince.WalkToBG(C.Wizard.Position.WithOffset(-60,0),false, eFace.Right);
			yield return E.WaitSkip();
			GlobalScript.Script.Hover = false;
			C.Wizard.AnimIdle = "CrouchSteal";	
			yield return E.WaitSkip();
			Prop("DoorbellDown").Hide();
			yield return E.WaitSkip();
			GlobalScript.Script.Hover = true;
			C.Wizard.AnimIdle = "Idle";	
			yield return C.Wizard.Say("What am I doing, I don't want this hunk a junk", 117);
			yield return C.Wizard.Face(C.Prince);
			yield return C.Wizard.Say("Here, you have it", 118);
			yield return E.WaitSkip();
			yield return C.Prince.Say("A new spell!", 121);
			C.Prince.AddInventory(I.Open);
			GlobalScript.Script.FollowOn();
			
		}
		yield return E.Break;
	}

	public IEnumerator PrinceClickPropDoorbellDown( IProp prop )
	{
		yield return C.FaceClicked();
		yield return C.Prince.Say("I wonder if it was supposed to do that?", 122);
		
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		yield return E.WaitSkip();
		C.Prince.AnimIdle = "Crouch";
		I.Open.AddAsActive();
		Prop("DoorbellDown").Hide();
		yield return C.Prince.Say("A new spell!", 121);
		yield return E.WaitSkip();
		C.Prince.AnimIdle = "Idle";
		
	}

	public IEnumerator WizardClickPropDoorbellDown( IProp prop )
	{
		//E.HandleLookAt( Prop("DoorbellUp") );
		
		
		yield return C.WalkToClicked();
		yield return C.FaceClicked();
		//Wizard(116): DISPOSSESS!
		GlobalScript.Script.FollowOff();
		C.Prince.WalkToBG(C.Wizard.Position.WithOffset(-60,0),false, eFace.Right);
		yield return E.WaitSkip();
		GlobalScript.Script.Hover = false;
		C.Wizard.AnimIdle = "CrouchSteal";	
		yield return E.WaitSkip();
		Prop("DoorbellDown").Hide();
		yield return E.WaitSkip();
		GlobalScript.Script.Hover = true;
		C.Wizard.AnimIdle = "Idle";	
		yield return C.Wizard.Say("What am I doing, I don't want this hunk a junk", 117);
		yield return C.Wizard.Face(C.Prince);
		yield return C.Wizard.Say("Here, you have it", 118);
		yield return E.WaitSkip();
		yield return C.Prince.Say("A new spell!", 121);
		C.Prince.AddInventory(I.Open);
		GlobalScript.Script.FollowOn();
	}

	public IEnumerator PrinceClickHotspotGate( IHotspot hotspot )
	{
		yield return C.FaceClicked();
		yield return C.Prince.Say("Look at those fingies, they must go through a lot of moisturizer ", 123);
		yield return E.Break;
	}

	public IEnumerator WizardClickHotspotGate( IHotspot hotspot )
	{
		yield return C.FaceClicked();
		yield return C.Wizard.Say("Classic skull fortress gate design", 119);
		yield return E.Break;
	}

	public IEnumerator OnUseInvHotspotGate( IHotspot hotspot, IInventory item )
	{
		if ( item == I.Lift )
		{
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			C.Wizard.WalkToBG(-13, -101, false, eFace.Right);
			yield return E.WaitSkip();
			C.Prince.AnimIdle = "Crouch";
			yield return C.Prince.Say("TELEKINESIS!", 124);
			yield return E.WaitSkip();
			yield return C.Prince.Say("It... Won't... Budge", 125);
			yield return E.WaitSkip();
			C.Prince.AnimIdle = "Idle";
			yield return E.WaitSkip();		
			yield return C.Prince.FaceLeft();
			yield return E.WaitSkip();
			yield return C.Wizard.Say("What are those overgrown biceps even good for?", 120);
		}
		else if ( item == I.Punch )
		{
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
		
			C.Wizard.WalkToBG(-13, -101, false, eFace.Right);
			GlobalScript.Script.UnhandledUseInv( hotspot as IQuestClickable, item as Inventory );
			yield return C.Wizard.Say("Pathetic", 121);
		}
		else if ( item == I.Open && E.FirstOccurance("doorbell") )
		{
			// Character appears (as dismbodied voice initially)
			yield return C.Prince.WalkTo(76, -102);
			yield return C.FaceClicked();
			GlobalScript.Script.FollowOff();
			C.Wizard.WalkToBG(157, -114, false, eFace.UpLeft);
		
			yield return C.Prince.Say("OPEN!", 115);
			yield return E.WaitSkip();
			C.Plr.PlayAnimationBG("Doorbell");
			yield return C.Plr.WaitForAnimTrigger("Press");
			Audio.Play("Doorbell");
			yield return E.WaitWhile( () => C.Plr.Animating );
			I.Active = null;
			C.Maria.Clickable = true;
			C.Maria.Room = R.Current;
			yield return E.WaitSkip();
			yield return C.Maria.Say("Hey, you two schlubs, get off my stoop", 0);
			yield return C.Wizard.FaceRight();
			yield return C.Wizard.Say("Hey, who's there? We're armed, show yerself", 122);
			yield return C.Maria.Say("Behind you, wiezenheimer", 1);
			C.Wizard.WalkToBG(-95, -83,false, eFace.Right);
			yield return C.Prince.FaceLeft();
			yield return C.Prince.Say("Oh, you have one of those invisibility spells, yes?", 126);
			//C.Prince.WalkToBG(168, -103, false, eFace.UpRight);
			yield return C.Maria.Say("What? No! I've eaten myself. What's it to you?", 2);
			
			m_gate = eGate.Talked;
			GlobalScript.Script.m_goal = eGoal.Spew;
			GlobalScript.Script.FollowOn();
		}
		yield return E.Break;
		
		
		
	}

	public IEnumerator WizardClickHotspotSkull( IHotspot hotspot )
	{
		yield return C.FaceClicked();
		yield return C.Wizard.Say("Now this is a crib I could call my own", 123);
		yield return E.Break;
	}

	public IEnumerator PrinceClickHotspotSkull( IHotspot hotspot )
	{
		yield return C.FaceClicked();
		yield return C.Prince.Say("What are the odds our quest will take us in here?", 127);
		yield return E.WaitSkip();
		yield return C.Prince.Say("They'd be up there I think", 128);
		yield return E.Break;
	}

	
	public IEnumerator IntroduceMaria()
	{
		yield return C.Maria.Say("Maria Il Boccone", 3);
		yield return C.Maria.Say("I'm the guardian of the gate. You want in? I'm your gal", 4);
		yield return C.Wizard.Say("Let us in then", 124);
		yield return C.Maria.Say("Yous are deaf as a Coney Island whitefish", 5);
		yield return C.Maria.Say("I've. Eaten. Myself", 6);
		yield return C.Wizard.Say("Then. Un. Eat. Yourself", 125);
		yield return C.Maria.Say("Barf myself up? Ha! I have an iron constitution", 7);
		yield return C.Maria.Say("You'll just have to wait until I digest myself, and... well...   ", 8);
		yield return E.WaitSkip();
		yield return C.Wizard.Say("Oh, lovely", 126);
		
	}

	public IEnumerator PrinceClickCharacterMaria( ICharacter character )
	{
		
		if ( E.FirstOccurance("introMaria") )
		{
			yield return C.Prince.WalkTo(76, -102);	
			C.Wizard.WalkToBG(-93, -87, false, eFace.Right);
			yield return C.FaceClicked();
			yield return C.Prince.Say("Well... ah, nice to meet you, I'm Prince Xandar", 129);
			yield return E.WaitFor( IntroduceMaria );
		}
		else if ( m_gate == eGate.Barfed )
		{
			yield return C.Prince.WalkTo(72, -105);
			yield return C.FaceClicked();
			C.Section("Ask gate girl to open the gate now she's back");
			yield return C.Prince.Say("Do you think you could open the gate now?", 130);
			yield return E.WaitFor( OpenGate );
		}
		
		else 
		{
			yield return C.Prince.WalkTo(76, -102);
			C.Wizard.WalkToBG(-93, -87, false, eFace.Right);
			
			int occ = E.Occurrance("prinChatMaria");
			if ( occ ==  0 )
			{
				C.Section("Prince talk to gate girl A");
				yield return C.Prince.Say("So, why? Did you want to eat yourself... I mean?", 131);
				yield return C.Maria.Say("Bet you couldn't do it", 9);
				yield return C.Prince.Say("I don't-", 132);
				yield return C.Maria.Say("My brother Antonio, took him a week to eat himself", 10);
				yield return C.Maria.Say("Me? An afternoon", 11);
				yield return C.Maria.Say("Showed him", 12);
				m_talkedBrothers = true;
			}
			else if ( occ == 1)
			{
				C.Section("Prince talk to gate girl B");
				yield return C.Prince.Say("You have more siblings?", 133);
				yield return C.Maria.Say("Five older brothers, all real slime-balls", 13);
				yield return C.Prince.Say("Ugh, brothers", 134);
				yield return C.Prince.Say("'Prince Xandar, why can't you be more like Crown Prince Zardon'", 135);
				yield return C.Prince.Say("I bet dad didn't even notice I took this mission", 136);
				yield return C.Maria.Say("Screw 'em I say", 14);
			}
			else
			{
				C.Section("Prince talk to gate girl C");
				yield return C.Prince.Say("Nice chatting", 137);
				yield return C.Maria.Say("Likewise, amico", 15);
			}
			
		}
		yield return E.Break;
	}


	public IEnumerator WizardClickCharacterMaria( ICharacter character )
	{
		
		
		if ( E.FirstOccurance("introMaria") )
		{
			yield return C.Wizard.WalkTo(-93, -87);
			C.Prince.WalkToBG(76, -102,false, eFace.Left);
			yield return C.Wizard.FaceRight();
			yield return C.Wizard.Say("So hey, I'm Maldrek", 127);
			yield return E.WaitFor(IntroduceMaria);
		}
		else if ( m_gate == eGate.Barfed )
		{
			yield return C.Wizard.WalkTo(69, -112);
			yield return C.Wizard.FaceRight();
			C.Section("ask gate girl to open the gate now she's back");
			yield return C.Wizard.Say("Hey, if you're done honkin', open that gate", 128);
			yield return E.WaitFor( OpenGate );
		}
		else 
		{
			yield return C.Wizard.WalkTo(-93, -87);
			C.Prince.WalkToBG(76, -102,false, eFace.Left);
			yield return C.Wizard.FaceRight();
			int occ = E.Occurrance("wizchatMaria");
			if ( occ ==  0 )
			{
				C.Section("Wizard talk to gate girl A");
				yield return C.Wizard.Say("What goes on in this place?", 129);
				yield return C.Maria.Say("How'd I know that?", 16);
				yield return E.WaitSkip();
				yield return C.Maria.Say("The boss, Elsbeth... That's Dread-Queen Maxilla to you...", 17);
				yield return C.Maria.Say("She keeps tight lipped about her operation", 18);
				yield return C.Wizard.Say("No mean feat with that fleshless skull of hers", 130);
			}
			else if ( occ ==  1 )
			{
				C.Section("Wizard talk to gate girl B");
				yield return C.Wizard.Say("How'd you taste anyway?", 131);
				yield return C.Wizard.Say("Thinkin' about cooking up some of my own excess parts", 132);
				yield return C.Maria.Say("Damn good is how I tasted", 19);
				yield return C.Maria.Say("But then we Labyrinthians are known to be right finger-lickin'", 20);
				yield return C.Wizard.Say("Ain't that the truth", 133);
			}
			else 
			{
				C.Section("Wizard talk to gate girl C");
				yield return C.Wizard.Say("Any other way in to here?", 134);
				yield return C.Maria.Say("I'm it, gavone", 21);
				yield return C.Wizard.Say("I'd stick my fingers down your throat, but you've eaten it", 135);
			}
			
		}
		
		yield return E.Break;
	}

	IEnumerator OpenGate()
	{
		yield return C.Maria.Say("Well, I guess", 22);
		
		if ( m_talkedBrothers )
		{
			yield return E.WaitSkip();
			yield return C.Prince.Say("Take care, and, hey- Ignore your stupid brothers", 138);
			yield return E.WaitSkip();
			yield return C.Prince.Say("Don't let them define who you are", 139);
			yield return E.WaitSkip();
			yield return C.Maria.Say("Yeah buddy, you know something? You're right. Same to you", 23);
		}
		yield return E.WaitSkip();
		//C.Maria.FaceRight();
		// TODO: open door
		C.Wizard.WalkToBG(21f,-145f,true, eFace.UpRight);
		yield return E.WaitSkip();
		C.Prince.WalkToBG(1.8f,-84.6f, true, eFace.Right);
		yield return E.WaitSkip(1.0f);
		yield return Prop("Gate").PlayAnimation("GateOpen");
		Prop("Gate").Hide();
		yield return E.WaitSkip(1.0f);
		C.Prince.WalkToBG(86, -66,true);
		C.Wizard.WalkToBG(128, -68,true);
		E.FadeOutBG(0.4f);
		Audio.StopMusic(1.2f);
		yield return C.Display("And so our two heroes journey again into the unknown. What will they discover??", 9);
		yield return E.ChangeRoom( R.Darkness );
		yield return E.FadeIn(0.6f);
		// tOO: position them at different places, etc
		yield return E.WaitSkip(1.5f);
		C.Prince.Position = Point("Pos1");
		yield return C.Prince.Say("Dark in here", 140);
		yield return E.WaitSkip();
		C.Wizard.Position = Point("Pos2");
		yield return C.Wizard.Say("Do you smell \"Space Chicken\"?", 136);
		yield return E.WaitSkip();
		C.Prince.Position = Point("Pos3");
		yield return C.Prince.Say("What's that glurping sound?", 141);
		yield return E.WaitSkip(0.25f);
		C.Prince.Position = Point("Pos4");
		yield return C.Prince.Say("Maldrek, is that your- Argh!", 142);
		yield return E.WaitSkip();
		C.Wizard.Position = Point("Pos5");
		yield return C.Wizard.Say("Hey, let go of me you stinking- Ooof!", 137);
		
		// Change to factory, start monologue then fade in slowly
		E.ChangeRoomBG(R.Factory);
	}

	public IEnumerator OnUseInvCharacterMaria( ICharacter character, IInventory item )
	{
		if ( item == I.Vomit )
		{
			C.Section("Use vomit on gate girl");
			yield return C.Plr.WalkTo(Point("PourPotion"));
			yield return C.FaceClicked();
			GlobalScript.Script.FollowOff();
			C.Prince.WalkToBG(131, -108,true, eFace.Left);
			
			yield return C.Wizard.Say("VOMITOUS OOZE!", 138);
			yield return E.WaitSkip();
			//Display: TODO: Use vom spell on gate girl, dog eats it, and voms
			I.Vomit.Remove();
			Prop("Potion").Show();
			m_gate = eGate.UsedPotion;
			// Prop("Spew").Show();
			// wizard watch
			yield return E.WaitSkip();
			C.Wizard.WalkToBG(  75, -118,false, eFace.UpLeft);
			//C.Wizard.FaceDownRight();
			yield return E.WaitSkip();
			yield return C.Maria.Say("What is that, eggnog?", 25);
			yield return C.Maria.Say("Why you going around tipping eggnog on the ground? What's wrong with you?", 24);
			yield return E.WaitSkip();
			yield return C.Wizard.Say("Well, I thought maybe...", 139);
			
			m_dogWalking = false;
			yield return C.Dog.WalkTo( Point("DogSpew"),true );
			yield return C.Dog.Face( Prop("Potion") );	
			yield return E.WaitSkip();
			C.Dog.AnimIdle = "Lick";
			yield return E.WaitSkip(1.0f);
			yield return E.WaitSkip(1.0f);
			C.Dog.AnimIdle = "Idle";
			yield return E.WaitSkip(1.0f);
			//Audio.Play("SpewDogBig");
			//..
			//Camera.Shake();
			// Todo: vomit sound
			C.Dog.AnimIdle = "Empty";
			yield return C.Dog.PlayAnimation("Vomit");
			Prop("Spew").Show();
			Prop("Potion").Hide();
			C.Dog.AnimWalk = "EmptyWalk";
			yield return E.WaitSkip(1.0f);
			C.Prince.SayBG("Eeeww", 143);
			yield return E.WaitSkip(0.25f);
			yield return C.Maria.Say("Ugh, that's disgusting, oh, I can smell it", 26);
			yield return C.Dog.FaceLeft();
			yield return E.WaitSkip(0.25f);
			C.Dog.WalkSpeed = new Vector2(85,85);
			C.Dog.WalkToBG(-263, 154,true);
			yield return E.WaitSkip(0.25f);
			yield return C.Maria.Say("Almost enough to make a gal puke", 27);
			
			GlobalScript.Script.FollowOn();
			
		}
		yield return E.Break;
	}

	public IEnumerator UpdateBlocking()
	{
		
		if ( m_gate <= eGate.UsedPotion &&  m_dogWalking )
		{
			// dog walk to spew & lick it
			if ( C.Dog.Walking == false && Utils.GetTimeIncrementPassed(3) )
			{
				// move to next point
				C.Dog.WalkToBG( Point("Dog"+(m_dogId.Next()+1)), true );
			}
		}
		yield return E.Break;
	}

	public void Update()
	{
		 m_eyeTimer -= Time.deltaTime;
		 if ( m_eyeTimer <= 0 )
		 {
			 m_eyeTimer = Random.Range(0.1f,5.0f);
			 int eyeD = m_eyeD.Next();
		
			 Prop(EYES[eyeD]).PlayAnimation(EYES[eyeD]);
		 }
	}

	public IEnumerator WizardClickCharacterDog( ICharacter character )
	{
		yield return C.FaceClicked();
		yield return C.Wizard.Say("Stupid mongrel. Shoo. Shoo!", 140);
		yield return E.Break;
	}

	public IEnumerator PrinceClickCharacterDog( ICharacter character )
	{
		yield return C.FaceClicked();
		yield return C.Prince.Say("Who's a good boy? ", 144);
		if ( C.Dog.FirstUse )
		{
			yield return C.Prince.Say("Who's a tippy toppy little boy?", 145);
			yield return C.Prince.Say("You are! Yes, you!", 146);
			yield return C.Wizard.Say("Oh stow it, ya vestibular bulb", 141);
		}
		yield return E.Break;
	}

	public IEnumerator OnUseInvCharacterDog( ICharacter character, IInventory item )
	{
		if ( item == I.Vomit )
		{
			yield return C.FaceClicked();
			yield return C.Wizard.Say("I'm not wasting magic on that flea-bitten hybrid vgrep", 142);
		}
		yield return E.Break;
	}

	public IEnumerator WizardClickPropSpew( IProp prop )
	{
		yield return C.FaceClicked();
		yield return C.Wizard.Say("Hey, it don't look half bad, quivering there like a puddin'", 143);
		yield return E.Break;
	}

	public IEnumerator PrinceClickPropSpew( IProp prop )
	{
		yield return C.FaceClicked();
		yield return C.Prince.Say("It's quite impressive how bad that smells", 147);
		yield return E.Break;
	}

	public IEnumerator OnUseInvPropSpew( IProp prop, IInventory item )
	{
		if ( item == I.Lift )
		{
			C.Section("prince pokes the dog vom");
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			yield return C.Prince.Say("TELEKINESIS!", 148);
			yield return E.WaitSkip();	
			
			Audio.Play("VomitTouchLoop").FadeIn(0.15f);
			C.Prince.AnimIdle = "Crouch";
			yield return E.WaitSkip();
			yield return C.Prince.Say("I can't lift it, it's running through my fingers like a ball of snotty eels", 149);
			yield return C.Maria.Say("Ack, I threw up a bit in my mouth... ", 28);
			Audio.Stop("VomitTouchLoop",0.15f);
			C.Prince.AnimIdle = "Idle";
			m_gate = eGate.PokedSpew;
		}
		else if ( item == I.Steal )
		{
			C.Section("wizard pokes the dog vom");
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			yield return C.Wizard.Say("DISPOSSESS!", 144);
			yield return E.WaitSkip();
			GlobalScript.Script.Hover = false;
			
			Audio.Play("VomitTouchLoop").FadeIn(0.15f);
			C.Wizard.AnimIdle = "CrouchSteal";
			yield return E.WaitSkip(1.0f);
			yield return C.Wizard.Say("Ooh, feels lovely, sliding between my fingers, like a bowl of corn sneezed on itself", 145);
			yield return C.Maria.Say("Ack- aah, stop it, you're gonna make me barf too!", 29);
			GlobalScript.Script.Hover = true;
			Audio.Stop("VomitTouchLoop",0.15f);
			C.Wizard.AnimIdle = "Idle";
			yield return C.Wizard.Say("Can't keep hold of it though, slippery stuff", 146);
			m_gate = eGate.PokedSpew;
		}
		else if ( item == I.Eat )
		{
			C.Section("wizard eats the dog vom");
			//WalkToClicked
			//FaceClicked
			
			Camera.SetPositionOverride(70,0,3);
			
			yield return C.Plr.WalkTo(Point("EatVomit"));
			yield return C.FaceClicked();
			 
			GlobalScript.Script.FollowOff();
			C.Prince.WalkToBG(-134, -81, false, eFace.Right);
			yield return C.Wizard.Say("Huh, doesn't look too bad, I've eaten worse	", 147);
			yield return E.WaitSkip();
			yield return C.Prince.Say("Maldrek, no...", 150);
			yield return E.WaitSkip();
			yield return C.Wizard.Say("ABSORB!", 148);
			yield return C.Plr.FaceUpRight();  
			yield return E.WaitSkip();
			GlobalScript.Script.Hover = false;
			C.Wizard.AnimIdle = "EatVom";
			Audio.Play("EatLoop").FadeIn(0.15f);
			Audio.Play("VomitTouchLoop").FadeIn(0.15f);
			yield return E.WaitSkip();
			C.Maria.Position = Point("Maria1");
			yield return C.Maria.Say("Urk, no, don't!", 30);
			yield return E.WaitSkip();
			yield return C.Wizard.Say("Hrmm, scrhlp", 149);
			C.Maria.Position = Point("Maria2");
			yield return C.Maria.Say("You're gonna make me... Hulp", 31);
			yield return C.Wizard.Say("Bit hard on the dog-bile for my tastes", 150);
			yield return E.WaitSkip();
			yield return C.Wizard.Say("Slllrp, subtle notes of jellied-liver and cow intestine...", 151);
			C.Maria.Position = Point("Maria3");	
			yield return C.Maria.Say("Hurk... I'm gonna...", 32);
			yield return E.WaitSkip(0.25f);
			
			Audio.Stop("EatLoop",0.15f);
			Audio.Stop("VomitTouchLoop",0.15f);
			Prop("Spew").Hide();
			Prop("Potion").Hide();
			yield return E.WaitSkip(0.25f);
			GlobalScript.Script.Hover = true;
			C.Wizard.AnimIdle = "Idle";	
			yield return E.WaitSkip();
			C.Wizard.WalkToBG(52, -100,false, eFace.Right);
			
			//Audio.Play("Spew");
			
			//Maria(33): Huurrrrrkkkkk! (vomit sound)
			// Todo: Maria appears
			C.Maria.Position = Point("MariaShown");
			C.Maria.ClickableColliderId = 1;
			C.Maria.Facing = eFace.Left;
			C.Wizard.AnimIdle = "Idle";	
			C.Maria.Show();	
			yield return C.Maria.PlayAnimation("Vomit");
			m_gate = eGate.Barfed;
			GlobalScript.Script.FollowOn();
			
			Camera.ResetPositionOverride(2);
		}
		yield return E.Break;
	}

	public IEnumerator WizardClickCharacterMaxilla( ICharacter character )
	{

		yield return E.Break;
	}

	public IEnumerator PrinceClickCharacterMaxilla( ICharacter character )
	{

		yield return E.Break;
	}


	public IEnumerator OnUseInvHotspotSkull( IHotspot hotspot, IInventory item )
	{
		if ( I.Open.Active )
			yield return E.HandleInventory(Hotspot("Gate"), item);
		yield return E.Break;
	}
}
