using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools.Quest;
using PowerScript;
using UnityEngine.UI;

public class GuiPlrChange : MonoBehaviour
{
    [SerializeField] Image m_buttonImage = null;
    [SerializeField] GuiComponent m_guiComp = null;

    [SerializeField] Sprite m_charPrinceOnly = null;
    [SerializeField] Sprite m_charEnable = null;
    [SerializeField] Sprite m_charPrince = null;
    [SerializeField] Sprite m_charWiz = null;

    float m_flashTime = 0;    

    void Update()
    {        
        if ( GlobalScript.Script is GlobalScript && GlobalScript.Script.m_swapEnabled == false )
        {
            m_buttonImage.sprite = m_charPrinceOnly;
        }
        if ( m_flashTime > 0 )
        {
            m_flashTime -= Time.deltaTime;
            if ( m_flashTime < 0 )
            {
                ChangePlayer();
            }
            else if ( m_flashTime % 0.5f > 0.25f )
            {
                m_buttonImage.sprite = m_charEnable;
            }
            else 
            {
                m_buttonImage.sprite = m_charPrince;
            }
        }

    }

    public void FlashOn()
    {
        m_flashTime = 2;
        GetComponentInParent<GuiDropDownBar>().HighlightForTime(3);
        m_guiComp.GetData().Description = "&75 Switch to Maldrek";
    }

    public void ConsumeClick()
    {
        //PowerQuest.Get.OnGuiClicked();
    }

    public void ChangePlayer()
    {
        // Game needs to run a sequence, so hide the gui
        //GetComponentInParent<GuiDropDownBar>().Hide();
		
		// UPGRADE TODO - FIX THIS:
        PowerQuest.Get.OnGuiClicked();

        if (GlobalScript.Script is GlobalScript &&  GlobalScript.Script.m_swapEnabled == false )
        {
        
        }
        else 
        {
            if ( C.Prince.IsPlayer )
            {
                // Switch to wiz
                PowerQuest.Get.Player = C.Wizard;
                C.Wizard.Clickable = false;
                C.Prince.Clickable = true;
                m_buttonImage.sprite = m_charWiz;
                
                m_guiComp.GetData().Description = "&76 Switch to Prince Xandar";
                
            }
            else 
            {
                PowerQuest.Get.Player = C.Prince;         
                C.Wizard.Clickable = true;
                C.Prince.Clickable = false;       
                m_buttonImage.sprite = m_charPrince;
                m_guiComp.GetData().Description = "&77 Switch to Maldrek";
            }

            // Want to trigger a pause when changed. Ideally doesn't pasuse afterwards either.
			
			// UPGRADE TODO - FIX THIS:
			PowerQuest.Get.OnGuiClicked();
            GetComponentInParent<GuiDropDownBar>().Hide();
            GetComponentInParent<GuiDropDownBar>().SetForceOff(true);
        }
    }
}
