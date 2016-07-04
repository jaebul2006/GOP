using UnityEngine;
using System;
using System.Collections;
using System.Text;

using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using MiniJSON;


// 최대 강화, 초월확장수, 맥스확률수치 받아서 기능 넣기
public partial class DataMgr : MonoBehaviour
{
    #region Singleton
    private static DataMgr m_Instance;
    public static DataMgr Inst
    {
        get
        {
            if (null == m_Instance)
                m_Instance = FindObjectOfType(typeof(DataMgr)) as DataMgr;
            return m_Instance;
        }
    }
    #endregion

    // 자원.
    public Dictionary<string, Texture2D>    m_Res_Tex = new Dictionary<string, Texture2D>();
    public Dictionary<string, GameObject>   m_Res_Obj = new Dictionary<string, GameObject>();

    // 공개 객체들.
    public serverManager m_SerMgr;
    public int m_TeamIdx;
    public tagUserSummon m_CurSelectCardNum;

    //게임 안에서 로드할때 필요한 정보들.
    [System.NonSerialized]
    public int m_SelectStageIndex = -1;   //.    

    [System.NonSerialized]
    public Dictionary<int, tagAuroraSelect>             m_AuroraSelect; // Level , 정보.

    // 데이터 모음.
    public Dictionary<int, tagSaticSummon>              m_DB_Summon             = new Dictionary<int, tagSaticSummon>();
    public Dictionary<int, tagSaticMonster>             m_DB_Monster            = new Dictionary<int, tagSaticMonster>();
    public Dictionary<int, tagSaticSummonSkill>         m_DB_SummonSkill        = new Dictionary<int, tagSaticSummonSkill>();
    public Dictionary<int, tagSaticMonsterSkill>        m_DB_MonsterSkill       = new Dictionary<int, tagSaticMonsterSkill>();
    public Dictionary<int, tagSaticMissionDay>          m_DB_MissionDay         = new Dictionary<int, tagSaticMissionDay>();
    public Dictionary<int, tagSaticMissionWeek>         m_DB_MissionWeek        = new Dictionary<int, tagSaticMissionWeek>();
    public Dictionary<int, tagSaticMissionMonth>        m_DB_MissionMonth       = new Dictionary<int, tagSaticMissionMonth>();
    public Dictionary<int, tagSaticAchievements>        m_DB_Achievements       = new Dictionary<int, tagSaticAchievements>();
    public Dictionary<int, tagSaticAdventureStage>      m_DB_AdventureStage     = new Dictionary<int, tagSaticAdventureStage>();
    public Dictionary<int, tagSaticInfinity_Tower>      m_DB_InfinityTower      = new Dictionary<int, tagSaticInfinity_Tower>();
    public Dictionary<int, tagSaticShop>                m_DB_Shop               = new Dictionary<int, tagSaticShop>();
    public Dictionary<int, tagSaticExp>                 m_DB_Exp                = new Dictionary<int, tagSaticExp>();
    public Dictionary<int, tagSaticAuroraSkill>         m_DB_Aurora             = new Dictionary<int, tagSaticAuroraSkill>(); // 스킬 정보.
    public Dictionary<int, tagSaticAuroraUnlockPrice>   m_DB_AuroraUnlockPrice  = new Dictionary<int, tagSaticAuroraUnlockPrice>(); //언락하는데 들어가는 비용.
    public Dictionary<int, tagSaticPlayerExp>           m_DB_PlayerExp          = new Dictionary<int, tagSaticPlayerExp>();    
    public Dictionary<int, tagSaticCombined>            m_DB_Combined           = new Dictionary<int, tagSaticCombined>();
    public Dictionary<int, tagSummonPriceSeed>          m_DB_SummonPriceSeed    = new Dictionary<int, tagSummonPriceSeed>();
    public Dictionary<int, tagUpgradeRate>              m_DB_UpgradeRate        = new Dictionary<int, tagUpgradeRate>();

