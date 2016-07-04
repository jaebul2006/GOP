﻿using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

//┌───────────────────────────────────────────────────┐.
//│제    목 : serverManager.
//│설    명 : 클라 메인. 
//│.
//│.
//└───────────────────────────────────────────────────┘.
public partial class serverManager : CNetClient_V2 
{
	// 네트웍 관련함수.
	//private int 			m_NetTimeLimit;	
	//	private string 		m_strConError = "";
	[System.NonSerialized] public  object          m_Lock = new object();
	private bool            m_IsLogin = false;        
    private int             m_ReTryCount;
	
	void Update()
	{
		lock(m_Lock)
		{                 
            // 연결실패.
            if( GetConnectState() == emConnectionState.Fail)
			{
                m_ReTryCount--;
                SetConnectState( emConnectionState.None );
                if(m_ReTryCount <=0)
                {
                    CMessageBox.Create( DataMgr.Inst.GetLocal("Failed to connect to server.\nWill you try again?"), 2, onConnect_Eror, CMessageBox.emMSG_MARK.Error);
                }
                else
                {
                    Connect(); 
                }
            }

			if( GetConnectState() == emConnectionState.Succ)
			{
                // 보내기.
                while( m_qSendData.Count > 0)
                {
                    CSendData Temp =  m_qSendData.Dequeue();
                    Send_Json(Temp.m_jsonMsg, Temp.m_SendProtocol);
                    Temp.m_TempRecvData.m_NetTimeLimit = Time.time+Temp.m_TempRecvData.m_NetTimeLimit;
                    
                    m_RecvData.Add(Temp.m_TempRecvData);
                    Temp.m_TempRecvData = null;                    
                    Temp = null;
                }

                //읽기.
                // tagPacketItam Item;
                // if ( ReadPacket(out Item))
                // {
                //       if(m_LoadingMark != null)
                //           m_LoadingMark.DownCount();
                // 
                // 	if( (TCP_PROTOCOL)Item.uiType != TCP_PROTOCOL.PT_PING) m_SendCount-=1;
                // 
                // 	ReadJSon( Encoding.Unicode.GetString(Item.byPacket, 0, Item.byPacket.Length), (TCP_PROTOCOL)Item.uiType);
                // 	
                // 	if(m_SendCount <= 0)
                // 	{
                // 		//DataMgr.Inst.LogConsol("RecvPacket 함수 종료");
                // 		return;
                // 	}
                // }
				CheckReadPacket();

                if (m_SendCount <= 0) return;

                // 테이터 안왔을때.
                if( m_RecvData.Count > 0 )
                { 
                    for(int i=0; i<m_RecvData.Count; i++)
                    {                                            
                        if( m_RecvData[i].m_NetTimeLimit < Time.time)
                        {
                            if(m_LoadingMark != null)        
                                m_LoadingMark.DownCount();

                            m_RecvData[i].m_MsgDelegate(false, null, null, ErrorCode.EC_TimeOver);                        
                            m_RecvData.Remove(m_RecvData[i]);
                            i--;
                        }
                    }
                }

                if( m_RecvData.Count <= 0 )
                    NetRelease();
			}
		}
	}

    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    // 읽기.
    protected override void Read_Timing( tagPacketItam a_PacketItam )
    {
        Debug.Log("Read_Timing");

        if( m_LoadingMark != null)
            m_LoadingMark.DownCount();

        if( (TCP_PROTOCOL)a_PacketItam.uiType != TCP_PROTOCOL.PT_PING) m_SendCount-=1;

        ReadJSon( Encoding.Unicode.GetString(a_PacketItam.byPacket, 
            0, a_PacketItam.byPacket.Length), (TCP_PROTOCOL)a_PacketItam.uiType);		
    }

    // 값이 오고있다는 신호를 보낸다.
    protected override void Packet_Reading( 
        int a_uiType, //받는중인 패킷 타입. 
        int a_uiRemainSize, //지금까지 받은 파일.
        int a_uiMaxSize ) //받아야할 파일 크기.
    {         
        if(a_uiType == (int)TCP_PROTOCOL.PT_JSON_SC_StaticDB_Select_SUCC )
        {
            if(m_Del_StdDB_DLBar != null) 
                m_Del_StdDB_DLBar(a_uiRemainSize, a_uiMaxSize);
        }
    }

