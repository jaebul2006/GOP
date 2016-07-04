using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Panel_UI_Aurora : MonoBehaviour
{
    public UILabel                  m_lbLevel; // 플레이어 레벨.
    public UILabel                  m_SkillMemual; // 스킬 설명 
    public GameObject               m_SlotParent;
    public Panel_BuyAurora_Popup    m_BuyAurora_Popup;

    private GameObject       m_SlotPrefabs;
    private List<AuroraSlot> m_SlotList = null;

    private GameObject m_RootLobby;
    void SetUp(GameObject a_Root)
    { m_RootLobby = a_Root; }


    // Use this for initialization.
    void Start(  )
    {
        m_SlotPrefabs = Resources.Load("Prefabs/AuroraSlot") as GameObject;
        m_lbLevel.text = "Lv:"+DataMgr.Inst.m_UserInfo.Level.ToString("00");

        if (m_SlotList == null)
        {            
            int nSlotHeight = m_SlotPrefabs.GetComponent<UISprite>().height;
            int i = 0;
            m_SlotList = new List<AuroraSlot>();
            foreach (KeyValuePair<int, DataMgr.tagAuroraSelect> obj in DataMgr.Inst.m_AuroraSelect)
            {
                //obj.Value.nIDX
                //카드 설정.
                GameObject Temp = (GameObject)Instantiate(m_SlotPrefabs);
                Temp.transform.parent = m_SlotParent.transform;
                Temp.gameObject.name = "AuroraSlot_" + i;
                Temp.transform.localPosition = new Vector3(0, -(i * nSlotHeight), 0);
                Temp.transform.localScale = Vector3.one;
                AuroraSlot SlotCom = Temp.GetComponent<AuroraSlot>();
                SlotCom.Begin(obj.Value.nLevel, m_BuyAurora_Popup, this);
                
                for(int j=0; j < obj.Value.m_Aurora.Count; j++ )
                {
                    int nIndex = -1;
                    switch (DataMgr.Inst.m_DB_Aurora[ obj.Value.m_Aurora[j] ].eEffect)
                    {                        
                        case DataMgr.emAurora_TYPE.HP: nIndex = 0; break;
                        case DataMgr.emAurora_TYPE.ATK: nIndex = 0; break;
                        case DataMgr.emAurora_TYPE.Attr_WA: nIndex = 0; break;
                        case DataMgr.emAurora_TYPE.Attr_SU: nIndex = 1; break;
                        case DataMgr.emAurora_TYPE.Attr_YUNG: nIndex = 2; break;
                        case DataMgr.emAurora_TYPE.Attr_ARM: nIndex = 3; break;
                    }
                    if ( nIndex != -1)
                        SlotCom.PushSkill(nIndex, DataMgr.Inst.m_DB_Aurora[obj.Value.m_Aurora[j]]);
                }
                m_SlotList.Add(SlotCom);
                i++;
            }

            m_SlotParent.GetComponent<UISprite>().height = (nSlotHeight * i);
            m_SlotParent.GetComponent<BoxCollider>().center = new Vector3(0, -((nSlotHeight * i) / 2.0f), 0);                
        }        

        SlotSetUp();
    }
	
    void SlotSetUp()
    {
        foreach (AuroraSlot obj_i in m_SlotList)
            obj_i.SetUpSkill();
    }

	// Update is called once per frame.
	void Update ()
    {
	
	}

    void OnEnable()
    {
        if (m_SlotList != null) SlotSetUp();
    }

    public void btn_back()
    {        
        DataMgr.Inst.SetBackPageState();
    }
}