    public DataMgr.tagSaticShop m_Shop_SummonSlot;
    public int m_Shop_SummonSlot_AddCount;
    // 최대 강화, 초월확장수, 맥스확률수치.
    // 유저정보.
    public long m_CrownFillTime; // 왕관 채워주는속도.
    public int  m_SummonLevelEx;
    public int  m_SummonUpgradeMaxRate;
    public tagAccrue                                m_Accrue            = new tagAccrue();
    public tagUserInfo                              m_UserInfo          = new tagUserInfo();
    public Dictionary<int, tagUserSummon>           m_UserSummonList    = new Dictionary<int, tagUserSummon>();
    public Dictionary<int, tagUserMissionDay>       m_UserMissionDay    = new Dictionary<int, tagUserMissionDay>();
    public Dictionary<int, tagUserMissionWeek>      m_UserMissionWeek   = new Dictionary<int, tagUserMissionWeek>();
    public Dictionary<int, tagUserMissionMonth>     m_UserMissionMonth  = new Dictionary<int, tagUserMissionMonth>();
    public List<CNoticeData>                        m_NoticeList        = new List<CNoticeData>();
    

    //----------------------------------------------------
    //속성 상극 찾기.
    public float GetAtti_DamageCon( emCardAttribute a_Atti_Attack, emCardAttribute a_Atti_Damage)
    {
        float fCon = 0.0f;
        if( a_Atti_Attack == emCardAttribute.emCAB_WA)
        {
            if (a_Atti_Damage == emCardAttribute.emCAB_YUNG) fCon = 1.5f;//상극.
        //    else if(a_Atti_Damage == emCardAttribute.emCAB_SU) fCon = 0.85f;//역상극.
            else fCon = 1.0f;
        }
        else if( a_Atti_Attack == emCardAttribute.emCAB_SU)
        {
            if (a_Atti_Damage == emCardAttribute.emCAB_WA) fCon = 1.5f;//상극.
        //    else if(a_Atti_Damage == emCardAttribute.emCAB_ARM) fCon = 0.85f;//역상극.
            else fCon = 1.0f;
        }
        else if( a_Atti_Attack == emCardAttribute.emCAB_YUNG)
        {
            if (a_Atti_Damage == emCardAttribute.emCAB_ARM) fCon = 1.5f;//상극.
        //    else if(a_Atti_Damage == emCardAttribute.emCAB_WA) fCon = 0.85f;//역상극.
            else fCon = 1.0f;
        }
        else if( a_Atti_Attack == emCardAttribute.emCAB_ARM)
        {
            if (a_Atti_Damage == emCardAttribute.emCAB_SU) fCon = 1.5f;//상극.
        //    else if(a_Atti_Damage == emCardAttribute.emCAB_YUNG) fCon = 0.85f;//역상극.
            else fCon = 1.0f;
        }

        return fCon;
    }
    
    // 없는 속성 정렬.
    public class tagAttriCount
    {
        public DataMgr.emCardAttribute Attri;
        public int Count;
    };
    
    public List<DataMgr.tagAttriCount> m_liNotUseAttriOrder = new List<tagAttriCount>(); // 가정 적게쓰는 속성 정렬.
    // 팀이 갱생될때마다 호출된다.
    private void NotUseAttriOrder_Reflesh()
    {           
        if(m_liNotUseAttriOrder.Count == 0)
        { 
            // 없는 속성 찿아낸다.
            for (int i = 0; i<(int)DataMgr.emCardAttribute.emCAB_END; i++)
            {
                tagAttriCount ac = new tagAttriCount();
                ac.Attri = (DataMgr.emCardAttribute)i;
                ac.Count = 0;
                m_liNotUseAttriOrder.Add(ac);
            }
        }

        for (int i=0; i< Defines.DEF_MAX_TEAM_ITEM; i++)
        {
            if (DataMgr.Inst.m_UserInfo.Team[DataMgr.Inst.m_TeamIdx][i] == -1) continue;
            foreach (tagAttriCount Obj in m_liNotUseAttriOrder)
            {
                if(Obj.Attri == DataMgr.Inst.m_UserSummonList[DataMgr.Inst.m_UserInfo.Team[DataMgr.Inst.m_TeamIdx][i]].Data.eAttr)
                {
                    Obj.Count += 1;
                }
            }
        }

        //X => X.Count > X.Count
        m_liNotUseAttriOrder.Sort(delegate (tagAttriCount x, tagAttriCount y)
        {
            if (x == null || y == null) return 0;
            return x.Count.CompareTo(y.Count);                        
        });   
        
        if( m_Page[(int)(DataMgr.emMAIN_MODE.SEL_STAGE)] != null )
            m_Page[(int)(DataMgr.emMAIN_MODE.SEL_STAGE)].SendMessage("TeamReflash", m_Lobby.gameObject, SendMessageOptions.DontRequireReceiver);

        if (m_Lobby != null) m_Lobby.MyBallReflesh();
    }