	void OnApplicationQuit()
	{
        Debug.Log("HAN:OnApplicationQuit");		
        LogOut();		        
	}
	 
    /*
	void OnApplicationPause(bool a_PauseStatus)
	{
        Debug.Log("HAN:OnApplicationPause");
		if(a_PauseStatus == true)
		{
			if(m_IsLogin == true)
			{ 
				if(!m_NetClient.GetIsConnect()) if(!m_NetClient.Connect()) return;
//				DataMgr.Inst.SendUserData();
//				DataMgr.Inst.SendCardData();            
//				DataMgr.Inst.SendStageData();
//               DataMgr.Inst.SendMissionData();
//				DataMgr.Inst.m_SQLManager.UserDB_SendMyGetCardList();
				NetRelease();
			}
		}		
	}*/
	
	void OnDisable()
	{ 
		if(m_IsLogin == true)
		{ 
//			if(!m_NetClient.GetIsConnect()) if(!m_NetClient.Connect()) return;
//			DataMgr.Inst.SendUserData();
//			DataMgr.Inst.SendCardData();        
//			DataMgr.Inst.SendStageData();
//          DataMgr.Inst.SendMissionData();
//			DataMgr.Inst.m_SQLManager.UserDB_SendMyGetCardList();        
//			NetRelease();
		}
		
	}
	
	


	//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-.
	//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=.
	// SEND 함수들. 
	//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-.
	//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=.        

	public delegate void Delegate_Result(bool a_Result, Dictionary<string, object> a_dicJson, string a_Str, ErrorCode a_ErrorCode = ErrorCode.EC_END );
    private Deligate                   m_LoadingMark;


    class CSendData
    {
        public TCP_PROTOCOL  m_SendProtocol;        
        public string                   m_jsonMsg;        
        public CRecvData               m_TempRecvData;
    }
    class CRecvData
    {
        public TCP_PROTOCOL  m_Protocol;        
        public Delegate_Result          m_MsgDelegate;
        public float                    m_NetTimeLimit;
    }
    
    private Queue<CSendData>          m_qSendData = new Queue<CSendData>();        
    private List<CRecvData>           m_RecvData = new List<CRecvData>();
    //private Delegate_Result[]   m_MsgDelegate = new Delegate_Result[ (int)MyNetClass.TCP_PROTOCOL.PT_JSON_END ];    
    
    void SendFunc(string a_strJson, TCP_PROTOCOL a_SendMsg,
        TCP_PROTOCOL a_RecvMsg, Delegate_Result a_Delegate, float a_LimitTime = 5f)
    {   
     	lock(m_Lock)
		{	
            if( m_LoadingMark != null) m_LoadingMark.UpCount();
            else m_LoadingMark = Deligate.Create();
        
            CRecvData RDTemp    = new CRecvData();
            RDTemp.m_Protocol   = a_RecvMsg;
            RDTemp.m_NetTimeLimit = a_LimitTime;
            RDTemp.m_MsgDelegate= a_Delegate;


            CSendData SDTemp = new CSendData();
            SDTemp.m_SendProtocol   = a_SendMsg;
            SDTemp.m_jsonMsg        = a_strJson;
            SDTemp.m_TempRecvData   = RDTemp;
            m_ReTryCount = 2;

            // 연결.
            // 실패하면 재시도, 취소(게임종료).
            if(GetConnectState() != emConnectionState.Succ)
            {
                Connect();
                m_qSendData.Enqueue(SDTemp);
            }
            else
            {
                Send_Json(a_strJson, a_SendMsg);
                RDTemp.m_NetTimeLimit = Time.time+a_LimitTime;
                m_RecvData.Add(RDTemp);
                RDTemp = null;
                SDTemp = null;
            }
        }
    }
    
    void onConnect_Eror(bool value)
    {
        if(value)
        { 
            Connect(); 
        }
        else        
            Application.Quit();        
    }

