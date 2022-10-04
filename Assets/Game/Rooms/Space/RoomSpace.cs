using UnityEngine;
using System.Collections;
using PowerScript;
using PowerTools.Quest;

public class RoomSpace : RoomScript<RoomSpace>
{
	
	float m_timer = 0;
	bool m_firstUpdate = true;	
	bool m_bob = false;
	float m_bobRatio = 0;	
	Vector2 m_trailOffset = Vector2.zero;
	
	Color m_btnCol = Color.white;
	public void OnEnterRoom()
	{
		if ( R.Current.FirstTimeVisited )
			m_trailOffset = Prop("Trail").Position - Prop("Chars").Position;
		
		if ( R.Current.FirstTimeVisited && R.Previous != R.Skull && R.Previous != R.Labyrinth )
		{
			// First time enter screen as title screen
		
			// Hide the inventory and info bar in the title
			//G.InfoBar.Visible = false;
			G.Inventory.Visible = false;
			//G.InfoBar.Clickable = false;
			G.Inventory.Clickable = false;
			G.Toolbar.Clickable = false;
			G.Toolbar.Visible = false;
		
			// Later we could start some music here
			//SystemAudio.PlayMusic("MusicSlowStrings", 1);
			//PowerQuest.Get.UseFancyParalaxSnapping = false;
			
			Prop("Prince").Clickable = false;
			Prop("Wizard").Clickable = false;
			
			C.Prince.AddInventory(I.Spellbook);
			C.Prince.AddInventory(I.Map);
			
			
		}
		else 
		{
			// Flying between planets
			Prop("Title").Hide();
			
			
			Prop("BtnPlay").Hide();
			Prop("BtnRestart").Hide();	 
			//Prop("BtnQuit").Hide();
			Prop("BtnSubtitles").Hide();  
			
							  
			Prop("Chars").Animation = "Chars"; // TODO: chars with loot anim
			Audio.PlayMusic("MusicHyperJump",0.4f,0.2f);
			
			if (R.Previous == R.Factory )
			{
				Prop("Chars").PlayAnimation("CharsEnd");
			}
		}
		
		Prop("Chars").Visible = false;
		Prop("Trail").Visible = false;  
		
		
		Prop("BgJunkA").SetPosition(-1000,0);
		Prop("BgJunkB").SetPosition(-1000,0);
		Prop("BgJunkC").SetPosition(-1000,0);
		
		
		Prop("FgJunkA").SetPosition(-1000,0);
		Prop("FgJunkB").SetPosition(-1000,0);
		Prop("FgJunkC").SetPosition(-1000,0);
		
		// Start some junk moving
		// bgjunk A
		{
			Prop("BgJunkA").Position = new Vector2(Random.Range(-200,240), Random.Range(-135,135));
			Prop("BgJunkA").Visible = true;
			Prop("BgJunkA").Animation = "BGJunk "+Random.Range(0,3);
			Prop("BgJunkA").MoveToBG(new Vector2(-1001, Prop("BgJunkA").Position.y), Random.Range(90,120) );
		}
		// fgjunk A
		{
			Prop("FgJunkA").Position = new Vector2(Random.Range(-200,240), Random.Range(-135,135));
			Prop("FgJunkA").Visible = true;
			Prop("FgJunkA").Animation = "FGJunk "+Random.Range(0,5);
			Prop("fgJunkA").MoveToBG(new Vector2(-501, Prop("FgJunkA").Position.y), Random.Range(300,500) );
		}
		
		Camera.SetPositionOverride(0,0,0);
		
		m_firstUpdate = true;
		
		m_bob = false;
		m_bobRatio  = 0;
		
		Audio.PlayAmbientSound("SoundAmbient-SpaceFlight",1,0.4f);
		
		
		C.Prince.Facing = eFace.Right;
		C.Prince.AnimIdle = "Space";
		C.Prince.Clickable = false;  
		
		C.Wizard.Facing = eFace.Right;
		C.Wizard.AnimIdle = "Space";
		C.Wizard.Clickable = false;
		C.Wizard.Room = R.Space;
		
		C.Prince.Visible = false;
		C.Wizard.Visible = false;
		
		GlobalScript.Script.Hover = false;
		GlobalScript.Script.FollowOff();
		GlobalScript.Script.ShadowPrince = false;
		GlobalScript.Script.ShadowWizard = false;
		
		C.Prince.Moveable = false;
		
		
		
	}
	
