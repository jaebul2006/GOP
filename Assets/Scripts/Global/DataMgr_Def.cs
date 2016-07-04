using UnityEngine;
using System;
using System.Collections;
using System.Text;

using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using MiniJSON;

public partial class DataMgr : MonoBehaviour 
{
    public enum emLanguage { emLanguage_English , emLanguage_Korea };

    // 이넘문 =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=.

    // 카드 속성. 무 화 수 영 암.
    public enum emCardAttribute
	{   // 상극은 공격력 1.5배.         
		// 데미지 주는쪽 : 데미지받는쪽.		
		emCAB_WA = 0,   // 화 상극 : 명.
		emCAB_SU,    	// 수 상극 : 화.
		emCAB_YUNG,     // 명 상극 : 암.
		emCAB_ARM,    	// 암 상극 : 명.
		
		emCAB_END
	};

    public enum emSHOP_TAB
    {
        em_RUBY = 1,
        em_GOLD,
        em_CROWN,
        em_CARD,
        em_END
    };

    public enum emSKILL_TYPE 
    {   
        BallChange,     // X 만큼 무작위로 해당 캐릭터 퍼즐로 변경한다.
        ATK,            // 공격한다. 
        AtkAll,         // 모든적 공격한다. 
        Heal,           // 힐링한다.
    };

    public enum emAurora_TYPE
    {         
        Attr_WA,
        Attr_SU,    
        Attr_YUNG,  
        Attr_ARM,
        HP,
        ATK
    };

    public class tagAuroraSelect
    {
        private bool m_oneSkill_Lock = false; // 사용 스킬이 1개일때 Lock 이다.
        public int nLevel; // 레벨 
                     
        //생성 함수.   
        static public Dictionary<int, tagAuroraSelect> Setup()
        {
            Dictionary<int, tagAuroraSelect> TempList = new Dictionary<int, tagAuroraSelect>();
            
            foreach (KeyValuePair<int, DataMgr.tagSaticAuroraUnlockPrice> obj_i in DataMgr.Inst.m_DB_AuroraUnlockPrice)
            {
                tagAuroraSelect Temp = new tagAuroraSelect();                
                Temp.nLevel = obj_i.Value.nLevel;
                int nCount=0;
                foreach (KeyValuePair<int, DataMgr.tagSaticAuroraSkill> obj_j in DataMgr.Inst.m_DB_Aurora)
                {
                    if (obj_j.Value.nLevel == obj_i.Value.nLevel)
                    {
                        Temp.m_Aurora.Add(obj_j.Value.nIDX);
                        nCount++;
                    }
                }

                if (nCount == 1)
                {
                    Temp.m_oneSkill_Lock = true;
                    Temp._SelectIdx = Temp.m_Aurora[0];
                }

                TempList.Add(Temp.nLevel, Temp);
            }

            return TempList;
        }

        // 접근자 관련.
        public List<int> m_Aurora = new List<int>(); // DB_Aurora Index 리스트
        //PlayerPrefs.GetInt("SelAurora_" + m_nLevel, 0);        
        private int _SelectIdx = 0; // 선택한 IDX
        public int SelectAuroraIdx // 오로라 아이디 직접사용.
        {
            get
            {
                if (m_oneSkill_Lock) return _SelectIdx;                
                if (_SelectIdx == 0)
                    _SelectIdx = PlayerPrefs.GetInt("SelAurora_" + nLevel, m_Aurora[0]);
                return _SelectIdx;
            }
            set
            {
                if (m_oneSkill_Lock) return;
                _SelectIdx = value;
                PlayerPrefs.SetInt("SelAurora_" + nLevel, value);
            }
        }

