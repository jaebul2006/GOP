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

public class Panel_UI_Upgrade : MonoBehaviour
{	
	public UISprite     m_Title;            // 타이틀제목 강화/합성	. 
	public UILabel      m_lbPayGold;        // 소모골드.
	public UILabel      m_lbPercent;        // 표시해줄 확률.
	public GameObject   m_BtnExecute;       // 강화/합성 실행버튼.
    public UISprite     m_sprExecuteText;   // 버튼 테스트.
    public GameObject   m_Eff1;             // 1번자리 이펙트.
    public GameObject   m_Eff2;             // 2번자리 이펙트.
    public UILabel      m_FailedMsg;        // 강화 모니터.

    private GameObject              m_mainCard;
    private GameObject              m_subCard;
    private DataMgr.tagUserSummon   m_mainSummon;
    private DataMgr.tagUserSummon   m_subSummon;    
    private bool m_SubSlotLock;

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
        m_lbPayGold.text = "0";

        m_SubSlotLock = false;
        m_subCard = null;
        m_BtnExecute.GetComponent<Collider>().enabled = false;		
        m_lbPercent.text = "0%+(0%)";
        m_FailedMsg.alpha = 0f;
    }
		
    // 페이지 시작 및 메인카드 설정.
    public void Begin(DataMgr.tagUserSummon a_Obj )
    {        
        m_Title.spriteName = "Enchant_font";//"img_main_title_strengthen";
        m_sprExecuteText.spriteName = "Enchant_font";//"img_btn_strangthen_text";
        
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

        RefleshLebel();

        if (m_subCard) Destroy(m_subCard);
        m_subSummon = null;
        m_SubSlotLock = false;
    }

    TweenAlpha TA = null;
    void BeginFailedMsg()
    {             
        EndFailedMsg2();

        TA = m_FailedMsg.gameObject.AddComponent<TweenAlpha>();
        TA.from             = 0.0f;
        TA.to               = 1.0f;
        TA.duration         = 0.7f;
        TA.style            = UITweener.Style.Once;
		TA.eventReceiver 	= gameObject;
		TA.method 			= UITweener.Method.Linear;
        TA.callWhenFinished = "EndFailedMsg";    

    }
    void EndFailedMsg()
    {
        Invoke("EndFailedMsg2", 1f);
    }
    void EndFailedMsg2()
    {
        if(TA== null) return;
        m_FailedMsg.alpha = 0f;
        Destroy(TA);
        TA = null;
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
		m_subCard.transform.localPosition = new Vector3(205,260,0);
		m_subCard.transform.localScale = Vector3.one;
        m_subCard.SendMessage("setChar", a_CharData, SendMessageOptions.DontRequireReceiver);


        RefleshLebel();
    }

    // 강화 버튼을 누루면 실행된다(강화, 합성을 진행할지 여부를 묻는 팝업 출력.).
    void startPusion()
    {
        int nMyHabeValue = 0;

        string Error = "";
        if (DataMgr.Inst.m_DB_SummonPriceSeed[m_mainSummon.nGrade].strCode_Upgrade == "G")
        { nMyHabeValue = DataMgr.Inst.m_UserInfo.Gold; Error = DataMgr.Inst.GetLocal("your deficit Gold !!"); }

        if (DataMgr.Inst.m_DB_SummonPriceSeed[m_mainSummon.nGrade].strCode_Upgrade == "R")
        { nMyHabeValue = DataMgr.Inst.m_UserInfo.Ruby; Error = DataMgr.Inst.GetLocal("your deficit Ruby !!"); }

        if (DataMgr.Inst.m_DB_SummonPriceSeed[m_mainSummon.nGrade].strCode_Upgrade == "F")
        { nMyHabeValue = DataMgr.Inst.m_UserInfo.FriendPoint; Error = DataMgr.Inst.GetLocal("your deficit GoldApple !!"); }

        // 골드 부족.
        if (nMyHabeValue < DataMgr.Inst.m_DB_SummonPriceSeed[m_mainSummon.nGrade].GetUpgade(m_mainSummon))
        {
            CMessageBox.Create(Error, 1, btn_back2);
            return;
        }
                    
        CMessageBox.Create( DataMgr.Inst.GetLocal("Starting to Enchant."), 2, CollBack_Ok_ExeResult);
                
    }

    // 확인을 누룰때. 
    private int [] m_nMtrl = new int[1];
    public void CollBack_Ok_ExeResult(bool bOK)
    {
        if (m_mainCard == null || m_subCard == null) return;

        //m_TeamSlot
        if (bOK)
        {
            //이펙트를 나타내고 2초뒤에 결과를 출력한다.            
            m_nMtrl[0] = m_subSummon.nKey;
            Debug.Log("강화 메시지보냄");
            
            DataMgr.Inst.m_SerMgr.CardUpgrade(m_mainSummon.nKey, m_nMtrl, CollBack_Result);

            m_BtnExecute.GetComponent<Collider>().enabled = false;
            m_SubSlotLock = true;
        }
    }

    // 서버에서 결과 받음.
    private string [] m_strUpgradeResult_ArrRate;
    private string [] m_strUpgradeResult_ArrBonus;
    public void CollBack_Result(bool a_Result, Dictionary<string, object> a_dicJson, string a_Str, serverManager.ErrorCode a_ErrorCode)
    {
        if (a_Result)
        {
            m_Eff1.SetActive(true);
            m_Eff2.SetActive(true);
            Invoke("Collback_Effert", 1.0f);

            Debug.Log("강화 메시지받음");
            // OutCode : string, S.
            // OutValue : string, 결과 카드리스트.
            // PayCode : string, 지불 코드.
            // PayValue : string, 지불 제해진 금액.
            // "Rsl_R" : string  // 강화 성공순서.
            // "Rsl_B" : string  // 강화확률 보너스 순서.
            DataMgr.Inst.DataPasing(a_dicJson["OutCode"].ToString(), a_dicJson["OutValue"].ToString());

            if(a_dicJson["PayCode"].ToString() != "H")
                DataMgr.Inst.DataPasing(a_dicJson["PayCode"].ToString(), a_dicJson["PayValue"].ToString());

            m_strUpgradeResult_ArrRate = a_dicJson["Rsl_R"].ToString().Split('_');
            m_strUpgradeResult_ArrBonus = a_dicJson["Rsl_B"].ToString().Split('_');
        }
        else
            m_SubSlotLock = false;
    }

    //강화 및 합성 결과를 나타냄.
    void Collback_Effert()
    {                            
        Destroy(m_subCard);
        m_subSummon = null;
        DataMgr.Inst.GetPage(DataMgr.emMAIN_MODE.MYTEAM).SendMessage("createCardList", true, SendMessageOptions.DontRequireReceiver);
        m_mainSummon = DataMgr.Inst.m_UserSummonList[m_mainSummon.nKey];
        
        if (m_strUpgradeResult_ArrRate[0] == "1")
        {
            // 팝업창.
            GameObject ObjRtn = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/Panel_Char_Popup_Requital"));
            ObjRtn.transform.parent = m_RootLobby.transform;
            ObjRtn.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            ObjRtn.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            ObjRtn.GetComponent<Panel_Char_Popup_Requital>().Begin(m_mainSummon, Collback_PopupOk);

            m_mainCard.SendMessage("setChar", m_mainSummon, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            m_SubSlotLock = false;
            BeginFailedMsg();
        }
            

        RefleshLebel();

        m_Eff1.SetActive(false);
        m_Eff2.SetActive(false);                              

        if (m_mainSummon.nUpgrade >= DataMgr.Inst.m_DB_Summon[m_mainSummon.Data.nIDX].nMaxUpgrade) btn_back(); // 최대강화 레벨도달하면 뒤로가기.
    }

    public void Collback_PopupOk()
    {
        m_SubSlotLock = false;
    }


    // 지불 금액과. 지불 텍스트를 테스트한다.
    void RefleshLebel()
    {
        m_lbPayGold.text = DataMgr.Inst.m_DB_SummonPriceSeed[m_mainSummon.nGrade].GetUpgade(m_mainSummon).ToString();
        
        int Percent = 0;
        if (m_subSummon != null)
        {
            Percent = DataMgr.Inst.m_DB_UpgradeRate[Math.Abs(m_subSummon.nGrade - m_mainSummon.nGrade)].nRate;
        }

        float fMainRate = (Percent/1000.0f)*100.0f;
        float fAddRate = (m_mainSummon.nAddRate/ (float)DataMgr.Inst.m_SummonUpgradeMaxRate ) * 100.0f;

        m_lbPercent.text = fMainRate.ToString()+ "%" + "+(" + fAddRate.ToString() + "%)";
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