	public IEnumerator OnEnterRoomAfterFade()
	{
		
		
		if ( R.Current.FirstTimeVisited && R.Previous != R.Skull && R.Previous != R.Labyrinth && R.Previous != R.Factory )
		{
			// Title screen stuff
		
			Prop("BtnPlay").Hide();
			Prop("BtnRestart").Hide();
			//Prop("BtnQuit").Hide();
			Prop("BtnSubtitles").Hide();
		
			yield return E.WaitSkip();
			
			yield return Systems.Text.ImportFromCSVWeb("./translation.csv");
			if ( IsString.Empty(Systems.Text.ImportCSVWebResult) )
			{
				if ( E.Settings.GetLanguages().Length > 1 )
				{
					E.Settings.Language=E.Settings.GetLanguages()[1].m_code;
					//yield return C.Display("Translation loaded!", 15);
				}
			}
			else
			{
				string importResult = "Translation Failed to load!\n "+Systems.Text.ImportCSVWebResult;
				yield return C.Display(importResult);
				yield return C.Display("Press I to retry CSV Import", 16);
			}
			yield return E.WaitSkip();
		
		
			// Start cutscene, so this can be skipped by pressing ESC
			E.StartCutscene();
			//Prop("Chars").SetPosition(-324,235);
			yield return E.WaitSkip(1.5f);
			Audio.PlayMusic("MusicTitle");
			yield return E.WaitSkip(1.0f);
		
			Prop("Chars").Visible = true;
			Prop("Trail").Visible = true;
			C.Prince.Visible = true;
			C.Wizard.Visible = true;
			yield return Prop("Chars").PlayAnimation("CharsIn");
			m_bob = true;
			yield return E.WaitSkip(1.5f);
			// Fade in the title prop
			Prop("Title").Visible = true;
			yield return Prop("Title").Fade(0,1,1.0f);
		
			yield return E.WaitSkip();
		
			{
				Prop("BtnSubtitles").Position = Prop("BtnSubtitles").Position.WithOffset(0,20);
				//Prop("BtnQuit").Position = Prop("BtnQuit").Position.WithOffset(0,20);
		
			}
		
			// Turn on the "new game" prop and fade it in
			Prop("BtnPlay").Enable();
			//Prop("BtnQuit").Enable();
			Prop("BtnSubtitles").Enable();
			//Prop("BtnQuit").FadeBG(0,1,1.0f);			
			Prop("BtnSubtitles").Instance.GetComponentInChildren<QuestText>().text = string.Format( SystemText.Localize("Language: {0}", 82), Settings.LanguageData.m_description );
			Prop("BtnSubtitles").FadeBG(0,1,1.0f);
			yield return Prop("BtnPlay").Fade(0,1,1.0f);
		
		
			m_btnCol =		 Prop("BtnPlay").Instance.GetComponentInChildren<QuestText>().color;
		
			// This is the point the game will skip to if ESC is pressed
			E.EndCutscene();
		}
		else if ( R.Previous == R.Factory )
		{
			Prop("BtnPlay").Hide();
			Prop("BtnRestart").Hide();
			//Prop("BtnQuit").Hide();
			Prop("BtnSubtitles").Hide();
			Prop("Chars").SetPosition(0,0);
			yield return E.WaitFor(EndSequence);
		}
		else
		{
			Prop("BtnPlay").Hide();
			Prop("BtnRestart").Hide();
			//Prop("BtnQuit").Hide();
			Prop("BtnSubtitles").Hide();
			// Flying between planets
			//....
			Prop("Chars").Visible = true;
			Prop("Trail").Visible = true;
			yield return Prop("Chars").PlayAnimation("CharsTravel");
			//Prop("Chars").PlayAnimation("CharsIn");
			//m_bob = true;
			//..
			// m_bob = false;
			//E.WaitWhile( ()=> m_bobRatio > 0 );
			//Prop("Chars").PlayAnimation("CharsOut");
			//....
			Prop("Chars").Visible = false;
			if ( R.Previous == R.Labyrinth )
				E.ChangeRoomBG(R.Skull);
			else
				E.ChangeRoomBG(R.Labyrinth);
		}
		
		yield return E.Break;
	}

