using UnityEngine;
using System;
//using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
//using Mono.Data.SqliteClient;
using System.Threading;

using System.IO;
using MiniJSON;

public partial class DataMgr : MonoBehaviour 
{
    private string   m_StaticDB   = null;

    // db가 있는지 확인.
    public bool IsStaticDB()
    {
        try
        { 
            if (Application.platform == RuntimePlatform.Android)//
            {
                FileStream FS = File.Open(Application.persistentDataPath + "/SmorStaticDB.bytes", FileMode.Open);
                if (FS != null)
                {
                    FS.Close();
                    return true;
                }
                else return false;
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                FileStream FS = File.Open(Application.temporaryCachePath + "/SmorStaticDB.bytes", FileMode.Open);
                if (FS != null)
                {
                    FS.Close();
                    return true;
                }
                else return false;
            }
            else
            {
                FileStream FS = File.Open(Application.dataPath + "/SmorStaticDB.bytes", FileMode.Open);
                if (FS != null)
                {
                    FS.Close();
                    return true;
                }
                else return false;
            }
        }
        catch(Exception e )
        {
            Log(e.Message);
            return false;
        }
    }

    // db 파일을 저장한다.
    public void SaveStaticDB(string a_Str)
    {
        try
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            byte[] Tempbuf = Encoding.Unicode.GetBytes(a_Str);
            File.WriteAllBytes(Application.persistentDataPath + "/SmorStaticDB.bytes" , Tempbuf );
#elif UNITY_IOS && !UNITY_EDITOR           
            byte[] Tempbuf = Encoding.Unicode.GetBytes(a_Str);
            string Dir = Application.temporaryCachePath + "/SmorStaticDB.bytes";
            File.WriteAllBytes(Dir, Tempbuf );
            UnityEngine.iOS.Device.SetNoBackupFlag(Dir);            
#elif UNITY_EDITOR
            byte[] Tempbuf = Encoding.Unicode.GetBytes(a_Str);
            Debug.Log( "Length:" + Tempbuf.Length );
            File.WriteAllBytes( Application.dataPath + "/SmorStaticDB.bytes", Tempbuf );            
#endif
        }
        catch (Exception e)
        {
            Log(e.Message);         
        }
    }

    //로드한다.
    public bool LoadStaticDB()
    {        
        if (Application.platform == RuntimePlatform.Android)//
        {                                                
            FileStream FR = File.OpenRead(Application.persistentDataPath + "/SmorStaticDB.bytes");
            byte[] Tempbuf = new byte[FR.Length];
            FR.Read(Tempbuf, 0, Tempbuf.Length);
            m_StaticDB = Encoding.Unicode.GetString(Tempbuf);
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)//
        {                                                
            FileStream FR = File.OpenRead(Application.temporaryCachePath + "/SmorStaticDB.bytes");
            byte[] Tempbuf = new byte[FR.Length];
            FR.Read(Tempbuf, 0, Tempbuf.Length);
            m_StaticDB = Encoding.Unicode.GetString(Tempbuf);
        }
        else
        {            
            FileStream FR = File.OpenRead(Application.dataPath + "/SmorStaticDB.bytes");
            byte[] Tempbuf = new byte[FR.Length];
            FR.Read(Tempbuf, 0, Tempbuf.Length);            
            m_StaticDB = Encoding.Unicode.GetString(Tempbuf);
            
        }

        Dictionary<string, object>  DicJson = Json.Deserialize(m_StaticDB) as Dictionary<string, object>;
        LoadStaticDB(DicJson);
        return true;
    }

    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    //로딩.
    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    public void LoadStaticDB(Dictionary<string, object> a_dicJson)
    {
        StaticDB_GetSummonData( a_dicJson["Summon"] as List<object> );
        StaticDB_GetMonsterData( a_dicJson["Monster"] as List<object> );
        StaticDB_GetSummonSkillData( a_dicJson["Summon_Skill"] as List<object> );
        StaticDB_GetMonsterSkillData( a_dicJson["Monster_Skill"] as List<object> );
        StaticDB_GetMissionDay( a_dicJson["Mission_Day"] as List<object> );
        StaticDB_GetMissionWeek( a_dicJson["Mission_Week"] as List<object> );
        StaticDB_GetMissionMonth( a_dicJson["Mission_Month"] as List<object> );
        StaticDB_GetAchievements( a_dicJson["Achievements"] as List<object> );
        StaticDB_GetShop(a_dicJson["Shop"] as List<object>);
        StaticDB_GetExp(a_dicJson["Exp"] as List<object>);
        StaticDB_GetSummonPriceSeed(a_dicJson["SummonPriceSeed"] as List<object>);
        StaticDB_GetUpgradeRate(a_dicJson["UpgradeRate"] as List<object>);
        StaticDB_GetAuroraSkill(a_dicJson["Aurora"] as List<object>);
        StaticDB_GetAuroraUnlockPrice(a_dicJson["AuroraUnlock"] as List<object>);
        StaticDB_GetPlayerExp(a_dicJson["PlayerExp"] as List<object>);
        StaticDB_GetAdvenStage(a_dicJson["Adventure_Stage"] as List<object>);

        m_AuroraSelect = tagAuroraSelect.Setup(); 
    }

    // 소환사 데이터.
    public void StaticDB_GetSummonData( List<object> a_Data)
    {
        if(a_Data == null ) { Log("Summon Error");  }
        m_DB_Summon.Clear();
		
		foreach( object Obj in a_Data )
		{
			Dictionary<string, object> Temp = (Dictionary<string, object>)Obj;
			DataMgr.tagSaticSummon RtnObj  = new DataMgr.tagSaticSummon();

			RtnObj.nIDX = CastInt(Temp["IDX"]);    // 고유넘버.
			RtnObj.strName = Temp["Name"].ToString();          // 이름.
			RtnObj.strResurceID = Temp["ResurceID"].ToString(); // 스킨넘버.        
		    
			// 속성.
			switch (CastInt(Temp["Attr"]))
			{			 
			    case 1: RtnObj.eAttr = DataMgr.emCardAttribute.emCAB_WA; break;  
			    case 2: RtnObj.eAttr = DataMgr.emCardAttribute.emCAB_SU; break; 
			    case 3: RtnObj.eAttr = DataMgr.emCardAttribute.emCAB_YUNG; break;  
			    case 4: RtnObj.eAttr = DataMgr.emCardAttribute.emCAB_ARM; break; 			    
			}		    

			RtnObj.nGrade       = CastInt(Temp["Grade"]);       // 등급.
            RtnObj.nMaxLevel	= CastInt(Temp["MaxLevel"]);
            RtnObj.nMaxUpgrade  = CastInt(Temp["MaxUpgrade"]);
			RtnObj.nAttack      = CastInt(Temp["Attack"]);      // 공격력. 
			RtnObj.nHp          = CastInt(Temp["Hp"]);          // 체력. 
            RtnObj.nMaxGuage    = CastInt(Temp["MaxGuage"]);    // 게이지. 
            RtnObj.nSkill       = CastInt(Temp["Skill"]);       // 체력.

            if(int.Parse(Temp["ShopLock"].ToString()) == 1) RtnObj.bShopLock = true;
			else RtnObj.bShopLock = false;
			
            RtnObj.nAttackUp    = CastInt(Temp["AttackUp"]);
            RtnObj.nHpUp        = CastInt(Temp["HpUp"]);
            RtnObj.nUpgradeUp   = CastInt(Temp["UpgradeUp"]);
									
			DataMgr.Inst.m_DB_Summon.Add(RtnObj.nIDX, RtnObj);
		}
    }

    // 몬스터데이터. 
    public void StaticDB_GetMonsterData(List<object> a_Data)
    {
        if (a_Data == null) { Log("Monster Error"); }
        m_DB_Monster.Clear();

        foreach (object Obj in a_Data)
        {
            Dictionary<string, object> Temp = (Dictionary<string, object>)Obj;
            DataMgr.tagSaticMonster RtnObj  = new DataMgr.tagSaticMonster();

            RtnObj.nIDX         = CastInt(Temp["IDX"]);    // 고유넘버.
            RtnObj.strName      = Temp["Name"].ToString();              // 이름.
            RtnObj.strResurceID = Temp["ResurceID"].ToString();         // 스킨넘버.        

            // 속성.                      
            switch (CastInt(Temp["Attr"]))
            {                
                case 1: RtnObj.eAttr = DataMgr.emCardAttribute.emCAB_WA; break;
                case 2: RtnObj.eAttr = DataMgr.emCardAttribute.emCAB_SU; break;
                case 3: RtnObj.eAttr = DataMgr.emCardAttribute.emCAB_YUNG; break;
                case 4: RtnObj.eAttr = DataMgr.emCardAttribute.emCAB_ARM; break;
            }

            RtnObj.nLevel       = CastInt(Temp["Level"]);
            
            RtnObj.nAttack      = CastInt(Temp["Attack"]);      // 공격력. 
            RtnObj.nHp          = CastInt(Temp["Hp"]);          // 방어력. 
            RtnObj.nKind        = CastInt(Temp["Kind"]);
             
            RtnObj.nWaitTurn        = CastInt(Temp["WaitTurn"]);
            RtnObj.nOutTurn         = CastInt(Temp["OutTurn"]);
            RtnObj.nAttackSkill     = CastInt(Temp["AttackSkill"]);
            RtnObj.strEntranceAni   = Temp["EntranceAni"].ToString();

            DataMgr.Inst.m_DB_Monster.Add(RtnObj.nIDX, RtnObj);
        }
    }

    // 소환사 스킬 데이터. 
    public void StaticDB_GetSummonSkillData(List<object> a_Data)
    {
        if (a_Data == null) { Log("SummonSkill Error"); }
        m_DB_SummonSkill.Clear();

        foreach (object Obj in a_Data)
        {
            Dictionary<string, object> Temp = (Dictionary<string, object>)Obj;
            DataMgr.tagSaticSummonSkill RtnObj = new DataMgr.tagSaticSummonSkill();

            RtnObj.nIDX             = CastInt(Temp["IDX"]);    // 고유넘버.
            RtnObj.strName          = Temp["Name"].ToString();              // 이름.
            RtnObj.nDerectionCode   = CastInt(Temp["DerectionCode"]);         // 스킨넘버.            
                        
            switch(Temp["Effect"].ToString())
            {
                case "BallChange":  RtnObj.eEffect = emSKILL_TYPE.BallChange; break;
                case "Atk":         RtnObj.eEffect = emSKILL_TYPE.ATK; break;
                case "AtkAll":      RtnObj.eEffect = emSKILL_TYPE.AtkAll; break;
                case "Heal":        RtnObj.eEffect = emSKILL_TYPE.Heal; break;
            }
            
            RtnObj.nEffectValue     = CastInt(Temp["EffectValue"]);
            RtnObj.strToolTip       = Temp["ToolTip"].ToString();

            DataMgr.Inst.m_DB_SummonSkill.Add(RtnObj.nIDX, RtnObj);
        }
    }

    // 몬스터 스킬 데이터. 
    public void StaticDB_GetMonsterSkillData(List<object> a_Data)
    {
        if (a_Data == null) { Log("MonsterSkill Error"); }
        m_DB_MonsterSkill.Clear();

        foreach (object Obj in a_Data)
        {
            Dictionary<string, object> Temp = (Dictionary<string, object>)Obj;
            DataMgr.tagSaticMonsterSkill RtnObj = new DataMgr.tagSaticMonsterSkill();

            RtnObj.nIDX             = CastInt(Temp["IDX"]);            // 고유넘버.
            RtnObj.strName          = Temp["Name"].ToString();         // 이름.
            RtnObj.strAni           = Temp["Ani"].ToString();          // 이름.
            RtnObj.nFx_Screen       = CastInt(Temp["Fx_Screen"]);      // 이름.

            DataMgr.Inst.m_DB_MonsterSkill.Add(RtnObj.nIDX, RtnObj);
        }
    }

    // 미션 데이터. 
    public void StaticDB_GetMissionDay(List<object> a_Data)
    {
        if (a_Data == null) { Log("MissionDay Error"); }
        m_DB_MissionDay.Clear();

        foreach (object Obj in a_Data)
        {
            Dictionary<string, object> Temp = (Dictionary<string, object>)Obj;
            DataMgr.tagSaticMissionDay RtnObj = new DataMgr.tagSaticMissionDay();

            RtnObj.nIDX             = CastInt(Temp["IDX"]);
            RtnObj.strTitle         = Temp["Title"].ToString();
            RtnObj.strContant       = Temp["Contant"].ToString();
            RtnObj.strTarget        = Temp["Target"].ToString();
            RtnObj.nTargetCount     = CastInt(Temp["TargetCount"]);
            RtnObj.strRewardCode    = Temp["RewardCode"].ToString();
            RtnObj.strRewardRate    = Temp["RewardRate"].ToString();

            DataMgr.Inst.m_DB_MissionDay.Add(RtnObj.nIDX, RtnObj);
        }
    }

    // 미션 데이터. 
    public void StaticDB_GetMissionWeek(List<object> a_Data)
    {
        if (a_Data == null) { Log("MissionWeek Error"); }
        m_DB_MissionWeek.Clear();

        foreach (object Obj in a_Data)
        {
            Dictionary<string, object> Temp = (Dictionary<string, object>)Obj;
            DataMgr.tagSaticMissionWeek RtnObj = new DataMgr.tagSaticMissionWeek();

            RtnObj.nIDX             = CastInt(Temp["IDX"]);
            RtnObj.strTitle         = Temp["Title"].ToString();
            RtnObj.strContant       = Temp["Contant"].ToString();
            RtnObj.strTarget        = Temp["Target"].ToString();
            RtnObj.nTargetCount     = CastInt(Temp["TargetCount"]);
            RtnObj.strRewardCode    = Temp["RewardCode"].ToString();
            RtnObj.strRewardRate    = Temp["RewardRate"].ToString();

            DataMgr.Inst.m_DB_MissionWeek.Add(RtnObj.nIDX, RtnObj);
        }
    }

    // 미션 데이터. 
    public void StaticDB_GetMissionMonth(List<object> a_Data)
    {
        if (a_Data == null) { Log("MissionMonth Error"); }
        m_DB_MissionMonth.Clear();

        foreach (object Obj in a_Data)
        {
            Dictionary<string, object> Temp = (Dictionary<string, object>)Obj;
            DataMgr.tagSaticMissionMonth RtnObj = new DataMgr.tagSaticMissionMonth();

            RtnObj.nIDX             = CastInt(Temp["IDX"]);
            RtnObj.strTitle         = Temp["Title"].ToString();
            RtnObj.strContant       = Temp["Contant"].ToString();
            RtnObj.strTarget        = Temp["Target"].ToString();
            RtnObj.nTargetCount     = CastInt(Temp["TargetCount"]);
            RtnObj.strRewardCode    = Temp["RewardCode"].ToString();
            RtnObj.strRewardRate    = Temp["RewardRate"].ToString();

            DataMgr.Inst.m_DB_MissionMonth.Add(RtnObj.nIDX, RtnObj);
        }
    }

    // 미션 데이터. 
    public void StaticDB_GetAchievements(List<object> a_Data)
    {
        if (a_Data == null) { Log("Achievements Error"); }
        m_DB_Achievements.Clear();

        foreach (object Obj in a_Data)
        {
            Dictionary<string, object> Temp = (Dictionary<string, object>)Obj;
            DataMgr.tagSaticAchievements RtnObj = new DataMgr.tagSaticAchievements();            

            RtnObj.nIDX             = CastInt(Temp["IDX"]);
            RtnObj.nLineCode        = CastInt(Temp["LineCode"]);
            RtnObj.nLineOrder       = CastInt(Temp["LineOrder"]);
            RtnObj.strTitle         = Temp["Title"].ToString();
            RtnObj.strContant       = Temp["Content"].ToString();
            RtnObj.strTarget        = Temp["Target"].ToString();
            RtnObj.nTargetCount     = CastInt(Temp["TargetCount"]);
            RtnObj.strRewardCode    = Temp["rewardCode"].ToString();
            RtnObj.strRewardRate    = Temp["rewardRate"].ToString();

            DataMgr.Inst.m_DB_Achievements.Add(RtnObj.nIDX, RtnObj);
        }
    }


    // 상점 데이터. 
    public void StaticDB_GetShop(List<object> a_Data)
    {
        if (a_Data == null) { Log("Shop Error"); }
        m_DB_Shop.Clear();

        foreach (object Obj in a_Data)
        {
            Dictionary<string, object> Temp = (Dictionary<string, object>)Obj;
            DataMgr.tagSaticShop RtnObj = new DataMgr.tagSaticShop();

            RtnObj.nIDX             = CastInt(Temp["IDX"]);
            RtnObj.strName          = Temp["Name"].ToString();
            RtnObj.strImgID         = Temp["ImgID"].ToString();

            switch(CastInt(Temp["Category"]))
            {
                case 1: RtnObj.eCategory = emSHOP_TAB.em_RUBY; break;
                case 2: RtnObj.eCategory = emSHOP_TAB.em_GOLD; break;
                case 3: RtnObj.eCategory = emSHOP_TAB.em_CROWN; break;
                case 4: RtnObj.eCategory = emSHOP_TAB.em_CARD; break;
            }                            

            RtnObj.strGetCode       = Temp["GetCode"].ToString(); 
            RtnObj.strGetRate       = Temp["GetRate"].ToString();
            RtnObj.strPayCode       = Temp["PayCode"].ToString();
            RtnObj.strPayCount      = Temp["PayCount"].ToString();
            RtnObj.strBuyLimitTime  = Temp["BuyLimitTime"].ToString();
            RtnObj.strToolTip       = Temp["ToolTip"].ToString();

            if(RtnObj.strGetCode.IndexOf('P', 0) != -1)
            { 
                m_Shop_SummonSlot = RtnObj;
                m_Shop_SummonSlot_AddCount = int.Parse(DataMgr.Inst.m_Shop_SummonSlot.strGetCode.Remove(DataMgr.Inst.m_Shop_SummonSlot.strGetCode.Length-1, 1));
            }

            DataMgr.Inst.m_DB_Shop.Add(RtnObj.nIDX, RtnObj);
        }
    }

    // 경험치 데이터. 
    public void StaticDB_GetExp(List<object> a_Data)
    {
        if (a_Data == null) { Log("Exp Error"); }
        m_DB_Exp.Clear();

        foreach (object Obj in a_Data)
        {
            Dictionary<string, object> Temp = (Dictionary<string, object>)Obj;
            DataMgr.tagSaticExp RtnObj = new DataMgr.tagSaticExp();
            
            RtnObj.nLevel = CastInt(Temp["Level"]);
            RtnObj.nExpBar = CastInt(Temp["ExpBar"]);
            RtnObj.nAccExpBar = CastInt(Temp["AccExpBar"]);

            DataMgr.Inst.m_DB_Exp.Add(RtnObj.nLevel, RtnObj);
        }
    }

    public void StaticDB_GetSummonPriceSeed(List<object> a_Data)
    {        
        if (a_Data == null) { Log("SummonPriceSeed Error"); }
        m_DB_SummonPriceSeed.Clear();        

        foreach (object Obj in a_Data)
        {
            Dictionary<string, object> Temp = (Dictionary<string, object>)Obj;
            DataMgr.tagSummonPriceSeed RtnObj = new DataMgr.tagSummonPriceSeed();
            
            RtnObj.nGrade           = CastInt(Temp["Grade"]);

            RtnObj.strCode_Upgrade  = Temp["Code_Upgrade"].ToString();
            RtnObj.nValue_Upgrade   = CastInt(Temp["Value_Upgrade"]);

            RtnObj.strCode_LevelEx  = Temp["Code_LevelEx"].ToString();
            RtnObj.nValue_LevelEx   = CastInt(Temp["Value_LevelEx"]);

            RtnObj.strCode_Sell     = Temp["Code_Sell"].ToString();
            RtnObj.nValue_Sell      = CastInt(Temp["Value_Sell"]);

            DataMgr.Inst.m_DB_SummonPriceSeed.Add(RtnObj.nGrade, RtnObj);
        }
    }


    public void StaticDB_GetUpgradeRate(List<object> a_Data)
    {        
        if (a_Data == null) { Log("UpgradeRate Error"); }
        m_DB_UpgradeRate.Clear();

        foreach (object Obj in a_Data)
        {
            Dictionary<string, object> Temp = (Dictionary<string, object>)Obj;
            DataMgr.tagUpgradeRate RtnObj = new DataMgr.tagUpgradeRate();

            RtnObj.nGrade_Differ    = CastInt(Temp["Grade_Differ"]);
            RtnObj.nRate            = CastInt(Temp["Rate"]);
            RtnObj.nBonusRate       = CastInt(Temp["BonusRate"]);            

            DataMgr.Inst.m_DB_UpgradeRate.Add(RtnObj.nGrade_Differ, RtnObj);
        }
    }


    //
    public void StaticDB_GetAuroraSkill(List<object> a_Data)
    {
        if (a_Data == null) { Log("AuroraSkill Error"); }
        m_DB_Aurora.Clear();

        foreach (object Obj in a_Data)
        {
            Dictionary<string, object> Temp = (Dictionary<string, object>)Obj;
            DataMgr.tagSaticAuroraSkill RtnObj = new DataMgr.tagSaticAuroraSkill();

            RtnObj.nIDX         = CastInt(Temp["IDX"]);
            RtnObj.strName      = Temp["Name"].ToString();
            RtnObj.nLevel       = CastInt(Temp["Level"]);
            RtnObj.strIconCode  = Temp["IconCode"].ToString();
            

            switch(Temp["Effect"].ToString())
            {
                case "HP": RtnObj.eEffect = emAurora_TYPE.HP; break;
                case "Atk": RtnObj.eEffect = emAurora_TYPE.ATK; break;
                case "Attr_1": RtnObj.eEffect = emAurora_TYPE.Attr_WA; break;
                case "Attr_2": RtnObj.eEffect = emAurora_TYPE.Attr_SU; break;
                case "Attr_3": RtnObj.eEffect = emAurora_TYPE.Attr_YUNG; break;
                case "Attr_4": RtnObj.eEffect = emAurora_TYPE.Attr_ARM; break;
            }
            

            RtnObj.nEffectValue = CastInt(Temp["EffectValue"]);            
            RtnObj.strToolTip   = Temp["ToolTip"].ToString();
                        
            DataMgr.Inst.m_DB_Aurora.Add(RtnObj.nIDX, RtnObj);
        }
    }

    
    public void StaticDB_GetAuroraUnlockPrice(List<object> a_Data)
    {
        if (a_Data == null) { Log("AuroraUnlock Error"); }
        m_DB_AuroraUnlockPrice.Clear();

        foreach (object Obj in a_Data)
        {
            Dictionary<string, object> Temp = (Dictionary<string, object>)Obj;
            DataMgr.tagSaticAuroraUnlockPrice RtnObj = new DataMgr.tagSaticAuroraUnlockPrice();

            RtnObj.nLevel       = CastInt(Temp["Level"]);
            RtnObj.strPayCode   = Temp["PayCode"].ToString();
            RtnObj.nPayValue    = CastInt(Temp["PayValue"]);

            DataMgr.Inst.m_DB_AuroraUnlockPrice.Add(RtnObj.nLevel, RtnObj);
        }
    }

    public void StaticDB_GetPlayerExp(List<object> a_Data)
    {
        if (a_Data == null) { Log("PlayerExp Error"); }
        m_DB_PlayerExp.Clear();

        foreach (object Obj in a_Data)
        {
            Dictionary<string, object> Temp = (Dictionary<string, object>)Obj;
            DataMgr.tagSaticPlayerExp RtnObj = new DataMgr.tagSaticPlayerExp();

            RtnObj.nLevel = CastInt(Temp["Level"]);
            RtnObj.nExpBar = CastInt(Temp["ExpBar"]);

            DataMgr.Inst.m_DB_PlayerExp.Add(RtnObj.nLevel, RtnObj);
        }
    }

    public void StaticDB_GetAdvenStage(List<object> a_Data)
    {
        if (a_Data == null) { Log("AdventureStage Error"); }
        m_DB_AdventureStage.Clear();

        foreach (object Obj in a_Data)
        {
            Dictionary<string, object> Temp = (Dictionary<string, object>)Obj;
            DataMgr.tagSaticAdventureStage RtnObj = new DataMgr.tagSaticAdventureStage();

            RtnObj.nIDX                     = CastInt(Temp["IDX"]);
            RtnObj.strName                  = Temp["Name"].ToString();
            RtnObj.strMapID                 = Temp["MapID"].ToString();
            RtnObj.nDifficulty              = CastInt(Temp["Difficulty"]);
            RtnObj.nStage                   = CastInt(Temp["Stage"]);
            RtnObj.nFloor                   = CastInt(Temp["Floor"]);
            RtnObj.nCrownUse                = CastInt(Temp["CrownUse"]);
            RtnObj.nMonNum                  = CastInt(Temp["MonNum"]);
            RtnObj.nTimeAttack_A            = CastInt(Temp["TimeAttack_A"]);
            RtnObj.nTimeAttack_B            = CastInt(Temp["TimeAttack_B"]);
            RtnObj.strMon_Line[0]          = Temp["Mon_Line_01"].ToString();
            RtnObj.strMon_Line[1]          = Temp["Mon_Line_02"].ToString();
            RtnObj.strMon_Line[2]          = Temp["Mon_Line_03"].ToString();
            RtnObj.strMon_Line[3]          = Temp["Mon_Line_04"].ToString();
            RtnObj.strMon_Line[4]          = Temp["Mon_Line_05"].ToString();
            RtnObj.strMon_Line[5]          = Temp["Mon_Line_06"].ToString();
            RtnObj.strMon_Line[6]          = Temp["Mon_Line_07"].ToString();
            RtnObj.strMon_Line[7]          = Temp["Mon_Line_08"].ToString();
            RtnObj.strMon_Line[8]          = Temp["Mon_Line_09"].ToString();
            RtnObj.strMon_Line[9]          = Temp["Mon_Line_10"].ToString();
            RtnObj.nReward_Gold             = CastInt(Temp["Reward_Gold"]);
            RtnObj.strReward_Item_Code      = Temp["Reward_Item_Code"].ToString();
            RtnObj.strReward_Item_Code_Rate = Temp["Reward_Item_Code_Rate"].ToString();
            RtnObj.nExp                     = CastInt(Temp["Exp"]);

            DataMgr.Inst.m_DB_AdventureStage.Add(RtnObj.nIDX, RtnObj);
        }
    }
    
}
