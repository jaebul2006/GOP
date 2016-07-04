using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class GameMgr : MonoBehaviour
{
    public enum GameState { Wait, NextStage, LoadStage, LoadRound, StartMove, GamIng, Failed, Victory }
    public enum Turn { Player, Enemy };
    
    public GameObject       m_UIRoot;
    public DamageEffect[]   m_DamageEffect;
    public GameObject       m_MapLink;

    public Camera           m_3DCamara;
    public Camera           m_2DCamara;

    public GameObject       m_Msg; // pause msg.
    public UILabel          m_lbMoney;
    
    public UILabel          m_Title;
    public UISlider[]       m_MonHp;
    public Hpbar            m_HpMgr;
    public GameTimer        m_Timer;
    public CSummonSlot[]    m_SumSlot;
    public GameObject[]     m_SumAtkPoint;
    public GameObject[]     m_MonAtkPoint;
    public BattlePuzzleMgr  m_PuzzleMgr;
    public GameObject       m_GameOver;
    public Panel_Load       m_Panel_Load;
    public Panel_Result     m_Panel_Result;
    public Panel_Warning    m_Panel_Warning;     
    public GameObject       m_RoundBar;
    public AudioSource      m_CoinSound;

    public UISprite         m_Fiber_BG;
    public UISprite         m_Fiber_Eff;
    public UISprite         m_Fiber_Eff_1;
    public UISprite         m_Fiber_Eff_2;
    public Panel_Warning    m_Fiber_text;

    private Map10X          m_Map = null;
    private GameState       m_GameStap = GameState.Wait;
    private CEnemy[]        m_Monster = null;
    private int             m_RoundMax = 0;
    private int             m_Round = -1;
    private Turn            m_Turn = Turn.Player;
    private int             m_GameStap2 = 0;
    private int             m_GameStap3 = 0;
//    private int[]           m_MonsterOnList = null;    
    private Vector3         m_HpEffPos;
    private int             m_TrunDrag;
    private bool            m_IsHpwarning;
    // 
    [System.NonSerialized] public DamageManager m_DamageManager;
    [System.NonSerialized] public int[] m_Attri_AttackCount = new int[ (int)DataMgr.emCardAttribute.emCAB_END ];
    [System.NonSerialized] public int   m_PlayTime_Second;
    [System.NonSerialized] public int   m_PlayGetGold;

    // 스테이지의 라운드 정보를 저장함.
    private class CRoundInfo
    {
        public int [] MonIndex;
    };
    private List<CRoundInfo> m_RoundInfo = new List<CRoundInfo>();

    void Awake()
    {
        DataMgr.Inst.GetLobby_Off();
        DataMgr.Inst.m_TopObj = m_UIRoot;
        m_Msg.SetActive(false);
        m_DamageManager = GetComponent<DamageManager>();
    }

    // Use this for initialization
    void Start()
    {                        
        m_HpEffPos = new Vector3(m_HpMgr.transform.FindChild("Bar").transform.localPosition.x-95f,
            m_HpMgr.transform.FindChild("Bar").transform.localPosition.y+130f, 0.0f);

        m_Monster = new CEnemy[Defines.DEF_MAX_Ememy];        
//        m_MonsterOnList = new int[Defines.DEF_MAX_Ememy];        
     
        m_Panel_Load.gameObject.SetActive(false);
        m_GameOver.SetActive(false);
        m_RoundBar.SetActive(false);
         
        m_Panel_Result.gameObject.SetActive(false);        
        m_Panel_Result.m_GameMgr        = this;
        m_Panel_Result.m_SendNextStage  = "onNextStage";
        m_Panel_Result.m_SendRetry      = "onRetry";
        m_Panel_Result.m_SendExit       = "BtnPause_Ok";

        for(int i=0; i<m_SumSlot.Length; i++)
        {
            m_SumSlot[i].m_Delegate_AlertSkill = AlertSkill;            
        }                 

        m_Fiber_BG.enabled = false;
        m_Fiber_Eff.enabled = false;
        m_Fiber_Eff_1.enabled = false;
        m_Fiber_Eff_2.enabled = false;        

        // 게임 시작.       
        m_GameStap = GameState.LoadStage;
        StartCoroutine(SecondCount());
    }
    
    TweenAlpha m_Fiber_BG_TA;
    TweenAlpha m_Fiber_Eff_TA;
    TweenAlpha m_Fiber_Eff1_TA;
    TweenAlpha m_Fiber_Eff2_TA;
    TweenPosition m_FiberText_TA;

    void FiberON()
    {
        if(m_Fiber_BG.enabled) return;
        m_Fiber_BG.enabled = true;
        m_Fiber_Eff.enabled = true;
        m_Fiber_Eff_1.enabled = true;
        m_Fiber_Eff_2.enabled = true;        
        m_Fiber_text.Begin();

        m_Fiber_BG_TA 		            = m_Fiber_BG.gameObject.AddComponent<TweenAlpha> ();
		m_Fiber_BG_TA.from 			    = 0.0f;
		m_Fiber_BG_TA.to 			    = 1.0f;
		m_Fiber_BG_TA.style 		    = UITweener.Style.Once;
		m_Fiber_BG_TA.duration 		    = 1.0f;
		m_Fiber_BG_TA.method 			= UITweener.Method.Linear;

        m_Fiber_Eff_TA 		            = m_Fiber_Eff.gameObject.AddComponent<TweenAlpha> ();
		m_Fiber_Eff_TA.from 			= 1.0f;
		m_Fiber_Eff_TA.to 				= 0.0f;
		m_Fiber_Eff_TA.style 			= UITweener.Style.PingPong;
		m_Fiber_Eff_TA.duration 		= 0.5f;
		m_Fiber_Eff_TA.method 			= UITweener.Method.Linear;

        m_Fiber_Eff1_TA 		        = m_Fiber_Eff_1.gameObject.AddComponent<TweenAlpha> ();
		m_Fiber_Eff1_TA.from 			= 1.0f;
		m_Fiber_Eff1_TA.to 				= 0.0f;
		m_Fiber_Eff1_TA.style 			= UITweener.Style.PingPong;
		m_Fiber_Eff1_TA.duration 		= 0.4f;
		m_Fiber_Eff1_TA.method 			= UITweener.Method.Linear;

        m_Fiber_Eff2_TA 		        = m_Fiber_Eff_2.gameObject.AddComponent<TweenAlpha> ();
		m_Fiber_Eff2_TA.from 			= 0.0f;
		m_Fiber_Eff2_TA.to 				= 1.0f;
		m_Fiber_Eff2_TA.style 			= UITweener.Style.PingPong;
		m_Fiber_Eff2_TA.duration 		= 0.4f;
		m_Fiber_Eff2_TA.method 			= UITweener.Method.Linear;
    }

    void FiberOFF()
    {        
        Destroy(m_Fiber_BG_TA);
        Destroy(m_Fiber_Eff_TA);
        Destroy(m_Fiber_Eff1_TA);
        Destroy(m_Fiber_Eff2_TA);

        m_Fiber_BG.enabled = false;
        m_Fiber_Eff.enabled = false;        
        m_Fiber_Eff_1.enabled = false;
        m_Fiber_Eff_2.enabled = false;
        //m_Fiber_text.End();
    }

    // 다음 스테이지 찿는다.
    void NextStage()
    {
        int NowStage = DataMgr.Inst.m_SelectStageIndex;

        for (int i=0; i<DataMgr.Inst.m_DB_AdventureStage.Count; i++)
        {
            NowStage++;
            if(DataMgr.Inst.m_DB_AdventureStage.ContainsKey(NowStage))
            {
                DataMgr.Inst.m_SelectStageIndex = NowStage;
                break;
            }
        }
    }

    // 스테이지를 다로 로딩한다.
    void loadStage()
    {
        m_RoundInfo.Clear();
        
        int nRound=1;
        string Temp="";
        for( int i=0; i<Defines.DEF_MAX_STAGE_ROUND; i++ )
        {
            Temp = DataMgr.Inst.m_DB_AdventureStage[DataMgr.Inst.m_SelectStageIndex].strMon_Line[i];
            if(Temp == "-1")
            {
                nRound = i; 
                break;
            }                
            
            string [] str = DataMgr.Inst.m_DB_AdventureStage[DataMgr.Inst.m_SelectStageIndex].strMon_Line[i].Split('_'); 

            CRoundInfo RoundInfo = new CRoundInfo();
            RoundInfo.MonIndex = new int[str.Length];

            for(int j=0; j<str.Length; j++)
            {
                RoundInfo.MonIndex[j] = int.Parse( str[j] );
            }

            m_RoundInfo.Add(RoundInfo);
        }            
        
        m_RoundMax          = nRound-1;    
        m_Round             = -1;
        m_lbMoney.text      = "0";
        m_PlayGetGold       = 0;
        m_PlayTime_Second   = 0;
        m_IsHpwarning       = false;


        m_Title.text = "Stage : "+DataMgr.Inst.m_DB_AdventureStage[DataMgr.Inst.m_SelectStageIndex].strName +
            " " + DataMgr.Inst.m_DB_AdventureStage[DataMgr.Inst.m_SelectStageIndex].nStage + 
            "-" + DataMgr.Inst.m_DB_AdventureStage[DataMgr.Inst.m_SelectStageIndex].nFloor;                

        // 소환사 체력 세팅.
        float MaxHP = 0f;
        for (int i = 0; i < Defines.DEF_MAX_TEAM_ITEM; i++)
        {            
            if (m_SumSlot[i].gameObject.activeSelf)
            {
                MaxHP += m_SumSlot[i].m_Info.GetHp();
                m_SumSlot[i].ResetGauge();
            }
        }
        m_HpMgr.SetUp((int)MaxHP);
        
        // 맵세팅.        
        if(m_Map != null) m_Map.Close();
        m_Map = Map10X.Create(m_MapLink);
        m_PuzzleMgr.Lock();
    }

    // 1초를 세는 함수.
    IEnumerator SecondCount()
    {
        while(true)
        {
            yield return new WaitForSeconds(1.0f);
            if( m_GameStap == GameState.GamIng )
                m_PlayTime_Second++;
        }
    }    
    
    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    // 라운드 관련 코드.
    void SettingRoundBar()
    {
        m_RoundBar.SetActive(true);
        m_RoundBar.GetComponent<UIWidget>().alpha=1.0f;
        UISprite Stick = m_RoundBar.transform.FindChild("SprStick").GetComponent<UISprite>();
        UISprite Num1 = m_RoundBar.transform.FindChild("Number1").GetComponent<UISprite>();
        UISprite Num2 = m_RoundBar.transform.FindChild("Number2").GetComponent<UISprite>();
        UISprite Num3 = m_RoundBar.transform.FindChild("Number3").GetComponent<UISprite>();
        UISprite Num4 = m_RoundBar.transform.FindChild("Number4").GetComponent<UISprite>();
        
        //m_Round, m_RoundMax
        float nInterval = 5;
        float Shw = Stick.width/2;
        if( m_RoundMax < 10)
        {
            Num1.enabled = false;
            Num2.enabled = true;
            Num3.enabled = true;
            Num4.enabled = false;

            Num2.spriteName = "Round_" + (m_Round+1);
            Num3.spriteName = "Round_" + (m_RoundMax+1);
            
            Num2.MakePixelPerfect();
            Num3.MakePixelPerfect();

            Num2.transform.localPosition = new Vector3( -Shw-(Num2.width/2f)-nInterval, 
                Num2.transform.localPosition.y, 0f);
            Num3.transform.localPosition = new Vector3( Shw+(Num3.width/2f)+nInterval, 
                Num3.transform.localPosition.y, 0f);
        }
        else
        {
            Num1.enabled = true;
            Num2.enabled = true;
            Num3.enabled = true;
            Num4.enabled = true;

            Num1.spriteName = "Round_"+((m_Round+1)/10);
            Num2.spriteName = "Round_"+((m_Round+1)%10);
            Num3.spriteName = "Round_"+((m_RoundMax+1)/10);
            Num4.spriteName = "Round_"+((m_RoundMax+1)%10);
            
            Num1.MakePixelPerfect();
            Num2.MakePixelPerfect();
            Num3.MakePixelPerfect();
            Num4.MakePixelPerfect();            

            Num2.transform.localPosition = new Vector3( -Shw-(Num2.width/2f)-nInterval, 
                Num2.transform.localPosition.y, 0f);
            Num3.transform.localPosition = new Vector3( Shw+(Num3.width/2f)+nInterval, 
                Num3.transform.localPosition.y, 0f);

            Num1.transform.localPosition = new Vector3( Num2.transform.localPosition.x-(Num2.width/2f)-(Num1.width/2f), 
                Num1.transform.localPosition.y, 0f);

            Num4.transform.localPosition = new Vector3( Num3.transform.localPosition.x+(Num3.width/2f)+(Num3.width/2f),
                Num4.transform.localPosition.y, 0f);
        }
       
        Invoke("RoundFadeOut", 2f);
    }

    private TweenAlpha m_RoundBar_TA;
    void RoundFadeOut()
    {
        m_RoundBar_TA 		            = m_RoundBar.AddComponent<TweenAlpha>();
		m_RoundBar_TA.from 			    = 1.0f;
		m_RoundBar_TA.to 				= 0.0f;
		m_RoundBar_TA.style 			= UITweener.Style.Once;
		m_RoundBar_TA.eventReceiver 	= gameObject;
		m_RoundBar_TA.duration 		    = 0.7f;
		m_RoundBar_TA.method 			= UITweener.Method.Linear;
		m_RoundBar_TA.callWhenFinished  = "RoundFadeOut_End";                
    }

    void RoundFadeOut_End()
    {
        Destroy(m_RoundBar_TA);
        m_RoundBar.SetActive(false);
    }

    void RefleshAttrimark()
    {
        // 몬스터 상성 표시.
        for(int i=0; i<m_SumSlot.Length; i++)
        {
            if( m_SumSlot[i].gameObject.activeSelf)
            {           
                m_SumSlot[i].SetAttrimark(false);

                for(int j=0; j<m_Monster.Length; j++)
                {
                    if(m_Monster[j] == null) continue;                        

                    float fAttriCont = DataMgr.Inst.GetAtti_DamageCon( m_SumSlot[i].m_Info.Data.eAttr, m_Monster[j].m_Info.eAttr);
                    
                    if( fAttriCont > 1.2f) // 1.25f
                    {
                        m_SumSlot[i].SetAttrimark(true);
                        break;
                    }                        
                    //else if( fAttriCont < 0.9f) // 0.85f                        
                    //else if( fAttriCont > 1) //                            
                }
            }
        }
    }

    //---------------------------------------------------------.
    void Update()
    {      
        if (m_GameStap == GameState.NextStage)
        {
            if(m_GameStap2 == 0)
            {
                m_Panel_Result.gameObject.SetActive(true);
                m_Panel_Result.Begin_Direction( DataMgr.Inst.m_DB_AdventureStage[DataMgr.Inst.m_SelectStageIndex] );
                m_GameStap2 = 1;
            }
            else if(m_GameStap2 == 2)
            {
                NextStage();
                m_GameStap2 = 3;
            }
            else if(m_GameStap2 == 3)
            {
                // 열쇠 체크.
                if(DataMgr.Inst.m_DB_AdventureStage[DataMgr.Inst.m_SelectStageIndex].nCrownUse <= DataMgr.Inst.m_UserInfo.Crown)
                {                     
                    m_GameStap = GameState.LoadStage;
                    m_GameStap2 = 0;
                    m_Panel_Result.gameObject.SetActive(false);                    
                }
                else
                {
                    CMessageBox.Create( DataMgr.Inst.GetLocal("Not enough Crown."), 1, onCrownLack);
                    m_GameStap2 = 4;
                }
            }
            
        }        
        else if (m_GameStap == GameState.LoadStage)
        {
            m_Panel_Load.gameObject.SetActive(true);
            loadStage();
            m_GameStap = GameState.LoadRound;
        }
        else if (m_GameStap == GameState.LoadRound)
        {          
            m_Panel_Load.gameObject.SetActive(true);
            m_Round++;
            SettingRoundBar(); 
            for(int i=0; i<m_MonHp.Length; i++)
            { 
                m_MonHp[i].gameObject.SetActive(false);
            }
                
            int nMonIdx;
            for(int i=0; i<m_RoundInfo[m_Round].MonIndex.Length; i++)
            {   
                nMonIdx = m_RoundInfo[m_Round].MonIndex[i];
                if(nMonIdx != -1)
                {
                    if( i == 2 && DataMgr.Inst.m_DB_Monster[nMonIdx].nKind == 1 )
                        m_Monster[i] = CEnemy.Create(this, m_MonHp[5], 5, m_Map.GetMonPos(5), DataMgr.Inst.m_DB_Monster[nMonIdx], true );
                    else
                        m_Monster[i] = CEnemy.Create(this, m_MonHp[i], i, m_Map.GetMonPos(i), DataMgr.Inst.m_DB_Monster[nMonIdx], false );                                            
                }
                else
                {
                    m_Monster[i] = null;
                }
            }

            for( int i=0; i<m_Attri_AttackCount.Length; i++)
                m_Attri_AttackCount[i] = 0;
                       
            RefleshAttrimark();

            m_Timer.EndTime();
            m_TrunDrag = 0;
            m_Panel_Load.FadeOut();
            m_Map.StartPos();
            m_Map.MoveTo(-63, gameObject, "StartMapMoveEnd");
            m_GameStap = GameState.StartMove;
        }
        else if (m_GameStap == GameState.StartMove)
        {

        }
        else if (m_GameStap == GameState.GamIng)
        {   
            // 게임 승패 체크.
            // 유저 승리.       
            int nMonNullcount=0;
            for(int i=0; i<m_Monster.Length; i++)
            {
                if(m_Monster[i] == null) nMonNullcount++;                
            }            

            // 승리.
            if( m_Monster.Length == nMonNullcount )
            {
                m_Timer.EndTime();
                m_GameStap3 = 0;                
                m_GameStap = GameState.Victory;
                return ;
            }

            // 테스트에만 사용할 강제승리 
            if( DataMgr.Inst.TestKeyInput( KeyCode.Keypad4) )
            {
                for(int i=0; i<m_Monster.Length; i++)
                {
                    if(m_Monster[i] == null) continue;
                                                                
                    Destroy(m_Monster[i].gameObject);
                    m_Monster[i] = null;                    
                }
                
                m_GameStap3 = 0;
                m_GameStap = GameState.Victory;
                return;
            }


            // 패배.
            if( m_HpMgr.GetHp() <= 0 )
            {
                m_GameStap3 = 0;
                m_GameStap = GameState.Failed;                                
                return;
            }

            if(m_HpMgr.CheckHP_Warning() && !m_IsHpwarning )
            {
                m_Panel_Warning.Begin();
                m_IsHpwarning = true;
            }

            // 턴스타드.
            if(m_Turn == Turn.Player )
            {
                bool isDieMonster = false;
                for(int i=0; i<m_Monster.Length; i++)
                {
                    if(m_Monster[i] == null) continue;

                    if(m_Monster[i].GetDeath() && m_EffectBulletCount == 0)                        
                    {
                        Destroy(m_Monster[i].gameObject);
                        m_Monster[i] = null;
                        isDieMonster = true;
                    }
                }

                if(isDieMonster)
                    RefleshAttrimark();

                if(m_GameStap2 == 2)
                {                    
                    // 턴을 .
                    // 다음턴에 한명이라도 공격할 적이있으면 턴을 끈다.
                    // 없으면 그냥 넘긴다.
                    //다음턴에 공격할 적이 없으면 턴을 끌면 안된다.
                    // 타이밍 끝났을때.                    

                    if(m_Timer.GetIsTimeOn() == false)
                    {   
                        bool IsAtkTurn = false;    
                        for(int i=0; i<m_Monster.Length; i++)
                        {            
                            if ( m_Monster[i] == null ) continue;

                            if ( m_Monster[i].IsMyTurn() ) // 해당 몬스터가 공격한 차례인지.
                            {
                                IsAtkTurn = true;                                
                                break;
                            }
                        }

                        bool bExeTurn = false;
                        if(m_TrunDrag >= Defines.DEF_MAX_TRUN_DRAG)
                        {
                            if(IsAtkTurn) m_PuzzleMgr.Lock();
                            else bExeTurn = true;
                        }
                        else
                        {
                            if(m_TrunDrag > 0)
                                FiberON();
                        }
                        
  
                        if(m_EffectBulletCount == 0 || bExeTurn)
                        { 
                            m_TrunDrag = 0;
                            FiberOFF();
                            m_Timer.EndTime();
                            bool bTamp=true;
                            for(int i=0; i<m_SumSlot.Length; i++)
                            {
                                if(!m_SumSlot[i].gameObject.activeSelf) continue;
                                if(m_SumSlot[i].m_isSkilling) bTamp = false;
                            }
                                        
                            // 적에게 턴을 넘긴다.
                            if(bTamp == true) 
                                SetEnemyTurn();
                        }                        
                    }
                }
            }
            else if(m_Turn == Turn.Enemy )
            {
                if(m_GameStap2 == 0)
                {                    
                    StartCoroutine( EnemyAttack() );
                    m_GameStap2 = 1;
                }
                else if(m_GameStap2 == 1)
                {
                    bool bTamp=true;
                    for(int i=0; i<m_Monster.Length; i++)
                    {
                        if(m_Monster[i] == null) continue;
                        if(m_Monster[i].GetIsAttack()) bTamp = false;
                    }

                    // 아군에게 턴을 넘긴다.
                    if(bTamp)
                        SetUserTurn();
                    
                }
            }
        }
        else if (m_GameStap == GameState.Victory)
        {
            if(m_GameStap3 == 0)
            {
                m_PuzzleMgr.AllBallBoom();
                m_Map.MoveTo(-68, gameObject, "EndMapMoveEnd");                      
                m_GameStap3 = 1;
            }
        }
        else if(m_GameStap == GameState.Failed)
        {
            if(m_GameStap3 == 0)
            {
                DataMgr.Inst.m_SerMgr.AdventureEnd_Failed( DataMgr.Inst.m_DB_AdventureStage[DataMgr.Inst.m_SelectStageIndex].nIDX, AdventureEnd_Result);
                m_GameStap3 = 1;
            }
            
        }
    }
    
    // 게임 마무리. 처리.
    public void AdventureEnd_Result(bool a_Result, Dictionary<string, object> a_dicJson, string a_Str, serverManager.ErrorCode a_ErrorCode)
    {
        if(a_Result == true)
        { 
            DataMgr.Inst.m_UserInfo.Crown = DataMgr.Inst.CastInt( a_dicJson["Crown"] );
            DataMgr.Inst.m_Accrue._AdventureFailed++;
            GameOver();
        }        
    }

    // 다음 라운드로 이동한다.
    void EndMapMoveEnd()
    {
        m_Panel_Load.gameObject.SetActive(true);
        if(m_Round == m_RoundMax)        
            m_GameStap = GameState.NextStage;        
        else        
            m_GameStap = GameState.LoadRound;
        
        m_GameStap2 = 0;
        m_GameStap3 = 0;
    }

    //턴 지정------------------------------------------------------.
    void SetUserTurn()
    {
        m_GameStap2 = 1;
        m_PuzzleMgr.UkLock();
        m_Turn = Turn.Player;
 
        for(int i=0; i<m_SumSlot.Length; i++)
        {
            if (m_SumSlot[i].gameObject.activeSelf)
            {              
                m_SumSlot[i].OnOver();
            }
        }        
    }

    void SetEnemyTurn()
    {   
        for(int i=0; i<m_SumSlot.Length; i++)
        {
            if (m_SumSlot[i].gameObject.activeSelf)
            {              
                m_SumSlot[i].OffOver();
            }
        }

//        for(int i=0; i<m_Monster.Length; i++)
//        {            
//            if ( m_Monster[i] == null ) continue;

//            if ( m_Monster[i].IsMyTurn() ) // 해당 몬스터가 공격한 차례인지.
//            {
//                m_PuzzleMgr.Lock();
//                break;
//            }
//        }

        m_Turn = Turn.Enemy; 
        m_GameStap2 = 0;   
    }

    //시작시 맵이동.
    void StartMapMoveEnd()
    {
        if( m_GameStap == GameState.StartMove || m_GameStap2 == 0 )
        {
            // 게임 시작 셋팅.
            m_PuzzleMgr.StartPuzzle( DeleteBall, Delegate_ClickBall, GetGoldEnd);

            for(int i=0; i<m_Monster.Length; i++) 
            {
                if(m_Monster[i] == null) continue;
                m_Monster[i].Action_Entrance();
            }

            SetUserTurn();
            m_GameStap = GameState.GamIng;
        }
    }

    //유저 턴관련 ----------------------------------------------------------------.
    // 내턴시작.
    // 공을 클릭했을때 호출됨.
    // 공을처음 클릭했을때 처리.
    public void Delegate_ClickBall( int a_Index )
    {
        if(m_Turn == Turn.Player && m_GameStap2 == 1)
        {                
            m_Timer.Begin(gameObject, "TurnEnd");
            
            for(int i=0; i<m_Monster.Length; i++)
            {                
                if(m_Monster[i] != null)
                { 
                    m_Monster[i].Count_Turn();
                }                    
            }
            m_GameStap2 = 2;
        }
    }
    
    private  List<int> m_TempMonAtkList = new List<int>();
    //소환사가 적을 공격한다.
    public void DeleteBall(int a_Index, int a_nCount)
    {
        // 인공지능.
        // 속성 50% => 레벨 30% => 체력 20%
        if(m_GameStap != GameState.GamIng) return;
        if (a_Index == -1) return;

        int TeamSum = -1;
        DataMgr.tagUserSummon Summon = null;
        
        for (int i = 0; i < Defines.DEF_MAX_TEAM_ITEM; i++) 
        {
            if( DataMgr.Inst.m_UserInfo.Team[DataMgr.Inst.m_TeamIdx][i] == a_Index)
            {
                TeamSum = i;           
                Summon = m_SumSlot[i].m_Info;
                m_SumSlot[i].AddGauge();
                break;
            }
        }        
        
        int nSelectMonster = SkillBrain(TeamSum);
        if(nSelectMonster == -1) return;

        m_Attri_AttackCount[(int)Summon.Data.eAttr] += a_nCount;
        PlayerAttack( TeamSum, nSelectMonster, a_nCount);          
    }

    // 돈올라간다.
    void GetGoldEnd()
    {
        m_CoinSound.Play();
        m_PlayGetGold++;
        m_lbMoney.text = m_PlayGetGold.ToString();
    }

    //적 턴관련 ----------------------------------------------------------------.
    // 적이 아군을 공격한다.
    //[System.NonSerialized] public int m_AttackEndCount;
    IEnumerator EnemyAttack()
    {
        yield return new WaitForSeconds(0.1f);
        int Idx=0;        
        for(int i=0; i<Defines.DEF_ATTACK_ORDER.Length; i++)
        {
            Idx = Defines.DEF_ATTACK_ORDER[i];
            
            if ( Idx == 5) Idx = 2;
            if ( m_Monster[Idx] == null ) continue;

            if ( m_Monster[Idx].m_IndexPos == Defines.DEF_ATTACK_ORDER[i] &&
                 m_Monster[Idx].IsMyTurn() ) // 해당 몬스터가 공격한 차례인지.
            {                
                m_Monster[Idx].Attack();
                yield return new WaitForSeconds(0.2f);
            }
        }
        yield return null;
    }
    
    
    //-------------------------------------------------------------
    void GameOver()
    {
        m_GameOver.SetActive(true);
                
        TweenAlpha TA = m_GameOver.AddComponent<TweenAlpha>();       
        TA.from               = 0.0f;        
        TA.to                 = 1.0f;                    
        TA.style              = UITweener.Style.Once;
        TA.duration           = 3;
        TA.method             = UITweener.Method.Linear;        
        TA.callWhenFinished   = "GameOver2";
        TA.eventReceiver      = gameObject;
        m_PuzzleMgr.GameOver();
    }
    void GameOver2() {}
    public void onBtn_GameOver()
    {
        BtnPause_Ok();
    }
        
    //-------------------------------------------------------------
    //.
    void onNextStage()
    {
        if(m_GameStap == GameState.NextStage)
        {
            if( m_GameStap2 == 1)
                m_GameStap2 = 2;
        }
    }

    void onRetry()
    {        
        if(m_GameStap == GameState.NextStage)
        {   
            if( m_GameStap2 == 1)
                m_GameStap2 = 3;
        }
    }

    void BtnShuffle()
    {        
        m_PuzzleMgr.ForceBall( (BattlePuzzleMgr.emForceBallDrt)Random.Range(0, 3) );
    }

    //.
    public void BtnPause()
    {
        m_Msg.SetActive(true);
        m_PuzzleMgr.Lock();
    }

    public void onCrownLack(bool b)
    {
        DataMgr.Inst.GetLobby_On();
        Destroy(gameObject);
    }

    private bool KeyExit = false;
    public void BtnPause_Ok()
    {
        if(KeyExit == false)
        {
            DataMgr.Inst.m_SerMgr.AdventureEnd_Failed(DataMgr.Inst.m_DB_AdventureStage[DataMgr.Inst.m_SelectStageIndex].nIDX, BtnPause_Ok_AdventureEnd_Result);
            KeyExit = true;
        }
    }
    public void BtnPause_Ok_AdventureEnd_Result(bool a_Result, Dictionary<string, object> a_dicJson, string a_Str, serverManager.ErrorCode a_ErrorCode)
    {
        if(a_Result == true)
        {
            DataMgr.Inst.m_UserInfo.Crown = DataMgr.Inst.CastInt( a_dicJson["Crown"] );
            if(a_dicJson.ContainsKey("pLevel") ) DataMgr.Inst.m_UserInfo._Level = DataMgr.Inst.CastInt( a_dicJson["pLevel"] );

            DataMgr.Inst.m_Accrue._AdventureFailed++;
            DataMgr.Inst.GetLobby_On();
            Destroy(gameObject);
        }
    }

    public void BtnPause_Cansel()
    {
        m_Msg.SetActive(false);
        m_PuzzleMgr.UkLock();
    }
}
