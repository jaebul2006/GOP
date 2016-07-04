using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Panel_UI_SelStage : MonoBehaviour
{
    public UILabel              m_StageName;
    public UITexture []         m_CardIcon;    

    public SpringPanel          m_SpringPanel;    
    public UIGrid               m_Grid;
    public UIPopupList          m_Difficulty_PopupList;

    private GameObject          m_SlotPrefabs;
    private List<SelStageSlot>  m_SlotList = new List<SelStageSlot>();
    
    private int                 m_StageIndex = 0;
    private List<int>           m_liStageNum = null;    
    private float               m_PanelStartPosY;

    private GameObject m_RootLobby;
    void SetUp(GameObject a_Root)
    { m_RootLobby = a_Root; }
             
    void Awake()
    {
        m_SlotPrefabs = Resources.Load("Prefabs/SelStageSlot") as GameObject;
        m_liStageNum = new List<int>();
        m_PanelStartPosY = m_SpringPanel.transform.localPosition.y;

        // 스테이지 리스트 생성.
        bool bIs = false;
        foreach(KeyValuePair<int, DataMgr.tagSaticAdventureStage> obj in DataMgr.Inst.m_DB_AdventureStage)
        {
            bIs = false;
            foreach (int StageNum in m_liStageNum)
            {
                if (obj.Value.nStage == StageNum)
                   bIs = true;
            }

            if(bIs == false)
                m_liStageNum.Add(obj.Value.nStage);
        }

        m_liStageNum.Sort();


        //=-=-=--=-=-=--=-=-=--=-=-=--=-=-=--=-=-=--=-=-=--=-=-=-=-=.
        int nDifficulty = DataMgr.Inst.m_UserInfo.nAdven_difficulty;
        
        if(nDifficulty == 1)
        {
            m_Difficulty_PopupList.value = m_Difficulty_PopupList.items[0];            
        }
        if(nDifficulty == 2)
        {
            m_Difficulty_PopupList.value = m_Difficulty_PopupList.items[1];
        }
                             
        m_StageIndex = (DataMgr.Inst.m_UserInfo.nAdven_Stage-1);        
    }
	

    bool    m_bSlotList_Reflesh = false;
    int     m_nSlotList_Argu = 0;
    int     m_nSlotList_Stap = 0;

	//Update is called once per frame.
	void Update ()
    {        
	    if(m_bSlotList_Reflesh)
        {
            if(m_nSlotList_Stap == 0)
            {
                for(int i=0; i< m_Grid.transform.childCount; i++)
                {               
                    NGUITools.Destroy( m_Grid.transform.GetChild(i).gameObject );
                }
                for(int i=0; i< m_SlotList.Count; i++)
                {
                    if(m_SlotList[i].gameObject != null)
                    {
                        m_SlotList[i].transform.parent = null;
                        Destroy(m_SlotList[i].gameObject);               
                    }
                }

                m_Grid.transform.DetachChildren();

                Reposition();

                m_SlotList.Clear();
                m_nSlotList_Stap = 1;
            }
            else if(m_nSlotList_Stap == 1)
            {
                if(m_Grid.transform.childCount == 0)
                {
                    m_nSlotList_Stap = 2;
                }
            }
            else if(m_nSlotList_Stap == 2)
            {
                CreateStage1(m_nSlotList_Argu);
                m_bSlotList_Reflesh = false;
                m_nSlotList_Stap = 0;
            }
        }
	}

    private bool m_Begin = false;
    public void Begin()
    {                
        if (m_liStageNum == null) return;
        Debug.Log("스테이지로드:Begin");
        CreateStage(0);
        TeamReflash(  );        
    }

    public void OnDisable() 
    {
    }
    

    public void onBtnPopupList()
    {
        if(DataMgr.Inst.GetPageState() != DataMgr.emMAIN_MODE.SEL_STAGE ) return;
        if (m_liStageNum == null) return;
        Debug.Log("스테이지로드:onBtnPopupList");
        CreateStage(1);
        SendMessage("CreateStage", 1, SendMessageOptions.DontRequireReceiver);
    }

    void onBtnLeft()
    {
        m_StageIndex--;
        if (m_StageIndex < 0)
        { m_StageIndex = 0; }
        else CreateStage(2);            
    }

    void onBtnRight()
    {
        m_StageIndex++;
        if (m_StageIndex >= m_liStageNum.Count)
        { m_StageIndex = m_liStageNum.Count - 1; }
        else CreateStage(2);        
    }

    //팀 설정 페이지 들어가기.
    public void onBtnTeamSetting()
    {        
        DataMgr.Inst.SetPageState(DataMgr.emMAIN_MODE.MYTEAM, DataMgr.emMAIN_MODE.SEL_STAGE);
    }

    
    // 다음 스테이지 검색.
    // 0:페이지켯을때, 1:팝업페이지, 2:화살표.    
    void CreateStage( int a_Kind )
    {
        if(m_nSlotList_Stap != 0) return;
        
        m_nSlotList_Argu = a_Kind;
        m_bSlotList_Reflesh = true;                            
    }
    public void CreateStage1( int a_Kind )
    {   
        //마지막판을 깻으면 다음판 열어준다.
        bool    bIsLastDifficulty  = true;
        bool    bIsLastStage       = true;
        int     LastFloor           = 0;

        foreach ( KeyValuePair<int, DataMgr.tagSaticAdventureStage> Elimant in DataMgr.Inst.m_DB_AdventureStage )
        {
            if (Elimant.Value.nDifficulty == (DataMgr.Inst.m_UserInfo.nAdven_difficulty+1)) bIsLastDifficulty = false;
            
            if(DataMgr.Inst.m_UserInfo.nAdven_difficulty == Elimant.Value.nDifficulty)
            {
                if(Elimant.Value.nStage == (DataMgr.Inst.m_UserInfo.nAdven_Stage+1)) bIsLastStage = false;
                if(Elimant.Value.nStage == DataMgr.Inst.m_UserInfo.nAdven_Stage)
                {
                    if(LastFloor < Elimant.Value.nFloor)
                        LastFloor = Elimant.Value.nFloor;
                }
            }
        }
        
        int nDifficulty = DataMgr.Inst.m_UserInfo.nAdven_difficulty;
        int nStage      = DataMgr.Inst.m_UserInfo.nAdven_Stage;
        int nNextFloor  = DataMgr.Inst.m_UserInfo.nAdven_Floor;

        if(!bIsLastStage)
        {
            if(LastFloor > nNextFloor) nNextFloor++;
            else 
            {
                nStage++;
                nNextFloor = 1;
            }            
        }        
        else
        {
            if(bIsLastDifficulty)
            {
                if(LastFloor > nNextFloor) nNextFloor++;
            }
            else
            {
                if(LastFloor > nNextFloor) nNextFloor++;
                else
                {
                    nDifficulty++;
                    nStage = 1;
                    nNextFloor = 1;
                }
            }
        }
        
        int nHeight = m_SlotPrefabs.GetComponent<UISprite>().height;
                
        // 0:페이지켯을때, 1:팝업페이지, 2:화살표.
        int nSelect_Difficulty=1;
        if(a_Kind == 2 || a_Kind == 1)
        {
             if( m_Difficulty_PopupList.value == "Normal" ) nSelect_Difficulty = 1;
             else if( m_Difficulty_PopupList.value == "Hard" ) nSelect_Difficulty = 2;                            
             if(a_Kind == 1) m_StageIndex = (nStage-1);
        }
        else if(a_Kind == 0)
        {
            if (nDifficulty == 1) m_Difficulty_PopupList.value = "Normal";
            if (nDifficulty == 2) m_Difficulty_PopupList.value = "Hard";
            nSelect_Difficulty = nDifficulty;
            m_StageIndex = (nStage-1);
        }

        int aDebug=0;
        while(true)
        {
           Debug.Log("aDebug:" + aDebug);
            aDebug++;
            bool bWrite = false;
            foreach ( KeyValuePair<int, DataMgr.tagSaticAdventureStage> obj in DataMgr.Inst.m_DB_AdventureStage)
            {
                if( m_liStageNum[m_StageIndex] == obj.Value.nStage && 
                    nSelect_Difficulty == obj.Value.nDifficulty )
                {
                    m_StageName.text = obj.Value.strName;

                    //obj.Value.nIDX.
                    //카드 설정.
                    GameObject Temp = (GameObject)Instantiate(m_SlotPrefabs);
                    Temp.transform.parent = m_Grid.transform;
                    Temp.gameObject.name = "SelStageSlot_" + obj.Value.nFloor;
                    Temp.transform.localPosition = new Vector3(0, 0, 0);
                    Temp.transform.localScale = Vector3.one;
                    SelStageSlot SlotCom = Temp.GetComponent<SelStageSlot>();
                    SlotCom.Begin(obj.Value.nIDX, m_RootLobby, nDifficulty, nStage, nNextFloor);
                    m_SlotList.Add(SlotCom);                    
                    bWrite = true;
                }
            }

            if (bWrite == false)
            {
                m_StageIndex++;

                if (m_StageIndex >= m_liStageNum.Count)
                    m_StageIndex = m_liStageNum.Count - 1;

            }
            else break;
        }

        Invoke("Reposition", Time.fixedDeltaTime);
    }

    private Vector3 vGridPos = new Vector3();
    void Reposition()
    {
        int Index = DataMgr.Inst.m_UserInfo.nAdven_Floor-1;
        if(Index >= 9) Index = 0;

        if(Index > 6) Index = 6;
        vGridPos.y = (m_SlotPrefabs.GetComponent<UISprite>().height*Index)+m_PanelStartPosY;
        SpringPanel.Begin( m_SpringPanel.gameObject, vGridPos,  vGridPos.y);
        m_Grid.Reposition();        
    }

    void TeamReflash()
    {
        int Index=0;
        for(int i=0; i< DataMgr.Inst.m_UserInfo.Team[DataMgr.Inst.m_TeamIdx].Length; i++)
        {
            Index = DataMgr.Inst.m_UserInfo.Team[DataMgr.Inst.m_TeamIdx][i];

            if(Index != -1)
            { 
                m_CardIcon[i].mainTexture = Resources.Load("Textures/Char/icon_mon_" +
                    DataMgr.Inst.m_UserSummonList[Index].Data.strResurceID) as Texture;
            }
            else
            {
                m_CardIcon[i].mainTexture = null;
            }
        }
    }

    public void onbtnBack()
    {
        DataMgr.Inst.SetBackPageState();
    }
}
