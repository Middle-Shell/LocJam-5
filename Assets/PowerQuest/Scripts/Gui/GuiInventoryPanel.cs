using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PowerTools;

namespace PowerTools.Quest
{

public class GuiInventoryPanel : MonoBehaviour
{
	class GuiInventoryPanelItem 
	{
		public int m_inventoryItem = -1;
		public GameObject m_obj = null;
		public SpriteAnim m_sprite = null;
		public Button m_button = null;
	};
	class GuiInventoryPanelItems : List<GuiInventoryPanelItem> {}

	//[SerializeField] Vector2 m_itemSize = Vector2.one;
	[SerializeField] GameObject m_itemPrefab = null;
	[SerializeField] Transform m_grid = null;
	[Tooltip("Name of character to show inventory of. If empty will use the current player")]
	[SerializeField] string m_targetCharacter = null;

	GuiInventoryPanelItems m_items = new GuiInventoryPanelItems();

	void UpdateButtons()
	{
		Character player = string.IsNullOrEmpty(m_targetCharacter)  ? PowerQuest.Get.GetPlayer() : PowerQuest.Get.GetCharacter(m_targetCharacter);

		List<Character.CollectedItem> inventory = player.GetInventory();

		// add to end if not enough
		while ( m_items.Count < inventory.Count )
		{
			GuiInventoryPanelItem item = new GuiInventoryPanelItem();
			item.m_obj = GameObject.Instantiate(m_itemPrefab) as GameObject;
			if ( item.m_obj != null )
			{
				item.m_obj.transform.SetParent(m_grid,false);
				item.m_sprite = item.m_obj.GetComponentInChildren<SpriteAnim>();
			}
			m_items.Add(item);
		}

		// remove from end if too many
		while ( m_items.Count > inventory.Count )
		{
			GameObject obj = m_items[m_items.Count-1].m_obj;
			if ( obj != null )
				GameObject.Destroy(obj);
			m_items.RemoveAt(m_items.Count-1);
		}

		// Check if it's playing the correct anim, and if not, play it.
		for ( int i = 0; i < inventory.Count; ++i )
		{
			GuiInventoryPanelItem guiItem = m_items[i];
			Inventory inventoryItem = PowerQuest.Get.GetInventory(inventory[i].m_name);
			if ( guiItem.m_sprite.ClipName != inventoryItem.AnimGui )
			{
				AnimationClip anim =  PowerQuest.Get.GetInventoryAnimation( inventoryItem.AnimGui );
				if ( anim == null )
				{
				    Debug.LogWarning("Couldn't find inventory anim "+inventoryItem.AnimGui);
					inventoryItem.AnimGui = guiItem.m_sprite.ClipName; // so don't keep getting repeat warnings
				}
				guiItem.m_sprite.Play(anim);
			}	
			guiItem.m_obj.GetComponent<GuiComponent>().GetData().Description = inventoryItem.Description;
		}
	}

	// Update is called once per frame
	void Start()
	{
		UpdateButtons();
	}

	void Update() 
	{
		UpdateButtons();
	}

	// Sent via message from each item- TODO: use new event system for this as god intended
	void MsgOnItemClick(PointerEventData eventData)
	{
		Character player = string.IsNullOrEmpty(m_targetCharacter)  ? PowerQuest.Get.GetPlayer() : PowerQuest.Get.GetCharacter(m_targetCharacter);

		List<Character.CollectedItem> inventory = player.GetInventory();
		int invIndex = m_items.FindIndex(item=>item.m_obj == eventData.pointerPress);
		if ( inventory.IsIndexValid(invIndex) )
		{
			bool shouldUnpause = PowerQuest.Get.OnInventoryClick(inventory[invIndex].m_name, eventData.button);
			if ( shouldUnpause )
			{
				// Game needs to run a sequence, so hide the gui
				transform.parent.GetComponent<GuiDropDownBar>().Hide();
			}
		}

		//Debug.Log("ItemClick- "+eventData.pointerPress+" button: "+eventData.button.ToString());
		 
	}
}
}