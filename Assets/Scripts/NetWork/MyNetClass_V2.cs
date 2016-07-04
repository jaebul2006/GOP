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
public partial class serverManager : CNetClient_V2 
{
    // 프로토콜 타입 .
	public enum TCP_PROTOCOL //: uint
	{			 
        PT_VERSION	= 0x10,
	    PT_PING,
	    PT_JSON_SC_FAILD,

	    PT_JSON_VIRSION_UPDATE,
	    PT_JSON_VIRSION_UPDATE_SUCC,
	    PT_JSON_VIRSION_SELECT,
	    PT_JSON_VIRSION_SELECT_SUCC,

	    PT_JSON_CS_TIME,

	    PT_JSON_SC_TIME_SUCC,
	    PT_JSON_CS_DISCONNECT,
	    PT_JSON_CS_LOGOUT,
	    PT_JSON_CS_LOGIN,
	    PT_JSON_SC_LOGIN_SUCC,

	    PT_JSON_CS_CreateMember,
	    PT_JSON_SC_CreateMember_SUCC,

	    PT_JSON_CS_AccrueDataCommit,
	    PT_JSON_SC_AccrueDataCommit_SUCC,

	    PT_JSON_CS_USER_SEARCH,
	    PT_JSON_SC_USER_SEARCH_SUCC,
	    
	    PT_JSON_CS_FRIEND_RECOMMEND_USER_LIST,
	    PT_JSON_SC_FRIEND_RECOMMEND_USER_LIST_SUCC,
	    PT_JSON_CS_FRIEND_NOTIFICATION, 
	    PT_JSON_SC_FRIEND_NOTIFICATION_SUCC,
	    PT_JSON_CS_FRIEND_AGREE_LIST,
	    PT_JSON_SC_FRIEND_AGREE_LIST_SUCC,
	    PT_JSON_CS_AGREE_FRIEND,
	    PT_JSON_SC_AGREE_FRIEND_SUCC,
	    PT_JSON_CS_FRIEND_LIST,
	    PT_JSON_SC_FRIEND_LIST_SUCC,
	    PT_JSON_CS_FRIEND_DELETE,
	    PT_JSON_SC_FRIEND_DELETE_SUCC,
	    PT_JSON_CS_FRIEND_SEND_PRECENT,
	    PT_JSON_SC_FRIEND_SEND_PRECENT_SUCC,

	    PT_JSON_CS_PARCEL_SEND,
	    PT_JSON_SC_PARCEL_SEND_SUCC,
	    PT_JSON_CS_PARCEL_RECV,
	    PT_JSON_SC_PARCEL_RECV_SUCC,
	    PT_JSON_CS_PARCEL_LIST,
	    PT_JSON_SC_PARCEL_LIST_SUCC,

	    PT_JSON_CS_SET_AURORA_ATTRI,
	    PT_JSON_SC_SET_AURORA_ATTRI_SUCC,

        PT_JSON_CS_AURORA_OPEN,        
        PT_JSON_SC_AURORA_OPEN_SUCC,

        PT_JSON_CS_GET_MISSION_INFO,
	    PT_JSON_SC_GET_MISSION_INFO_SUCC,
	    PT_JSON_CS_MISSION_COMPLETION,
	    PT_JSON_SC_MISSION_COMPLETION_SUCC,
	    PT_JSON_CS_MISSION_COUNT_UPDATE,
	    PT_JSON_SC_MISSION_COUNT_UPDATE_SUCC,

	    PT_JSON_CS_StaticDB_Select,
	    PT_JSON_SC_StaticDB_Select_SUCC,

        PT_JSON_CS_SHOP_BUY,
	    PT_JSON_SC_SHOP_BUY_SUCC,
        
        PT_JSON_CS_SellCard,
	    PT_JSON_SC_SellCard_SUCC,
        	
	    PT_JSON_CS_CardUpgrade,	
	    PT_JSON_SC_CardUpgrade_SUCC,
	    
	    PT_JSON_CS_CardLevelMaxEx,
	    PT_JSON_SC_CardLevelMaxEx_SUCC,

