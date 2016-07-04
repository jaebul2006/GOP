using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Lobby : MonoBehaviour
{
    public AudioSource      m_BGM_MainUi;
    public UIPanel          m_TopUI;
    public UIPanel          m_EtcUI;
    public UIPanel          m_BtnUI;
    public GameObject       m_HelpGirl;
    public Panel_Load       m_Panel_Load;


#if UNITY_ANDROID    
    public AndroidBridge    m_Payment;
#elif UNITY_IOS
    public IOSPayment       m_Payment;    
#endif    

    public MyBall[]         m_MyBall;
    // 비공개변수들.
    private UILabel         m_LbLevel;
    private UIProgressBar   m_pgLevelExp;    

    void Awake()
    {
        DataMgr.Inst.m_TopObj = gameObject;
        m_BGM_MainUi.loop = true;
    }

    // Use this for initialization.
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        m_EtcUI.transform.FindChild("LbID").GetComponent<UILabel>().text = DataMgr.Inst.m_UserInfo.ID;

#if UNITY_ANDROID    
        m_Payment = GameObject.FindWithTag("AndroidBridge").GetComponent<AndroidBridge>();
#elif UNITY_IOS
        m_Payment =  GameObject.FindWithTag("IOSPayment").GetComponent<IOSPayment>();
#endif
        
        DataMgr.Inst.Setup_Lobby(this);
        DataMgr.Inst.m_UserInfo.RefleshTopUI();
        m_LbLevel = m_EtcUI.transform.FindChild("LbLevel").GetComponent<UILabel>();
        m_pgLevelExp = m_EtcUI.transform.FindChild("Exp").GetComponent<UIProgressBar>();
        RefleshLevelInfo();

        #if UNITY_ANDROID 
        DataMgr.Inst.m_TapJoyMgr.RefleshGoldApple();
        #endif        
    //    m_BGM_MainUi.Play();
        
        if(PlayerPrefs.GetInt("FirstLogin", -1) == -1)
        {
            DataMgr.Inst.SetPageState( DataMgr.emMAIN_MODE.TUTORIAL );
            PlayerPrefs.SetInt("FirstLogin", 1);
        }
    }
    
    
    public void MyBallReflesh()
    {
        int NotAttriCount=0;        
        for(int i=0; i<m_MyBall.Length; i++)
        {
            m_MyBall[i].SetUp(DataMgr.Inst.m_UserInfo.Team[DataMgr.Inst.m_TeamIdx][i], ref NotAttriCount);
        }
    }

    void OnEnable()
    {        
        RefleshLevelInfo();
        MyBallReflesh();
        m_BGM_MainUi.Play();                
    }    

    void OnDisable()
    {
        m_BGM_MainUi.Stop();
    }

    // Update is called once per frame
    void Update()
    {        
	    if(DataMgr.Inst.TestKeyInput(KeyCode.Keypad7))
        {
            DataMgr.Inst.m_SerMgr.ShopBuy(1001, "테스트", Buy_Result);
        }
        if(DataMgr.Inst.TestKeyInput(KeyCode.Keypad8))
        {
            DataMgr.Inst.TestPayRaiz("2.99");            
        }    
        if(DataMgr.Inst.TestKeyInput(KeyCode.Keypad9))
        {
            DataMgr.Inst.m_SerMgr.ADD_FriendPoint(100, GoldApple_Result);
        }

	}

     public void Buy_Result(bool a_Result, Dictionary<string, object> a_dicJson, string a_Str, serverManager.ErrorCode a_ErrorCode)
    {
        Debug.Log("구입 메시지");
        if(a_Result)
        {
            if(a_dicJson["PayCode"].ToString() != "H")
                DataMgr.Inst.DataPasing( a_dicJson["PayCode"].ToString(), a_dicJson["PayValue"].ToString());

            DataMgr.Inst.DataPasing( a_dicJson["OutCode"].ToString(), a_dicJson["OutValue"].ToString());            
            CMessageBox.Create( DataMgr.Inst.GetLocal("Purchase succeeded."), 1, onBuy_Eror);
        }
        else
        {
            CMessageBox.Create( DataMgr.Inst.GetLocal("Failed to Buy"), 1, onBuy_Eror);
        }
    }

    public void GoldApple_Result(bool a_Result, Dictionary<string, object> a_dicJson, string a_Str, serverManager.ErrorCode a_ErrorCode)
    {
        Debug.Log("구입 메시지");
        if(a_Result)
        {            
            DataMgr.Inst.DataPasing( a_dicJson["OutCode"].ToString(), a_dicJson["OutValue"].ToString());
            
            CMessageBox.Create("test Add GoldApple !", 1, onBuy_Eror);            
        }
        else
        {
            CMessageBox.Create( "test Add GoldApple !", 1, onBuy_Eror);
        }
    }

    public void onBuy_Eror(bool b) { }

    public void RefleshLevelInfo()
    {
        if(m_LbLevel == null) return;
        m_LbLevel.text = "Lv " + DataMgr.Inst.m_UserInfo.Level;        

        if(DataMgr.Inst.m_DB_PlayerExp[DataMgr.Inst.m_UserInfo.Level].nExpBar == 0 )
            m_pgLevelExp.value = 0f;
        else 
            m_pgLevelExp.value = (DataMgr.Inst.m_Accrue._AdventureClear + DataMgr.Inst.m_Accrue._AdventureFailed) / DataMgr.Inst.m_DB_PlayerExp[DataMgr.Inst.m_UserInfo.Level].nExpBar;
    }



    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    public void Call_Back()
    {
        DataMgr.Inst.SetPageState(DataMgr.emMAIN_MODE.MENU);
    }

    // 소환수 가방.
    public void call_SummonBag()
    {
        DataMgr.Inst.SetPageState(DataMgr.emMAIN_MODE.MYTEAM);
    }

    // 도감.
    public void call_Dic()
    {
    }

    // 상점.
    public void call_Shop()
    {
        DataMgr.Inst.SetPageState(DataMgr.emMAIN_MODE.SHOP);
    }

    // 옵션.
    public void call_Option()
    {
        DataMgr.Inst.SetPageState(DataMgr.emMAIN_MODE.OPTION);
    }

    //모험.
    public void call_Adventure()
    {
        DataMgr.Inst.SetPageState(DataMgr.emMAIN_MODE.SEL_STAGE);
    }

    // 미션.
    public void call_Mission()
    {

    }

    //친구.
    public void call_Friend()
    {

    }

    //Aurora.
    public void call_Aurora()
    {
        DataMgr.Inst.SetPageState(DataMgr.emMAIN_MODE.AURORA);
    }

    public void call_NoticeList()
    {
        DataMgr.Inst.m_SerMgr.Select_NoticeList(NoticeList_Result);
    }

    void NoticeList_Result(bool a_Result, Dictionary<string, object> a_dicJson,
        string a_Str, serverManager.ErrorCode a_ErrorCode)
    {
        if(!a_Result)
        {            
            CMessageBox.Create("Cannot connect to server", 1, null, CMessageBox.emMSG_MARK.Error);
            return;
        }            

        if( a_dicJson == null || a_dicJson.Count <= 0)
        {
            CMessageBox.Create("there is no more notification to deliver.", 1, null);
            return;
        }

        List<object> list = a_dicJson["List"] as List<object>;
        DataMgr.Inst.SetNoticeList(list);
        DataMgr.Inst.SetPageState( DataMgr.emMAIN_MODE.NOTIFY );
    }
}