	public IEnumerator PrinceClickPropBtnPlay( Prop prop )
	{
		
		/*
		if ( E.GetSaveSlotData(1) != null )
		{
			yield return E.WaitSkip(0.25f);
			// Turn on the inventory and info bar now that we're starting a game
			G.InfoBar.Visible = true;
			G.Inventory.Visible = true;
			G.InfoBar.Clickable = true;
			G.Inventory.Clickable = true;
			G.Toolbar.Visible = true;
			G.Toolbar.Clickable = true;
		
			E.FadeColor = Color.white;
		
			Prop("BtnPlay").Hide();
			Prop("BtnRestart").Hide();
		
			Prop("BtnSubtitles").Hide();
			Prop("BtnQuit").Hide();
		
			Prop("Title").MoveToBG(new Vector2(0, 150), 200);
		
			E.RestoreSave(1);
			//Audio.Play("StaticBurst");
		}
		else*/
		{
			yield return E.HandleInteract( Prop("BtnRestart") );
		}
		yield return E.Break;
	}

	public IEnumerator PrinceClickPropBtnRestart( Prop prop )
	{
		yield return E.WaitFor(StartGame);
		
		yield return E.Break;
	}

	public IEnumerator StartGame()
	{
		
		// Turn on the inventory and info bar now that we're starting a game
		G.InfoBar.Visible = true;
		//G.Inventory.Visible = true;
		G.InfoBar.Clickable = true;
		//G.Inventory.Clickable = true;
		G.Toolbar.Visible = true;
		G.Toolbar.Clickable = true;
		
		//E.FadeColor = Color.white;
		
		Prop("BtnPlay").Hide();
		Prop("BtnRestart").Hide();
		Prop("BtnSubtitles").Hide();
		//Prop("BtnQuit").Hide();
		//Prop("BtnSubtitles").Hide();
		
		
		//Camera.SetZoom(1.5f,6);
		Prop("Title").MoveToBG(new Vector2(0, 200), 150);
		Audio.PlayMusic("MusicSpace",3.0f,2f);
		yield return E.WaitSkip(1.0f);
		
		yield return C.Display("We rejoin our two galactic heroes as they depart Spacekeep Mana-1 on another mission from the Council of Planets", 0);
		yield return E.WaitSkip();
		
		//Prince: Onward Maldrek!
		//Wizard: Stop saying that
		
		Prop("Wizard").Clickable = true;
		Prop("Prince").Clickable = true;
		Hotspot("Space").Clickable = true;
	}


	public IEnumerator UpdateBlocking()
	{
		if ( Input.GetKey(KeyCode.Tab) && Input.GetKey(KeyCode.E) )
		{
			//yield return C.Display("Skip to end", 14);
			yield return E.WaitFor( EndSequence );
		}
		
		
		if ( Prop("BtnPlay").Visible )
		{
			Prop("BtnPlay").Instance.GetComponentInChildren<QuestText>().color = (E.GetMouseOverClickable() == Prop("BtnPlay") ? Color.white : m_btnCol);
			Prop("BtnRestart").Instance.GetComponentInChildren<QuestText>().color = (E.GetMouseOverClickable() == Prop("BtnRestart") ? Color.white : m_btnCol);
			Prop("BtnSubtitles").Instance.GetComponentInChildren<QuestText>().color = (E.GetMouseOverClickable() == Prop("BtnSubtitles") ? Color.white : m_btnCol);
			//Prop("BtnQuit").Instance.GetComponentInChildren<QuestText>().color = (E.GetMouseOverClickable() == Prop("BtnQuit") ? Color.white : m_btnCol);
		}
		
		yield return E.Break;
	}

