using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PowerScript;
using PowerTools.Quest;
using PowerTools;


[QuestAutoCompletable]
public enum eGoal
{
	HealGuy,
	Investigate,
	Hypderdrive,
	Doorbell,  // get it and use it on gate
	Spew, // includes using open on doorian
	EscapeSack,
};

///	Global Script: The home for your game specific logic
/**		
 * - The functions in this script are used in every room in your game.
 * - Put game-wide variables in here and you can access them from the quest script editor 'Globals.' (or from other quest scripts with 'GlobalScript.Script.')
 * - If you've used Adventure Game Studio, this is equivalent to the Global Script in that
*/
public partial class GlobalScript : GlobalScriptBase<GlobalScript>
{
	
	// NB: These regions are just to keep things tidy, they're not required.
	#region Global Game Variables
	
	public bool m_swapEnabled = false;
	
	ShuffledIndex m_charmId = new ShuffledIndex(14);
	
	public bool m_inSack = false;
	
	public eGoal m_goal = eGoal.HealGuy;
	
	
	bool m_shadowPrince = false;
	bool m_shadowWizard = false;
	
	
	float m_hoverTimer = 0;
	float m_hoverRatio = 0.5f;
	float m_hoverHeight = 60;
	
	// Wizard procedurally hovers
	bool m_hover = false;
	
	// Character follows behind
	public bool m_follow = false;
	
	// Character always turns to face (default off)
	bool m_face = false;
	bool m_reimport = false;
	//int m_reimport2 = 0;

	public void FollowOn()
	{
		m_follow = true;
	}
	
	public void FollowOff()
	{
		m_follow = false;
	}
	public void FaceOn()
	{
		m_face = true;
	}
	
	public void FaceOff()
	{
		m_face = false;
	}
	
	public bool ShadowPrince 
	{ get { return m_shadowPrince; } 
		set
		{
				m_shadowPrince = value;
		}
	}
	public bool ShadowWizard 
	{ get { return m_shadowWizard; } 
		set
		{
				m_shadowWizard = value;
		}
	}
	public bool Hover 
	{ 
		get {return m_hover;}
		set
		{
			m_hover = value;
		}
	}

	#endregion
	#region Global Game Functions
	
	/// Called when game first starts
	public void OnGameStart()
	{     
	} 

	/// Called after restoring a game. Use this if you need to update any references based on saved data.
	public void OnPostRestore(int version)
	{
	}

	/// Blocking script called whenever you enter a room, before fading in. Non-blocking functions only
	public void OnEnterRoom()
	{
	}

	/// Blocking script called whenever you enter a room, after fade in is complete
	public IEnumerator OnEnterRoomAfterFade()
	{
		yield return E.Break;
	}

	/// Blocking script called whenever you exit a room, as it fades out
	public IEnumerator OnExitRoom( IRoom oldRoom, IRoom newRoom )
	{
		yield return E.Break;
	} 

	/// Blocking script called every frame when nothing's blocking, you can call blocking functions in here that you'd like to occur anywhere in the game
	public IEnumerator UpdateBlocking()
	{
		
		if (m_reimport )
		{
			m_reimport = false;
		
			yield return Systems.Text.ImportFromCSVWeb("./translation.csv");
			if ( IsString.Empty(Systems.Text.ImportCSVWebResult) )
			{
				yield return C.Display("Translation loaded!", 17);				
				if ( E.Settings.GetLanguages().Length > 1 )
				{
					E.Settings.Language=E.Settings.GetLanguages()[1].m_code;
					//yield return C.Display("Translation loaded!", 18);
				}
			}
			else 
			{
				string importResult = "Translation Failed to load!\n "+Systems.Text.ImportCSVWebResult;
				yield return C.Display(importResult);
				yield return C.Display($"Translation Failed to load!\n {Systems.Text.ImportCSVWebResult}", 19);
			}
		}
		/*
		if ( m_reimport2 > 0 )
		{
			string path = "./translation.csv";
			switch(m_reimport2)
			{
				case 1: path ="Translation.csv"; break;
				case 2: path ="//Translation.csv"; break;
				case 3: path ="./translation.csv"; break;
				case 4: path ="/TemplateData/Translation.csv"; break;
				case 5: path ="TemplateData/Translation.csv"; break;
				case 6: path ="//TemplateData/Translation.csv"; break;
				case 7: path ="/Build/Translation.csv"; break;
				case 8: path ="Build/Translation.csv"; break;
				case 9: path ="//Build/Translation.csv"; break;
			}
			m_reimport2 = 0;

			yield return Systems.Text.ImportFromCSVWeb(path);
			if ( IsString.Empty(Systems.Text.ImportCSVWebResult) )
			{
				yield return C.Display("Translation loaded!", 20);
				E.Settings.Language="Translation";
			}
			else 
			{
				string importResult = $"Translation Failed to load from: {path}\n{Systems.Text.ImportCSVWebResult}";
				yield return C.Display(importResult);
			}

		}*/
		
		yield return E.Break;
	}

