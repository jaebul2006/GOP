using UnityEngine;
using System;
using System.Collections;
//using System.Data;
using System.Text;
//using Mono.Data.SqliteClient;
using System.Collections.Generic;
using System.IO;

public class TeamInfoScript : MonoBehaviour 
{    
    public GameObject [] m_TeamSlot;
    public GameObject [] m_SelTeamMark;	

    //public UIToggle[] Btns = new UIToggle[3];
    public UIToggle[] Btns;	    	
	
	bool changeReady = false;
	
	public GameObject bag;
	public GameObject change;
	
	public int use_team = 0;
	public int use_char = 0;

    private DataMgr.tagUserSummon m_varCharData;
    public DataMgr.tagUserSummon m_CharData
    {
        get
        {
            return m_varCharData;
        }
    }



    //팀 정보 초기화.
    //DataMgr.Inst.m_TeamIdx 변수를 통해 현재 선택된 팀 판단.
    void Start () 
	{
        if(Defines.DEF_MAX_TEAM_ITEM != m_TeamSlot.Length) DataMgr.Inst.LogError("m_TeamSlot가 정의된수와 다름니다.");
        if(Defines.DEF_MAX_TEAM_ITEM != m_SelTeamMark.Length) DataMgr.Inst.LogError("m_TeamSlot가 정의된수와 다름니다.");

        if (bag == null) use_char = 0;



        for (int i = 0; i < m_SelTeamMark.Length; i++)
            m_SelTeamMark[i].SetActive(false);


        for (int i = 0; i < Defines.DEF_MAX_TEAM_ITEM; i++)
        {
            if (DataMgr.Inst.GetTempTeam(DataMgr.Inst.m_TeamIdx,i) >= 0)
            {
                DataMgr.tagUserSummon myTeam = DataMgr.Inst.m_UserSummonList[DataMgr.Inst.GetTempTeam(DataMgr.Inst.m_TeamIdx,i)];
                m_TeamSlot[i].SendMessage("setChar", myTeam, SendMessageOptions.DontRequireReceiver);

                if (bag == null && DataMgr.Inst.m_TeamIdx == 0)
                {
                    m_SelTeamMark[i].SetActive(true);
                    use_char = 0;
                }
            }
            else m_TeamSlot[i].SendMessage("setEmpty", SendMessageOptions.DontRequireReceiver);
            
        }        

        if (change != null) change.SetActive(false);		
		CheckTap();
	}