	// 로그인 눌렀을때.			
	public void Login(string a_strID, string a_strPW, Delegate_Result a_Delegate)
	{
		lock(m_Lock)
		{	
		    Debug.Log("HAN : Login");
            m_IsLogin = false;	            			
            string strJson = "{ \"ID\":\""+a_strID+"\", \"UID\":\""+SystemInfo.deviceUniqueIdentifier+"\", \"PW\":\""+a_strPW+"\", \"HV\": \""+Application.version+"\"}";
            SendFunc(strJson, TCP_PROTOCOL.PT_JSON_CS_LOGIN,
                TCP_PROTOCOL.PT_JSON_SC_LOGIN_SUCC, a_Delegate);                              
		}
	}

    // 로그인 눌렀을때.			
	public void LogOut()
	{
		lock(m_Lock)
		{
            if(GetConnectState() != emConnectionState.Succ)
                Connect_Direct();

            string strJson = "{ \"ID\":\""+DataMgr.Inst.m_UserInfo.ID+"\","+ "\"Session\":\"" + DataMgr.Inst.m_UserInfo.Session + "\", \"UID\":\""+SystemInfo.deviceUniqueIdentifier+"\"}";
            Send_Json(strJson, TCP_PROTOCOL.PT_JSON_CS_LOGOUT);            
            NetRelease();
		}
	}	


	public void CreateMember(string a_strID, string a_strPW, string a_strNicName, Delegate_Result a_Delegate)
	{
        lock (m_Lock)
		{                        
			Debug.Log("HAN : CreateMember");			
            string strJson = "{ \"ID\":\""+a_strID+"\", \"UID\":\""+SystemInfo.deviceUniqueIdentifier+"\", \"PW\":\""+a_strPW+"\", \"NicName\":\""+a_strNicName+"\" }";
            SendFunc(strJson, TCP_PROTOCOL.PT_JSON_CS_CreateMember, TCP_PROTOCOL.PT_JSON_SC_CreateMember_SUCC, a_Delegate);
            
		}
    }

    
    private Defines.Delegate_Intint m_Del_StdDB_DLBar = null;
    public void StaticDB(Delegate_Result a_Delegate,  Defines.Delegate_Intint a_Del_StdDB_DLBar)
	{
        lock (m_Lock)
		{                        
            Debug.Log("HAN : StaticDB");                        
            m_Del_StdDB_DLBar = a_Del_StdDB_DLBar;
            string strJson = "{\"ID\":\"" + DataMgr.Inst.m_UserInfo.ID + "\","+"\"Session\":\"" + DataMgr.Inst.m_UserInfo.Session + "\"}";
            SendFunc(strJson, TCP_PROTOCOL.PT_JSON_CS_StaticDB_Select,
                TCP_PROTOCOL.PT_JSON_SC_StaticDB_Select_SUCC, a_Delegate, 25f);
		}
    }

    public void SellCard_One( int Idx, Delegate_Result a_Delegate)
    {
        lock (m_Lock)
        {                        
            Debug.Log("HAN : SellCard_One");            
            string strJson = "{ \"ID\" : \"" + DataMgr.Inst.m_UserInfo.ID + "\"," +
                  "\"Session\" : \"" + DataMgr.Inst.m_UserInfo.Session + "\"," +
                  "\"IDXS\" : [\"" + Idx + "\"]}";
            SendFunc(strJson, TCP_PROTOCOL.PT_JSON_CS_SellCard,
                TCP_PROTOCOL.PT_JSON_SC_SellCard_SUCC, a_Delegate);
        }
    }

    // 초월
    public void CardLevelEx(int KeyIDX, int MtrlIDX, Delegate_Result a_Delegate)
    {
        lock (m_Lock)
        {                        
            Debug.Log("HAN : CardLevelEx");            
            string strJson = "{\"ID\":\"" + DataMgr.Inst.m_UserInfo.ID + "\"," +
                "\"Session\":\"" + DataMgr.Inst.m_UserInfo.Session + "\"," +
                "\"Mtrl\":\"" + MtrlIDX + "\"," +
                "\"Key\":\"" + KeyIDX + "\"}";
    
            SendFunc(strJson, TCP_PROTOCOL.PT_JSON_CS_CardLevelMaxEx, 
                    TCP_PROTOCOL.PT_JSON_SC_CardLevelMaxEx_SUCC,
                a_Delegate);
        }
    }