	/// Called every frame. Non-blocking functions only
	public void Update()
	{
		if ( Camera.GetHasPositionOverrideOrTransition() == false && G.Inventory.Instance != null && G.Inventory.Instance.GetComponentInParent<GuiDropDownBar>() != null )
		{
			G.Inventory.Instance.GetComponentInParent<GuiDropDownBar>().SetForceOff(false);
		}
		
		ICharacter character = C.Prince.IsPlayer ? C.Wizard : C.Prince;
		if ( m_follow && character.VisibleInRoom && C.Plr != character && C.Player.TargetPosition.x != 0 && character.TargetPosition.x != 0 )
		{
			// follow code
			const float closeDist = 50;
			float diff = character.TargetPosition.x - C.Player.TargetPosition.x; // dist in direction of character
			if ( Utils.GetTimeIncrementPassed(1.0f) )
			{
				// If too close, walk to side
				if ( Mathf.Abs(diff) < closeDist )
				{
					Vector2 targetPos = new Vector2(C.Player.TargetPosition.x + (Mathf.Sign(diff)*100), character.TargetPosition.y+10);
					if ( R.Current.Instance != null && R.Current.Instance.GetComponent<RoomComponent>() != null )
					{
						targetPos = R.Current.Instance.GetComponent<RoomComponent>().GetPathfinder().GetClosestPointToArea(targetPos);
						float diff2 = targetPos.x - C.Player.TargetPosition.x;
						if ( Mathf.Abs(diff2) < closeDist ) // try opposite direction
							targetPos = new Vector2(C.Player.TargetPosition.x + (-Mathf.Sign(diff)*100), character.TargetPosition.y+10);
					}
		
					character.WalkToBG( targetPos );
		
				}
				else if ( E.GetCameraGui() != null && Camera != null)
				{
					// Check camera target pos is within camera
					// Camera to position
					float toPlayer = character.TargetPosition.x - Camera.GetPosition().x;
					float cameraHalfWidth = E.GetCameraGui().orthographicSize * E.GetCameraGui().aspect;
					if ( Mathf.Abs(toPlayer) > cameraHalfWidth - 30 )
					{
		
						// walk on-screen. Maybe to pre-defined point?
						Vector2 targetPos = new Vector2(Camera.GetPosition().x+Mathf.Sign(toPlayer)*(cameraHalfWidth - 60), character.TargetPosition.y+10);
						character.WalkToBG( targetPos );
					}
		
				}
			}
		
			// if too far (offscreen) move to be visible (could use regions for this though like in coatrack)
		
			// face other character
			if ( character.Walking == false && Utils.GetTimeIncrementPassed(0.50123f) && Mathf.Abs(diff) > closeDist ) // dont face if about to walk away
			{
				character.FaceBG( C.Player );
			}
		}
		else if ( m_face && character.VisibleInRoom )
		{
			// face other character
			if ( character.Walking == false && Utils.GetTimeIncrementPassed(0.3f) )
			{
				character.FaceBG( C.Player );
			}
		
		}
		
		if ( m_hover && C.Wizard.Instance != null )
		{
			float sinSpeed = 3;
			float sinHeight = 5;
			float perlinHeight = 5;
			float perlinSpeed = 0.5f;
		
			m_hoverTimer += Time.deltaTime;
			m_hoverRatio = (sinHeight * Mathf.Sin(m_hoverTimer*sinSpeed)) +( perlinHeight * Mathf.PerlinNoise(0,m_hoverTimer*perlinSpeed));
		
			PowerSprite wiz = C.Wizard.Instance.GetComponent<PowerSprite>();
			wiz.Offset = wiz.Offset.WithY(Utils.Snap( m_hoverHeight + m_hoverRatio * 1));
		
		}
		
		if ( C.Wizard.Instance != null )
		{
			Transform trans = C.Wizard.Instance.transform.Find("Shadow");
			if ( trans != null )
			{
				trans.gameObject.SetActive(m_shadowWizard);
				// Set color
				if ( R.Labyrinth.Current )
					trans.GetComponent<SpriteRenderer>().color = E.ColorShadowLab;
				else if ( R.Factory.Current )
					trans.GetComponent<SpriteRenderer>().color = E.ColorShadowFactory;
				else
					trans.GetComponent<SpriteRenderer>().color = E.ColorShadowSkull;
			}
		
		}
		if ( C.Prince.Instance != null )
		{
			Transform trans = C.Prince.Instance.transform.Find("Shadow");
			if ( trans != null )
			{
				trans.gameObject.SetActive(m_shadowPrince);
		
				// Set color
				if ( R.Labyrinth.Current )
					trans.GetComponent<SpriteRenderer>().color = E.ColorShadowLab;
				else if ( R.Factory.Current )
					trans.GetComponent<SpriteRenderer>().color = E.ColorShadowFactory;
				else
					trans.GetComponent<SpriteRenderer>().color = E.ColorShadowSkull;
			}
		}
		
		if ( Input.GetKeyDown(KeyCode.I) )
			m_reimport = true;

		int lang = 0;
		
		if ( Input.GetKeyDown(KeyCode.Alpha1) )
			lang = 1;
		if ( Input.GetKeyDown(KeyCode.Alpha2) )
			lang = 2;
		if ( Input.GetKeyDown(KeyCode.Alpha3) )
			lang = 3;
		if ( Input.GetKeyDown(KeyCode.Alpha4) )
			lang = 4;
		if ( Input.GetKeyDown(KeyCode.Alpha5) )
			lang = 5;
		if ( Input.GetKeyDown(KeyCode.Alpha6) )
			lang = 6;
		if ( Input.GetKeyDown(KeyCode.Alpha7) )
			lang = 7;
		if ( Input.GetKeyDown(KeyCode.Alpha7) )
			lang = 7;
		if ( Input.GetKeyDown(KeyCode.Alpha8) )
			lang = 8;
		if ( Input.GetKeyDown(KeyCode.Alpha9) )
			lang = 9;
		if ( lang > 0 )
		{
			lang--;
			if ( E.Settings.GetLanguages().Length > lang )
			{
				E.Settings.Language=E.Settings.GetLanguages()[lang].m_code;
				string langStr = string.Format( SystemText.Localize("Language: {0}", 82), Settings.LanguageData.m_description );
				E.DisplayBG(langStr);
			}

		}
		if ( Input.GetKeyDown(KeyCode.L) )
		{
			Settings.LanguageId = (int)Mathf.Repeat(Settings.LanguageId + 1,Settings.GetLanguages().Length);
			string langStr = string.Format( SystemText.Localize("Language: {0}", 82), Settings.LanguageData.m_description );
			E.DisplayBG(langStr);
		}
			
			/*
				
				if ( E.Settings.GetLanguages().Length > 1 )
				{
					E.Settings.Language=E.Settings.GetLanguages()[1].m_code;
					//yield return C.Display("Translation loaded!", 21);
				}

			/*
		if ( Input.GetKeyDown(KeyCode.Alpha1) )
			m_reimport2 = 1;
		if ( Input.GetKeyDown(KeyCode.Alpha2) )
			m_reimport2 = 2;
		if ( Input.GetKeyDown(KeyCode.Alpha3) )
			m_reimport2 = 3;
		if ( Input.GetKeyDown(KeyCode.Alpha4) )
			m_reimport2 = 4;
		if ( Input.GetKeyDown(KeyCode.Alpha5) )
			m_reimport2 = 5;
		if ( Input.GetKeyDown(KeyCode.Alpha6) )
			m_reimport2 = 6;
		if ( Input.GetKeyDown(KeyCode.Alpha7) )
			m_reimport2 = 7;
		if ( Input.GetKeyDown(KeyCode.Alpha7) )
			m_reimport2 = 7;
		if ( Input.GetKeyDown(KeyCode.Alpha8) )
			m_reimport2 = 8;
		if ( Input.GetKeyDown(KeyCode.Alpha9) )
			m_reimport2 = 9;*/
	}

