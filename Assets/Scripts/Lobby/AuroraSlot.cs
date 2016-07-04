using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 오로라 저장 정보. 
// PlayerPrefs.GetInt("SelAurora_" + m_nLevel, -1);
// 0:보동, 1:선택.
public class AuroraSlot : MonoBehaviour
{    
    [System.NonSerialized]    
    public int          m_nLevel;
    public UILabel      m_Level;    
    public UISprite[]   m_SprIcon;    
    public UISprite     m_Line1_Up;
    public UISprite     m_Line1_Down;
    public UISprite     m_Line1_Right;

    public UISprite     m_Line2_Right;
    public UISprite     m_Line3_Right;

    public GameObject   m_ToolTip;

    private bool[]       m_IsLoakIcon = new bool[4];
    private int  []     m_nAuroraSkill;
    private Panel_BuyAurora_Popup m_BuyAurora_Popup;
    private Panel_UI_Aurora m_Panel_UI_Aurora;

    // Use this for initialization
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        if (m_nToolTipKey == 1)
        {
            if (m_Limit < System.Environment.TickCount)
            {
                
                float fY = 193.0f - Mathf.Abs(m_Panel_UI_Aurora.transform.FindChild("Scroll View").transform.localPosition.y);
                fY -= Mathf.Abs(transform.localPosition.y);
                fY = Mathf.Abs(fY);
                if (fY < 10.0f)
                    m_ToolTip.transform.localPosition = new Vector3(0.0f, -210, 0.0f);
                else
                    m_ToolTip.transform.localPosition = new Vector3(0.0f, 22, 0.0f);


                m_ToolTip.SetActive(true);
                m_nToolTipKey = 2;
            }
        }

    }

    public void Begin(int a_Level, Panel_BuyAurora_Popup a_BuyAurora_Popup, Panel_UI_Aurora a_Panel_UI_Aurora)
    {
        m_Panel_UI_Aurora = a_Panel_UI_Aurora;
        m_BuyAurora_Popup = a_BuyAurora_Popup;
        m_nAuroraSkill = new int[4] {-1, -1, -1, -1 };
        m_Line1_Up.enabled = false;
        m_Line1_Down.enabled = false;
        m_Line1_Right.enabled = false;
        m_Line2_Right.enabled = false;
        m_Line3_Right.enabled = false;
        m_ToolTip.SetActive(false);
        for (int i = 0; i < m_SprIcon.Length; i++)
        {
            m_SprIcon[i].enabled = false;
            m_IsLoakIcon[i] = false;            
        }

        m_nLevel = a_Level;
        m_Level.text = "Lv."+a_Level;
    }

    public void PushSkill(int a_IconPosIndex, DataMgr.tagSaticAuroraSkill a_AuroraInfo)
    { m_nAuroraSkill[a_IconPosIndex] = a_AuroraInfo.nIDX; }

    public void SetUpSkill()
    {
        int nCountIcon=0;
        for(int i=0; i< m_nAuroraSkill.Length; i++ )
        {
            if (m_nAuroraSkill[i] != -1) nCountIcon++;
        }        
        
        int Rtn = DataMgr.Inst.IsBuyAurora(m_nLevel);
        // 1사용, 2구입가능, 3불가.
        ////-1 보통, -2 활성, -3비활성
        if (nCountIcon == 1)
        {
            if (Rtn == 1)
            {
                m_SprIcon[0].spriteName = DataMgr.Inst.m_DB_Aurora[m_nAuroraSkill[0]].strIconCode + "-1";
                m_IsLoakIcon[0] = false;
            }                
            else if (Rtn == 2)
            {
                m_SprIcon[0].spriteName = DataMgr.Inst.m_DB_Aurora[m_nAuroraSkill[0]].strIconCode + "-3";
                m_IsLoakIcon[0] = true;
            }
            else
            {
                m_SprIcon[0].spriteName = DataMgr.Inst.m_DB_Aurora[m_nAuroraSkill[0]].strIconCode + "-3";
                m_IsLoakIcon[0] = false;
            }
                
            m_SprIcon[0].enabled    = true;            
        }
        else
        {
            int Sel = DataMgr.Inst.m_AuroraSelect[m_nLevel].SelectPosIdx;  
            for (int i = 0; i < m_nAuroraSkill.Length; i++)
            {
                if (Rtn == 1) 
                {
                    if(Sel == i)
                        m_SprIcon[i].spriteName = DataMgr.Inst.m_DB_Aurora[m_nAuroraSkill[i]].strIconCode + "-2";
                    else
                        m_SprIcon[i].spriteName = DataMgr.Inst.m_DB_Aurora[m_nAuroraSkill[i]].strIconCode + "-1";

                    m_IsLoakIcon[i] = true;
                }
                else if (Rtn == 2)
                {
                    m_SprIcon[i].spriteName = DataMgr.Inst.m_DB_Aurora[m_nAuroraSkill[i]].strIconCode + "-3";
                    m_IsLoakIcon[i] = true;
                }
                else
                {
                    m_SprIcon[i].spriteName = DataMgr.Inst.m_DB_Aurora[m_nAuroraSkill[i]].strIconCode + "-3";
                    m_IsLoakIcon[i] = false;
                }
                
                m_SprIcon[i].enabled = true;
                
            }
        }

    }

    public void AuroraOpen_Result(bool a_Result, Dictionary<string, object> a_dicJson, string a_Str, serverManager.ErrorCode a_ErrorCode)
    {
        if(a_Result)
        {
            if(a_dicJson["PayCode"].ToString() != "H")
                DataMgr.Inst.DataPasing( a_dicJson["PayCode"].ToString(), a_dicJson["PayValue"].ToString());

            DataMgr.Inst.m_UserInfo.Aurora_maxLevel = int.Parse(a_dicJson["OutValue"].ToString());

            m_Panel_UI_Aurora.SendMessage("SlotSetUp",SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            if(serverManager.ErrorCode.EC_DeficitMoney == a_ErrorCode)
            {
                CMessageBox.Create(DataMgr.Inst.GetLocal("You have not enough Moneys."), 1, null);   
            }
            else
            {                                
                CMessageBox.Create( DataMgr.Inst.GetLocal("Failed to Open Aurora Skill! Call to Manager. (ErrorCode:"+(int)a_ErrorCode+")"), 1, null);                
            }
        }
    }

    void SelectOrBuy( int a_nIndex )
    {
        if (m_nToolTipKey == 2) return;
        
        int Rtn = DataMgr.Inst.IsBuyAurora(m_nLevel);
        if (Rtn == 2)
        {   // 구입                                    
            // 가격 예외처리
            if(DataMgr.Inst.m_DB_AuroraUnlockPrice[m_nLevel].strPayCode == "Ruby")
            {
                if( DataMgr.Inst.m_DB_AuroraUnlockPrice[m_nLevel].nPayValue > DataMgr.Inst.m_UserInfo.Ruby)
                {                    
                    CMessageBox.Create( DataMgr.Inst.GetLocal("You have not enough ruby."), 1, null);
                    return;
                }
            }
            else if(DataMgr.Inst.m_DB_AuroraUnlockPrice[m_nLevel].strPayCode == "Gold")
            {
                if( DataMgr.Inst.m_DB_AuroraUnlockPrice[m_nLevel].nPayValue > DataMgr.Inst.m_UserInfo.Gold)
                {                 
                    CMessageBox.Create( DataMgr.Inst.GetLocal("You have not enough gold."), 1, null);
                    return;
                }
            }
            else if(DataMgr.Inst.m_DB_AuroraUnlockPrice[m_nLevel].strPayCode == "FriendPoint")
            {
                if( DataMgr.Inst.m_DB_AuroraUnlockPrice[m_nLevel].nPayValue > DataMgr.Inst.m_UserInfo.FriendPoint)
                {                 
                    CMessageBox.Create( DataMgr.Inst.GetLocal("Golden apple is not enough."), 1, null);
                    return;
                }
            }            

            m_BuyAurora_Popup.Begin(m_SprIcon, m_nLevel, AuroraOpen_Result);
        }
        else if (Rtn == 1)
        {
            DataMgr.Inst.m_AuroraSelect[m_nLevel].SelectPosIdx = a_nIndex;            
            
            for ( int i=0; i<m_SprIcon.Length; i++ )
            {                
                if (i == a_nIndex)
                    m_SprIcon[i].spriteName = DataMgr.Inst.m_DB_Aurora[m_nAuroraSkill[i]].strIconCode + "-2";
                else
                    m_SprIcon[i].spriteName = DataMgr.Inst.m_DB_Aurora[m_nAuroraSkill[i]].strIconCode + "-1";
            }
        }
            
    }

    // 버튼 관련. 
    // 툴팁 및 플스 선택.
    void onBtnPress_1() { BeginTooltip(0); }
    void onBtnPress_2() { BeginTooltip(1); }
    void onBtnPress_3() { BeginTooltip(2); }
    void onBtnPress_4() { BeginTooltip(3); }

    void onBtnRelease_1() { EndToolTiip(0); }
    void onBtnRelease_2() { EndToolTiip(1); }
    void onBtnRelease_3() { EndToolTiip(2); }
    void onBtnRelease_4() { EndToolTiip(3); }

    private int m_nToolTipKey = 0;
    private int m_Limit=0;
    void BeginTooltip( int a_Index )
    {                    
       m_Limit = System.Environment.TickCount+1000;            

       m_ToolTip.transform.FindChild("Name").GetComponent<UILabel>().text =
       DataMgr.Inst.m_DB_Aurora[m_nAuroraSkill[a_Index]].strName;

       if (DataMgr.Inst.m_DB_Aurora[m_nAuroraSkill[a_Index]].strToolTip.Contains("{0}"))
       {
           m_ToolTip.transform.FindChild("Content").GetComponent<UILabel>().text =
           string.Format(DataMgr.Inst.m_DB_Aurora[m_nAuroraSkill[a_Index]].strToolTip,
               DataMgr.Inst.m_DB_Aurora[m_nAuroraSkill[a_Index]].nEffectValue );
       }
       else
           m_ToolTip.transform.FindChild("Content").GetComponent<UILabel>().text =
               DataMgr.Inst.m_DB_Aurora[m_nAuroraSkill[a_Index]].strToolTip;

        m_ToolTip.SetActive(false);
        m_nToolTipKey = 1;        
    }

    void EndToolTiip(int a_Index)
    {
        if (m_nToolTipKey == 2)
        {
            m_nToolTipKey = 0;
            m_ToolTip.SetActive(false);
        }
        else 
        {
            m_nToolTipKey = 0;
            if (m_IsLoakIcon[a_Index]) SelectOrBuy(a_Index);            
        }
    }


}
