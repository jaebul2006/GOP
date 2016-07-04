using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattlePuzzleMgr : MonoBehaviour 
{
//    class tagAttriCount
//    {
//        public DataMgr.emCardAttribute Attri;
//        public int Count;
//    };

    public class BallListInfo
    {
        public int IDX;
        public DataMgr.emCardAttribute Attri;

        public int BoomNumber()
        {
            if (IDX != -1)
            {
                return DataMgr.Inst.m_UserSummonList[IDX].Data.nIDX;
            }
            else
            {
                return (int)Attri;
            }
        }
    };

    public  AudioSource         m_SndCombo1;
    public  AudioSource         m_SndCombo2;
    public  AudioSource         m_SndCombo3;
    public  AudioSource         m_SndCombo4;

    public  AudioSource         m_SndPuzzleFff1;
    public  AudioSource         m_SndPuzzleFff2;

    public  GameObject          m_UIRoot;
    public  Camera              m_BattleCamera;
    public  UIWidget            m_FireBallPos;  // 공배출위치.
    public  GameObject          m_BG;
    public  EdgeCollider2D      m_EdgeCollider2D;
    public  GameObject          m_CoinTarget;

    private int                 m_MaxLine = 7;
    private int                 m_StreenSizeX;
    private int                 m_StreenSizeY;
    private GameObject          m_ObjBall;          // 볼 프리팹.
    private GameObject          m_ObjBallLine;      // 선을 그릴 프리팹.
    private int                 m_BallRadis;        // 볼의 반지름.
    private Vector2 []          m_LineFireBall;     // 발사할 공 라인당 위치.
    private List<Ball>          m_BoolList;         // 생성한 전체 볼리스트.
    private int                 m_BallNaming = 0;
    private List<BallListInfo>  m_BallIdxList;  // 떨어트릴 볼리스트.
//    private List<tagAttriCount> m_liAttriOrder = new List<tagAttriCount>(); // 가정 적게쓰는 속성 정렬.

    private bool                m_bLMousebtnKey = false;
    private Vector2             m_vTouch = new Vector2(); // 클릭 및 터치 위치.   

    private List<Ball>          m_DregList;     // 드레그한 공 리스트.
    private List<GameObject>    m_DregLineList; // 드레그할때 생기는 선 리스트.
    
    // 공 추가에 사용할 변수.
    private BallListInfo        m_AddFire_Idx = null;
    private int                 m_AddFire_Count = 0;
    private int []              m_ShufflePosIdx;
    private bool []             m_bCheckOverBallKey; // 공을 제거할수 있는지 검사할것인지의 여부.
    
    private bool                m_bGoPuzzleStart; // 맨처음 창이동때.
    private bool                m_Lock = false;

    public void Lock() 
    { 
        m_Lock = true; 

        foreach( GameObject Obj in m_DregLineList)
            Destroy(Obj);
        
        m_DregLineList.Clear();

        foreach( Ball Obj in m_DregList)        
            Obj.SetToggle(false);                                        
        
        m_DregList.Clear();        
    }

    public void UkLock() 
    { 
        m_Lock = false; 
    }


    // 공을 없앴때 마다 호출한다.
    public delegate void Delegate_DeleteBall( int a_Index, int a_nCount );
    public Delegate_DeleteBall m_Delegate_DeleteBall = null;    

    // 공을 클릭할때 호출.
    public delegate void Delegate_ClickBall( int a_Index );
    public Delegate_ClickBall m_Delegate_ClickBall = null;    
    
    public Defines.Delegate_None m_Delegate_GetGoldEnd;


    public void StartPuzzle(Delegate_DeleteBall a_Delegate_DeleteBall, 
        Delegate_ClickBall a_Delegate_ClickBall,
        Defines.Delegate_None a_Delegate_GetGoldEnd )
    {
        m_Delegate_DeleteBall   = a_Delegate_DeleteBall;
        m_Delegate_ClickBall    = a_Delegate_ClickBall;
        m_Delegate_GetGoldEnd   = a_Delegate_GetGoldEnd;
        StartFire(5);
    }    

    void Awake()
    {
        m_DregList      = new List<Ball>();
        m_DregLineList  = new List<GameObject>();
        m_BoolList      = new List<Ball>();
        m_ObjBall       = Resources.Load("Prefabs/Ball") as GameObject;
        m_ObjBallLine   = Resources.Load("Prefabs/BallLine") as GameObject;

        // 갯수 개산 및 사이즈 계산.
        m_BallRadis = m_ObjBall.GetComponent<UITexture>().width;
        m_MaxLine = (m_FireBallPos.width - 2) / m_BallRadis;

        // float fRedius =  / m_MaxLine;
        // m_ObjBall.GetComponent<UITexture>().width = (int)fRedius;
        // m_ObjBall.GetComponent<UITexture>().height = (int)fRedius;
        // m_ObjBall.GetComponent<CircleCollider2D>().radius = fRedius/2f;
        // m_ObjBall.transform.FindChild("Toggle").GetComponent<UISprite>().width = (int)fRedius;
        // m_ObjBall.transform.FindChild("Toggle").GetComponent<UISprite>().height = (int)fRedius;

        m_LineFireBall  = new Vector2[m_MaxLine];        
        m_ShufflePosIdx = new int[m_LineFireBall.Length];        
        
        m_StreenSizeX = m_UIRoot.GetComponent<UIRoot>().minimumHeight;
        m_StreenSizeY = m_UIRoot.GetComponent<UIRoot>().maximumHeight;

        m_bCheckOverBallKey = new bool[4];


        
        /*/ 없는 속성 찿아낸다.
        for (int i = 0; i<(int)DataMgr.emCardAttribute.emCAB_END; i++)
        {
            tagAttriCount ac = new tagAttriCount();
            ac.Attri = (DataMgr.emCardAttribute)i;
            ac.Count = 0;
            m_liAttriOrder.Add(ac);
        }

        for (int i=0; i< Defines.DEF_MAX_TEAM_ITEM; i++)
        {
            if (DataMgr.Inst.m_UserInfo.Team[DataMgr.Inst.m_TeamIdx][i] == -1) continue;
            foreach (tagAttriCount Obj in m_liAttriOrder)
            {                
                if(Obj.Attri == DataMgr.Inst.m_UserSummonList[DataMgr.Inst.m_UserInfo.Team[DataMgr.Inst.m_TeamIdx][i]].Data.eAttr)
                {
                    Obj.Count += 1;
                }
            }
        }

        //X => X.Count > X.Count
        m_liAttriOrder.Sort(delegate (tagAttriCount x, tagAttriCount y)
        {
            if (x == null || y == null) return 0;
            return x.Count.CompareTo(y.Count);                        
        });*/

        m_BallIdxList = new List<BallListInfo>();

        // 값을 넣는다.
        int ObjCount = 0;
        for (int i = 0; i < Defines.DEF_MAX_TEAM_ITEM; i++)
        {
            int Index = DataMgr.Inst.m_UserInfo.Team[DataMgr.Inst.m_TeamIdx][i];
            BallListInfo Item = new BallListInfo();
            Item.IDX = Index;
            if (Index != -1)
            {
                Item.Attri = DataMgr.Inst.m_UserSummonList[Index].Data.eAttr;
            }
            else
            {
                Item.Attri = DataMgr.Inst.m_liNotUseAttriOrder[ObjCount].Attri;
                ObjCount++;
            }

            m_BallIdxList.Add(Item);
        }

        for (int i=0; i<m_LineFireBall.Length; i++)
        {
            m_ShufflePosIdx[i] = i;
            m_LineFireBall[i].Set(
                2 + (m_FireBallPos.transform.localPosition.x + (m_BallRadis / 2)) + (i* m_BallRadis),
                m_FireBallPos.transform.localPosition.y - (m_BallRadis / 2));
        }
    }

	void Start()
    {        
	}

    public void AllBallBoom()
    {
        foreach( GameObject Obj in m_DregLineList)                    
            Destroy(Obj);
        
        
        m_DregLineList.Clear();

        foreach( Ball Obj in m_DregList)        
            Obj.SetToggle(false);                                        
        
        m_DregList.Clear();        

        foreach( Ball Obj in m_BoolList)
        {            
            Obj.Exeplotion();            
        }
        m_BoolList.Clear();
    }

    public void GameOver()
    {
        m_EdgeCollider2D.enabled = false;
        Invoke("GameOver2", 2.0f);
    }

    
    void GameOver2()
    {
        foreach( GameObject Obj in m_DregLineList)
        Destroy(Obj);
        
        m_DregLineList.Clear();

        foreach( Ball Obj in m_DregList)        
            Obj.SetToggle(false);                                        
        
        m_DregList.Clear();        

        foreach( Ball Obj in m_BoolList)
        {
            Destroy(Obj.gameObject);
        }
        m_BoolList.Clear();
    }


    public enum emForceBallDrt { FBD_LEFT, FBD_CENTER, FBD_RIGHT };
    public void ForceBall(emForceBallDrt a_ForceBallDrt)
    {                
        float X = 0.0f;
        int DrtRnad = 0;

        foreach (Ball NowBall in m_BoolList)
        {                
            X = (float)Random.Range(50, 500) / 100.0f;
            DrtRnad = Random.Range(0, 100);

            if(a_ForceBallDrt == emForceBallDrt.FBD_LEFT) { if(DrtRnad < 75) X *= -1; }
            if(a_ForceBallDrt == emForceBallDrt.FBD_CENTER) { if(DrtRnad < 50) X *= -1; }            
            if(a_ForceBallDrt == emForceBallDrt.FBD_RIGHT) { if(DrtRnad < 25) X *= -1; }

            NowBall.GetComponent<Rigidbody2D>().AddForce(
                new Vector2( X, 
                    ((NowBall.transform.localPosition.y*-1) + (float)(Random.Range(30, 100)))/300.0f ), 
                    ForceMode2D.Impulse );
        }                        
    }

    private int m_ForceBallStap = 0;    
    public void OnForceBall()
    {
        if(m_ForceBallStap == 0)
        {
            ForceBall(emForceBallDrt.FBD_LEFT);
            m_ForceBallStap = 1;            
        }
        else if(m_ForceBallStap == 1)
        {
            ForceBall(emForceBallDrt.FBD_CENTER);
            m_ForceBallStap = 2;            
        }
        else if(m_ForceBallStap == 2)
        {
            ForceBall(emForceBallDrt.FBD_RIGHT);
            m_ForceBallStap=0;            
        }        
    }
    
    private int m_nBollFaceOffLimit;
    private int m_nBollFaceOff_Idx=0;
	void Update() 
    {  
        if(m_Lock ) return;

        if (DataMgr.Inst.TestKeyInput(KeyCode.W)) ForceBall(emForceBallDrt.FBD_LEFT);
        if (DataMgr.Inst.TestKeyInput(KeyCode.E)) ForceBall(emForceBallDrt.FBD_CENTER);
        if (DataMgr.Inst.TestKeyInput(KeyCode.R)) ForceBall(emForceBallDrt.FBD_RIGHT);
        
        bool bPushing = false;        

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    
        bPushing    = Input.GetMouseButton(0);
        
        if (bPushing)
        {                            
//            m_vTouch.x = Input.mousePosition.x-( m_StreenSizeX /2f);
//            m_vTouch.y = Input.mousePosition.y-( m_StreenSizeY /2f);

            m_vTouch = m_BattleCamera.ScreenToWorldPoint(Input.mousePosition); 

            m_vTouch.x -= m_UIRoot.transform.localPosition.x;
            m_vTouch.y -= m_UIRoot.transform.localPosition.y;

            m_vTouch.x /= m_UIRoot.transform.localScale.x;
            m_vTouch.y /= m_UIRoot.transform.localScale.y;
        }

#else
        int nTouchCount = Input.touchCount;
        for (int i = 0; i < nTouchCount; i++)
		{
		    Touch touch = Input.GetTouch(i);
		    
		    if (touch.phase == TouchPhase.Ended || 
                touch.phase == TouchPhase.Moved || 
                touch.phase == TouchPhase.Began ||
                touch.phase == TouchPhase.Stationary)
		    {		    	
//                m_vTouch.x = touch.position.x-( m_StreenSizeX /2f);
//                m_vTouch.y = touch.position.y-( m_StreenSizeY /2f);

                m_vTouch = m_BattleCamera.ScreenToWorldPoint(touch.position); 

                m_vTouch.x -= m_UIRoot.transform.localPosition.x;
                m_vTouch.y -= m_UIRoot.transform.localPosition.y;

                m_vTouch.x /= m_UIRoot.transform.localScale.x;
                m_vTouch.y /= m_UIRoot.transform.localScale.y;

		    	bPushing = true;
                break;
		    }
		}    
#endif
        
        if(m_nBollFaceOffLimit < System.Environment.TickCount)
        {
            int IDX=0;
            for (int i = 0; i < Defines.DEF_MAX_TEAM_ITEM; i++)        
            {
                if(DataMgr.Inst.m_UserInfo.Team[DataMgr.Inst.m_TeamIdx][m_nBollFaceOff_Idx] != -1)
                {
                    IDX = DataMgr.Inst.m_UserInfo.Team[DataMgr.Inst.m_TeamIdx][m_nBollFaceOff_Idx];
                    m_nBollFaceOff_Idx++;
                    m_nBollFaceOff_Idx %= Defines.DEF_MAX_TEAM_ITEM;
                    break;
                }
                else
                { 
                    m_nBollFaceOff_Idx++;
                    m_nBollFaceOff_Idx %= Defines.DEF_MAX_TEAM_ITEM;
                }
                    
            }
                
            m_nBollFaceOffLimit = System.Environment.TickCount + (Random.Range(5, 10) *1000);
            
            foreach( Ball Obj in m_BoolList)
            {
                if(Obj.GetIdx().IDX == IDX)
                {                                        
                    Obj.FaceOff();
                                        
                }
            }
        }        


        if(bPushing)
        {        
            foreach (Ball NowBall in m_BoolList)
            {                                            
                if(NowBall.CheckMouseInBall(m_vTouch))
                {
                    if(m_Delegate_ClickBall != null) m_Delegate_ClickBall( NowBall.GetIdx().IDX );

                    if(m_DregList.Count > 0 )
                    {                         
                        Ball BeforeBall = m_DregList[m_DregList.Count-1];
                        if(m_DregList.Count > 1 && m_DregList[m_DregList.Count-2].name ==  NowBall.name)
                        {                           
                            GameObject Line = m_DregLineList[m_DregLineList.Count-1];                            
                            m_DregLineList.Remove(Line);
                            Destroy(Line);
                            
                            BeforeBall.SetToggle(false);
                            m_DregList.Remove(BeforeBall);
                        }
                        else if(m_DregList[0].GetIdx().BoomNumber() == NowBall.GetIdx().BoomNumber() && NowBall.IsToggle()== false)
                        {   
                            // 정보계산.                                                                                    
                            Vector2 vStart = BeforeBall.transform.position;
                            Vector2 vEnd   = NowBall.transform.position;

                            // 각도계산.
                            float fDist_l;
                            float Radian = (float)getAngle(BeforeBall.transform.localPosition, NowBall.transform.localPosition, out fDist_l);
                            Radian = Radian/(float)(System.Math.PI/180.0);

                            // 거리계산.
                            float fDist_w = 0;                                                        
                            fDist_w = Vector2.Distance(vStart, vEnd);

                            // 충돌체크.                      
                            Vector2 vDerection = new Vector2();
                            vDerection = vEnd - vStart;
                            RaycastHit2D [] Hit = Physics2D.RaycastAll( vStart, vDerection, fDist_w);

                            bool bBreakKey = false;
                            if(Hit != null && Hit.Length > 0)
                            {                                
                                for(int i=0; i<Hit.Length; i++)
                                {
                                    Ball Temp = Hit[i].collider.gameObject.GetComponent<Ball>();
                                    if(Temp != null ) 
                                    {                       
                                        if(NowBall.name != Temp.name && BeforeBall.name != Temp.name)
                                        { 
                                            if(Temp.GetIdx().BoomNumber() != BeforeBall.GetIdx().BoomNumber())
                                            { bBreakKey = true; break; }

                                            if(Temp.IsToggle() )
                                            { bBreakKey = true; break; }

                                            if(!Temp.IsToggle() && Temp.GetIdx().BoomNumber() == BeforeBall.GetIdx().BoomNumber())
                                            { bBreakKey = true; break; }
                                        }
                                    }
                                }
                            }
                            if(bBreakKey) break;

                            // 선만들기.
                            GameObject Obj              = Instantiate(m_ObjBallLine) as GameObject;
                            Obj.transform.parent        = gameObject.transform;
                            Obj.GetComponent<BallLine>().SetUp(BeforeBall.gameObject, NowBall.gameObject);
                            m_DregLineList.Add( Obj );
                            NowBall.SetToggle(true);
                            m_DregList.Add( NowBall );
                        }
                    }
                    else
                    {
                        NowBall.SetToggle(true);
                        m_DregList.Add( NowBall );
                    }
                        
                    break;
                }
            } 

            m_bLMousebtnKey = true;   
        }
        else if(m_bLMousebtnKey)
        {
            foreach( GameObject Obj in m_DregLineList)                          
                Destroy(Obj);
            
            m_DregLineList.Clear();

            if(m_DregList.Count > 2)
            {                 
                m_AddFire_Count = 0;
                m_AddFire_Idx   = null;      
                foreach( Ball Obj in m_DregList)
                {                    
                    m_AddFire_Idx = Obj.GetIdx();
                    m_AddFire_Count++;

                    Obj.SetToggle(false);
                    Obj.Exeplotion();
                    m_BoolList.Remove(Obj);
                    
                    if(m_AddFire_Idx.IDX != -1)
                        InitCoin(Obj.gameObject.transform.localPosition, 3);
                }
                m_DregList.Clear();

                if(m_Delegate_DeleteBall != null)                
                    m_Delegate_DeleteBall(m_AddFire_Idx.IDX, m_AddFire_Count);
                
                if(m_AddFire_Count <= 4) m_SndCombo1.Play();
                else if(m_AddFire_Count <= 6) m_SndCombo2.Play();
                else if(m_AddFire_Count <= 8) m_SndCombo3.Play();
                else m_SndCombo4.Play();
                
                if(m_AddFire_Count < 7) m_SndPuzzleFff1.Play();
                else m_SndPuzzleFff2.Play();
                

                AddFire();// 공추가.
            }
            else
            {
                foreach( Ball Obj in m_DregList)
                {
                    Obj.SetToggle(false);                                        
                }
                m_DregList.Clear();
            }

            m_bLMousebtnKey = false;
        }

        if(DataMgr.Inst.TestKeyInput( KeyCode.A ))
        {
            m_bCheckOverBallKey[0] = true;
            m_bCheckOverBallKey[1] = false;
            m_bCheckOverBallKey[2] = false;
            m_bCheckOverBallKey[3] = false;
            CheckOverBall(false);
        }
        else if(DataMgr.Inst.TestKeyInput( KeyCode.S ))
        {
            m_bCheckOverBallKey[0] = false;
            m_bCheckOverBallKey[1] = true;
            m_bCheckOverBallKey[2] = false;
            m_bCheckOverBallKey[3] = false;
            CheckOverBall(false);
        }
        else if(DataMgr.Inst.TestKeyInput( KeyCode.D ))
        {
            m_bCheckOverBallKey[0] = false;       
            m_bCheckOverBallKey[1] = false;
            m_bCheckOverBallKey[2] = true;
            m_bCheckOverBallKey[3] = false;
            CheckOverBall(false);
        }        
        else if(DataMgr.Inst.TestKeyInput( KeyCode.F ))
        {
            m_bCheckOverBallKey[0] = false;
            m_bCheckOverBallKey[1] = false;
            m_bCheckOverBallKey[2] = false;
            m_bCheckOverBallKey[3] = true;
            CheckOverBall(false);
        }
        if(DataMgr.Inst.TestKeyInput( KeyCode.Q )) // 끄기.
        {
            foreach(Ball Ball in m_BoolList )
            {
                Ball.m_VirtualToggle_State = 0;
                Ball.m_bVirtualToggle = false;
                Ball.SetToggle(false);
            }
        }

        // 자동퍼즐 .
        if(DataMgr.Inst.TestKeyInput( KeyCode.Z )) AutoDelete( 0 );
        if(DataMgr.Inst.TestKeyInput( KeyCode.X )) AutoDelete( 1 );
        if(DataMgr.Inst.TestKeyInput( KeyCode.C )) AutoDelete( 2 );
        if(DataMgr.Inst.TestKeyInput( KeyCode.V )) AutoDelete( 3 );
            
        

	}
   
    // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-= 
    // 돈 이펙트를 생성하는 함수.
    void InitCoin(Vector3 vPos, int a_nCount)
    {
        for(int i=0; i<a_nCount; i++)
        {
            GameObject GameCoin = Instantiate( Resources.Load("Prefabs/GameCoin") ) as GameObject;
            GameCoin.transform.parent       = m_UIRoot.transform.FindChild("Panel_UI");            
            GameCoin.transform.localScale   = new Vector3( 1f, 1f, 1f);
            GameCoin.transform.localPosition = new Vector3(vPos.x, vPos.y, vPos.z);
            GameCoin.GetComponent<UISprite>().depth = 100;
            
            GainCoinDrop Script = GameCoin.GetComponent<GainCoinDrop>();
            Script._projectile  = GameCoin.transform;
            Script._my_tm       = GameCoin.transform;
            Script.m_Target     = m_CoinTarget;
            Script.m_Send       = m_Delegate_GetGoldEnd;

            Script.DirSet( Random.Range(0, 2)==0 ? true:false );
        }        
    }

    
    public void CheckHint()
    {
         StartCoroutine(coCheckHint());
    }

    IEnumerator coCheckHint()
    {
        m_bCheckOverBallKey[0] = false;
        m_bCheckOverBallKey[1] = false;
        m_bCheckOverBallKey[2] = false;
        m_bCheckOverBallKey[3] = false;
        m_bCheckOverBallKey[ Random.Range(0, m_BallIdxList.Count) ] = true;
        CheckOverBall(false);

        yield return new WaitForSeconds( 3.0f );

        foreach(Ball Ball in m_BoolList )
        {
            Ball.m_VirtualToggle_State = 0;
            Ball.m_bVirtualToggle      = false;
            Ball.SetToggle(false);
        }
    }

    static public double getAngle( Vector2 value1, Vector2 value2, out float a_Dist)
    { 
        float dist_squt = Vector2.Distance(value1 , value2);        
        a_Dist = dist_squt;
        float dist_y = value2.y - value1.y;
        float dist_x = value2.x - value1.x;        
        double Angle = System.Math.Atan2( dist_y, dist_x);

        if(Angle < 0.0f )
        {
        //    Angle+=System.Math.PI;
        }
        return Angle;
    }



    /// <summary>
    /// 발사한다 공을.
    /// </summary>
    /// <param name="count"></param>
    public void StartFire( int a_Count )
    {
        for(int i=0; i<a_Count; i++)
        {            
            for(int j=0; j<m_LineFireBall.Length; j++)
            {                
                GameObject Obj = Instantiate( m_ObjBall) as GameObject;
                Obj.transform.parent = gameObject.transform;
                Obj.name = "Ball_"+m_BallNaming;
                m_BallNaming++;
                                
                Ball BALL = Obj.GetComponent<Ball>();                
                BALL.SetUp(m_UIRoot, m_BallIdxList[Random.Range(0, m_BallIdxList.Count)],
                    m_LineFireBall[j].x, m_LineFireBall[j].y+( i*m_BallRadis ) );
                m_BoolList.Add(BALL);
            }
        }
    }
    

    // 랜덤위치에서 공이 떨어진다.        
    void AddFire()
    {
        BallListInfo[] FallBallStack;                   // 떨어트릴공의 리스트        
        int BallCauntAtLine = m_LineFireBall.Length>>1; // 한라인당 떨어질 볼갯수.                            
        
        // 리스트 떨구어야할 공보다 작으면 동적할당.        
        FallBallStack = new BallListInfo[m_AddFire_Count]; 

        // 값 셋팅.
        int nEqueCount = m_AddFire_Count/3;
        for (int i=0; i<FallBallStack.Length; i++)
        {
            if (i < nEqueCount)
                FallBallStack[i] = m_AddFire_Idx;
            else
                FallBallStack[i] = m_BallIdxList[Random.Range(0, m_BallIdxList.Count)];
        }
        
        //석기.
        int nMax = FallBallStack.Length*3;
        int Dest = 0, Sour = 0;
        BallListInfo Temp = null;
        for(int i=0; i<nMax; i++)
        {
            Dest = Random.Range(0, FallBallStack.Length);
            Sour = Random.Range(0, FallBallStack.Length);
            Temp = FallBallStack[Sour];
            FallBallStack[Sour] = FallBallStack[Dest];
            FallBallStack[Dest] = Temp;
        }
 
        
                                
        int LineCount = -1;
        int nPosindex = 0;
        for(int i=0; i<FallBallStack.Length; i++)
        {
            GameObject Obj = Instantiate( m_ObjBall) as GameObject;
            Obj.transform.parent = gameObject.transform;
            Obj.name = "Ball_"+m_BallNaming;
            m_BallNaming++;

            Ball BALL = Obj.GetComponent<Ball>();
            nPosindex = i % BallCauntAtLine;
            if( nPosindex == 0)
            {
                Shuffle();
                LineCount++;
            }
            
            BALL.SetUp(m_UIRoot, FallBallStack[i], m_LineFireBall[m_ShufflePosIdx[nPosindex]].x, 
                m_LineFireBall[m_ShufflePosIdx[nPosindex]].y + (LineCount*m_BallRadis) );
            m_BoolList.Add(BALL);            
        }
    }

    // 공을 떨어트릴 위치를 섞는다.
    void Shuffle()
    {
        int Dest = 0, Sour = 0, Temp = 0;
        for (int i=0; i<m_ShufflePosIdx.Length; i++)
        {
            Dest = Random.Range(0, m_ShufflePosIdx.Length);
            Sour = Random.Range(0, m_ShufflePosIdx.Length);
            Temp = m_ShufflePosIdx[Sour];
            m_ShufflePosIdx[Sour] = m_ShufflePosIdx[Dest];
            m_ShufflePosIdx[Dest] = Temp;
        }
    }

    // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    // 공을 무작위로 체인지한다.
    public void ChangeBallCount( int a_TeamIdx, int a_nCount)
    {        
        BallListInfo BallIdx = null;
        foreach (BallListInfo Obj in m_BallIdxList)
        {
            if(a_TeamIdx == Obj.IDX)
            {
                BallIdx = Obj;
                break;
            }
        }

        if(BallIdx == null) return;

        int nCount = a_nCount;
        foreach ( Ball Obj in m_BoolList )
        {
            if (Obj.GetIdx().IDX != a_TeamIdx)
            {
                Obj.Reset(BallIdx);
                nCount--; if(nCount < 0) break;
            }
        }
    }

    //-=-=-=-=-=-=-=-=-=-=-=-=-=-==-=
    // 주변사방검사. 
    // 제거할수 있는 공이 잇는지 검사. 
    // 하나의 색상씩 검사한다. 
    // 모두 검사 랜덤하게 한줄 검사.     

    //없앴을수 있는 모든공을 검사.    
    // m_bCheckOverBallKey 없애고 싶은 위치에 true를 넣는다.
    // m_BallIdxList   이변수에 공 종류 저장되있음
    void CheckOverBall(bool a_OneAreaOver)
    {
        int vCount = 0;
        foreach(Ball Ball in m_BoolList )
        {
            Ball.m_VirtualToggle_State = 0;
            Ball.m_bVirtualToggle = false;
        }
        
        for(int i=0; i<m_BallIdxList.Count; i++)
        {
            if(!m_bCheckOverBallKey[i]) continue;
            foreach(Ball Ball_1 in m_BoolList )
            {
                if (Ball_1.GetIdx().BoomNumber() != m_BallIdxList[i].BoomNumber()) continue;
                
                vCount = 1;
                RecallFunc_CheckOverBall(Ball_1, m_BallIdxList[i], ref vCount);

                foreach(Ball Ball_Check in m_BoolList )
                {
                    if (Ball_Check.GetIdx().BoomNumber() != m_BallIdxList[i].BoomNumber()) continue;
                            
                    if( Ball_Check.m_VirtualToggle_State == 1 )
                    {
                        Ball_Check.m_VirtualToggle_State = 2;
                        if(vCount > 2 && Ball_Check.m_bVirtualToggle)
                        {
                            Ball_Check.SetToggle(true);                            
                            if(a_OneAreaOver) return;
                        }
                    }
                }
            }
        }
    }    
    
    // 
    // 0~3을 입력한다.
    private List<Ball>          m_AutoDregList = new List<Ball>();     // 드레그한 공 리스트.
    void AutoDelete(int a_BallIdx)
    {
        if(a_BallIdx < 0 || a_BallIdx > 3) return;
        int vCount = 0;
        foreach(Ball Ball in m_BoolList )
        {
            Ball.m_VirtualToggle_State = 0;
            Ball.m_bVirtualToggle = false;
        }        

        bool bDel = false;
        m_AutoDregList.Clear();
        foreach(Ball Ball_1 in m_BoolList )
        {
            if (Ball_1.GetIdx().BoomNumber() != m_BallIdxList[a_BallIdx].BoomNumber()) continue;                
            vCount = 1;
            RecallFunc_CheckOverBall(Ball_1, m_BallIdxList[a_BallIdx], ref vCount);

            foreach(Ball Ball_Check in m_BoolList )
            {
                if (Ball_Check.GetIdx().BoomNumber() != m_BallIdxList[a_BallIdx].BoomNumber()) continue;
                        
                if( Ball_Check.m_VirtualToggle_State == 1 )
                {
                    Ball_Check.m_VirtualToggle_State = 2;
                    if(vCount > 2 && Ball_Check.m_bVirtualToggle)
                    {
                        if(m_Delegate_ClickBall != null) m_Delegate_ClickBall( Ball_Check.GetIdx().IDX );
                        m_AutoDregList.Add( Ball_Check );
                        bDel = true;
                    }
                }
            }
            if(bDel) break;
        }

        if(bDel == false) return;

        m_AddFire_Count = 0;
        m_AddFire_Idx   = null;

        foreach( Ball Obj in m_AutoDregList)
        {                    

            m_AddFire_Idx = Obj .GetIdx();
            m_AddFire_Count++;                            
            Obj .SetToggle(false);
            Obj .Exeplotion();
            m_BoolList.Remove(Obj );
                
            if(m_AddFire_Idx.IDX != -1)
                InitCoin(Obj.gameObject.transform.localPosition, 3);                                            
        }

        if(m_Delegate_DeleteBall != null)                
            m_Delegate_DeleteBall(m_AddFire_Idx.IDX, m_AddFire_Count);
        
        if(m_AddFire_Count <= 4) m_SndCombo1.Play();
        else if(m_AddFire_Count <= 6) m_SndCombo2.Play();
        else if(m_AddFire_Count <= 8) m_SndCombo3.Play();
        else m_SndCombo4.Play();
        
        if(m_AddFire_Count < 7) m_SndPuzzleFff1.Play();
        else m_SndPuzzleFff2.Play();
        
        AddFire();// 공추가.
    }

    public void RecallFunc_CheckOverBall(Ball Ball_Before, BallListInfo nCheckIndex, ref int nCount)
    {
        for (int i=0; i<m_BoolList.Count; i++ )
        {
            if ( m_BoolList[i].name == Ball_Before.name ) continue;
            if ( m_BoolList[i].GetIdx().BoomNumber() != nCheckIndex.BoomNumber()) continue;
            if ( m_BoolList[i].m_bVirtualToggle ) continue;
            
            // 정보계산.
            Vector2 vStart = Ball_Before.transform.position;
            Vector2 vEnd   = m_BoolList[i].transform.position;

            // 각도계산.
            float fDist_l;
            float Radian = (float)getAngle( Ball_Before.transform.localPosition, 
                m_BoolList[i].transform.localPosition, out fDist_l);
                Radian = Radian/(float)(System.Math.PI/180.0f);

            // 거리계산.
            float fDist_w = 0;
            fDist_w = Vector2.Distance(vStart, vEnd);

            // 충돌체크.
            Vector2 vDerection = new Vector2();
            vDerection = vEnd - vStart;
            RaycastHit2D [] Hit = Physics2D.RaycastAll( vStart, vDerection, fDist_w );
                
            bool bBreakKey = false;
            if (Hit != null && Hit.Length > 0)
            {
                for (int j=0; j<Hit.Length; j++)
                {
                    Ball Temp = Hit[j].collider.gameObject.GetComponent<Ball>();
                    if (Temp != null )
                    {
                        if (m_BoolList[i].name != Temp.name && Ball_Before.name != Temp.name)
                        {
                            if (Temp.GetIdx().BoomNumber() != Ball_Before.GetIdx().BoomNumber())
                            { bBreakKey = true; break; }

                            if (Temp.m_bVirtualToggle )
                            { bBreakKey = true; break; }

                            if (!Temp.m_bVirtualToggle && Temp.GetIdx().BoomNumber() == Ball_Before.GetIdx().BoomNumber())
                            { bBreakKey = true; break; }
                        }
                    }
                }
            }

            if (bBreakKey) continue;
            nCount++;

            Debug.Log(""+Ball_Before.name+":"+m_BoolList[i].name+":"+nCount);
            Ball_Before.m_bVirtualToggle = true;
            m_BoolList[i].m_bVirtualToggle = true;

            if(Ball_Before.m_VirtualToggle_State == 0) Ball_Before.m_VirtualToggle_State = 1;
            if(m_BoolList[i].m_VirtualToggle_State == 0) m_BoolList[i].m_VirtualToggle_State = 1;

            RecallFunc_CheckOverBall( m_BoolList[i] , nCheckIndex, ref nCount); 
        }
    }



}