    //----------------------------------------------------

    //로컬라이징.
    private Dictionary<string, string>      m_Localization = new Dictionary<string, string>();
    private emLanguage                      m_Language = emLanguage.emLanguage_English;

    private long m_lCast = 0L;
    public int CastInt( object a_obj )
    {
        m_lCast = (long)a_obj;
        return (int)m_lCast;
    }

    public void Log(string a_str)
    {        
    #if UNITY_EDITOR
        Debug.Log(a_str);
    #endif
    }
    
    public void LogError(string a_str)
    { 
    #if UNITY_EDITOR
		Debug.LogError(a_str);
    #endif
    }
    

    //로컬라이징에 관련된 코드=============================================.
    public void SetLanguage( emLanguage a_Language ) { m_Language = a_Language; }
    public emLanguage GetLanguage() { return m_Language; }

    public void LocalDB_Clear() { m_Localization.Clear(); }
    public void LocalDB_ADDFeild(string a_Key, string a_Value) { m_Localization.Add(a_Key, a_Value); }
    public string GetLocal( string a_Key )
    {
        return a_Key;//임시.
        
        if( m_Localization.ContainsKey(a_Key) )
        {
            return m_Localization[a_Key];
        }
        else
        {
            Log("GetLocal : is not key : " + a_Key);
            return a_Key;
        }
    } //end.