    // 강화.
    public void CardUpgrade(int a_Key, int [] a_Mtrls, Delegate_Result a_Delegate)
    {
        lock (m_Lock)
        {            
            Debug.Log("HAN : CardUpgrade");            

            string Mtrls="";
            for (int i=0; i<a_Mtrls.Length; i++)
            {
                Mtrls += a_Mtrls[i];
                if(i < (a_Mtrls.Length-1) ) Mtrls += "\",\"";
            }

            string strJson = "{ \"ID\" : \"" + DataMgr.Inst.m_UserInfo.ID + "\"," +
                "\"Session\" : \"" + DataMgr.Inst.m_UserInfo.Session + "\"," +
                "\"Key\" : \"" + a_Key + "\"," + 
                "\"Mtrls\" : [\"" + Mtrls + "\"]}";

            SendFunc(strJson, TCP_PROTOCOL.PT_JSON_CS_CardUpgrade, TCP_PROTOCOL.PT_JSON_SC_CardUpgrade_SUCC, a_Delegate);
        }
    }

    // 합성.
    public void CardSynthesis(int a_Key1, int a_Key2, Delegate_Result a_Delegate)
    {
        lock (m_Lock)
        {            
            Debug.Log("HAN : CardSynthesis");            
            string strJson = "{ \"ID\" : \"" + DataMgr.Inst.m_UserInfo.ID + "\"," +
                "\"Session\" : \"" + DataMgr.Inst.m_UserInfo.Session + "\"," +
                "\"Key1\" : \"" + a_Key1 + "\"," +
                "\"Key2\" : \"" + a_Key2 + "\"}";

            SendFunc(strJson, TCP_PROTOCOL.PT_JSON_CS_Synthesis, 
                TCP_PROTOCOL.PT_JSON_SC_Synthesis_SUCC, a_Delegate);
        }
    }

    // 팀 업데이트.
    public void UpdateTeam(int[] a_Team1, int[] a_Team2, int[] a_Team3, Delegate_Result a_Delegate)
    {
        lock (m_Lock)
        {                     
            Debug.Log("HAN : UpdateTeam");            
            string Team1 = "", Team2 = "", Team3 = "";
            string Team="";
            if (a_Team1 != null)
            {
                for (int i = 0; i < a_Team1.Length; i++)
                {
                    Team1 += a_Team1[i];
                    if (i < (a_Team1.Length - 1)) Team1 += "_";
                }
                Team = "\"Team1\" : \"" + Team1 + "\",";
            }
            if(a_Team2 != null)
            {
                for (int i = 0; i < a_Team2.Length; i++)
                {
                    Team2 += a_Team2[i];
                    if (i < (a_Team2.Length - 1)) Team2 += "_";
                }
                Team += "\"Team2\" : \"" + Team2 + "\",";
            }
            if(a_Team3 != null)
            {
                for (int i = 0; i < a_Team3.Length; i++)
                {
                    Team3 += a_Team3[i];
                    if (i < (a_Team3.Length - 1)) Team3 += "_";
                }
                Team += "\"Team3\" : \"" + Team3 + "\"}";
            }

            string strJson = "{ \"ID\" : \"" + DataMgr.Inst.m_UserInfo.ID + "\"," +
                "\"Session\" : \"" + DataMgr.Inst.m_UserInfo.Session + "\"," + Team;

            SendFunc(strJson, TCP_PROTOCOL.PT_JSON_CS_UserInfoUpdate, 
                TCP_PROTOCOL.PT_JSON_SC_UserInfoUpdate_SUCC, a_Delegate);
        }
    }

    // 팀 업데이트.
    public void UpdateCrownADD(int a_CrownADD, Delegate_Result a_Delegate)
    {
        lock (m_Lock)
        {
            Debug.Log("HAN : UpdateCrownADD");
            string strJson = "{\"ID\":\"" + DataMgr.Inst.m_UserInfo.ID + "\"," +
                             "\"Session\":\"" + DataMgr.Inst.m_UserInfo.Session + "\"," +
                             "\"CrownADD\":" + a_CrownADD + "}";
        
            SendFunc(strJson, TCP_PROTOCOL.PT_JSON_CS_UserInfoUpdate, 
                TCP_PROTOCOL.PT_JSON_SC_UserInfoUpdate_SUCC, a_Delegate);
        }
    }