        // 선택한 IDX 순서. 0~3리턴 
        public int SelectPosIdx
        {
            get
            {
                if (m_oneSkill_Lock) return 0;

                if (_SelectIdx == 0)
                    _SelectIdx = PlayerPrefs.GetInt("SelAurora_" + nLevel, m_Aurora[0]);

                for(int i=0; i< m_Aurora.Count; i++)
                {
                    if(_SelectIdx == m_Aurora[i]) return i;
                }

                return 0;
            }
            set
            {
                if (m_oneSkill_Lock) return;
                int Value = m_Aurora[value];
                if (value < 0 || value >= m_Aurora.Count) Value = m_Aurora[0];
                
                _SelectIdx = Value;
                PlayerPrefs.SetInt("SelAurora_" + nLevel, _SelectIdx);
            }
        }
    };

    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    public class tagAccrue
    {
        public Int64    _Ruby;
        public Int64    _Gold;
        public int      _FriendPoint;
        public int      _MonsterKill;
        public int      _SummonUpTry;
        public int      _SummonUpSucc;
        public int      _GetSummon;
        public int      _CombinedExe;
        public int      _TranscendExe;
        public int      _Level30;
        public int      _SendFriendPoint;
        public int      _AdventureClear;
        public int      _AdventureFailed;
        public int      _SynthesisExe;
        public Int64    _AttriAtk1;
        public Int64    _AttriAtk2;
        public Int64    _AttriAtk3;
        public Int64    _AttriAtk4;
        public Dictionary<string, int> AdventureGrade;
    };

    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=.
    // 유저 디비.
    public class tagUserInfo
    {
        public string   ID;
        public string   Session;
        public string   NicName;
        public int      _Level;
        public int      SummonSlot;
        public int      _CrownSlot;
        public int [][] Team;

        public int      Aurora_maxLevel;                

        public int      nAdven_difficulty;
        public int      nAdven_Stage;
        public int      nAdven_Floor;

        // 접근자 관련 변수들.
        private int         _Gold;
        private int         _Ruby;
        private int         _FriendPoint;
        private int         _Crown;

        private UILabel     _TopUilb_Gold           = null;
        private UILabel     _TopUilb_Ruby           = null;
        private UILabel     _TopUilb_FriendPoint    = null;
        private UILabel     _TopUilb_Crown          = null;
        private UILabel     _TopUilb_CrownTime      = null;        

        private UILabel     _TopUilb_Level           = null;
        private UILabel     _Auroralb_Level          = null;        

        public int       Level
        {
            get { return _Level; }
            set
            {
                _Level = value;
                if (_TopUilb_Level == null)
                {
                    if(DataMgr.Inst.m_Lobby != null)
                        _TopUilb_Level = DataMgr.Inst.m_Lobby.m_EtcUI.transform.FindChild("LbLevel").GetComponent<UILabel>();
                }
                if (_TopUilb_Level != null) _TopUilb_Level.text = "Lv " + _Level.ToString();

                                
                if (_Auroralb_Level == null)
                {
                    if(DataMgr.Inst.m_Lobby != null)
                        _Auroralb_Level = DataMgr.Inst.m_Page[(int)(DataMgr.emMAIN_MODE.AURORA)].GetComponent<Panel_UI_Aurora>().m_lbLevel;
                }
                if (_Auroralb_Level != null) _Auroralb_Level.text = "Lv:"+_Level.ToString("00");
            }
        }

        public int       Gold
        {
            get { return _Gold; }
            set
            {
                _Gold = value;
                if (_TopUilb_Gold == null)
                {
                    if(DataMgr.Inst.m_Lobby != null)
                        _TopUilb_Gold = DataMgr.Inst.m_Lobby.m_TopUI.transform.FindChild("TextGold").FindChild("Label").GetComponent<UILabel>();
                }
                if (_TopUilb_Gold != null) _TopUilb_Gold.text = _Gold.ToString();
            }
        }

        public int Ruby
        {
            get { return _Ruby; }
            set
            {
                _Ruby = value;
                if (_TopUilb_Ruby == null)
                {
                    if (DataMgr.Inst.m_Lobby != null)
                        _TopUilb_Ruby = DataMgr.Inst.m_Lobby.m_TopUI.transform.FindChild("TextRuby").FindChild("Label").GetComponent<UILabel>();
                }
                if (_TopUilb_Ruby != null) _TopUilb_Ruby.text = _Ruby.ToString();
            }
        }

        public int FriendPoint
        {
            get { return _FriendPoint; }
            set
            {
                _FriendPoint = value;
                if (_TopUilb_FriendPoint == null)
                {
                    if (DataMgr.Inst.m_Lobby != null)
                        _TopUilb_FriendPoint = DataMgr.Inst.m_Lobby.m_TopUI.transform.FindChild("TextFriendPoint").FindChild("Label").GetComponent<UILabel>();
                }
                if (_TopUilb_FriendPoint != null) _TopUilb_FriendPoint.text = _FriendPoint.ToString();
            }
        }