    //로그인 성공시에 호출. 
    public void LoginSetting(string a_ID, Dictionary<string, object> a_Data)
    {        
        m_TeamIdx = 0;
        m_CrownFillTime = 600;                        
        SetServerTime(a_Data["Time"].ToString());

        Dictionary<string, object> Accrue = a_Data["AccrueData"] as Dictionary<string, object>;                
        m_Accrue._Ruby              = Int64.Parse(Accrue["Ruby"].ToString());
        m_Accrue._Gold              = Int64.Parse(Accrue["Gold"].ToString());
        m_Accrue._FriendPoint       = int.Parse(Accrue["FriendPoint"].ToString());

        m_Accrue._MonsterKill       = int.Parse(Accrue["MonsterKill"].ToString());
        m_Accrue._SummonUpTry       = int.Parse(Accrue["SummonUpTry"].ToString());
        m_Accrue._SummonUpSucc      = int.Parse(Accrue["SummonUpSucc"].ToString());
        m_Accrue._GetSummon         = int.Parse(Accrue["GetSummon"].ToString());
        m_Accrue._CombinedExe       = int.Parse(Accrue["CombinedExe"].ToString());
        m_Accrue._TranscendExe      = int.Parse(Accrue["TranscendExe"].ToString());
        m_Accrue._Level30           = int.Parse(Accrue["Level30"].ToString());
        m_Accrue._SendFriendPoint   = int.Parse(Accrue["SendFriendPoint"].ToString());
        m_Accrue._AdventureClear    = int.Parse(Accrue["AdventureClear"].ToString());
        m_Accrue._AdventureFailed   = int.Parse(Accrue["AdventureFailed"].ToString());
        m_Accrue._SynthesisExe      = int.Parse(Accrue["SynthesisExe"].ToString());

        m_Accrue._AttriAtk1         = Int64.Parse(Accrue["AttriAtk1"].ToString());
        m_Accrue._AttriAtk2         = Int64.Parse(Accrue["AttriAtk2"].ToString());
        m_Accrue._AttriAtk3         = Int64.Parse(Accrue["AttriAtk3"].ToString());
        m_Accrue._AttriAtk4         = Int64.Parse(Accrue["AttriAtk4"].ToString());

        // m_Accrue.AdventureGrade = 

        // 유저정보.
        m_SummonLevelEx = int.Parse(a_Data["LevelEx"].ToString());
        m_SummonUpgradeMaxRate = int.Parse(a_Data["MaxRate"].ToString());

        Dictionary<string, object> member = a_Data["Member"] as Dictionary<string, object>;
        
        m_UserInfo.NicName      = member["NicName"].ToString();
        m_UserInfo.Level        = int.Parse(member["Level"].ToString());
        m_UserInfo.Gold         = int.Parse(member["Gold"].ToString());
        m_UserInfo.Ruby         = int.Parse(member["Ruby"].ToString());
        m_UserInfo.FriendPoint  = int.Parse(member["FriendPoint"].ToString());
        m_UserInfo.Crown        = int.Parse(member["Crown"].ToString());
        m_UserInfo.SummonSlot   = int.Parse(member["SummonSlot"].ToString());
        m_UserInfo.CrownSlot    = int.Parse(member["CrownSlot"].ToString());

        m_UserInfo.Team = new int[Defines.DEF_MAX_TEAM][];
        m_TempTeam = new int[Defines.DEF_MAX_TEAM][];
        for (int i = 0; i < m_UserInfo.Team.Length; i++)
        {
            m_UserInfo.Team[i] = new int[Defines.DEF_MAX_TEAM_ITEM];
            m_TempTeam[i] = new int[Defines.DEF_MAX_TEAM_ITEM];            
        }

        m_UserInfo.SetTeam(0, member["Team1"].ToString());
        m_UserInfo.SetTeam(1, member["Team2"].ToString());
        m_UserInfo.SetTeam(2, member["Team3"].ToString());        

        Dictionary<string, object> Aurora = a_Data["Aurora"] as Dictionary<string, object>;        
        m_UserInfo.Aurora_maxLevel = int.Parse(Aurora["maxLevel"].ToString());        

        // 소환사 정보.
        Dictionary<string, object> CardList = Json.Deserialize(member["CardList"].ToString()) as Dictionary<string, object>;
        
        tagUserSummon Item;
        foreach( KeyValuePair<string, object> obj in CardList )
        {
            Item = new tagUserSummon();
            Item.SetUp( int.Parse(obj.Key.ToString()), obj.Value.ToString() );
            m_UserSummonList.Add(Item.nKey, Item);
        }

        // 스테이지 정보.
        if(member["AdvenStage"].ToString() == "" || member["AdvenStage"] == null)
        {
            m_UserInfo.nAdven_difficulty = 1;
            m_UserInfo.nAdven_Stage = 1;    
            m_UserInfo.nAdven_Floor = 0;    
        }
        else
        {
            string [] AdvenStage = member["AdvenStage"].ToString().Split('_');
            if(AdvenStage.Length == 3)
            {
                m_UserInfo.nAdven_difficulty    = int.Parse(AdvenStage[0]);
                m_UserInfo.nAdven_Stage         = int.Parse(AdvenStage[1]);
                m_UserInfo.nAdven_Floor         = int.Parse(AdvenStage[2]);
            }        
        }

        // Team check
        // 임시로 m_TempTeam 사용한다.
        bool bModify = false;
        
        for(int i=0; i<Defines.DEF_MAX_TEAM; i++)
        {
            for (int j = 0; j < Defines.DEF_MAX_TEAM_ITEM; j++)
            {
                if( m_UserInfo.Team[i][j] >= 0 )
                {
                    bool bIsKey = false;
                    foreach (KeyValuePair<int, tagUserSummon> Obj in m_UserSummonList)
                    {
                        if (m_UserInfo.Team[i][j] == Obj.Value.nKey)
                        {
                            bIsKey = true;
                            break;
                        }
                    }
                    if(!bIsKey)
                    {
                        bModify = true;
                        m_UserInfo.Team[i][j] = -1;
                    }
                        
                }
            }
        }        
        
        NotUseAttriOrder_Reflesh();

        if(bModify)
        {
            SetupTempTeam();
            m_IsTeamModify = true;
            CommitTempTeam();
        }
    }

