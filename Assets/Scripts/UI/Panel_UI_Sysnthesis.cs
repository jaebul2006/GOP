using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class Panel_UI_Sysnthesis : MonoBehaviour
{
	public UILabel      m_lbPayGold;        // 소모골드
	public UILabel      m_lbPercent;        // 표시해줄 확률		
	public GameObject   m_BtnExecute;       // 강화/합성 실행버튼     
    public GameObject   m_Eff1;             // 1번자리 이펙트
    public GameObject   m_Eff2;             // 2번자리 이펙트

    private GameObject              m_mainCard;
    private GameObject              m_subCard;
    private DataMgr.tagUserSummon   m_mainSummon;
    private DataMgr.tagUserSummon   m_subSummon;
    private int     m_useGold = 0;
    private int     m_usePer = 0;
    private bool    m_SubSlotLock;

    private GameObject m_RootLobby;
    void SetUp(GameObject a_Root)
    { m_RootLobby = a_Root; }

    void Awake()
	{

	}
	
	void Start()
	{
        m_Eff1.SetActive(false);
        m_Eff2.SetActive(false);        
        m_lbPayGold.text = "";

        m_SubSlotLock = false;
        m_subCard = null;
        m_BtnExecute.GetComponent<Collider>().enabled = false;
    }
		
    // 페이지 시작 및 메인카드 설정.
    public void Begin(DataMgr.tagUserSummon a_Obj )
    {                
        
        m_Eff1.SetActive(false);
        m_Eff2.SetActive(false);
        m_BtnExecute.GetComponent<Collider>().enabled = false;        

        // 카드 설정        
        m_mainSummon = a_Obj;
        m_mainCard = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/SummonSlot"));
        m_mainCard.transform.parent = transform;
        m_mainCard.gameObject.name = "SummonSlot";
        m_mainCard.transform.localPosition = new Vector3(-205, 260, 0);
        m_mainCard.transform.localScale = Vector3.one;
        m_mainCard.SendMessage("setChar", a_Obj, SendMessageOptions.DontRequireReceiver);                

        if (m_subCard) Destroy(m_subCard);
        m_subSummon = null;
        m_SubSlotLock = false;
    }

	// 재료카드 세팅.
	void setSubCard(DataMgr.tagUserSummon a_CharData)
	{
        if (m_SubSlotLock) return;
        if (m_subCard != null) Destroy(m_subCard);

        m_BtnExecute.GetComponent<Collider>().enabled = true;
        
        m_subSummon = a_CharData;
        m_subCard = (GameObject)Instantiate((GameObject)Resources.Load ("Prefabs/SummonSlot"));
		m_subCard.transform.parent = transform;
		m_subCard.gameObject.name = "SummonSlot";
		m_subCard.transform.localPosition = new Vector3(205, 260, 0);
		m_subCard.transform.localScale = Vector3.one;
        m_subCard.SendMessage("setChar", a_CharData, SendMessageOptions.DontRequireReceiver);
    }



    //합성 버튼을 누루면 실행된다(강화, 합성을 진행할지 여부를 묻는 팝업 출력.).
    public void startPusion()
    { 
        CMessageBox.Create( DataMgr.Inst.GetLocal("Synthesis begins."), 2, CollBack_Ok_ExeResult);
    }

    // 확인을 누룰때.     
    public void CollBack_Ok_ExeResult(bool bOK)
    {
        if (m_mainCard == null || m_subCard == null) return;

        //m_TeamSlot
        if (bOK)
        {            
            //이펙트를 나타내고 2초뒤에 결과를 출력한다.            
            
            Debug.Log("합성 메시지보냄");
            DataMgr.Inst.m_SerMgr.CardSynthesis(m_mainSummon.nKey, m_subSummon.nKey, CollBack_Result);
            m_BtnExecute.GetComponent<Collider>().enabled = false;
            m_SubSlotLock = true;        
        }
    }



    private int m_NewCardKey;
    public void CollBack_Result(bool a_Result, Dictionary<string, object> a_dicJson, string a_Str, serverManager.ErrorCode a_ErrorCode)
    {
        if (a_Result)
        {
            m_Eff1.SetActive(true);
            m_Eff2.SetActive(true);
            Invoke("Collback_Effert", 1.0f);

            Debug.Log("합성 메시지받음");
            //OutCode : string // S 카드지정
            //OutValue : string // 합성이 적용된 카드리스트  
            //S : string // 새로운 카드

            DataMgr.Inst.DataPasing( a_dicJson["OutCode"].ToString(), a_dicJson["OutValue"].ToString() );
            m_NewCardKey = int.Parse( a_dicJson["S"].ToString() );
        }
        else
            m_SubSlotLock = false;
    }

    //강화 및 합성 결과를 나타냄.
    void Collback_Effert()
    {
        m_Eff1.SetActive(false);
        m_Eff2.SetActive(false);
        m_SubSlotLock = false;

        Destroy(m_mainCard);
        Destroy(m_subCard);
        m_subSummon = null;
        m_mainSummon = null;

        DataMgr.Inst.GetPage(DataMgr.emMAIN_MODE.MYTEAM).SendMessage("createCardList", true, SendMessageOptions.DontRequireReceiver);        
        
        GameObject ObjRtn = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/Panel_Char_Popup_Requital"));
        ObjRtn.transform.parent = m_RootLobby.transform;
        ObjRtn.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        ObjRtn.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        ObjRtn.GetComponent<Panel_Char_Popup_Requital>().Begin(DataMgr.Inst.m_UserSummonList[m_NewCardKey], Collback_PopupOk);
    }

    public void Collback_PopupOk()
    {
        btn_back();
    }


	void btn_back()
	{
        Destroy(m_mainCard);
        Destroy(m_subCard);
        m_mainSummon = null;
        m_subSummon = null;
        DataMgr.Inst.SetPageState(DataMgr.emMAIN_MODE.MYTEAM);
        DataMgr.Inst.GetPage(DataMgr.emMAIN_MODE.MYTEAM).SendMessage("createCardList", true, SendMessageOptions.DontRequireReceiver);        
	}

    void btn_back2( bool b) {btn_back();}
}