        public int Crown
        {
            get { return _Crown; }
            set
            {
                _Crown = value;
                if (_TopUilb_Crown == null)
                {
                    if (DataMgr.Inst.m_Lobby != null)
                        _TopUilb_Crown = DataMgr.Inst.m_Lobby.m_TopUI.transform.FindChild("TextCrawn").FindChild("Label").GetComponent<UILabel>();
                }

                if(_TopUilb_Crown != null) _TopUilb_Crown.text = _Crown.ToString() +"/"+ _CrownSlot;
            }
        }
        
        public int       CrownSlot
        {
            get { return _CrownSlot;}
            set
            {
                _CrownSlot = value;
                if (_TopUilb_Crown == null)
                {
                    if (DataMgr.Inst.m_Lobby != null)
                        _TopUilb_Crown = DataMgr.Inst.m_Lobby.m_TopUI.transform.FindChild("TextCrawn").FindChild("Label").GetComponent<UILabel>();
                }

                if(_TopUilb_Crown != null) _TopUilb_Crown.text = _Crown.ToString() +"/"+ _CrownSlot;
            }
        }

        public void CrownTime( long a_Time )
        {
            if(_TopUilb_CrownTime == null)
            {
                if (DataMgr.Inst.m_Lobby != null)
                    _TopUilb_CrownTime = DataMgr.Inst.m_Lobby.m_TopUI.transform.FindChild("LbCrownText").GetComponent<UILabel>();
            }

            if (_TopUilb_CrownTime != null)
            {
                if (a_Time > 0L)
                {
                    long _Time = DataMgr.Inst.m_CrownFillTime - a_Time;
                    long _Sec = _Time % 60;
                    long _minut = _Time / 60;
                    string str = string.Format("{0:0}:{1:00}", _minut, _Sec);
                    _TopUilb_CrownTime.text = str;
                }
                else
                {
                    _TopUilb_CrownTime.text = "";
                }
            }            
        }

        public void RefleshTopUI()
        {
           Gold         = _Gold;
           Ruby         = _Ruby;
           FriendPoint  = _FriendPoint;
           Crown        = _Crown;
        }

        public void SetTeam( int Index, string a_str)
        {
            try
            {
                string[] Team1 = a_str.Split('_');
                Team[Index][0] = int.Parse(Team1[0]);
                Team[Index][1] = int.Parse(Team1[1]);
                Team[Index][2] = int.Parse(Team1[2]);
                Team[Index][3] = int.Parse(Team1[3]);
            }
            catch(Exception e)
            {
                DataMgr.Inst.Log(e.Message);
            }
        }
    };

    //CardIDX_EXP_등급_강화_레벨_최대레벨(기본30)_강화추가확률.
    public class tagUserSummon
    {
        public int nKey;
        public int nIdxSummon;        
        public int nExp;
        public int nGrade;
        public int nUpgrade;
        public int nLevel;
        public int nMaxLevel;
        public int nAddRate;

        public tagSaticSummon Data;

        public void SetUp(int a_nKey, string a_str)
        {
            try
            {
                nKey = a_nKey;
                string[] str = a_str.Split('_');
                nIdxSummon = int.Parse(str[0].ToString());
                Data = DataMgr.Inst.m_DB_Summon[nIdxSummon];
                nExp = int.Parse(str[1].ToString());
                //nGrade = int.Parse(str[2].ToString());
                nGrade = Data.nGrade;
                nUpgrade = int.Parse(str[3].ToString());
                nLevel = int.Parse(str[4].ToString());
                nMaxLevel = int.Parse(str[5].ToString());
                nAddRate = int.Parse(str[6].ToString());
            }
            catch (Exception e)
            {
                DataMgr.Inst.Log(e.Message);
            }
        }
        
        //
        public float GetAttack()
        { 
            //=(공격력+(공격력*((공격력 상승치/1000)*(레벨-1))))*(1+((강화 상승치/1000)*강화))*(1+((스킬 상승치/1000)))

            //= (공격력+(공격력*((공격상승치/1000)*(레벨- 1))))*(1+((강화상승치/1000)*강화))            
            float fAttack       = (float)Data.nAttack * ( 1f+ (DataMgr.Inst.GetAurora_Attri_Atk(Data.eAttr)/1000f) );
            float fAttackUp     = (float)Data.nAttackUp; 
            float fUpgradeUp    = (float)Data.nUpgradeUp;
            float fUpgrade      = (float)nUpgrade;
            float fLevel        = (float)nLevel;
                            
            return (fAttack+(fAttack*((fAttackUp/1000f)*(fLevel - 1f)))) * (1f+((fUpgradeUp/1000f)*fUpgrade)) * (1f+((DataMgr.Inst.GetAurora_Atk()/1000f)));
        }

