using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour 
{
    private CNetClient  m_NetClient;
	
    //hhlee@solideng.co.kr
    // Use this for initialization.
	void Start()
    {
	    m_NetClient = new CNetClient("211.232.157.53", 3051, false);
        m_NetClient.Connect();
	}
	
	// Update is called once per frame.
	void Update()
    {
	
	}

    void ConnectComplet()
    {

    }


}
/*
[2016-2-25 20:8:50 ] CS :  --PT_JSON_CS_AdventureEnd--
:

{ "ID":"3", "Session":"1", "Succ":1, "IDX":1006, "Time":62, "PGold":162, 
"AttriAtk1":7, "AttriAtk2":0, "AttriAtk3":0, "AttriAtk4":0, "Team":0}

[2016-2-25 20:8:50 ] 데이터 크리티컬 설정 (3)
[2016-2-25 20:8:50 ] 데이터 크리티컬 해제 (3)
[2016-2-25 20:8:50 ] 데이터 크리티컬 설정 (3)
[2016-2-25 20:8:50 ] 데이터 크리티컬 해제 (3)
[2016-2-25 20:8:50 ] Exception ... : CMsSqlDB::GetItemCodeUpdate()
[2016-2-25 20:8:50 ] Exception : CMsSqlDB::AdventureEnd_Quray() 0000000005E1DA90
[2016-2-25 20:8:51 ] SC :  --PT_JSON_SC_FAILD--:{ Protocal:80, ErrorCode : (6) EC_SV_EXCEPTION }
[2016-2-25 20:8:51 ] DisconnectedUser : OnIoDisconnected() : (Index:104)
//*/