        PT_JSON_CS_Synthesis,
	    PT_JSON_SC_Synthesis_SUCC,

        PT_JSON_CS_UserInfoUpdate,
        PT_JSON_SC_UserInfoUpdate_SUCC,

        PT_JSON_CS_AdventureEnd,
        PT_JSON_SC_AdventureEnd_SUCC,
        	
	    PT_JSON_CS_GetNotice_SEL,
	    PT_JSON_SC_GetNotice_SEL_SUCC,	

        PT_JSON_CS_ADD_Moneys,	
	    PT_JSON_SC_ADD_Moneys_SUCC,

        PT_JSON_END
	};  
	
	public enum ErrorCode : int
	{
		EC_SUCCECE_NoError = 0,
		EC_FAILED_EXCEPTION,
		EC_JsonParsing,
		EC_ARGUMENT,
		EC_DB_ETC,
		EC_DB_EXCEPTION,
		EC_SV_EXCEPTION,
		EC_NoneUser,        
		EC_DB_ZeroList,
        EC_DeficitMoney,		// 돈이 부족하다.
		EC_DIFFER_DIV_CONNECT, // 다른기기에서 로그인.
		EC_SessionExpire, // 세션이만료되었다.

		// 로그인.		
		EC_LOGIN_PwNot,
		EC_LOGIN_HVersion,								

		EC_CreateMember_IsID,
		EC_CreateMember_IsNicName,										
		EC_CreateMember_ID_LangthZoro, // 아이디 길이0.
		EC_CreateMember_ID_LangthOver, // 아이디 길이초과	
		EC_CreateMember_ID_Special, // 아이디에 특수문자포함.
		

		EC_CreateMember_PW_LangthZoro, // 비밀번호 길이0.		
		EC_CreateMember_PW_LangthOver, // 비밀번호길이초과	


		// 유저 검색.		
		EC_USER_SEARCH_DB_TwoUser,	// 성공은 했으나 검색해보니 유저가 둘이다.				

		// 친구요청.		
		EC_FRIEND_NOTIFICATION_DB_OVERLAPED_NOTIFI,
		EC_FRIEND_NOTIFICATION_DB_CROSS_FRIEND,		// 상대방이 이미 친구요청을 했다.
		EC_FRIEND_NOTIFICATION_DB_ALREADY_FRIEND,	// 이미 친구다.		
		
		// 친구수락.
		EC_AGREE_FRIEND_DB_FIND_FAILED_FRIEND_NOTIFI,		// DB 요청을 찾을수가 없다.		

		//선물 보내기.
		EC_FRIEND_SEND_PRECENT_DB_NoneUser_SendTime, // 보낸시간 등록실패.
		EC_FRIEND_SEND_PRECENT_DB_NoneUser_Insert,   // 선물 등록실패.		
		
		// 택배 보내기.
		EC_PARCEL_SEND_DB_FiledInsert,		
		
		// 택배 받기
		EC_PARCEL_RECV_UserInfoNone,
		EC_PARCEL_RECV_ParcelNone,		

		// 오로라 속성 설정.
		EC_AURORA_SET_ATTRI_FailedInsert,								
						
		EC_MISSION_COMPLETION_NotKind,		// 없는 종류.
		EC_MISSION_COMPLETION_NotIDX,		// 없는 인덱스.
		EC_MISSION_COMPLETION_Alreay,		// 이미 완료됨.
				
		EC_SHOP_BUY_NotCode,
		EC_SHOP_BUY_FailedDB,		
										
		EC_Synthesis_NotMaxlevel,
		EC_Synthesis_NotMaxUpgrade,
		EC_Synthesis_DifferGrade,
			
		EC_AuroraOpen_AlreadyUse,		// 이미 사용가능한 오로라 스킬
		EC_AuroraOpen_AlreadyBought,	// 이미 구입한 스킬이다.
		EC_AuroraOpen_StrangeLevel,		// 구입할려는스킬이 현재레벨에서 +1 이 아니다.
		
		EC_END,