	/// Blocking script called whenever the player clicks anywwere. This function is called before any other click interaction. If this function blocks, it will stop any other interaction from happening.
	public IEnumerator OnAnyClick()
	{
		yield return E.Break;
	}

	/// Blocking script called whenever the player tries to walk somewhere. Even if `C.Player.Moveable` is set to false.
	public IEnumerator OnWalkTo()
	{
		yield return E.Break;
	}

	/// Called when the mouse is clicked in the game screen. Use this to customise your game interface by calling E.ProcessClick() with the verb that should be used. By default this is set up for a 2 click interface
	public void OnMouseClick( bool leftClick, bool rightClick )
	{
		
		// Clear inventory on Right click, or left click on empty space, or on hotspot with cursor set to "None"
		if ( C.Player.HasActiveInventory && ( rightClick || (E.GetMouseOverClickable() == null && leftClick ) || Cursor.NoneCursorActive ) )
		{					
			C.Player.ActiveInventory = null;
		}
		else if ( Cursor.NoneCursorActive )
		{
			// Special case for clickables with cursor set to "None"- Don't do anything		
		}
		else if ( leftClick )
		{
			// Handle left click
			if ( E.GetMouseOverClickable() != null )
			{	
				// Left clicked a clickable
				if ( C.Player.HasActiveInventory && Cursor.InventoryCursorOverridden == false )
				{
					// Left click with active inventory, use the inventory item
					E.ProcessClick( eQuestVerb.Inventory );
				}
				else
				{
					// Left click on item, so use it
					if ( C.Prince.IsPlayer )
						E.ProcessClick(eQuestVerb.Use);
					else
						E.ProcessClick(eQuestVerb.Look);
				}
			}
			else
			{
				// Left click empty space, walk
				E.ProcessClick( eQuestVerb.Walk );
			}
		}
	}

