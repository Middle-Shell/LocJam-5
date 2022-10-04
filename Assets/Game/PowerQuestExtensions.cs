using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using PowerTools;

namespace PowerTools.Quest
{

/// If you want to add your own functions/variables to PowerQuest stuff, add them here. 
/**
 * Variables added to these classes is automatically saved, and will show up in the "Data" of objects in the inspecor
 * Adding functions to the interfaces will make them accessable in QuestScript editor.
 */

/// Functions/Properties added here are accessable from the 'E.' object in quest script
public partial interface IPowerQuest
{
	Color ColorBG {get;}
	Color ColorFlashRed {get;}
	Color ColorFlashWhite {get;}
	Color ColorShadowLab {get;}
	Color ColorShadowSkull {get;}
	Color ColorShadowFactory {get;}


	Coroutine StartQuestCoroutine( IEnumerator routine );
	Coroutine ScreenFlash( Color col, float time = 0.1f, float fadeTime = 0.15f );
	/// Starts a sequence in the background that can be stopped again StopCustomBackgrouindSequence(). Use it for background conversations. Since they can't be skippable, use SayBG and WaitForDialog.
	void StartCustomBackgroundSequence( PowerQuest.DelegateWaitForFunction function );
	/// Stops the current custom background sequence. Only the base function will be stopped, you'll have to manually stop characters walking, and call E.CancelSay(); to stop them talking (if desired)
	void StopCustomBackgroundSequence();

	void UpdateClickableCollider(IQuestClickableInterface clickable);
	
	/// Waits until the current dialog has finished, allowing it to be skipped, useful when you start a SayBG, do a bunch of other things, then want to skip the rest of the SayBG()
	Coroutine WaitForDialogSkip();
	
}

public partial class PowerQuest
{
	[Header("Drifter Constants")]
	[SerializeField] Color m_colorBG = Color.black;
	[SerializeField] Color m_colorMockBG = Color.black;
	[SerializeField] Color m_colorFlashRed = Color.black;
	[SerializeField] Color m_colorFlashWhite = Color.black;
	[SerializeField] Color m_colorShadowLab = Color.black;
	[SerializeField] Color m_colorShadowSkull = Color.black;
	[SerializeField] Color m_colorShadowFactory = Color.black;

	
	public Color ColorBG {get{return m_colorBG;}}
	public Color ColorMockBG {get{return m_colorMockBG;}}
	public Color ColorFlashRed {get{return m_colorFlashRed;}}
	public Color ColorFlashWhite {get{return m_colorFlashWhite;}}
	
	public Color ColorShadowLab {get{return m_colorShadowLab;}}
	public Color ColorShadowSkull {get{return m_colorShadowSkull;}}
	public Color ColorShadowFactory {get{return m_colorShadowFactory;}}

	public Coroutine WaitForDialogSkip() { return StartQuestCoroutine(CoroutineWaitForDialogSkip()); }
	
	IEnumerator CoroutineWaitForDialogSkip()
	{ 		
		m_waitingForBGDialogSkip = true;
		yield return WaitWhile(()=> {		
			// Check if any players are currently talking
			return m_displayActive || m_characters.Exists( item => item.Talking );
		} ); 
		m_waitingForBGDialogSkip = false;
	}	

	// Partial function to implement for a hook on Start, just before "OnGameStart" is called in global script
	partial void ExOnGameStart()
	{		
		AddSaveData("customBGSeq", m_customSequenceData, CustomBGSequenceOnPostRestore);
	}

	// Partial function to implement for a hook on Update
	partial void ExUpdate()
	{
		//UpdateCustomBackgroundSequence();
	}

	// Partial function to implement for a hook in the main blocking loop. After queued sequences are resumed, but before interactions are processed, etc.
	partial void ExOnMainLoop()
	{

	}


	public List<IQuestClickable> GetAllClickables(bool activeOnly)
	{
		List<IQuestClickable> result = new List<IQuestClickable>();
		result.AddRange( m_currentRoom.GetHotspots().ToArray() );
		result.AddRange( m_currentRoom.GetProps().ToArray() );

		if ( activeOnly )
		{
			foreach ( Character item in m_characters )
			{
				if ( item.Room == m_currentRoom )
					result.Add(item);
			}

			result.RemoveAll(item=> item.Clickable == false || item.Instance == null );
		}
		else 
		{
			result.AddRange(m_characters.ToArray());
		}

		return result;
	}