	ShuffledIndex m_bgId = new ShuffledIndex(7);
	ShuffledIndex m_fgId = new ShuffledIndex(7);

	public void Update()
	{
		
		if ( m_bob || m_bobRatio > 0)
		{
			if ( m_bob && m_bobRatio < 1 )
				m_bobRatio = Mathf.MoveTowards(m_bobRatio,1,Time.deltaTime * 1);
			else if ( m_bob == false )
				m_bobRatio = Mathf.MoveTowards(m_bobRatio,0,Time.deltaTime * 1);
		
			// Move camera with perlin noise
			m_timer += Time.deltaTime;
			float amplitudeX = 10.0f;
			float amplitudeY = 10.0f;
			float speedX = 1.0f;
			float speedY = 1.0f;
			Vector2 bobPos = new Vector2((Mathf.PerlinNoise(m_timer*speedX, 0)*amplitudeX*2)-amplitudeX, (Mathf.PerlinNoise(0, m_timer*speedY)*amplitudeY*2)-amplitudeY) * Utils.EaseCubic(m_bobRatio);
		
			if ( Prop("Chars").Animating == false || Prop("Chars").Animation == "CharsEnd" )
				Prop("Chars").Position = bobPos;
		}
		
		
		Vector2 newPos = Prop("Chars").Instance.transform.position;
		Prop("Chars").Position = newPos;
		Prop("Trail").Position = newPos+m_trailOffset;
		
		Prop("Prince").Position = newPos;
		Prop("Wizard").Position = newPos;
		//C.Prince.Position = newPos.WithOffset(0,50);
		C.Prince.Position = newPos.WithOffset(9,-84);
		if ( C.Prince.Visible && C.Prince.Instance != null )
			C.Prince.Instance.transform.position = C.Prince.Position.WithZ(C.Prince.Instance.transform.position.z);
		if ( C.Wizard.Visible && C.Wizard.Instance != null )
		{
			C.Wizard.Position = newPos.WithOffset(12,-117);
			C.Wizard.Instance.transform.position = C.Wizard.Position.WithZ(C.Wizard.Instance.transform.position.z);
		}
		else
			C.Wizard.Position = newPos;
		
		Prop("Chars").Baseline = -newPos.y;
		Prop("Trail").Baseline = -(newPos.y+m_trailOffset.y)+1;
		
		//Camera.SetPositionOverride(	(Mathf.PerlinNoise(m_timer*speedX, 0)*amplitudeX*2)-amplitudeX, (Mathf.PerlinNoise(0, m_timer*speedY)*amplitudeY*2)-amplitudeY );
		
		
		if ( Prop("BgJunkA").Position.x <= -500 && Utils.GetTimeIncrementPassed(3.123f))
		{
			Prop("BgJunkA").Position = new Vector2(500, Random.Range(-135,135));
			Prop("BgJunkA").Visible = true;
			Prop("BgJunkA").Animation = "BGJunk "+Random.Range(0,3);
			Prop("BgJunkA").MoveToBG(new Vector2(-1001, Prop("BgJunkA").Position.y), Random.Range(90,120) );
		}
		else if(Prop("BgJunkB").Position.x <= -500 && Utils.GetTimeIncrementPassed(2.123f))
		{
			Prop("BgJunkB").Position = new Vector2(500, Random.Range(-135,135));
			Prop("BgJunkB").Visible = true;
			Prop("BgJunkB").Animation = "BGJunk "+Random.Range(3,5);
			Prop("BgJunkB").MoveToBG(new Vector2(-501, Prop("BgJunkB").Position.y), Random.Range(140,180) );
		}
		else if( Prop("BgJunkC").Position.x <= -500 && Utils.GetTimeIncrementPassed(3f))
		{
			Prop("BgJunkC").Position = new Vector2(500, Random.Range(-135,135));
			Prop("BgJunkC").Visible = true;
			Prop("BgJunkC").Animation = "BGJunk "+Random.Range(5,7);
			Prop("BgJunkC").MoveToBG(new Vector2(-501, Prop("BgJunkC").Position.y), Random.Range(200,250) );
		}
		
		if ( Prop("FgJunkA").Position.x <= -500  && Utils.GetTimeIncrementPassed(3.123f))
		{
			Prop("FgJunkA").Position = new Vector2(300, Random.Range(-135,135));
			Prop("FgJunkA").Visible = true;
			Prop("FgJunkA").Animation = "FGJunk "+Random.Range(0,5);
			Prop("fgJunkA").MoveToBG(new Vector2(-501, Prop("FgJunkA").Position.y), Random.Range(300,500) );
		}
		else if(Prop("FgJunkB").Position.x <= -500  && Utils.GetTimeIncrementPassed(2.321f))
		{
			Prop("FgJunkB").Position = new Vector2(300, Random.Range(-135,135));
			Prop("FgJunkB").Visible = true;
			Prop("FgJunkB").Animation = "FGJunk "+Random.Range(0,5);
			Prop("fgJunkB").MoveToBG(new Vector2(-501, Prop("FgJunkB").Position.y), Random.Range(600,1000) );
		}
		else if( Prop("FgJunkC").Position.x <= -500 && Utils.GetTimeIncrementPassed(3) )
		{
			Prop("FgJunkC").Position = new Vector2(300, Random.Range(-135,135));
			Prop("FgJunkC").Visible = true;
			Prop("FgJunkC").Animation = "FGJunk "+Random.Range(5,7);
			Prop("FgJunkC").MoveToBG(new Vector2(-501, Prop("FgJunkC").Position.y), Random.Range(1200,2000) );
		}
		
		float bgSpeed = 60;
		if ( m_firstUpdate )
		{
			Prop("Bg").SetPosition(0,0);
			Prop("Bg2").SetPosition(480,0);
			Prop("Bg").MoveToBG( new Vector2(-481,0),bgSpeed);
			Prop("Bg2").MoveToBG( new Vector2(-481,0),bgSpeed);
		}
		if ( Prop("Bg").Position.x <= -480)
		{
			Prop("Bg").SetPosition(Prop("Bg2").Position.x+480,0);
			Prop("Bg").MoveToBG( new Vector2(-481,0),bgSpeed);
		}
		
		if ( Prop("Bg2").Position.x <= -480 )
		{
			Prop("Bg2").SetPosition(Prop("Bg").Position.x+480,0);
			Prop("Bg2").MoveToBG( new Vector2(-481,0),bgSpeed);
		}
		
		
		
		m_firstUpdate = false;
	}