    public void ShopBuy( int a_IDX, string a_strCashPaymentNumber, Delegate_Result a_Delegate)
    {
        lock (m_Lock)
        {            
            Debug.Log("HAN : ShopBuy");            
            string strJson = "{ \"ID\" : \"" + DataMgr.Inst.m_UserInfo.ID + "\"," +
                     "\"Session\" : \"" + DataMgr.Inst.m_UserInfo.Session + "\",";

            if(a_strCashPaymentNumber != null)
                strJson += "\"CashPayNum\" : \"" + a_strCashPaymentNumber + "\",";

            strJson += "\"IDX\" : \"" + a_IDX + "\"}";

            SendFunc(strJson, TCP_PROTOCOL.PT_JSON_CS_SHOP_BUY,
                TCP_PROTOCOL.PT_JSON_SC_SHOP_BUY_SUCC, a_Delegate);
        }
    }

    public void AuroraOpen(int a_Level, Delegate_Result a_Delegate)
    {
        lock (m_Lock)
        {            
            Debug.Log("HAN : AuroraOpen");            
            string strJson = "{ \"ID\" : \"" + DataMgr.Inst.m_UserInfo.ID + "\"," +
                     "\"Session\" : \"" + DataMgr.Inst.m_UserInfo.Session + "\"," +
                     "\"Level\" : " + a_Level + "}";

            SendFunc(strJson, TCP_PROTOCOL.PT_JSON_CS_AURORA_OPEN, 
                TCP_PROTOCOL.PT_JSON_SC_AURORA_OPEN_SUCC, a_Delegate);
        }
    }

    public void AdventureEnd_Succ(int a_IDX, int a_ClearTime, int a_PuzzleGold, 
        int a_AttriAtk1, int a_AttriAtk2, int a_AttriAtk3, int a_AttriAtk4, 
        Delegate_Result a_Delegate)
    {
        lock (m_Lock)
        {            
            Debug.Log("HAN : AdventureEnd_Succ");
            string strJson = "{ \"ID\" : \"" + DataMgr.Inst.m_UserInfo.ID + "\"," +
                    "\"Session\" : \"" + DataMgr.Inst.m_UserInfo.Session + "\"," +
                    "\"Succ\" : " + 1 + "," +
                    "\"IDX\" : " + a_IDX + "," +
                    "\"Time\" : " + a_ClearTime + "," +
                    "\"PGold\" : " + a_PuzzleGold + "," +
                    "\"AttriAtk1\" : " + a_AttriAtk1 + "," +
                    "\"AttriAtk2\" : " + a_AttriAtk2 + "," +
                    "\"AttriAtk3\" : " + a_AttriAtk3 + "," +
                    "\"AttriAtk4\" : " + a_AttriAtk4 + "," +
                    "\"Team\" : " + DataMgr.Inst.m_TeamIdx + "}";

            SendFunc(strJson, TCP_PROTOCOL.PT_JSON_CS_AdventureEnd, 
                TCP_PROTOCOL.PT_JSON_SC_AdventureEnd_SUCC, a_Delegate);
        }
    }

    public void AdventureEnd_Failed(int a_IDX, Delegate_Result a_Delegate)
    {
        lock (m_Lock)
        {            
            Debug.Log("HAN : AdventureEnd_Failed");            
            string strJson = "{ \"ID\" : \"" + DataMgr.Inst.m_UserInfo.ID + "\"," +
                     "\"Session\" : \"" + DataMgr.Inst.m_UserInfo.Session + "\"," +
                     "\"Succ\" : " + 0 + "," +
                     "\"IDX\" : " + a_IDX + "}";

            SendFunc(strJson, TCP_PROTOCOL.PT_JSON_CS_AdventureEnd, 
                TCP_PROTOCOL.PT_JSON_SC_AdventureEnd_SUCC,a_Delegate);
        }
    }

    public void Select_NoticeList(Delegate_Result a_Delegate)
    {
        lock (m_Lock)
        {            
            Debug.Log("HAN : Select_NoticeList");
            string strJson = "{ \"ID\" : \"" + DataMgr.Inst.m_UserInfo.ID + "\"," +
                     "\"Session\" : \"" + DataMgr.Inst.m_UserInfo.Session + "\"}";

            SendFunc(strJson, TCP_PROTOCOL.PT_JSON_CS_GetNotice_SEL, 
                TCP_PROTOCOL.PT_JSON_SC_GetNotice_SEL_SUCC, a_Delegate);
        }
    }

