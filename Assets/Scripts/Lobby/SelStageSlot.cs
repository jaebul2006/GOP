﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelStageSlot : MonoBehaviour
{
    public UILabel      m_lbTitle;
    public UISprite     m_SprCurtn;
    public GameObject   m_BtnEnter;
    public UILabel      m_lbDeductionKey;
    
    private int m_nStageIndex;
    private GameObject m_RootLobby;
    
	// Use this for initialization.
	void Start()
    {
	
	}
	
	// Update is called once per frame.
	void Update()
    {
	
	}

    public void onBtnStart()
    {   
        // 팀에 최소 2명이상 설정되어있나 확인.                        
        int cnt = 0;
        for (int i = 0; i < Defines.DEF_MAX_TEAM_ITEM; i++)        
            if(DataMgr.Inst.GetTempTeam(DataMgr.Inst.m_TeamIdx, i) >= 0) 
                cnt++;
        
        if (cnt <= 1)
        {
            CMessageBox.Create( DataMgr.Inst.GetLocal("Need at least 2 Summon placed to play."), 1, onBuy_Eror);
            return;
        }

        if( DataMgr.Inst.m_UserInfo.Crown < DataMgr.Inst.m_DB_AdventureStage[m_nStageIndex].nCrownUse)
        {
            CMessageBox.Create( DataMgr.Inst.GetLocal("Not enough Crown"), 1, onBuy_Eror);
            return;
        }

        // 소환수 인벤토리 꽉참.
        if ( DataMgr.Inst.m_UserInfo.SummonSlot <= DataMgr.Inst.m_UserSummonList.Count )
        {
            CMessageBox.Create( DataMgr.Inst.GetLocal("Hero slot is not enough but do Will you enter ?"), 2, IntoGame);
            return;
        }
        else IntoGame(true);        
    }

    void IntoGame( bool a_bIs )
    {
        if(!a_bIs) return;
        // 열쇠 있으면 실행.
        if (DataMgr.Inst.m_DB_AdventureStage[m_nStageIndex].nCrownUse <= DataMgr.Inst.m_UserInfo.Crown)
        {
            DataMgr.Inst.m_SelectStageIndex = m_nStageIndex;
            DataMgr.Inst.GetLodingOn();
            //Application.LoadLevel("GameScene");              
            Application.LoadLevelAsync("GameScene");
        }       
        else
        {
            CMessageBox.Create( DataMgr.Inst.GetLocal("Not enough Crown."), 1, onBuy_Eror);
        }
    }

    public void onBuy_Eror(bool b)
    {

    }

    public void Begin(int a_nStageIndex, GameObject a_RootLobby, 
        int a_nDifficulty, int a_nStage, int a_nNextFloor )
    {
        m_RootLobby     = a_RootLobby;
        m_nStageIndex   = a_nStageIndex;
        m_lbTitle.text  = DataMgr.Inst.m_DB_AdventureStage[m_nStageIndex].strName + " " +
            DataMgr.Inst.m_DB_AdventureStage[m_nStageIndex].nStage + "-" +
            DataMgr.Inst.m_DB_AdventureStage[m_nStageIndex].nFloor;
        
        m_lbDeductionKey.text = "-"+DataMgr.Inst.m_DB_AdventureStage[m_nStageIndex].nCrownUse.ToString();

        // 활성화.
        bool IsOn = false;
        // 스테이지 활성/비활성.
        if(DataMgr.Inst.m_DB_AdventureStage[m_nStageIndex].nDifficulty == a_nDifficulty)
        {
            if(DataMgr.Inst.m_DB_AdventureStage[m_nStageIndex].nStage == a_nStage)
            {
                if(DataMgr.Inst.m_DB_AdventureStage[m_nStageIndex].nFloor <= a_nNextFloor ) 
                    IsOn = true;
            }
            else if(DataMgr.Inst.m_DB_AdventureStage[m_nStageIndex].nStage < a_nStage) 
                IsOn = true;
        }                
        else if(DataMgr.Inst.m_DB_AdventureStage[m_nStageIndex].nDifficulty < a_nDifficulty)
        { 
            IsOn = true;
        }
        
        
        if (Defines.DEF_STAGE_ALL_UnLock)
            IsOn = true;        

        if(!IsOn)
        {
            m_BtnEnter.GetComponent<BoxCollider>().enabled = false;
            m_SprCurtn.enabled = true;
        }
        else
        {
            m_BtnEnter.GetComponent<BoxCollider>().enabled = true;
            m_SprCurtn.enabled = false;
        }
    }
}