	//팀 정보를 갱신한다.
	void UpdateTeam()
    {
        for (int i = 0; i < Defines.DEF_MAX_TEAM_ITEM; i++)
        {
            if (DataMgr.Inst.GetTempTeam(DataMgr.Inst.m_TeamIdx,i) >= 0)
            {
                DataMgr.tagUserSummon myTeam = DataMgr.Inst.m_UserSummonList[DataMgr.Inst.GetTempTeam(DataMgr.Inst.m_TeamIdx,i)];
                m_TeamSlot[i].SendMessage("setChar", myTeam, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                m_TeamSlot[i].SendMessage("setEmpty", SendMessageOptions.DontRequireReceiver);
            }
        }
    }	    
    

	// than level up, grade up, upgrade up.
	public void Reflash()
	{
		CheckTap();
        for (int i = 0; i< Defines.DEF_MAX_TEAM_ITEM; i++)
        {
            if (DataMgr.Inst.GetTempTeam(DataMgr.Inst.m_TeamIdx,i) >= 0)
                m_TeamSlot[i].SendMessage("setReflash", SendMessageOptions.DontRequireReceiver);
        }            
	}
	
	//선택된 팀의 탭을 활성화.
	void CheckTap()
	{		
		for(int i=0; i<Btns.Length; i++)
		{
			if(i == DataMgr.Inst.m_TeamIdx) Btns[i].startsActive = true;
			else Btns[i].startsActive = false;
		}
	}

    //현재캐릭터가 현재팀에 설정가능한 위치를 화살표로 나타낸다.
    public void viewSelectArrow(DataMgr.tagUserSummon data)
	{
        m_varCharData = data;
		changeReady = true;
		

        int b_have = -1;

        for (int i = 0; i< Defines.DEF_MAX_TEAM_ITEM; i++)
        {
            m_SelTeamMark[i].SetActive(false);

            if (DataMgr.Inst.GetTempTeam(DataMgr.Inst.m_TeamIdx, i) >= 0)
            {
                // 같은 캐릭터가 있음
                //if (data.nIdxSummon == DataMgr.Inst.m_UserSummonList[DataMgr.Inst.GetTempTeam(DataMgr.Inst.m_TeamIdx, i)].nIdxSummon)
                //    b_have = i;

                if (data.Data.strResurceID.Remove(0,1)  == DataMgr.Inst.m_UserSummonList[DataMgr.Inst.GetTempTeam(DataMgr.Inst.m_TeamIdx, i)].Data.strResurceID.Remove(0,1))
                    b_have = i;
            }
        }

             if(b_have ==-1) { for (int i=0; i<m_SelTeamMark.Length; i++)  m_SelTeamMark[i].SetActive(true); }
        else if(b_have == 0) { m_SelTeamMark[0].SetActive(true); }
        else if(b_have == 1) { m_SelTeamMark[1].SetActive(true); }
        else if(b_have == 2) { m_SelTeamMark[2].SetActive(true); }
        else if(b_have == 3) { m_SelTeamMark[3].SetActive(true); }
		
		change.SetActive(true);
	}

	//설정가능 상태 제거.
	void hideSelectArrow()
	{
		changeReady = false;
        for (int i = 0; i < m_SelTeamMark.Length; i++)
            m_SelTeamMark[i].SetActive(false);        
	}

	//팀해제시 팀에서 제거.
	public void popCard(DataMgr.tagUserSummon data)
	{
        for (int i = 0; i < Defines.DEF_MAX_TEAM_ITEM; i++)
        {
            if (DataMgr.Inst.GetTempTeam(DataMgr.Inst.m_TeamIdx,i) == data.nKey)
            {
                m_TeamSlot[i].SendMessage("setEmpty");
                DataMgr.Inst.SetTempTeam(DataMgr.Inst.m_TeamIdx, i, -1);
            }
        }

        if (bag != null) bag.SendMessage("createCardList", true, SendMessageOptions.DontRequireReceiver);
    }

    //팀설정시 캐릭터 변경 및 소환수 목록 업데이트.
    public void changeCard(GameObject obj)
	{
		if(changeReady)
		{
            for (int i = 0; i < Defines.DEF_MAX_TEAM_ITEM; i++)
            {
                if (obj == m_TeamSlot[i])
                {
                    if (!m_SelTeamMark[i].activeSelf) return;
                    DataMgr.Inst.SetTempTeam(DataMgr.Inst.m_TeamIdx, i, m_varCharData.nKey);
                }
            }
        
            obj.SendMessage("setChar", m_CharData, SendMessageOptions.DontRequireReceiver);
			hideSelectArrow();
            if (bag != null) bag.SendMessage("createCardList", true, SendMessageOptions.DontRequireReceiver);
            change.SetActive(false);
		}
        else
        {
			if(obj.GetComponent<TeamSlotScript>().m_CharData == null) return;
			
			if(bag != null) bag.SendMessage("cardClick", obj, SendMessageOptions.DontRequireReceiver);
		}
	}

	//팀배치 설정중 팀슬롯을 제외한 부분을 터치하면 팀배치 취소.
	void Cancel()
	{
		if(!changeReady) return;
		
		hideSelectArrow();
		change.SetActive(false);
		DataMgr.Inst.m_CurSelectCardNum = null;        
    }




    //각 팀 선택시 정보 갱신.
    public void SelectTeam1()
    {
		DataMgr.Inst.m_TeamIdx = 0;
		UpdateTeam ();
		if(bag != null) bag.SendMessage ("createCardList", true, SendMessageOptions.DontRequireReceiver);
		else CheckSelect(0);
	}

    public void SelectTeam2()
    {
		DataMgr.Inst.m_TeamIdx = 1;
		UpdateTeam ();
		if(bag != null) bag.SendMessage ("createCardList", true, SendMessageOptions.DontRequireReceiver);
		else CheckSelect(1);
	}

    public void SelectTeam3()
    {
		DataMgr.Inst.m_TeamIdx = 2;
		UpdateTeam ();
		if(bag != null) bag.SendMessage ("createCardList", true, SendMessageOptions.DontRequireReceiver);
		else CheckSelect(2);
	}

	//TV보스대전 입장시 선택된 캐릭터 화살표로 표시.
	void CheckSelect(int idx)
    {
		if(use_team != idx)
        {
            for(int i=0; i<m_SelTeamMark.Length; i++)
            {
                m_SelTeamMark[i].SetActive(false);
            }			
		}
        else
        {
			     if(use_char == 0) m_SelTeamMark[0].SetActive(true);
			else if(use_char == 1) m_SelTeamMark[1].SetActive(true);
            else if(use_char == 2) m_SelTeamMark[2].SetActive(true); 
            else if(use_char == 3) m_SelTeamMark[3].SetActive(true);
        }
	}

	//팀에 설정된 첫번째 캐릭터를 TV보스대전 입장 캐릭터로 설정.
	void InitSelectChar(GameObject obj)
    {		
	//	obj.GetComponent<NetViewManager>().SendSelectChar(use_team, use_char);
	}

	//TV보스대전 입장중 각 슬롯 선택시 TV로 현재 선택한 캐릭터를 전송.
	void SelectSlot1()
    {
		if(bag == null && DataMgr.Inst.GetTempTeam(DataMgr.Inst.m_TeamIdx, 0) == -1) return;
				
		use_team = DataMgr.Inst.m_TeamIdx;
		use_char = 0;

		for (int i = 0; i < m_SelTeamMark.Length; i++)
        {
            if(use_char == i) m_SelTeamMark[i].SetActive(true);
            else m_SelTeamMark[i].SetActive(false);
        }

        if (GameObject.Find("netViewObject0") != null)
        {
		//	GameObject.Find("netViewObject0").GetComponent<NetViewManager>().SendSelectChar(use_team, use_char);
		}
	}
	
	void SelectSlot2()
    {
		if(bag == null && DataMgr.Inst.GetTempTeam(DataMgr.Inst.m_TeamIdx, 1) == -1) return;
				
		use_team = DataMgr.Inst.m_TeamIdx;
		use_char = 1;
        for (int i = 0; i < m_SelTeamMark.Length; i++)
        {
            if (use_char == i) m_SelTeamMark[i].SetActive(true);
            else m_SelTeamMark[i].SetActive(false);
        }
        if (GameObject.Find("netViewObject0") != null)
        {
		//	GameObject.Find("netViewObject0").GetComponent<NetViewManager>().SendSelectChar(use_team, use_char);
		}
	}
	
	void SelectSlot3()
    {
		if(bag == null && DataMgr.Inst.GetTempTeam(DataMgr.Inst.m_TeamIdx, 2) == -1) return;
				
		use_team = DataMgr.Inst.m_TeamIdx;
		use_char = 2;
        for (int i = 0; i < m_SelTeamMark.Length; i++)
        {
            if (use_char == i) m_SelTeamMark[i].SetActive(true);
            else m_SelTeamMark[i].SetActive(false);
        }
        if (GameObject.Find("netViewObject0") != null)
        {
		//	GameObject.Find("netViewObject0").GetComponent<NetViewManager>().SendSelectChar(use_team, use_char);
		}
	}

    void SelectSlot4()
    {
        if (bag == null && DataMgr.Inst.GetTempTeam(DataMgr.Inst.m_TeamIdx, 3) == -1) return;

        use_team = DataMgr.Inst.m_TeamIdx;
        use_char = 3;
        for (int i = 0; i < m_SelTeamMark.Length; i++)
        {
            if (use_char == i) m_SelTeamMark[i].SetActive(true);
            else m_SelTeamMark[i].SetActive(false);
        }
        if (GameObject.Find("netViewObject0") != null)
        {
            //	GameObject.Find("netViewObject0").GetComponent<NetViewManager>().SendSelectChar(use_team, use_char);
        }
    }
}