    public void ADD_FriendPoint(int a_Point, Delegate_Result a_Delegate)
    {
        lock (m_Lock)
        {
            Debug.Log("HAN : ADD_FriendPoint");
            string strJson = "{ \"ID\" : \"" + DataMgr.Inst.m_UserInfo.ID + "\"," +
                "\"UID\" : \"" + SystemInfo.deviceUniqueIdentifier + "\"," +
                "\"Session\" : \"" + DataMgr.Inst.m_UserInfo.Session + "\"," +
                "\"FP\" : \"" + a_Point + "\"}";

            SendFunc(strJson, TCP_PROTOCOL.PT_JSON_CS_ADD_Moneys,
                TCP_PROTOCOL.PT_JSON_SC_ADD_Moneys_SUCC, a_Delegate);
        }
    }

    //-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-.
    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=.
    //-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-.
    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=.
    // 네트워크 값을 받는함수.	
    // json을 ReadPacket함수로부터 받아온다.
    void ReadJSon(string str, TCP_PROTOCOL Protocol)
	{        
        CRecvData RecvData = null;
        if( Protocol != TCP_PROTOCOL.PT_JSON_SC_FAILD )
        {
            foreach(CRecvData Element in m_RecvData)
            {
                if(Element.m_Protocol == Protocol)
                {
                    RecvData = Element;
                    m_RecvData.Remove(Element);
                    break;
                }
            }
        }
        
		switch (Protocol)
		{
			// 핑 체크 하기.
		    case TCP_PROTOCOL.PT_PING:
		    {                
			    Send_Ping();			    
		    }
			break;                        
			
			// 실패.
		    case TCP_PROTOCOL.PT_JSON_SC_FAILD:
		    {
                ReadJSonEND();
			    Dictionary<string, object> dicJson = Json.Deserialize(str) as Dictionary<string, object>;
			    object objErrorCode = (object)dicJson["ErrorCode"];
                int nPro = int.Parse(dicJson["Pro"].ToString());         
                
                foreach(CRecvData Element in m_RecvData)
                {                           
                    if( (int)Element.m_Protocol == nPro)
                    {
                        RecvData = Element;
                        m_RecvData.Remove(Element);
                        break;
                    }
                }                
           
                ErrorCode emErrorCode = (ErrorCode)int.Parse(objErrorCode.ToString());

                if(emErrorCode == ErrorCode.EC_DIFFER_DIV_CONNECT)
                {
                    CMessageBox.Create( DataMgr.Inst.GetLocal("Logged in to different device.\nGame will end."), 1, Btn_Error);
                    return;
                }          
                if(emErrorCode == ErrorCode.EC_SessionExpire)
                {
                    CMessageBox.Create( DataMgr.Inst.GetLocal("Session was Expiring"), 1, Btn_Error);
                    return;
                }
//                if(a_ErrorCode == MyNetClass.ErrorCode.EC_OVERLAPED_CONNECT)
//                {
//                    CMessageBox.Create( DataMgr.Inst.GetLocal("Duplicate log in attempt has been detected."),
//                    DataMgr.Inst.GetLocal("Alert"), 1, onCheckver_Eror);                
//                }

                Debug.Log("실패 메시지 받음/" + emErrorCode +"/" + (TCP_PROTOCOL)(nPro));
                if(RecvData != null)
                    RecvData.m_MsgDelegate( false, null, null, emErrorCode);                
		    }
			break;

            default:
            {                  
            	ReadJSonEND();	                
                Dictionary<string, object> DicJson;
                if(str == "" || str == null) DicJson = null;
                else DicJson = Json.Deserialize(str) as Dictionary<string, object>;
                
                if(RecvData != null)		    
			        RecvData.m_MsgDelegate( true, DicJson, str);
            }
            break;
		}
	}
	
	void ReadJSonEND()
	{ 
		// 연결끝는다.                
		if(m_SendCount > 0) return;

        lock(m_Lock)
		{			
		    NetRelease();
        }
	}

    void Btn_Error( bool a_Value )
    {
        Application.Quit();
    }

}