	// Brief screen flash using FadeIn/Out
	public Coroutine ScreenFlash( Color col, float time = 0.1f, float fadeTime = 0.15f ) { return StartQuestCoroutine( CoroutineScreenFlash(col,time,fadeTime) ); }
	IEnumerator CoroutineScreenFlash( Color col, float time, float fadeTime )
	{
		Color startColour = FadeColor;
		bool fadedOut = GetMenuManager().GetFadeRatio() > 0.5f;

		FadeColorRestore();
		FadeColor = col;

		if ( fadedOut )
			FadeInBG(0);
		FadeOutBG(0);
		if ( time > 0 )
			yield return Wait(time);
		if ( fadedOut == false )
		{
			yield return FadeIn(fadeTime);
		}
		else 
		{			
			// Lerp from flash colour to original
			float timer = 0;
			while ( timer < fadeTime && GetSkippingCutscene() == false )
			{
				timer += Time.deltaTime;
				FadeColorRestore();
				FadeColor = Color.Lerp(col,startColour,timer/fadeTime);
				yield return null;
			}
		}

		FadeColorRestore();
		yield return Break;
	
	}
	[System.Serializable]
	public class CustomSequenceData
	{
		[SerializeField] public string room = string.Empty;
		[SerializeField] public string function = string.Empty;
	}
	[SerializeField] public CustomSequenceData m_customSequenceData = new CustomSequenceData();
	Coroutine m_customBackgroundSequence = null;
	Coroutine m_customBackgroundSequenceCaller = null;

	void CustomBGSequenceOnPostRestore()
	{		
		if ( string.IsNullOrEmpty( m_customSequenceData.room ) == false )
		{			
			// Need to find the function and run it			
			QuestScript scriptClass = this.GetRoom(m_customSequenceData.room).GetScript();
			if  ( scriptClass != null )		
			{
				string function = m_customSequenceData.function;
				System.Reflection.MethodInfo method = scriptClass.GetType().GetMethod( function, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
				if ( method != null )
				{					
					// Start sequence
					
					StartCustomBackgroundSequence(method.CreateDelegate(typeof(DelegateWaitForFunction),scriptClass) as DelegateWaitForFunction);
					/* -- This results in a dynamic function being added. don't think it's what we
					IEnumerator currentSequenceEnumerator = method.Invoke(scriptClass,null) as IEnumerator;
					if ( currentSequenceEnumerator != null )
					{
						StartCustomBackgroundSequence( ()=>currentSequenceEnumerator );
						m_customSequenceData.function = function;
						//StartCoroutine( CoroutineCustomBackgroundSequenceCoroutine(currentSequenceEnumerator) );
						//m_customBackgroundSequence = StartQuestCoroutine(currentSequenceEnumerator);
					}*/
				}
			}	
		}
	}	
	public void StartCustomBackgroundSequence( DelegateWaitForFunction function )
	{
		StopCustomBackgroundSequence();
		m_customBackgroundSequenceCaller = StartCoroutine(CoroutineCustomBackgroundSequenceCoroutine(function));
		/*m_customSequenceData.m_customBGSequenceRoom = GetCurrentRoom().ScriptName;
		m_customSequenceData.m_customBGSequenceFunction = function.Method.Name;
		m_customBackgroundSequence = function();
		*/
	}


	public void StopCustomBackgroundSequence()
	{
		/*
		if ( m_nurseCopTalking )
		{
			m_nurseCopTalking = false;
			PowerQuest.Get.CancelSay();
			if ( m_coroutineNurseCopTalk != null )
				PowerQuest.Get.StopCoroutine(m_coroutineNurseCopTalk);			
			// NB: HAve to pause a frame before cancel say is called or the next one gets immediately started in some cases
			yield return null;
			PowerQuest.Get.CancelSay();
		}*/
		if ( m_customBackgroundSequenceCaller != null )
			StopCoroutine(m_customBackgroundSequenceCaller);
		if ( m_customBackgroundSequence != null )
			StopCoroutine(m_customBackgroundSequence);

		m_customBackgroundSequenceCaller = null;
		m_customSequenceData.room = null;
		m_customSequenceData.function = null;
	}
	
	IEnumerator CoroutineCustomBackgroundSequenceCoroutine(DelegateWaitForFunction function)
	{
		// Set data so we can restore from save game
		m_customSequenceData.room = GetCurrentRoom().ScriptName;
		m_customSequenceData.function = function.Method.Name;

		// Wait for the coroutine
		m_customBackgroundSequence = StartQuestCoroutine(function());
		yield return m_customBackgroundSequence;
		
		// cleanup - maybe also want to do this on room exit? This is getting hairier and hairier
		m_customBackgroundSequenceCaller = null;
		m_customBackgroundSequence = null;
		m_customSequenceData.room = null;
		m_customSequenceData.function = null;
	}

	// Trying using sprite data to generate collider. This needs the sprite to be the currently visible one
	// Also won't handle saving/loading or even room changes. Probably better as a flag for "use sprite collision" and it'll update on sprite change
	// Or, drag sprites in with ID to choose which to use for collider based on ID/Name
	public void UpdateClickableCollider(IQuestClickableInterface clickable)
	{		
		if ( clickable.IClickable.Instance == null )
			return;

		SpriteRenderer renderer = clickable.IClickable.Instance.GetComponentInChildren<SpriteRenderer>();
		PolygonCollider2D polygonCollider = clickable.IClickable.Instance.GetComponentInChildren<PolygonCollider2D>();
		if( renderer == null || polygonCollider == null )
			return;

		Sprite sprite = renderer.sprite;
		if ( sprite == null )
			return;

		for (int i = 0; i < polygonCollider.pathCount; i++) polygonCollider.SetPath(i, new List<Vector2>());
		polygonCollider.pathCount = sprite.GetPhysicsShapeCount();
		
		List<Vector2> path = new List<Vector2>();		
		for (int i = 0; i < polygonCollider.pathCount; i++) 
		{
			path.Clear();
			sprite.GetPhysicsShape(i, path);
			polygonCollider.SetPath(i, path.ToArray());
		}
		// offset by powersprite offset
		PowerTools.PowerSprite powerSprite = renderer.GetComponent<PowerTools.PowerSprite>();		
		if ( powerSprite != null )
			polygonCollider.offset = powerSprite.Offset;
				
	}

}

/// Functions/Properties added here are accessable from the 'C.<characterName>.' object in quest script
public partial interface ICharacter
{
	/** Example: Adding health variable to characters /
	bool IsDead();
	float HealthPoints { get; set; }
	/**/