	public IEnumerator OnExitRoom( IRoom oldRoom, IRoom newRoom )
	{
		
		//PowerQuest.Get.UseFancyParalaxSnapping = true;
		C.Prince.Moveable = true;
		
		Camera.ResetPositionOverride(0);
		yield return E.Break;
	}

	public IEnumerator WizardClickPropPrince( IProp prop )
	{
		
		yield return E.Break;
	}

	public IEnumerator PrinceClickPropPrince( IProp prop )
	{
		if ( prop.UseCount == 0 )
		{
			yield return C.Prince.Say("I'm looking tippy toppy", 0);
		}
		else if ( prop.UseCount == 1 || Prop("Wizard").FirstUse ) // wait until you've been asked for map
		{
			yield return C.Prince.Say("I love having the cosmic wind in my hair", 1);
		}
		else if ( E.FirstOccurance("needMapClkPrince") )
		{
			yield return C.Prince.Say("Where's that star chart?", 2);
		}
		else
		{
			yield return C.Prince.Say("I should look in my inventoire!", 3);
		}
		yield return E.Break;
	}

	public IEnumerator PrinceClickPropWizard( IProp prop )
	{
		yield return C.Prince.Say("Onward, Maldrek!", 4);
		if ( prop.FirstUse )
		{
			yield return C.Wizard.Say("Stop saying that", 0);
			yield return C.Prince.Say("The interplanetary council of planets is counting on us", 5);
			yield return E.WaitSkip();
			yield return C.Wizard.Say("I'm pretty sure we were supposed to turn on to I97", 1);
			yield return C.Prince.Say("Nonsense!", 6);
		}
		yield return C.Wizard.Say("Give me that star chart", 2);
		G.Inventory.Visible = true;
		G.Inventory.Clickable = true;
		
		yield return E.Break;
	}

