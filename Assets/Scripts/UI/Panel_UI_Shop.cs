using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Panel_UI_Shop : MonoBehaviour
{
    public Transform    m_SlotParent;
    public UILabel      m_lbTebSommonRoulette;
    public UILabel      m_lbTebCrown;
    public UILabel      m_lbTebGold;
    public UILabel      m_lbTebRuby;
    public UISprite     m_SprTextTitle;
    public UIToggle[]   m_Btns = new UIToggle[4];
    public UISprite[]   m_Tabs = new UISprite[4];
    
    private DataMgr.emSHOP_TAB          m_TabCategory;
    private GameObject                  m_PrefabSlot;

    private GameObject m_RootLobby;
    void SetUp(GameObject a_Root)
    { 
        m_RootLobby = a_Root; 
    }

    // Use this for initialization
    void Start ()
    {
        m_PrefabSlot = Resources.Load("Prefabs/ShopSlot") as GameObject;
        m_TabCategory               = DataMgr.emSHOP_TAB.em_RUBY;
        m_lbTebSommonRoulette.text  = DataMgr.Inst.GetLocal("Hero gacha");
        m_lbTebCrown.text           = DataMgr.Inst.GetLocal("Crown");
        m_lbTebGold.text            = DataMgr.Inst.GetLocal("Gold");
        m_lbTebRuby.text            = DataMgr.Inst.GetLocal("Ruby");

        if (DataMgr.Inst.GetLanguage() == DataMgr.emLanguage.emLanguage_Korea)
        {
            GameObject Obj = (GameObject)Resources.Load("Atlas/Pre_UI_Labels");
            m_SprTextTitle.atlas = Obj.GetComponent<UIAtlas>();
        }
        else
        {
            GameObject Obj = (GameObject)Resources.Load("Atlas/Pre_UI_Labels_Eng");
            m_SprTextTitle.atlas = Obj.GetComponent<UIAtlas>();
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void LoadTap()
    {
        
        int BtnIndex = 0;
        for (int i = 0; i < m_Tabs.Length; i++)
        {
            if(m_Btns[i].GetComponent<UIToggle>().value)
            {
                if (i == 0) m_TabCategory = DataMgr.emSHOP_TAB.em_CARD;
                if (i == 1) m_TabCategory = DataMgr.emSHOP_TAB.em_CROWN;
                if (i == 2) m_TabCategory = DataMgr.emSHOP_TAB.em_GOLD;
                if (i == 3) m_TabCategory = DataMgr.emSHOP_TAB.em_RUBY;
                BtnIndex = i;
                break;
            }
        }

        if (m_TabCategory == DataMgr.emSHOP_TAB.em_CARD)
        {
            #if UNITY_EDITOR
            #elif UNITY_ANDROID
            #endif
        }

        // 모든 슬롯제거 제거.
        foreach (Transform Slot in m_SlotParent)
        {
            Destroy(Slot.gameObject);
        }

        foreach (KeyValuePair<int, DataMgr.tagSaticShop> Obj in DataMgr.Inst.m_DB_Shop)
        {
            if(Obj.Value.eCategory == m_TabCategory)
            { 
                GameObject slot = Instantiate(m_PrefabSlot) as GameObject;
                slot.transform.parent = m_SlotParent;
                slot.transform.localPosition = Vector3.zero;
                slot.transform.localScale = Vector3.one;
                slot.GetComponent<ShopSlot>().SetUp(Obj.Value, m_RootLobby);
            }
        }
        m_SlotParent.GetComponent<UIGrid>().repositionNow = true;

        // 리스트 최 상위로 위치 이동.
        m_SlotParent.GetComponent<SpringPanel>().target.y = 100;
        m_SlotParent.GetComponent<SpringPanel>().enabled = true;

        for (int i = 0; i < m_Tabs.Length; i++)
            m_Tabs[i].alpha = 0;

        m_Tabs[BtnIndex].alpha = 1;
    }
    
    public void btn_back()
    {
        DataMgr.Inst.SetBackPageState();
    }

    public void OnEnable()
    {
        //DataMgr.Inst.m_TapJoyMgr.Ready(TapJoyMgr.emOperWell.AppLaunch);
    }

    public void FreeCharge()
    {
#if UNITY_ANDROID
        DataMgr.Inst.m_TapJoyMgr.DirectOpen(TapJoyMgr.emOperWell.AppLaunch);            
        //DataMgr.Inst.m_TapJoyMgr.Open(TapJoyMgr.emOperWell.AppLaunch);
#endif
    }
}