        public float GetHp()
        {   //= (생명력+(생명력*((체력상승치/1000)*(레벨- 1)))) * (1+((강화상승치/1000)*강화))
            float fHp           = (float)Data.nHp;
            float fHpUp         = (float)Data.nHpUp; 
            float fUpgradeUp    = (float)Data.nUpgradeUp;
            float fUpgrade      = (float)nUpgrade;
            float fLevel        = (float)nLevel;

            return (fHp+(fHp*((fHpUp/1000f)*(fLevel - 1f)))) * (1f+((fUpgradeUp/1000f)*fUpgrade)) * (1f+((DataMgr.Inst.GetAurora_Hp()/1000f)));
        }
    };

    //IDX_TargetCount_CompletionTime_IsCompletion
    public class tagUserMissionDay
    {
        public int Key;
        public int nDayIdx;
        public int TargetCount;
        public int completionTime;
        public int Iscompletion;
    };

    public class tagUserMissionWeek
    {
        public int Key;
        public int nWeekIdx;
        public int TargetCount;
        public int completionTime;
        public int Iscompletion;
    };

    public class tagUserMissionMonth
    {
        public int Key;
        public int nMonthIdx;
        public int TargetCount;
        public int completionTime;
        public int Iscompletion;
    };
    
    public class tagUserAchievements
    {
        public int Key;
        public int LineCode;
        public int LineOrder;        
        public int TargetCount;
        public int completionTime;
        public int Iscompletion;
    };

    public class tagUserFriendList
    {
        public string Friend_ID;
        public DateTime LastPrecentTime;        
    };

    public class tagUserFriendNotifi
    {
        public int      key;
        public string   NotifiID;
        public string   AgreeID;
        public DateTime Time;
    };

    public class tagUserParceList
    {
        public int      Key;
        public string   Send_ID;
        public string   Recv_ID;
        public string   Code;        
        public DateTime SendTime;
        public int      SaveDay;
    };

    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    // 정적 디비.
    public class tagSaticSummon
    {
	    public int		nIDX;
	    public string	strName;
	    public string	strResurceID;
	    public emCardAttribute eAttr;
	    public int		nGrade;
	    public int		nMaxLevel;
	    public int		nMaxUpgrade;
	    public int		nAttack;
	    public int		nHp;
        public int      nMaxGuage;
        public int		nSkill;

        public int		nAttackUp;
	    public int		nHpUp;
	    public int		nUpgradeUp;
	    
        public bool		bShopLock;	    
    };

    public class tagSaticMonster
    {
	    public int		nIDX;
	    public string   strName;
	    public string   strResurceID;
	    public emCardAttribute eAttr;
	    public int		nLevel;	    
	    public int		nAttack;
	    public int		nHp;
	    public int		nKind;
        
        public int		nWaitTurn;
	    public int		nOutTurn;
	    public int		nAttackSkill;
        public string   strEntranceAni;		
    };

    public class tagSaticSummonSkill
    {
        public int          nIDX;
	    public string	    strName;        // 이름.             
	    public int		    nDerectionCode; // 연출코드.	    
	    public emSKILL_TYPE	eEffect;        // 효과. 
	    public int		    nEffectValue;   // 효과 값.
        public string       strToolTip;     // 설명.
    };

    public class tagSaticMonsterSkill
    {
	    public int			nIDX;
	    public string		strName;
	    public string		strAni;
	    public int			nFx_Screen;
    };

    public class tagSaticMissionDay
    {
	    public int			nIDX;
	    public string		strTitle;
	    public string		strContant;
	    public string		strTarget;
	    public int			nTargetCount;
	    public string		strRewardCode;
        public string       strRewardRate;
    };

    public class tagSaticMissionWeek
    {
	    public int			nIDX;
	    public string		strTitle;
	    public string		strContant;
	    public string		strTarget;
	    public int			nTargetCount;
	    public string		strRewardCode;
        public string       strRewardRate;
    };

    public class tagSaticMissionMonth
    {
	    public int			nIDX;
	    public string		strTitle;
	    public string		strContant;
	    public string		strTarget;
	    public int			nTargetCount;
	    public string		strRewardCode;
        public string       strRewardRate;
    };