	public IEnumerator OnUseInvPropWizard( IProp prop, IInventory item )
	{
		if ( I.Map.Active )
		{
			C.Section(" Use map on wizard at start");
		
			//Display: TODO: prince passes down map, which maldrek unfolds to look at
			Audio.Play("Map");
			C.Wizard.Visible = false;
			Prop("Chars").Animation = "CharsMap";
			yield return E.WaitSkip(1.0f);
			yield return C.Prince.Say("Shouldn't I do the map, so you can watch where you're going?", 7);
			yield return C.Wizard.Say("It's space, you vestibular bulb! Empty space for millions of klorbits", 3);
		
			yield return C.Prince.Say("What about space asteroids?", 8);
			m_bob = false;	
			yield return E.WaitWhile( ()=> m_bobRatio > 0 );
			Prop("Chars").PlayAnimationBG("CharsCrash");
			yield return C.Wizard.Say("Shut up! I'm trying to calculate delta-v for our orbital insertion", 4);
			yield return E.WaitSkip(1.0f);
			Audio.StopMusic(2.5f);
			E.FadeOutBG(2.0f);
			yield return E.WaitSkip();
			yield return C.Prince.Say("What about that big planet?", 9);
			yield return E.WaitWhile( ()=> E.GetFading() );
			E.ChangeRoomBG(R.Labyrinth);
		}
		else if (I.Spellbook.Active)
		{
			C.Section("Use spellbook on wizard at start");
			yield return C.Wizard.Say("I don't want your useless book of \"spells\"", 6);
		}
		yield return E.Break;
	}

	public IEnumerator OnUseInvPropPrince( IProp prop, IInventory item )
	{
		if ( I.Spellbook.Active )
		{
			C.Section("Use spellbook on self at intro");
			yield return C.Prince.Say("I get a little nauseous trying to read during hyperdrive jumps", 10);
			yield return C.Wizard.Say("If you're gonna honk, do it off the side", 7);
		}
		else if (I.Map.Active )
		{
			C.Section("Use map on self at intro");
			yield return C.Prince.Say("Maybe I should look at the map", 11);
			yield return C.Wizard.Say("No! We'll end up back on that dissolving corpse planet", 8);
			yield return C.Wizard.Say("Just give it here", 9);
		}
		yield return E.Break;
	}

	IEnumerator EndSequence()
	{
		
		
		Audio.PlayMusic("MusicSpace", 2.0f);
		
		
		C.Prince.Visible = true;
		C.Wizard.Visible = false;
		Prop("Chars").Animation = "CharsEnd";
		//C.Wizard.Visible = true;
		Prop("Chars").Visible = true;
		Prop("Trail").Visible = true;
		m_bob = true;
		m_bobRatio = 1;
		// TODO: Prop("chars") with big bag of chicken "evidence"
		
		yield return E.FadeIn(2);
		
		yield return E.WaitSkip(1.0f);
		yield return C.Prince.Say("Good thing I have you with me Maldrek", 12);
		yield return C.Prince.Say("I had no idea she had laser vision! She would have fried me all crispy in an instant", 13);
		yield return C.Wizard.Say("No sweat", 10);
		yield return E.WaitSkip();
		yield return C.Prince.Say("And installing yourself as the new ruler of Lobos...", 14);
		yield return C.Prince.Say("Why, you'll be able to work with the council to ensure no-one is ever turned into Space Chicken again!", 15);
		yield return E.WaitSkip();
		yield return C.Wizard.Say("Ah, naturally", 11);
		yield return E.WaitSkip(1.0f);
		yield return C.Prince.Say("I say... Don't eat ALL the evidence", 16);
		yield return E.WaitSkip(1.5f);
		
		
		yield return C.Display("And so our galactic heroes return to Spacekeep Mana-1, the universe is safe once again", 1);
		yield return E.WaitSkip(1.0f);
		yield return C.Display("Tune in next time for more...", 2);
		yield return E.WaitSkip(1.0f);
		// (TODO: roll credits!)
		Audio.PlayMusic("MusicTitle");
		E.AlwaysShowDisplayText = false;
		Settings.DialogDisplay = QuestSettings.eDialogDisplay.SpeechOnly;
		Prop("Credits").Visible = true;
		yield return C.Display("INTERGALACTIC WIZARD FORCE", 3);
		yield return E.WaitSkip(2.0f);
		
		yield return Prop("Credits").MoveTo(Point("CreditsEnd"),30);
		yield return E.WaitSkip(1.5f);
		
		// Restart instead of quitting now
		yield return E.FadeOut(10,true);
		E.Restart();
		
		yield return E.Break;
	}