	// Use this for initialization.
	void Start ()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(OneSecTime());
#if UNITY_ANDROID
        m_TapJoyMgr = GameObject.Find("Tapjoy").GetComponent<TapJoyMgr>();
#endif
    }
    
    // Update is called once per frame.
    void Update()
    {

    }

    //--------------------------------------------------------------------------------------------------
    // 팀 관리에 관한 함수들 
    // 팀에 관련된 변수. Temp 함수에 막 복사 해 놓다가 나중에 한번에 커및한다.
    private bool m_IsTeamModify = false;
    private int[][] m_TempTeam;
    public void SetupTempTeam()
    {
        m_IsTeamModify = false;
        
        for (int i = 0; i < m_TempTeam.Length; i++)
        {
            for (int j = 0; j < m_TempTeam[i].Length; j++)
            {
                m_TempTeam[i][j] = DataMgr.Inst.m_UserInfo.Team[i][j];
            }
        }
    }    

    public int GetTempTeam(int i, int j) { return m_TempTeam[i][j]; }
    public void SetTempTeam(int i, int j, int value)
    {
        m_IsTeamModify = true;
        m_TempTeam[i][j] = value;        
    }


    private Defines.Delegate_Bool m_CommitTeamAlert;
    public void CommitTempTeam(Defines.Delegate_Bool a_CollBack = null)
    {
        m_CommitTeamAlert = a_CollBack;
        if(m_IsTeamModify)                    
            m_SerMgr.UpdateTeam(m_TempTeam[0], m_TempTeam[1], m_TempTeam[2], UpdateTeam_Result);                    
        else
        {
            if(a_CollBack != null) a_CollBack(true);
            NotUseAttriOrder_Reflesh();
        }            
    }

    public void UpdateTeam_Result(bool a_Result, Dictionary<string, object> a_dicJson, string a_Str, serverManager.ErrorCode a_ErrorCode)
    {
        if (a_Result)
        {
            for (int i = 0; i < m_TempTeam.Length; i++)
            {
                for (int j = 0; j < m_TempTeam[i].Length; j++)
                {
                    m_UserInfo.Team[i][j] = m_TempTeam[i][j];
                }
            }
            if(m_CommitTeamAlert != null) m_CommitTeamAlert(true);

            NotUseAttriOrder_Reflesh();
        }
        else
        {
            if(m_CommitTeamAlert != null) m_CommitTeamAlert(false);
        }
        
    }



    //----------------------------------------------------------------------
    // 데이터 적용 
    public void DataPasing( string a_Code, string a_Value)
    {
           
        switch(a_Code)
        {
            case "J": // 조합식.
                
            break;

            case "D": // 캐릭터.
            case "S": 
                SetCardList(a_Value);
            break;

            case "G": m_UserInfo.Gold = int.Parse(a_Value); break; // 골드.
            case "R": m_UserInfo.Ruby = int.Parse(a_Value); break; // 루비.
            case "C": m_UserInfo.Crown = int.Parse(a_Value); break; // 왕관.
            case "F": m_UserInfo.FriendPoint = int.Parse(a_Value); break; // 친구포인트.
            case "H": break;// 캐쉬.

            case "K": m_UserInfo.CrownSlot = int.Parse(a_Value); m_CrownRecvLastTime=0; break;// 왕관 확장.
            case "P": m_UserInfo.SummonSlot = int.Parse(a_Value); break;// 소환사 확장.
        }
    }

    public void SetCardList(string a_Value)
    {
        // 소환사 정보.
        m_UserSummonList.Clear();
        Dictionary<string, object> CardList = Json.Deserialize(a_Value) as Dictionary<string, object>;

        tagUserSummon Item;
        foreach (KeyValuePair<string, object> obj in CardList)
        {
            Item = new tagUserSummon();
            Item.SetUp(int.Parse(obj.Key.ToString()), obj.Value.ToString());
            m_UserSummonList.Add(Item.nKey, Item);
        }
    }


    public float GetAurora_Hp()
    {
        float fRtn = 0.0f;
            
        foreach( KeyValuePair<int, tagAuroraSelect>  i_obj in m_AuroraSelect )
        {            
            if(DataMgr.Inst.IsBuyAurora(i_obj.Value.nLevel) != 1) continue;
            if( m_DB_Aurora[i_obj.Value.SelectAuroraIdx].eEffect == emAurora_TYPE.HP)
            {
                fRtn += m_DB_Aurora[i_obj.Value.SelectAuroraIdx].nEffectValue;
            }
        }
        return fRtn;
    }

    public float GetAurora_Atk()
    {
        float fRtn = 0.0f;

        foreach( KeyValuePair<int, tagAuroraSelect>  i_obj in m_AuroraSelect )
        {
            if(DataMgr.Inst.IsBuyAurora(i_obj.Value.nLevel) != 1) continue;

            if( m_DB_Aurora[i_obj.Value.SelectAuroraIdx].eEffect == emAurora_TYPE.ATK)
            {
                fRtn += m_DB_Aurora[i_obj.Value.SelectAuroraIdx].nEffectValue;
            }
        }
        return fRtn;
    }
    
    public float GetAurora_Attri_Atk( emCardAttribute a_Attri )
    {
        float fRtn = 0.0f;
        
        foreach( KeyValuePair<int, tagAuroraSelect>  i_obj in m_AuroraSelect )
        {
            
            if(DataMgr.Inst.IsBuyAurora(i_obj.Value.nLevel) != 1) continue;

            switch(a_Attri)
            {                
                case emCardAttribute.emCAB_WA:
                    if( m_DB_Aurora[i_obj.Value.SelectAuroraIdx].eEffect == emAurora_TYPE.Attr_WA)                    
                        fRtn += m_DB_Aurora[i_obj.Value.SelectAuroraIdx].nEffectValue;                    
                break;

		        case emCardAttribute.emCAB_SU:
                    if( m_DB_Aurora[i_obj.Value.SelectAuroraIdx].eEffect == emAurora_TYPE.Attr_SU)
                        fRtn += m_DB_Aurora[i_obj.Value.SelectAuroraIdx].nEffectValue;     
                break;

		        case emCardAttribute.emCAB_YUNG:
                    if( m_DB_Aurora[i_obj.Value.SelectAuroraIdx].eEffect == emAurora_TYPE.Attr_YUNG)
                        fRtn += m_DB_Aurora[i_obj.Value.SelectAuroraIdx].nEffectValue;     
                break;
		        
                case emCardAttribute.emCAB_ARM:
                    if( m_DB_Aurora[i_obj.Value.SelectAuroraIdx].eEffect == emAurora_TYPE.Attr_ARM)
                        fRtn += m_DB_Aurora[i_obj.Value.SelectAuroraIdx].nEffectValue;     
                break;
            }
            
        }
        return fRtn;
    }

    // 오로라를 구입할수 있는 탭있지 구분해준다.
    // 2 : 구입해야한다 (구입행위로 간주).
    // 1 : 이미 사용할수 있다 (스킬선택으로 간주).
    // 0 : 응답하지 않는다. (뒤에 있음).
    public int IsBuyAurora(int a_Level)
    {                
        if(a_Level <= DataMgr.Inst.m_UserInfo.Level) return 1; // 45 <= 40

        if (a_Level <= DataMgr.Inst.m_UserInfo.Aurora_maxLevel) return 1; // 45 <= 1
        

        foreach (KeyValuePair<int, DataMgr.tagSaticAuroraUnlockPrice> obj_i in DataMgr.Inst.m_DB_AuroraUnlockPrice)
        {
            if (obj_i.Value.nLevel <= DataMgr.Inst.m_UserInfo.Level) continue;

            if (obj_i.Value.nLevel > DataMgr.Inst.m_UserInfo.Aurora_maxLevel ) // i > 1
            {
                if (a_Level == obj_i.Value.nLevel) 
                {                    
                    return 2;
                }
                    
                break;
            }
        }

        return 0;
    }

    

    public void SetNoticeList(List<object> a_List)
    {
        m_NoticeList.Clear();
        
        foreach( object obj in a_List )
        {
            Dictionary<string, object> Dic = (Dictionary<string , object>)obj;
            CNoticeData Node = new CNoticeData();
            Node._Type       = CastInt(Dic["Type"]);
            Node._Time       = Dic["Time"].ToString();
            Node._Title      = Dic["Title"].ToString();
            Node._Content    = Dic["Content"].ToString();
            m_NoticeList.Add(Node);

            m_NoticeList.Sort((first, next ) => { return first._Type.CompareTo(next._Type);});
        }
    }
    
    //----------------------------------------------------.
    // cash 아이템 지급에 실패 했을때 재접속 하면 다시 지급시도한다.
    
    
    //----------------------------------------------------------------------.
    // 시간 관리,
    private DateTime    m_ServerDate;
    private long        m_ServerTime = 0;    
    
    public void SetServerTime(string a_Date)
    {
        m_ServerDate = DateTime.Parse(a_Date);
        TimeSpan t = (m_ServerDate - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
        m_ServerTime = (long)t.TotalSeconds;
        string strLastTime = PlayerPrefs.GetString("CrownRecvLastTime", "");

        if( strLastTime != "" && strLastTime != "0" && strLastTime.Length != 0 )
            m_CrownRecvLastTime = long.Parse(strLastTime);
        else
            m_CrownRecvLastTime = m_ServerTime;
    }

    IEnumerator OneSecTime()
    {
        while (true)
        {
            m_ServerTime += 1;
            m_UserInfo.CrownTime(FillCrown());            
            yield return new WaitForSeconds(1.0f);
        }        
    }


    //----------------------------------------------------.
    // .
    // 호출할때마다. 지난 시간 만큼 채워준당.
    // 앞으로 남은시작 리턴.    
    private int m_nCrownAdd = 0;
    private long m_CrownRecvLastTime = 0;
    private long m_Temp_CrownRecvLastTime = 0;
    long FillCrown()
    {
        if (m_nCrownAdd > 0) return 0L;
        if (m_UserInfo.Crown >= m_UserInfo.CrownSlot) return 0L;// 열쇠가 충분하면.
        if (m_CrownRecvLastTime == 0) { m_CrownRecvLastTime = m_ServerTime; }
        

        m_Temp_CrownRecvLastTime = m_CrownRecvLastTime;
        long Time = m_ServerTime - m_Temp_CrownRecvLastTime;

        if (Time < m_CrownFillTime) return Time;        
        
        if (Time >= m_CrownFillTime)
        {
            while(Time >= m_CrownFillTime)
            {
                m_nCrownAdd++;
                Time -= m_CrownFillTime;
                m_Temp_CrownRecvLastTime += m_CrownFillTime;
            }
            
            if ((m_UserInfo.Crown + m_nCrownAdd) >= m_UserInfo.CrownSlot)
            {
                m_Temp_CrownRecvLastTime = 0L;
                m_nCrownAdd = m_UserInfo.CrownSlot - m_UserInfo.Crown; // 나머지갯수 모두채움.
            }                

            m_SerMgr.UpdateCrownADD(m_nCrownAdd, UpdateCrownADD_Result);            
        }

        if(m_Temp_CrownRecvLastTime == 0) return 0L;
        if (m_UserInfo.Crown < m_UserInfo.CrownSlot) return m_CrownFillTime;
        return Time;
    }

    public void UpdateCrownADD_Result(bool a_Result, Dictionary<string, object> a_dicJson, string a_Str, 
        serverManager.ErrorCode a_ErrorCode)
    {
        if(a_Result == true)
        {
            m_UserInfo.Crown += m_nCrownAdd; // (임시코드)서버에 적용.            
            m_CrownRecvLastTime = m_Temp_CrownRecvLastTime;
            PlayerPrefs.SetString("CrownRecvLastTime", m_CrownRecvLastTime.ToString());
            m_nCrownAdd = 0;
        }
        else
        {
            // 시간을 더해 버린다.
            m_CrownRecvLastTime += (m_nCrownAdd * m_CrownFillTime);
            PlayerPrefs.SetString("CrownRecvLastTime", m_CrownRecvLastTime.ToString());
            m_nCrownAdd = 0;
        }
    }

    //테스트키입력 
    public bool TestKeyInput(KeyCode a_KeyCode)
    {
#if UNITY_EDITOR
        return Input.GetKeyUp(a_KeyCode);
#else 
        return false;
#endif
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetString("CrownRecvLastTime", m_CrownRecvLastTime.ToString());
    }
}