	#endregion
	#region Unhandled interactions

	/// Called when player interacted with something that had not specific "interact" script
	public IEnumerator UnhandledInteract(IQuestClickable mouseOver)
	{		
		if ( R.Current.ScriptName == "Title")
			yield break;
		
		yield return C.Prince.Say("I don't quite know what to make of that", 178);
		
	}

	/// Called when player looked at something that had not specific "Look at" script
	public IEnumerator UnhandledLookAt(IQuestClickable mouseOver)
	{
		if ( R.Current.ScriptName == "Title")
			yield break;
		
		yield return C.Wizard.Say("Ah, it is what it is", 200);
		yield break;
	}

	/// Called when player used inventory on an inventory item that didn't have a specific UseInv script
	public IEnumerator UnhandledUseInvInv(Inventory invA, Inventory invB)
	{
		if ( C.Prince.IsPlayer )
		{
			yield return C.Prince.Say("I can't use those together", 179);
		}
		else 
		{
			yield return C.Wizard.Say("That won't work!", 201);
		}
		
		yield return E.Break;
		
		
	}

	/// Called when player used inventory on something that didn't have a specific UseInv script
	public IEnumerator UnhandledUseInv(IQuestClickable mouseOver, Inventory item)
	{
		if ( R.Current.ScriptName == "Title")
			yield break;
			
		if ( R.Current == R.Factory && m_inSack )
		{
			C.Section("Try using things when in meatsack");
			// STuck at end in meatsack, can't move
			if ( C.Plr == C.Wizard )
			{
				if ( Random.value < 0.5f )
					yield return C.Wizard.Say("I can't phleppin' move", 202);
				else
					yield return C.Wizard.Say("I'm stuck by Gunder!", 203);
				yield break;
			}
			else 
			{
				if ( item == I.Levitate )
				{
					// Prince does levitate spell
					yield return E.WaitFor(RoomFactory.Script.Jump);
					yield break;
				}
				else if ( item != I.Charm )
				{
					if ( Random.value < 0.5f )
						yield return C.Prince.Say("My hands are trapped", 180);
					else 
						yield return C.Prince.Say("No use, I can't move my arms", 181);
					yield break;
				}
			}
		}
			
			
			
		if ( item == I.Levitate )
		{	
			C.Section("Levitate spell");
			yield return C.FaceClicked();
			//C.Plr.FaceDown();
			
			C.Prince.AddAnimationTrigger("Land",true, () => { Camera.Shake(1.0f); } );
			C.Prince.PlayAnimationBG("Jump");   
			yield return C.Prince.WaitForAnimTrigger("Down");		 
			C.Prince.SayBG("LEVITATE!", 182);	
			yield return E.WaitForDialog();
			int occ = E.Occurrance("levttl");
			if ( occ == 2 )
			{
				yield return C.Wizard.Face(C.Prince);
				yield return C.Wizard.Say("Yeah, real impressive", 204);
			}
			else if ( occ == 4 )
			{
				yield return C.Wizard.Face(C.Prince);
				yield return C.Wizard.Say("I think that was your best one yet", 205);
			}
			
		}
		else if ( item == I.Punch )
		{
			
			C.Section("Punch spell");
			if ( mouseOver.ClickableType == eQuestClickableType.Character )
			{
				yield return C.FaceClicked();
				yield return C.Prince.Say("I could never use my powers on another living being", 183);
				if ( E.Occurrance("punchpersn") == 2 )
				{
					yield return C.Wizard.Say("You punched that girl scout in the guts that time", 206);
					yield return C.Prince.Face(C.Wizard);
					yield return C.Prince.Say("You told me she was choking", 184);
					yield return C.Wizard.Say("Well, she might have been			", 207);
				}
				yield break;
			}
			
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			yield return C.Plr.FaceUp();
			
			//
			C.Prince.SayBG("FANTASTICAL FIST!", 185);   
			yield return E.WaitSkip(0.25f);
			yield return C.Prince.PlayAnimation("Punch");
			//C.Prince.WaitForAnimTrigger("Back");	
			//C.Prince.PauseAnimation(); 
			yield return E.WaitForDialog();
			//C.Prince.ResumeAnimation();
			
			
		}
		else if ( item == I.Charm )
		{
			C.Section("Charm spell- Princes's complements and pickup lines");
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			switch (m_charmId.Next())
			{
				case 0:
					yield return C.Prince.Say("I like your neck, you have a good neck", 186);
				break;
				case 1:
					yield return C.Prince.Say("Is heaven missing an angel?", 187);
				break;
				case 2:
					yield return C.Prince.Say("It's handy that I have my library card", 188);
					yield return C.Prince.Say("Because I'm checking you out", 189);
				break;
				case 3:
					yield return C.Prince.Say("I commend the fact that you have working elbows", 190);
				break;
				case 4:
					yield return C.Prince.Say("I don't have any raisins...", 191);
					yield return C.Prince.Say("But I sure would like a date", 192);
				break;
				case 5:
					yield return C.Prince.Say("Your head reminds me of a watermelon ", 193);
					yield return C.Prince.Say("It was very delicious by the way... The watermelon, that is", 194);
				break;
				case 6:
					yield return C.Prince.Say("Excuse me. I think you have something in your eye", 195);
					yield return C.Prince.Say("Oh, no, it's just a sparkle", 196);
				break;
				case 7:
					yield return C.Prince.Say("Remember me? ", 197);
					yield return C.Prince.Say("Oh, that's right, I've met you only in my dreams", 198);
				break;
				case 8:
					yield return C.Prince.Say("I was wondering if you had an extra heart", 199);
					yield return C.Prince.Say("Because mine was just stolen", 200);
				break;
				case 9:
					yield return C.Prince.Say("Are you a camera? ", 201);
					yield return C.Prince.Say("Because every time I look at you, I smile!", 202);
				break;
				case 10:
					yield return C.Prince.Say("If you were a phaser, you'd be set to stun", 203);
				break;		
				case 11:
					yield return C.Prince.Say("Your face reminds me of a pet dog I liked very much", 204);
					break;
				case 12:
					yield return C.Prince.Say("Very nice bone structure, I am sure your skeleton would be quite good for medical education", 205);
				break;
				case 13:
					yield return C.Prince.Say(" I bet you can lift real heavy things and put them back down again", 206);
					break;
				
				
				}
			
			C.Section(" Responses to Prince's pickup lines (All seperate)");
			if ( mouseOver == C.Doorian )
			{		
				switch ( E.Occurrance("dorchrm")%3 )
				{
				case 0:
				yield return C.Doorian.Say("That's funny, you're so FUNNY!", 47);
				break;
				case 1:
				yield return C.Doorian.Say("Stop it you! You'll swell my frames!", 48);
				break;
				case 2:
				yield return C.Doorian.Say("Flatter me all you want, it's one door per person!		", 49);
				break;
				}
			}
			else if ( mouseOver == C.Maria )
			{ 
				switch ( E.Occurrance("marchrm")%5 )
				{
				case 0:
				yield return C.Maria.Say("What are you trying to say?", 34);
				break; case 1:
				yield return C.Maria.Say("Is this guy kiddin' or what?", 35);
				break; case 2:
				yield return C.Maria.Say("You sound like my big brother Salvio", 36);
				break; case 3:
				yield return C.Maria.Say("Don't make me start bustin' heads", 37);
				break; case 4:
				yield return C.Maria.Say("If you're trying to make me puke, you're getting there", 38);
				break; 
				}
			}
			else if ( mouseOver == C.Wizard )
			{
				switch (E.Occurrance("wzchrm")%2)
				{
				case 0:
				yield return C.Wizard.Say("Save it for the broads", 208);
				break;
				case 1:
				yield return C.Wizard.Say("Let's just stay friends", 209);
				break;
				}
			}
			else if ( mouseOver == C.Flat )
			{
				if ( RoomLabyrinth.Script.m_flat < RoomLabyrinth.eFlat.Dead)
					yield return C.Flat.Say("Aww, thanks, eh!", 21);
				else
					yield return C.Wizard.Say("Bit late for that				", 210);
			}
			else if ( mouseOver == C.Merchant )
			{
				yield return C.Merchant.Say("If you're angling for a discount, you'll be disappointed ", 47);
			}
			else if ( mouseOver == C.Maxilla )
			{
				yield return C.Wizard.Say("Stow it, she ain't listenin'", 211);
			}
			
			int lines = E.Occurrance("pickupLine");
			if (lines == 2)
			{
			
				C.Section("Wiz responds to prince charm spell");
				yield return C.Wizard.Say("Have those ever worked?		", 212);
				yield return C.Prince.Face(C.Wizard);
				yield return C.Prince.Say("Not yet", 207);
			}
			else if (lines == 5)
			{
				C.Section("Wiz responds to prince charm spell 2");
				yield return C.Wizard.Say("Where you getting these anyway?	", 213);
				yield return C.Prince.Face(C.Wizard);
				yield return C.Prince.Say("I got them from the book you gave me", 208);
				yield return C.Wizard.Say("That one's garbage, I got a great new one on hypnosis", 214);
			}
			
		}
		else if ( item == I.Shoot )
		{
			C.Section("Wizard shoots gun");
			
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			Audio.Play("GunCock");
			C.Wizard.AnimIdle = "Gun";
			
			if ( mouseOver.ClickableType == eQuestClickableType.Character )
			{
				yield return C.Prince.Face(C.Wizard);
				yield return C.Prince.Say("Maldrek!", 209);
				if ( E.FirstOccurance("shootchar") )
				{
					yield return C.Prince.Say("Remember our oath", 210);
					yield return C.Wizard.Say("What? First do no harm?", 215);
					yield return C.Prince.Say("That's it", 211);
					//C.Wizard.AnimIdle = "Idle";
					//C.Wizard.Face(C.Prince);
					yield return C.Wizard.Say("Well, I did do that first. Wasn't a fan   ", 216);
				}	 
				yield return E.WaitSkip(1.0f);
				C.Wizard.AnimIdle = "Idle";
				yield return C.Wizard.Say("Fine		", 217);
				yield break;
			}
			
			yield return C.Wizard.Say("FORCE BOLT!", 218);
			yield return E.WaitSkip(0.25f);
			//C.Wizard.WaitForAnimTrigger("Shoot");
			// TODO: gun sfx & flash
			E.ScreenFlash( E.ColorFlashWhite);
			Audio.Play("Gunshot");
			yield return E.WaitSkip(0.25f);
			C.Wizard.AnimIdle = "Idle";	
		}
		else if ( item == I.Mindtrick )
		{
			C.Section("Mesmer spell (holds gun on person)");
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			yield return C.Wizard.Say("MESMER!", 219);
			yield return E.WaitSkip(0.25f);
			C.Wizard.AnimIdle = "Gun";
			yield return E.WaitSkip();
			Audio.Play("GunCock");
			yield return E.WaitSkip();
			switch (E.Occurrance("mindrck") %3)
			{	
			case 0:
			yield return C.Wizard.Say("Alright buddy, don't try anything", 220);
			break; case 1:
			yield return C.Wizard.Say("Hold it right there", 221);
			break; case 2:
			yield return C.Wizard.Say("No sudden moves, dirt-bag", 222);
			break;
			}
			yield return E.WaitSkip();
			C.Section("Responses to Wizard pulling out gun");
			if ( mouseOver == C.Doorian )
			{		
				yield return C.Doorian.Say("Ooh! Take me prisoner! Prisons have the most interesting doors, you know", 50);
			}
			else if ( mouseOver == C.Maria )
			{
				switch (E.Occurrance("marrmindrck") % 2 )
				{	
					case 0:
					yield return C.Maria.Say("I ain't scared of you", 39);
					break; case 1:
					yield return C.Maria.Say("I'll shove that piece so far up your backside, you won't be able to pick your nose without it goin' off", 40);
					break;
				}
			}
			else if ( mouseOver == C.Prince )
			{
				yield return C.Prince.Say("I'm your friend, Maldrek. You only have to ask", 212);
			}
			else if ( mouseOver == C.Merchant )
			{
				yield return C.Merchant.Say("Shoot if you want, ain't gonna do you no good", 48);
				yield return C.Merchant.Say("Got a stockpile of healing potions back here", 49);
			}
			yield return E.WaitSkip();
			C.Wizard.AnimIdle = "Idle";
				
		}
		else if ( item == I.Hyperdrive )
		{
			C.Section("use hypderdrive");
			yield return C.FaceClicked();
			yield return C.Wizard.Say("I ain't bending time and space just to travel there", 223);
		}
		else if ( item == I.Eat )
		{
			C.Section("use eat");
			yield return C.FaceClicked();
			yield return C.Wizard.Say("Eh, I ain't hungry enough", 224);
		}
		else if ( item == I.Steal )
		{ 
			C.Section("use steal");
			yield return C.FaceClicked();
			if ( mouseOver.ClickableType == eQuestClickableType.Character )
				yield return C.Wizard.Say("Don't think they have anything worth pinching", 225);
			else 
				yield return C.Wizard.Say("I don't need to pinch that", 226);
		}
		else if ( item == I.Vomit )
		{
			yield return C.FaceClicked();
			yield return C.Wizard.Say("Don't need that to blow chunks everywhere", 227);
		}
		else if ( item == I.Lift )
		{
			yield return C.FaceClicked();
			yield return C.Prince.Say("I don't think I should lift that with my mind", 213);
		}
		else if ( item == I.Open )
		{
			yield return C.WalkToClicked();
			yield return C.FaceClicked();
			yield return C.Prince.Say("OPEN!", 115);
			yield return E.WaitSkip();
			C.Plr.PlayAnimationBG("Doorbell");
			yield return C.Plr.WaitForAnimTrigger("Press");
			Audio.Play("Doorbell");
			yield return E.WaitWhile( () => C.Plr.Animating );
			yield return E.WaitSkip(0.25f);
			yield return C.Prince.Say("Nothing opened", 214);
		}
		else 
		{
			if ( C.Prince.IsPlayer )
			{
				if ( Random.value < 0.5f )
					yield return C.Prince.Say("Umm... I'm not sure about that", 215);
				else 
					yield return C.Prince.Say("I... don't think so", 216);
			}
			else
			{
				if ( Random.value < 0.5f )
					yield return C.Wizard.Say("Idiot, that won't work", 228);
				else 
					yield return C.Wizard.Say("That makes no sense", 229);
			}
		}
	}

	#endregion

}
