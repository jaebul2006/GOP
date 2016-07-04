using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Panel_Char_Popup : MonoBehaviour
{
    public UITexture    m_Card;
    public UISprite     m_CardBack;
    public UILabel 		charName;
	public UILabel 		level;
	public UILabel 		upgrade;
	public UILabel		explabel;

    public UILabel      m_lbAttr;       // jung hoon add.
    public UILabel      m_lbSkill;      // jung hoon add.
    public UISprite     m_BtnUpdage;    // 강화 jung hoon add.
    public UILabel      m_lbExp;        // 경험치 jung hoon add.
    public UILabel      m_lbTeamSet;    // 팀배치 jung hoon add.
    public UILabel      m_lbSell;       // 판매 jung hoon add.
    public UILabel      m_lbTeamUnSet;  // 팀해제 jung hoon add.    
    public UISprite     m_BtnPusion;    // 퓨전.
    
    public UISprite     m_AttrMark;
    public GameObject 	m_Grade;
	public UISprite		exp;
	public UILabel	    hp;
	public UILabel	    atk;
    public UILabel      m_SkillToolTip;

    public GameObject   push;
	public GameObject   pop;

    public UISprite upBtn;
	public UISprite puBtn;
	public UISprite settingBtn;    

    UISprite sellBtn;


    public TeamSlotScript m_TeamSlot;
    public TeamInfoScript m_TeamInfoMgr;

    void Awake ()
	{		
        m_lbAttr.text       = DataMgr.Inst.GetLocal("Attr") + " : ";
        m_lbSkill.text      = DataMgr.Inst.GetLocal("Skill") + " : ";
        m_lbExp.text        = DataMgr.Inst.GetLocal("Exp");
        m_lbTeamSet.text    = DataMgr.Inst.GetLocal("Team Placed");
        m_lbSell.text       = DataMgr.Inst.GetLocal("Selling");
        m_lbTeamUnSet.text  = DataMgr.Inst.GetLocal("Team release"); 
        

        m_BtnUpdage.transform.FindChild("Label").GetComponent<UILabel>().text = 
            DataMgr.Inst.GetLocal("Enchant");

        m_BtnPusion.transform.FindChild("Label").GetComponent<UILabel>().text = 
            DataMgr.Inst.GetLocal("Synthesis");

		sellBtn = push.transform.FindChild("SellBtn").GetComponent<UISprite>();
	}

	void Start ()
	{
        if( DataMgr.Inst.GetLanguage() == DataMgr.emLanguage.emLanguage_Korea)
        {
            m_BtnUpdage.width   = 90;
            m_BtnUpdage.height  = 50;                                    
            m_BtnUpdage.GetComponent<BoxCollider>().size = new Vector3( 90, 50);
            
            m_BtnPusion.width   = 90;
            m_BtnPusion.height  = 50;
            m_BtnPusion.GetComponent<BoxCollider>().size = new Vector3( 90, 50);
        }    
        else 
        {            
            m_BtnUpdage.width   = 120;
            m_BtnUpdage.height  = 50;                                    
            m_BtnUpdage.GetComponent<BoxCollider>().size = new Vector3( 120, 50);
            
            m_BtnPusion.width   = 120;
            m_BtnPusion.height  = 50;
            m_BtnPusion.GetComponent<BoxCollider>().size = new Vector3( 120, 50);            
        }
	}
	
	void Update () 
	{
		
	}
	
	//케릭터 세팅.    
	public void BeginPopup(TeamSlotScript a_TeamSlot, TeamInfoScript _TeamInfoMgr)
	{		
        m_TeamSlot = a_TeamSlot;
        m_TeamInfoMgr = _TeamInfoMgr;
        
        // 속성 세팅.
        int nAttr = 0;
                                                         
		     if(m_TeamSlot.m_CharData.Data.eAttr == DataMgr.emCardAttribute.emCAB_WA) nAttr = 0;
		else if(m_TeamSlot.m_CharData.Data.eAttr == DataMgr.emCardAttribute.emCAB_SU) nAttr = 1;  
		else if(m_TeamSlot.m_CharData.Data.eAttr == DataMgr.emCardAttribute.emCAB_YUNG) nAttr = 2; 
		else if(m_TeamSlot.m_CharData.Data.eAttr == DataMgr.emCardAttribute.emCAB_ARM) nAttr = 3;

        
        m_Card.mainTexture = Resources.Load("Textures/char/icon_mon_" + m_TeamSlot.m_CharData.Data.strResurceID) as Texture;
        m_CardBack.spriteName   = "CardBack_"+ nAttr;
        m_AttrMark.spriteName   = "icon_attr_0"+ (nAttr+2);

        charName.text 	= DataMgr.Inst.GetLocal(m_TeamSlot.m_CharData.Data.strName);
		level.text 		= "Lv. " + m_TeamSlot.m_CharData.nLevel;        

        if (m_TeamSlot.m_CharData.nLevel < m_TeamSlot.m_CharData.nMaxLevel)
        {            
            exp.fillAmount  = (float)m_TeamSlot.m_CharData.nExp / (float)DataMgr.Inst.m_DB_Exp[m_TeamSlot.m_CharData.nLevel ].nExpBar;
            explabel.text   = ((int)(exp.fillAmount*100)).ToString()+"%";
        }
        else 
        {
            exp.fillAmount  = 0.0f;
            explabel.text   = "0"+"%";
        }

        hp.text  = DataMgr.Inst.GetLocal("HP") + " : " + (int)m_TeamSlot.m_CharData.GetHp();
        atk.text = DataMgr.Inst.GetLocal("Attack") + " : " + (int)m_TeamSlot.m_CharData.GetAttack();		
		//grade.spriteName = "icon_grade_"+ m_TeamSlot.m_CharData.nGrade.ToString("00");
        m_Grade.transform.FindChild( "Star_"+m_TeamSlot.m_CharData.nGrade.ToString() ).gameObject.SetActive(true);

        if (m_TeamSlot.m_CharData.nUpgrade > 0) upgrade.text = "+"+ m_TeamSlot.m_CharData.nUpgrade;
		else upgrade.text = "";

        //풀강이면 끈다.
		if(m_TeamSlot.m_CharData.nUpgrade >= 5)
        {
			upBtn.alpha = 0.4f;			
            upBtn.GetComponent<Collider>().enabled = false;
        }

		if(m_TeamSlot.m_CharData.nUpgrade < 5 || m_TeamSlot.m_CharData.nLevel < 30)
        {
			puBtn.alpha = 0.4f;
			puBtn.GetComponent<Collider>().enabled = false;            
        }			

		if(m_TeamSlot.m_CharData.Data.nSkill > 0)
        {                        
            m_SkillToolTip.text = string.Format(DataMgr.Inst.m_DB_SummonSkill[m_TeamSlot.m_CharData.Data.nSkill].strToolTip, 
                DataMgr.Inst.m_DB_SummonSkill[m_TeamSlot.m_CharData.Data.nSkill].nEffectValue);
        }				

		// 리스트중 팀에 설정되어 있는지 체크하여 버튼들의 활성화를 처리한다.
		int index 		= m_TeamSlot.m_CharData.nKey;
		bool b_use = false;
		for (int i=0; i< Defines.DEF_MAX_TEAM; i++)
        {
			for(int j=0; j< Defines.DEF_MAX_TEAM_ITEM; j++)
            {
				if(index == DataMgr.Inst.GetTempTeam(i,j))
				{
					if(i == DataMgr.Inst.m_TeamIdx)
                    {
						b_use = true;
						equiped();
						puBtn.alpha = 0.4f;
						puBtn.GetComponent<Collider>().enabled = false;
						return;
					}
                    else
                    {
						b_use = true;
						unequiped();

						sellBtn.alpha = 0.4f;
						sellBtn.GetComponent<Collider>().enabled = false;

						puBtn.alpha = 0.4f;
						puBtn.GetComponent<Collider>().enabled = false;
					}
					
				}
			}
		}

		if(!b_use)
        {
			unequiped();
		}
	}    
	
	// 이미 팀에 설정되어 있는 경우.
	void equiped()
	{
		push.SetActive (false);
		if( !LastTeam1Summon() )
        {
			pop.SetActive (true);
		}
        else
        {
			settingBtn.alpha = 0.4f;
			settingBtn.GetComponent<Collider>().enabled = false;
		}
	}

    // 첫번째 소환수를 검사한다.
    public bool LastTeam1Summon()
    {        
        if (DataMgr.Inst.m_TeamIdx != 0) return false;

        bool result = false;
        int cnt = 0;        
        for (int i = 0; i < Defines.DEF_MAX_TEAM_ITEM; i++)
        {
            if (DataMgr.Inst.GetTempTeam(0, i) >= 0) cnt++;
        }

        if (cnt == 1) result = true;

        return result;
    }


    void unequiped()
	{
		pop.SetActive (false);
		push.SetActive (true);
	}

    private int m_nBtnKind = 0;
	//강화선택시처리.
	void ClickUpgrade()
	{
        if(m_nBtnKind == 0)
        {
            m_nBtnKind = 1;
            DataMgr.Inst.CommitTempTeam(Team_Result);
        }
	}

	//합성선택시처리.
	void ClickPusion()
	{
        if(m_nBtnKind == 0)
        { 
            m_nBtnKind = 2;
            DataMgr.Inst.CommitTempTeam(Team_Result);
        }
	}

    // MyNetClass.ErrorCode.EC_END
    public void Team_Result(bool a_Result)
    {
        if(a_Result)
        {
            if(m_nBtnKind == 1)
            {                
                m_nBtnKind = 0;
		        DataMgr.Inst.m_CurSelectCardNum = m_TeamSlot.m_CharData;		
                DataMgr.Inst.SetPageState(DataMgr.emMAIN_MODE.UPGRADE);
                DataMgr.Inst.GetPage(DataMgr.emMAIN_MODE.UPGRADE).SendMessage("Begin", m_TeamSlot.m_CharData, SendMessageOptions.DontRequireReceiver);
                DataMgr.Inst.GetPage(DataMgr.emMAIN_MODE.MYTEAM).SendMessage("createCardList", true, SendMessageOptions.DontRequireReceiver);
                Destroy(gameObject);
            }
            else if(m_nBtnKind == 2)
            {                
                m_nBtnKind = 0;
		        DataMgr.Inst.m_CurSelectCardNum = m_TeamSlot.m_CharData;
                DataMgr.Inst.SetPageState(DataMgr.emMAIN_MODE.PUSION);
                DataMgr.Inst.GetPage(DataMgr.emMAIN_MODE.PUSION).SendMessage("Begin", m_TeamSlot.m_CharData, SendMessageOptions.DontRequireReceiver);
                DataMgr.Inst.GetPage(DataMgr.emMAIN_MODE.MYTEAM).SendMessage("createCardList", true, SendMessageOptions.DontRequireReceiver);
                Destroy(gameObject);
            }
        }
        else 
            m_nBtnKind = 0;
        
    }



	//팀해제선택시처리.
	void ClickPop()
	{
        m_TeamInfoMgr.popCard(m_TeamSlot.m_CharData);
		Destroy(gameObject);
	}

	//팀참가선 택시처리.
	void ClickPush()
	{
        m_TeamInfoMgr.viewSelectArrow(m_TeamSlot.m_CharData);        
		Destroy(gameObject);
	}

	void ClickClose()
	{
		Destroy(gameObject);
	}

	// 판매 클릭.
	void ClickSell()
	{
        if(DataMgr.Inst.m_UserSummonList.Count > 2)
        { 
            DataMgr.Inst.GetPage(DataMgr.emMAIN_MODE.MYTEAM).
                GetComponent<Panel_UI_TeamManager>().m_Panel_Sell_popup.gameObject.SetActive(true);        

            DataMgr.Inst.GetPage(DataMgr.emMAIN_MODE.MYTEAM).
                GetComponent<Panel_UI_TeamManager>().m_Panel_Sell_popup.Begin(m_TeamSlot.m_CharData);
            ClickClose();   
        }
        else
        {            
            CMessageBox.Create(DataMgr.Inst.GetLocal("Need at least 2 Summon at character slot."), 0, null);
        }
    }

    // 확인을 누룰때.
    public void SellResult(bool bOK)
    {        
        if(bOK)
        {
            
        }        
    }
}