	string Pose { get; set; }
	string NextPose { set; }
	void ResetPose();

	bool InRegion(IRegion region);

	/// Start character saying something, unskippable. Useful in background conversations
	Coroutine SayNoSkip(string dialog, int id = -1);
	Coroutine SayBGSkip(string dialog, int id = -1);
	/*
	Coroutine FaceUpRight(bool instant = false);
	Coroutine FaceUpLeft(bool instant = false);
	Coroutine FaceDownRight(bool instant = false);
	Coroutine FaceDownLeft(bool instant = false);

	void FaceUpRightBG(bool instant = false);
	void FaceUpLeftBG(bool instant = false);
	void FaceDownRightBG(bool instant = false);
	void FaceDownLeftBG(bool instant = false);*/


}

public partial class Character
{
	/** Example: Adding health to characters /
	[SerializeField] float m_healthPoints = 0;
	public bool IsDead() { return m_healthPoints <= 0; }
	public float HealthPoints { get { return m_healthPoints; } set { m_healthPoints = value; } }
	/**/

	string m_poseIdle = null;
	string m_poseTalk = null;
	//bool m_poseAutoEnd = false;

	public string Pose 
	{ 
		get
		{
			return AnimIdle;
		}
		set 
		{
			ResetPose();
			m_poseIdle = AnimIdle;
			m_poseTalk = AnimTalk;
			AnimIdle = value;
			AnimTalk = value;
		} 
	}

	public string NextPose 
	{
		set 
		{			
			Pose = value;
			//m_poseAutoEnd = true;
			CallbackOnEndSay += ResetPose;
			PowerQuest.Get.CallbackOnEndCutscene += ResetPose;
		} 
	}
	
	// TODO: on skip cutscene- always reset "nextpose"
	public void ResetPose()
	{
		CallbackOnEndSay -= ResetPose;
		PowerQuest.Get.CallbackOnEndCutscene -= ResetPose;
		if ( string.IsNullOrEmpty(m_poseIdle) == false )
			AnimIdle = m_poseIdle;
		if ( string.IsNullOrEmpty(m_poseTalk) == false )
			AnimTalk = m_poseTalk;
		m_poseIdle = null;
		m_poseTalk = null;
		//m_poseAutoEnd = false;
	}


	public bool InRegion(IRegion region)
	{
		return region.GetCharacterOnRegion(this);
	}

	/// Start charcter saying something, unskippable. Useful in background conversations
	public Coroutine SayNoSkip(string dialog, int id = -1)
	{
		if ( m_coroutineSay != null )
		{			
			PowerQuest.Get.StopCoroutine(m_coroutineSay);
			EndSay();
			PowerQuest.Get.OnSay();
		}

		if ( CallbackOnSay != null )
			CallbackOnSay.Invoke(dialog,id);

		m_coroutineSay = CoroutineSayNoSkip(dialog, id);
		return PowerQuest.Get.StartCoroutine(m_coroutineSay); 
	}

