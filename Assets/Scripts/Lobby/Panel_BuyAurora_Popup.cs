using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Panel_BuyAurora_Popup : MonoBehaviour
{
    public UILabel      m_Title;
    public UISprite     m_IconCenter;
    public UISprite []  m_Icon4;
    public UILabel      m_lbPlace;

    private int m_nLevel;
    private serverManager.Delegate_Result m_Delegate;
    
    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void Begin(UISprite[] a_SprIcon, int a_nLevel , serverManager.Delegate_Result a_Delegate)
    {
        m_Delegate      = a_Delegate;
        m_nLevel        = a_nLevel;
        m_Title.text    = "Level " + a_nLevel;

        m_lbPlace.text = DataMgr.Inst.m_DB_AuroraUnlockPrice[a_nLevel].nPayValue + " " + DataMgr.Inst.m_DB_AuroraUnlockPrice[a_nLevel].strPayCode;

        int Count=0;
        for(int i=0; i<a_SprIcon.Length; i++)
        {
            m_Icon4[i].enabled = false;
            if (a_SprIcon[i].enabled)
                Count++;
        }

        if(Count == 1)
        {
            m_IconCenter.spriteName = a_SprIcon[0].spriteName;
            m_IconCenter.enabled = true;
        }
        else if (Count == 4)
        {
            m_IconCenter.enabled = false;
            for (int i = 0; i < a_SprIcon.Length; i++)
            {
                if(a_SprIcon[i].enabled)
                {
                    m_Icon4[i].spriteName = a_SprIcon[i].spriteName;
                    m_Icon4[i].enabled = true;
                }
                    
            }
        }
        gameObject.SetActive(true);
    }

    public void onBtnOK()
    {
        DataMgr.Inst.m_SerMgr.AuroraOpen(m_nLevel, m_Delegate);
        gameObject.SetActive(false);
    }

    public void onBtnCancel()
    {
        gameObject.SetActive(false);
    }
}