        EC_NotConnect,
        EC_TimeOver

    };
	public enum emConnectionState : int
	{
        None, Succ, Fail
    };
	private CStream m_Stream_Write;	
	private byte[] m_TempSendBuffer;
	private uint m_PacketOrder;
	private byte[] m_byNull; 
	
	public int m_SendCount = 0; // 데이터 전송횟수(send시 1증가, recv시 1차감, 0일때 client객체 release).	    

    private emConnectionState m_ConnectState = emConnectionState.None;
    private object m_ConnectStateLock = new object(); 
    public emConnectionState GetConnectState()
    {
        lock(m_ConnectStateLock) { return m_ConnectState; }        
    }
    public void SetConnectState( emConnectionState a_Value )
    {
        lock(m_ConnectStateLock) { m_ConnectState = a_Value; }    
    }

	void Awake()
    {
        if (Defines.DEF_PRODUCTION == true) //203.12.200.191
            SetUp("203.12.200.191", 4051, false);
        else // www.godsummoners.com / 127.0.0.1
            SetUp("www.godsummoners.com", 4051, true);      

        m_Stream_Write 		= new CStream();
		m_TempSendBuffer 	= new byte[m_CON_MAX_PACKET_LENGTH];
		m_PacketOrder 		= 0;
		m_byNull 			= new byte[2];
		m_byNull[0] 		= 0;
		m_byNull[1] 		= 0;
    }	

	public override void ConnectComplete(bool a_IsConnect) 
    {
        if(a_IsConnect)        
            SetConnectState(emConnectionState.Succ);        
        else        
            SetConnectState(emConnectionState.Fail);
    }

	//┌───────────────────────────────────────────────────┐.
	//│제    목 : Send_Json.
	//│설    명 : json형식을 보내는데 사용할 변수.
	//└───────────────────────────────────────────────────┘.        
	public void Send_Json(string str, TCP_PROTOCOL Protocol)
	{				
		if( TCP_PROTOCOL.PT_JSON_CS_DISCONNECT != Protocol && TCP_PROTOCOL.PT_JSON_CS_LOGOUT != Protocol )	    
			m_SendCount+=1;
		        		        
        byte[] Tempbuf = Encoding.Unicode.GetBytes(str);        
		int JsonLength = Tempbuf.Length;
        int nPacketSize = (m_CON_PacketHeaderSize + Tempbuf.Length + m_byNull.Length);

        if( nPacketSize > m_TempSendBuffer.Length)
        {
            m_TempSendBuffer = null;
            m_TempSendBuffer = new byte[nPacketSize];;
        }

		m_Stream_Write.SetWriteBuffer(m_TempSendBuffer);		
		m_Stream_Write.Write((uint)nPacketSize); // 전체길이 넣기. 
		m_Stream_Write.Write((uint)++m_PacketOrder); 		// 패킷순서. 
		m_Stream_Write.Write((uint)Protocol); 				// 타입.
		m_Stream_Write.Write(Tempbuf, JsonLength); 			// json 넣기.
		m_Stream_Write.Write( m_byNull, m_byNull.Length ); 	// json 넣기.                
		Send(m_TempSendBuffer, m_Stream_Write.GetSeek());
	}
	
	//┌───────────────────────────────────────────────────┐.
	//│제    목 : Send_Ping.
	//│설    명 : json형식을 보내는데 사용할 변수.
	//└───────────────────────────────────────────────────┘.
	public void Send_Ping()
	{
		m_Stream_Write.SetWriteBuffer(m_TempSendBuffer);
		m_Stream_Write.Write((uint)(m_CON_PacketHeaderSize)); // 전체길이 넣기.
		m_Stream_Write.Write((uint)++m_PacketOrder); // 패킷순서 .
		m_Stream_Write.Write((uint)TCP_PROTOCOL.PT_PING); // 타입.
        Send(m_TempSendBuffer, m_Stream_Write.GetSeek());
	}

    public void NetRelease()
    {
        m_SendCount = 0;
		Send_Json( "", TCP_PROTOCOL.PT_JSON_CS_DISCONNECT );
		CloseSocket();
        SetConnectState(emConnectionState.None);        
    }
}