	IEnumerator CoroutineSayNoSkip(string text, int id = -1)
	{
		StopWalking();
		if ( PowerQuest.Get.GetSkippingCutscene() ) // I guess still skippable if in cutscene
			yield break;

		StartSay( text, id );
		yield return PowerQuest.Get.WaitForDialog(PowerQuest.Get.GetTextDisplayTime(text), m_dialogAudioSource, PowerQuest.Get.GetShouldSayTextAutoAdvance(), false);		
		EndSay();
	}
	

	// Start character saying something in the background, but still skippable
	public Coroutine SayBGSkip(string dialog, int id = -1) 
	{
		if ( m_coroutineSay != null )
		{
			PowerQuest.Get.StopCoroutine(m_coroutineSay);
			EndSay();
		}
		m_coroutineSay = CoroutineSayBGSkip(dialog, id);
		return PowerQuest.Get.StartCoroutine(m_coroutineSay); 
	}

	IEnumerator CoroutineSayBGSkip(string text, int id = -1)
	{	
		StartSay( text, id );
		yield return PowerQuest.Get.WaitForDialog(PowerQuest.Get.GetTextDisplayTime(text), m_dialogAudioSource, true, true);
		EndSay();
	}

	/*
	public Coroutine FaceUR(bool instant = false) { return Face(eFace.UpRight, instant); }
	public Coroutine FaceUL(bool instant = false) { return Face(eFace.UpLeft, instant); }
	public Coroutine FaceDR(bool instant = false) { return Face(eFace.DownLeft, instant); }
	public Coroutine FaceDL(bool instant = false) { return Face(eFace.DownRight, instant); }
	
	public void FaceURBG(bool instant = false) { Face(eFace.UpRight, instant); }
	public void FaceULBG(bool instant = false) { Face(eFace.UpLeft, instant); }
	public void FaceDRBG(bool instant = false) { Face(eFace.DownLeft, instant); }
	public void FaceDLBG(bool instant = false) { Face(eFace.DownRight, instant); }
	*//*
	public Coroutine FaceUpRight(bool instant = false) { return Face(eFace.UpRight, instant); }
	public Coroutine FaceUpLeft(bool instant = false) { return Face(eFace.UpLeft, instant); }
	public Coroutine FaceDownRight(bool instant = false) { return Face(eFace.DownLeft, instant); }
	public Coroutine FaceDownLeft(bool instant = false) { return Face(eFace.DownRight, instant); }
	
	public void FaceUpRightBG(bool instant = false) { Face(eFace.UpRight, instant); }
	public void FaceUpLeftBG(bool instant = false) { Face(eFace.UpLeft, instant); }
	public void FaceDownRightBG(bool instant = false) { Face(eFace.DownLeft, instant); }
	public void FaceDownLeftBG(bool instant = false) { Face(eFace.DownRight, instant); }
	*/
}

/// Functions/Properties added here are accessable from the 'R.<RoomName>.' object in quest script
public partial interface IRoom
{
}

public partial class Room
{
}

/// Functions/Properties added here are accessable from the 'Props.<name>.' object in quest script
public partial interface IProp
{
	//float Alpha {get;set;}
}

public partial class Prop
{
	/*
	[SerializeField] float m_alpha = 1;

	public float Alpha { 
		get { return m_alpha; } 
		set 
		{ 
			m_alpha = value; 
			
			if ( Instance == null )
				return;

			SpriteRenderer[] sprites = Instance.GetComponentsInChildren<SpriteRenderer>();
			TextMesh[] texts = Instance.GetComponentsInChildren<TextMesh>();
			System.Array.ForEach( sprites, sprite => { if ( sprite != null ) sprite.color = sprite.color.WithAlpha( m_alpha ); });
			System.Array.ForEach( texts, text => { if ( text != null ) text.color = text.color.WithAlpha( m_alpha ); });
		} 
	}*/
}

/// Functions/Properties added here are accessable from the 'Hotspots.<name>.' object in quest script
public partial interface IHotspot
{
}

public partial class Hotspot
{
}

/// Functions/Properties added here are accessable from the 'Regions.<name>.' object in quest script
public partial interface IRegion
{
}

public partial class Region
{
}

/// Functions/Properties added here are accessable from the 'I.<itemName>.' object in quest script
public partial interface IInventory
{
}

public partial class Inventory
{
}

}