    public class tagSaticAchievements
    {
	    public int			nIDX;
	    public int			nLineCode;
	    public int			nLineOrder;
	    public string		strTitle;
	    public string		strContant;
	    public string		strTarget;
	    public int			nTargetCount;
	    public string		strRewardCode;
        public string       strRewardRate;
    };

    public class tagSaticAdventureStage
    {
	    public int		    nIDX;
	    public string	    strName;
	    public string	    strMapID;
        public int          nDifficulty;
        public int		    nStage;
	    public int		    nFloor;
        public int          nCrownUse;
        public int          nMonNum;
        public int          nTimeAttack_A;
        public int          nTimeAttack_B;
        public string []    strMon_Line = new string[10];
	    public int	 	    nReward_Gold;
	    public string	    strReward_Item_Code;
	    public string	    strReward_Item_Code_Rate;
        public int          nExp;
    };

    public class tagSaticInfinity_Tower
    {
	    public int			nIDX;
        public string       strName;
        public string		strMapID;
	    public int			nFloor;
	    public string		strMon_Line_001;
	    public string		strMon_Line_002;
	    public string		strMon_Line_003;
	    public string		strMon_Line_004;
	    public string		strMon_Line_005;
	    public string		strMon_Line_006;
	    public string		strMon_Line_007;
	    public string		strMon_Line_008;
	    public string		strMon_Line_009;
	    public string		strMon_Line_010;
	    public string		strReward_Code;
        public string       strReward_Rate;
    };

    public class tagSaticShop
    {
	    public int			nIDX;
	    public string		strName;
	    public string		strImgID;
        public emSHOP_TAB   eCategory;
	    public string		strGetCode;
	    public string		strGetRate;
	    public string		strPayCode;
	    public string	    strPayCount;
	    public string		strBuyLimitTime;
        public string       strToolTip;
    };

    public class tagSaticExp
    {
        public int nLevel;
        public int nExpBar;
        public int nAccExpBar;
    };

    public class tagSaticAuroraSkill
    {
        public int      nIDX;
        public string   strName;
        public int      nLevel;
        public string   strIconCode;
        public emAurora_TYPE   eEffect; 
        public int      nEffectValue; // 효과 적요값 (1/1000값)
        public string   strToolTip; // 설명 
    };

    public class tagSaticAuroraUnlockPrice
    {
        public int nLevel;
        public string strPayCode;
        public int nPayValue;
    };

    public class tagSaticPlayerExp
    {
        public int      nLevel;
        public int      nExpBar;
    };

    public class tagSaticCombined
    {
	    public int			nIDX;
	    public string		strName;			// 조합식 이름.
	    public string		strPayCode;			// 지불코드.
	    public int			nPayValue;			// 지불 금액.
	    public int			nResultSummon;		// 결과 몬스터.
	    public int			nSummonKey;			// 키 몬.
	    public int			nSummonMaterial1;	// 재료 몬.
	    public int			nSummonMaterial2;
	    public int			nSummonMaterial3;
        public int          nSummonMaterial4;
    };

    public class tagSummonPriceSeed
    {
	    public int		nGrade;
	    public string   strCode_Upgrade;
	    public int		nValue_Upgrade;
	    public string   strCode_LevelEx;
	    public int		nValue_LevelEx;
	    public string   strCode_Sell;
        public int      nValue_Sell;
        
        public int GetUpgade(tagUserSummon a_Summon )
        {
            return nValue_Upgrade + ( (nValue_Upgrade/5) * (a_Summon.nUpgrade) );
        }
        public int GetExLevel(tagUserSummon a_Summon )
        {
            int nCount = (a_Summon.Data.nMaxLevel - a_Summon.nMaxLevel)/ DataMgr.Inst.m_SummonLevelEx;
            return nValue_LevelEx + ((nValue_LevelEx / 5) * (nCount));
        }
        public int GetSell(tagUserSummon a_Summon )
        {
            return nValue_Sell + ( (nValue_Sell/10) * (a_Summon.nLevel-1));
        }
    };

    public class tagUpgradeRate
    {
	    public int		nGrade_Differ;
	    public int		nRate;	
	    public int		nBonusRate;   // 보너스확률 숫자형.
    };

    //공지 데이터.
	public class CNoticeData
	{
		public int _Type;
		public string _Time;
		public string _Title;
		public string _Content;
	};

}