	public IEnumerator PrinceClickHotspotSpace( IHotspot hotspot )
	{
		switch (hotspot.UseCount)
		{
		case 0:
			yield return C.Prince.Say("Ah, another mission in space", 17);
			yield return E.WaitSkip();
			yield return C.Prince.Say("What zany adventures will we get up to this time, Maldrek?", 18);
			yield return C.Wizard.Say("I dunno, Prince Xandar", 12);
			yield return E.WaitSkip();
			yield return C.Wizard.Say("And as long as I don't get my genitals stuck in an asteroid this time...", 13);
			yield return C.Wizard.Say("...I don't care", 14);
		break;
		case 1:
			yield return C.Prince.Say(" Ahh, space	", 19);
		break;
		case 2:
			yield return C.Prince.Say("Spacey Spacey Space", 20);
		break;
		case 3:
			yield return C.Prince.Say("Sure is... a lot of space", 21);
		break;
		case 4: 
			yield return C.Prince.Say("Ah yes. Space", 22);
		break;
		case 5:
			yield return C.Prince.Say("SPACE!", 23);
		break;
		case 6:
			yield return C.Prince.Say("s p a c e", 24);
		break;
		case 7:
			yield return C.Prince.Say("It's a great day for space", 25);
		break;
		default:
			yield return C.Prince.Say("Ahhhhhh Space", 26);
		break;
		}
		yield return E.Break;
	}

	public IEnumerator OnUseInvHotspotSpace( IHotspot hotspot, IInventory item )
	{
		yield return E.HandleInventory(Prop("Prince"), item);
		yield return E.Break;
	}

	public IEnumerator PrinceClickCharacterFlat( ICharacter character )
	{

		yield return E.Break;
	}

	public IEnumerator OnUseInvCharacterFlat( ICharacter character, IInventory item )
	{

		yield return E.Break;
	}

	public IEnumerator WizardClickCharacterMerchant( ICharacter character )
	{

		yield return E.Break;
	}

	public IEnumerator PrinceClickCharacterMerchant( ICharacter character )
	{

		yield return E.Break;
	}

	public IEnumerator WizardClickCharacterFlat( ICharacter character )
	{

		yield return E.Break;
	}

	public IEnumerator PrinceClickPropBtnSubtitles( IProp prop )
	{
		
		Settings.LanguageId = (int)Mathf.Repeat(Settings.LanguageId + 1,Settings.GetLanguages().Length);
		Prop("BtnSubtitles").Instance.GetComponentInChildren<QuestText>().text = string.Format( SystemText.Localize("Language: {0}", 82), Settings.LanguageData.m_description );
		
		yield return E.WaitSkip(0.25f);
		yield return E.ConsumeEvent;
		yield return E.Break;
	}

	public IEnumerator PrinceClickPropBtnQuit( IProp prop )
	{
		yield return E.WaitSkip();
		Application.Quit();
		yield return E.ConsumeEvent;
		yield return E.Break;
	}

	IEnumerator WizardClickPropBtnRestart( IProp prop )
	{

		yield return E.Break;
	}
}
