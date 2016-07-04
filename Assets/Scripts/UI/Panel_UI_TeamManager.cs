using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Panel_UI_TeamManager : MonoBehaviour 
{
    public UILabel      m_lb1Team;       // 1팀.
    public UILabel      m_lb2Team;       // 2팀.
    public UILabel      m_lb3Team;       // 3팀.
    public UILabel      m_lbCharicter;   // 캐릭터.
    public UILabel      m_lbSummon;      // 소환사.
    public UILabel      m_lbGradeBtn;    // 등급.
    public UILabel      m_lbLevelBtn;    // 레벨.
    public UILabel      m_lbAttriBtn;    // 속성.
    public UISprite     m_SprTitle;    

	public SpringPanel  springPanel;
	public UIScrollView scrollView;
	public UIGrid       list;

	string tapState = "Grade";
	float first_y = 0.0f;
//	float timer = 0.0f;	
	
	public TeamInfoScript   m_TeamInfoMgr;
	public UILabel          cardCount;
    public Panel_Sell_popup m_Panel_Sell_popup;
    public GameObject       m_BtnSlotAdd;

    GameObject InfoPopup;
	GameObject curPop = null;

    private GameObject m_RootLobby;
    void SetUp(GameObject a_Root)
    { m_RootLobby = a_Root; }

    void OnEnable()
    {
        if(DataMgr.Inst.m_UserInfo.SummonSlot >= 200) 
            m_BtnSlotAdd.SetActive(false);

        DataMgr.Inst.SetupTempTeam();
             if(DataMgr.Inst.m_TeamIdx == 0) m_TeamInfoMgr.SelectTeam1();
        else if(DataMgr.Inst.m_TeamIdx == 1) m_TeamInfoMgr.SelectTeam2();
        else if(DataMgr.Inst.m_TeamIdx == 2) m_TeamInfoMgr.SelectTeam3();
    }

    void Awake()
    {        
        first_y = springPanel.transform.localPosition.y;
		InfoPopup = Resources.Load ("Prefabs/Panel_Char_Popup") as GameObject;

        m_lb1Team.text      = "1"+DataMgr.Inst.GetLocal("Team");     
        m_lb2Team.text      = "2"+DataMgr.Inst.GetLocal("Team");     
        m_lb3Team.text      = "3"+DataMgr.Inst.GetLocal("Team");     
        m_lbCharicter.text  = DataMgr.Inst.GetLocal("Char");
        m_lbSummon.text     = DataMgr.Inst.GetLocal("Summon");
        m_lbGradeBtn.text   = DataMgr.Inst.GetLocal("Grade");
        m_lbLevelBtn.text   = DataMgr.Inst.GetLocal("Level");
        m_lbAttriBtn.text   = DataMgr.Inst.GetLocal("Attr");

        
        if (DataMgr.Inst.GetLanguage() == DataMgr.emLanguage.emLanguage_Korea)
        {
            GameObject Obj  = Resources.Load("Atlas/Pre_UI_Labels") as GameObject;
            m_SprTitle.atlas = Obj.GetComponent<UIAtlas>();
        }       
        else 
        {
            GameObject Obj = Resources.Load("Atlas/Pre_UI_Labels_Eng") as GameObject;
            m_SprTitle.atlas = Obj.GetComponent<UIAtlas>();
        }        

        m_Panel_Sell_popup.gameObject.SetActive(false);
	}    
	
	public void setList(string tap)
	{
		if(tapState == tap) return;

		tapState = tap;        
		createCardList(true);
	}
	
	// 리스트 생성  기존 리스트는 제거하고 새로운 리스트 생성.
	public void createCardList(bool b_repo)
	{
        // Debug.Log("gameState : "+BaseGame.STATE);
        m_TeamInfoMgr.Reflash(); // 팀갱신.
		foreach (Transform child in list.transform)
		{
			Destroy(child.gameObject);
		}

		//정렬을 위하여 데이터 변환.
		List<KeyValuePair<int, DataMgr.tagUserSummon>> temp = new List<KeyValuePair<int, DataMgr.tagUserSummon>>();

		foreach(KeyValuePair<int, DataMgr.tagUserSummon> KEY in DataMgr.Inst.m_UserSummonList)
		{
			if(DataMgr.Inst.GetPageState() == DataMgr.emMAIN_MODE.PUSION || 
			   DataMgr.Inst.GetPageState() == DataMgr.emMAIN_MODE.UPGRADE )
			{
				int UpPuCardIndex = DataMgr.Inst.m_CurSelectCardNum.nKey;
                
                     if( KEY.Value.nKey == UpPuCardIndex) continue;
				else if( KEY.Value.nKey == DataMgr.Inst.GetTempTeam(0, 0)) continue;
				else if( KEY.Value.nKey == DataMgr.Inst.GetTempTeam(0, 1)) continue;
				else if( KEY.Value.nKey == DataMgr.Inst.GetTempTeam(0, 2)) continue;
                else if( KEY.Value.nKey == DataMgr.Inst.GetTempTeam(0, 3)) continue;
                else if( KEY.Value.nKey == DataMgr.Inst.GetTempTeam(1, 0)) continue;
				else if( KEY.Value.nKey == DataMgr.Inst.GetTempTeam(1, 1)) continue;
				else if( KEY.Value.nKey == DataMgr.Inst.GetTempTeam(1, 2)) continue;
                else if( KEY.Value.nKey == DataMgr.Inst.GetTempTeam(1, 3)) continue;
                else if( KEY.Value.nKey == DataMgr.Inst.GetTempTeam(2, 0)) continue;
				else if( KEY.Value.nKey == DataMgr.Inst.GetTempTeam(2, 1)) continue;
				else if( KEY.Value.nKey == DataMgr.Inst.GetTempTeam(2, 2)) continue;
                else if( KEY.Value.nKey == DataMgr.Inst.GetTempTeam(2, 3)) continue;
            }
			
			if(DataMgr.Inst.GetPageState() == DataMgr.emMAIN_MODE.UPGRADE)
            {
				if( //KEY.Value.nUpgrade >= KEY.Value.Data.nMaxUpgrade || 
                    KEY.Value.nGrade > DataMgr.Inst.m_CurSelectCardNum.nGrade)
				{
					continue;
				}
			}
			
			if(DataMgr.Inst.GetPageState() == DataMgr.emMAIN_MODE.PUSION)
            { 
				if( KEY.Value.nUpgrade < KEY.Value.Data.nMaxUpgrade || 
                    KEY.Value.nLevel < KEY.Value.Data.nMaxLevel || 
                    KEY.Value.nGrade != DataMgr.Inst.m_CurSelectCardNum.nGrade || 
                    KEY.Value.nGrade >= 6)
				{
					continue;
				}
			}
            
            temp.Add(new KeyValuePair<int, DataMgr.tagUserSummon>( KEY.Key, KEY.Value ) );
		}

		//각 탭상태에 대한 정렬방식으로 데이터를 정렬한다. 
        if(DataMgr.Inst.GetPageState() == DataMgr.emMAIN_MODE.UPGRADE)
        {
            //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
            //start.
            for(int i=0; i<temp.Count-1; i++)
            {
				for(int j=i+1; j<temp.Count; j++)
                {
					if(temp[i].Value.nGrade < temp[j].Value.nGrade)
                    {
						KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
						line = temp[i];
						temp[i] = temp[j];
						temp[j] = line;
					}
                    else if(temp[i].Value.nGrade == temp[j].Value.nGrade)
                    {
						if(temp[i].Value.Data.eAttr > temp[j].Value.Data.eAttr)
                        {
							KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
							line = temp[i];
							temp[i] = temp[j];
							temp[j] = line;
						}
                        else if(temp[i].Value.Data.eAttr == temp[j].Value.Data.eAttr)
                        {
                            
                            if (temp[i].Value.nIdxSummon > temp[j].Value.nIdxSummon)
                            {
								KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
								line = temp[i];
								temp[i] = temp[j];
								temp[j] = line;
							}
                            else if(temp[i].Value.nIdxSummon == temp[j].Value.nIdxSummon)
                            {
								if(temp[i].Value.nLevel < temp[j].Value.nLevel)
                                {
									KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
									line = temp[i];
									temp[i] = temp[j];
									temp[j] = line;
								}
                                else if(temp[i].Value.nLevel == temp[j].Value.nLevel)
                                {
									if(temp[i].Value.nUpgrade < temp[j].Value.nUpgrade)
                                    {
										KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
										line = temp[i];
										temp[i] = temp[j];
										temp[j] = line;
									}
								}
							}
						}
					}
				}
			}
            //end.
            //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        }
		else if(tapState == "Grade")
        {
			for(int i=0; i<temp.Count-1; i++)
            {
				for(int j=i+1; j<temp.Count; j++)
                {
					if(temp[i].Value.nGrade > temp[j].Value.nGrade)
                    {
						KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
						line = temp[i];
						temp[i] = temp[j];
						temp[j] = line;
					}
                    else if(temp[i].Value.nGrade == temp[j].Value.nGrade)
                    {
						if(temp[i].Value.Data.eAttr < temp[j].Value.Data.eAttr)
                        {
							KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
							line = temp[i];
							temp[i] = temp[j];
							temp[j] = line;
						}
                        else if(temp[i].Value.Data.eAttr == temp[j].Value.Data.eAttr)
                        {
                            
                            if (temp[i].Value.nIdxSummon < temp[j].Value.nIdxSummon)
                            {
								KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
								line = temp[i];
								temp[i] = temp[j];
								temp[j] = line;
							}
                            else if(temp[i].Value.nIdxSummon == temp[j].Value.nIdxSummon)
                            {
								if(temp[i].Value.nLevel > temp[j].Value.nLevel)
                                {
									KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
									line = temp[i];
									temp[i] = temp[j];
									temp[j] = line;
								}
                                else if(temp[i].Value.nLevel == temp[j].Value.nLevel)
                                {
									if(temp[i].Value.nUpgrade > temp[j].Value.nUpgrade)
                                    {
										KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
										line = temp[i];
										temp[i] = temp[j];
										temp[j] = line;
									}
								}
							}
						}
					}
				}
			}
		}
        else if(tapState == "Attr")
        {
			for(int i=0; i<temp.Count-1; i++)
            {
				for(int j=i+1; j<temp.Count; j++)
                {
					if(temp[i].Value.Data.eAttr < temp[j].Value.Data.eAttr)
                    {
						KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
						line = temp[i];
						temp[i] = temp[j];
						temp[j] = line;
					}
                    else if(temp[i].Value.Data.eAttr == temp[j].Value.Data.eAttr)
                    {
						if(temp[i].Value.nGrade > temp[j].Value.nGrade)
                        {
							KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
							line = temp[i];
							temp[i] = temp[j];
							temp[j] = line;
						}
                        else if(temp[i].Value.nGrade == temp[j].Value.nGrade)
                        {
							if(temp[i].Value.nIdxSummon < temp[j].Value.nIdxSummon)
                            {
								KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
								line = temp[i];
								temp[i] = temp[j];
								temp[j] = line;
							}
                            else if(temp[i].Value.nIdxSummon == temp[j].Value.nIdxSummon)
                            {
								if(temp[i].Value.nLevel > temp[j].Value.nLevel)
                                {
									KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
									line = temp[i];
									temp[i] = temp[j];
									temp[j] = line;
								}
                                else if(temp[i].Value.nLevel == temp[j].Value.nLevel)
                                {
									if(temp[i].Value.nUpgrade > temp[j].Value.nUpgrade)
                                    {
										KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
										line = temp[i];
										temp[i] = temp[j];
										temp[j] = line;
									}
								}
							}
						}
					}
				}
			}
		}
        else if(tapState == "Lv")
        {
			for(int i=0; i<temp.Count-1; i++)
            {
				for(int j=i+1; j<temp.Count; j++)
                {
					if(temp[i].Value.nLevel > temp[j].Value.nLevel)
                    {
						KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
						line = temp[i];
						temp[i] = temp[j];
						temp[j] = line;
					}
                    else if(temp[i].Value.nLevel == temp[j].Value.nLevel)
                    {
						if(temp[i].Value.nGrade > temp[j].Value.nGrade)
                        {
							KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
							line = temp[i];
							temp[i] = temp[j];
							temp[j] = line;
						}
                        else if(temp[i].Value.nGrade == temp[j].Value.nGrade)
                        {
							if(temp[i].Value.Data.eAttr < temp[j].Value.Data.eAttr)
                            {
								KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
								line = temp[i];
								temp[i] = temp[j];
								temp[j] = line;
							}
                            else if(temp[i].Value.Data.eAttr == temp[j].Value.Data.eAttr)
                            {
								if(temp[i].Value.nIdxSummon < temp[j].Value.nIdxSummon)
                                {
									KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
									line = temp[i];
									temp[i] = temp[j];
									temp[j] = line;
								}
                                else if(temp[i].Value.nIdxSummon == temp[j].Value.nIdxSummon)
                                {
									if(temp[i].Value.nUpgrade > temp[j].Value.nUpgrade)
                                    {
										KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
										line    = temp[i];
										temp[i] = temp[j];
										temp[j] = line;
									}
								}
							}
						}
					}
				}
			}
		}

		//팀으로 설정되어있는 캐릭터들을 위쪽으로 정렬한다. 
		for(int i=0; i<Defines.DEF_MAX_TEAM; i++)
        {
			if(DataMgr.Inst.m_TeamIdx-1 == i) continue;

			int idx3 = temp.FindIndex(cs => cs.Value.nKey == DataMgr.Inst.GetTempTeam(i, 2) );
			if(idx3 > -1)
			{
				KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
				line = temp[idx3];
				temp.RemoveAt(idx3);
				temp.Add(line);
			}
			
			int idx2 = temp.FindIndex(cs => cs.Value.nKey == DataMgr.Inst.GetTempTeam(i, 1) );
			if(idx2 > -1)
			{   
				KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
				line = temp[idx2];
				temp.RemoveAt(idx2);
				temp.Add(line);
			}   
			
			int idx1 = temp.FindIndex(cs => cs.Value.nKey == DataMgr.Inst.GetTempTeam(i, 0));
			if(idx1 > -1)
			{
				KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
				line = temp[idx1];
				temp.RemoveAt(idx1);
				temp.Add(line);
			}
		}

		int c3_idx = temp.FindIndex(cs => cs.Value.nKey == DataMgr.Inst.GetTempTeam(DataMgr.Inst.m_TeamIdx, 2));
		if(c3_idx > -1)
		{
			KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
			line = temp[c3_idx];
			temp.RemoveAt(c3_idx);
			temp.Add(line);
		}
		
		int c2_idx = temp.FindIndex(cs => cs.Value.nKey == DataMgr.Inst.GetTempTeam(DataMgr.Inst.m_TeamIdx,1));
		if(c2_idx > -1)
		{
			KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
			line = temp[c2_idx];
			temp.RemoveAt(c2_idx);
			temp.Add(line);
		}
		
		int c1_idx = temp.FindIndex(cs => cs.Value.nKey == DataMgr.Inst.GetTempTeam(DataMgr.Inst.m_TeamIdx, 0));
		if(c1_idx > -1)
		{
			KeyValuePair<int, DataMgr.tagUserSummon> line = new KeyValuePair<int, DataMgr.tagUserSummon>();
			line = temp[c1_idx];
			temp.RemoveAt(c1_idx);
			temp.Add(line);
		}

        
        cardCount.text = DataMgr.Inst.m_UserSummonList.Count + "/" + DataMgr.Inst.m_UserInfo.SummonSlot;
		
		// 정렬된 정보로 카드 리스트 생성.
		for(int i = 0; i< temp.Count; i++)
		{
			string PREFAB = "SummonSlot";
			GameObject charObj = (GameObject)Instantiate((GameObject)Resources.Load ("Prefabs/"+PREFAB));
			charObj.transform.parent = list.transform;
			charObj.gameObject.name = PREFAB+(temp.Count-1-i).ToString("000");
			charObj.transform.localScale =  Vector3.one;
			
			charObj.SendMessage("setChar", temp[i].Value, SendMessageOptions.DontRequireReceiver);
			
			charObj.GetComponent<UIButtonMessage>().target = gameObject;
			charObj.GetComponent<UIButtonMessage>().functionName = "cardClick";

		}

		if(b_repo)
        {
			springPanel.target.y = first_y; // 리스트 최 상위로 위치 이동.
			springPanel.enabled = true;
			
			// 카드가 4개 이하인경우 드래그 불가 .
			if(temp.Count < 4)
			{
				scrollView.enabled = false;
			}
			else
			{
				scrollView.enabled = true;
			}
		}

		Invoke ("Reposition", Time.fixedDeltaTime);
	}

	void Reposition()
	{
		list.Reposition();// 리스트 정렬.
	}


    //카드 판매.    
    void sellCard(DataMgr.tagUserSummon a_SellCard)
	{
        //팀에서 제거한다.
        for (int i = 0; i < Defines.DEF_MAX_TEAM; i++)
        {
            for (int j = 0; j < Defines.DEF_MAX_TEAM; j++)
            {
                if (DataMgr.Inst.GetTempTeam(i, j) == a_SellCard.nKey)
                    DataMgr.Inst.SetTempTeam(i, j, -1);
            }
        }
        DataMgr.Inst.m_SerMgr.SellCard_One(a_SellCard.nKey, Recv_SellCard_Result);
    }


    public void Recv_SellCard_Result(bool a_Result, Dictionary<string, object> a_dicJson, 
        string a_Str, serverManager.ErrorCode a_ErrorCode)
    {
        if(a_Result)
        {            
            // 적용.
            if(a_dicJson["PayCode"].ToString() != "H")
                DataMgr.Inst.DataPasing(a_dicJson["PayCode"].ToString(), a_dicJson["PayValue"].ToString());

            DataMgr.Inst.DataPasing(a_dicJson["OutCode"].ToString(), a_dicJson["OutValue"].ToString());
            
            // 리스트 갱신.
            Destroy(curPop);
            createCardList(false);
        }
    }

	
	// 리스트 카드 클릭.
	void cardClick(GameObject obj)
	{
        if(m_TeamInfoMgr.GetComponent<TeamInfoScript>().change.activeSelf) return;
		
		// 팀설정 상태에서 설정가능한 자리선택시 현재 캐릭터를 해당위치에 팀으로 설정한다.
		if(DataMgr.Inst.GetPageState() == DataMgr.emMAIN_MODE.MYTEAM)
		{
			curPop = null; 			
			curPop = Instantiate(InfoPopup) as GameObject;
			curPop.transform.parent         = m_RootLobby.transform;
			curPop.transform.localScale     = Vector3.one;
			curPop.transform.localPosition  = Vector3.zero;
            TeamSlotScript TempCom = obj.GetComponent<TeamSlotScript>();
            curPop.GetComponent<Panel_Char_Popup>().BeginPopup(TempCom, m_TeamInfoMgr);
            //curPop.SendMessage("setChar", TempCom, SendMessageOptions.DontRequireReceiver);
		}
		
		// 강화, 합성시 재료 카드로 선택한다.
		else if(DataMgr.Inst.GetPageState() == DataMgr.emMAIN_MODE.UPGRADE)
		{
			DataMgr.tagUserSummon data = obj.GetComponent<TeamSlotScript>().m_CharData;
            DataMgr.Inst.GetPage(DataMgr.emMAIN_MODE.UPGRADE).SendMessage("setSubCard", data, SendMessageOptions.DontRequireReceiver);
        }
        else if(DataMgr.Inst.GetPageState() == DataMgr.emMAIN_MODE.PUSION)
        {
            DataMgr.tagUserSummon data = obj.GetComponent<TeamSlotScript>().m_CharData;            
            DataMgr.Inst.GetPage(DataMgr.emMAIN_MODE.PUSION).SendMessage("setSubCard", data, SendMessageOptions.DontRequireReceiver); ;
        }
        
		
		// 게임하기 팀편성 - 현재 설정된 팀카드 위에 화살표 표시.
		else if (DataMgr.Inst.GetPageState() == DataMgr.emMAIN_MODE.PLAY)
		{

		}
	}
    
    public void btn_back()
	{
        DataMgr.Inst.CommitTempTeam();
        DataMgr.Inst.SetBackPageState();        
    }

	void btn_lv()
	{
		setList ("Lv");
	}

	void btn_grade()
	{
		setList ("Grade");
	}

	void btn_attr()
	{
		setList ("Attr");
	}

	//소환수 슬롯 추가버튼 클릭시 처리.
	public void btn_add()
	{
        if (DataMgr.Inst.m_UserInfo.SummonSlot >= 200) // 나중에 추가해야한다.
        {			
			// 불가능 메시지 출력한다.
		}
        else
        {            
            
            CMessageBox.Create( ""+int.Parse(DataMgr.Inst.m_Shop_SummonSlot.strPayCount)+"Ruby\n"+
                DataMgr.Inst.m_Shop_SummonSlot_AddCount+DataMgr.Inst.GetLocal("Slot Will be expended."),
                2, onBtn_add);
		}
	}

    void onBtn_add(bool b)
    {

        if(DataMgr.Inst.m_Shop_SummonSlot.strPayCode == "Ruby")
        {
            if( int.Parse(DataMgr.Inst.m_Shop_SummonSlot.strPayCount) > DataMgr.Inst.m_UserInfo.Ruby)            
            {                
                CMessageBox.Create( DataMgr.Inst.GetLocal("You have not enough ruby."), 1, null);
                return;
            }
        }
        else if(DataMgr.Inst.m_Shop_SummonSlot.strPayCode == "Gold")
        {
            if( int.Parse(DataMgr.Inst.m_Shop_SummonSlot.strPayCount) > DataMgr.Inst.m_UserInfo.Gold)
            {                
                CMessageBox.Create( DataMgr.Inst.GetLocal("You have not enough gold."), 1, null);
                return;
            }
        }
        else if(DataMgr.Inst.m_Shop_SummonSlot.strPayCode == "FriendPoint")
        {
            if( int.Parse(DataMgr.Inst.m_Shop_SummonSlot.strPayCount) > DataMgr.Inst.m_UserInfo.FriendPoint)
            {                
                CMessageBox.Create( DataMgr.Inst.GetLocal("Golden apple is not enough."), 1, null);
                return;
            }
        }

        if(b)
        {
            // 구입 실행한다.
            DataMgr.Inst.m_SerMgr.ShopBuy( DataMgr.Inst.m_Shop_SummonSlot.nIDX, 
                null, InvenShop_Result);
        }        
    }

    void InvenShop_Result(bool a_Result, Dictionary<string, object> a_dicJson, string a_Str, serverManager.ErrorCode a_ErrorCode)
    {
        if(a_Result)
        {
            if(a_dicJson["PayCode"].ToString() != "H")
                DataMgr.Inst.DataPasing( a_dicJson["PayCode"].ToString(), a_dicJson["PayValue"].ToString());

            DataMgr.Inst.DataPasing( a_dicJson["OutCode"].ToString(), a_dicJson["OutValue"].ToString());
            updateSlotCnt();
            
            CMessageBox.Create( string.Format(DataMgr.Inst.GetLocal("Added {0} slots.") , DataMgr.Inst.m_Shop_SummonSlot_AddCount ), 1, null);

            if(DataMgr.Inst.m_UserInfo.SummonSlot >= 200) 
                m_BtnSlotAdd.SetActive(false);
        }        
        else if(a_ErrorCode == serverManager.ErrorCode.EC_DeficitMoney)
        {
            if(DataMgr.Inst.m_Shop_SummonSlot.strPayCode == "Ruby")                    
                CMessageBox.Create( DataMgr.Inst.GetLocal("You have not enough ruby."), 1, null);                                        
            else if(DataMgr.Inst.m_Shop_SummonSlot.strPayCode == "Gold")        
                CMessageBox.Create( DataMgr.Inst.GetLocal("You have not enough gold."), 1, null);                         
            else if(DataMgr.Inst.m_Shop_SummonSlot.strPayCode == "FriendPoint")                                                 
                CMessageBox.Create( DataMgr.Inst.GetLocal("Golden apple is not enough."), 1, null);    
        }
        else
        {
            CMessageBox.Create( DataMgr.Inst.GetLocal("Failed to request data."), 1, null);
        }
    }

	//현재 캐릭터 보유수 및 전체슬롯수 정보를 갱신한다.
	void updateSlotCnt()
    {        
        cardCount.text = DataMgr.Inst.m_UserSummonList.Count + "/" + DataMgr.Inst.m_UserInfo.SummonSlot;
    }
}
