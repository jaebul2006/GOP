using UnityEngine;
using System.Collections;
using System;

public class Ball : MonoBehaviour 
{
    public UITexture            m_Summon;
    public UITexture            m_Summon_1;
    public UISprite             m_sprPoint;
    public UITexture            m_sprToggle;
    private BattlePuzzleMgr.BallListInfo m_BallInfo;
    private bool                m_bToggle;
	private CircleCollider2D    m_Collider;    

    [System.NonSerialized]
    public bool                m_bVirtualToggle = false; // 없앴수있는지 체크 하는 사용할 변수.

    [System.NonSerialized]
    public int                 m_VirtualToggle_State = 0;
    public BattlePuzzleMgr.BallListInfo GetIdx() { return m_BallInfo; }
        
    public bool IsToggle() { return m_bToggle; }
    private GameObject m_UIRoot;    

    void Start () 
    {
        m_sprPoint.enabled = false;
        m_sprToggle.enabled = false;
        m_Collider = GetComponent<CircleCollider2D>();
        m_Summon.enabled = true;
        m_Summon_1.enabled = false;
	}
		
	void Update ()
    {
	}

    public void FaceOff()
    {
        m_Summon.enabled = false;
        m_Summon_1.enabled = true;        
        Invoke("FaceOff_End", 3f);
    }
    void FaceOff_End()
    {
        m_Summon.enabled = true;
        m_Summon_1.enabled = false;
    }

    public void SetUp(GameObject a_UIRoot, BattlePuzzleMgr.BallListInfo a_Info, float a_fX, float a_fY)
    {
        m_UIRoot = a_UIRoot;
        m_BallInfo = a_Info;
        DataMgr.emCardAttribute Attri = DataMgr.emCardAttribute.emCAB_END;
        
        if (m_BallInfo.IDX != -1)
        {            
            Attri = DataMgr.Inst.m_UserSummonList[m_BallInfo.IDX].Data.eAttr;            
            m_Summon.mainTexture = Resources.Load("Textures/Char/icon_mon_" + DataMgr.Inst.m_UserSummonList[m_BallInfo.IDX].Data.strResurceID ) as Texture;
            m_Summon_1.mainTexture = Resources.Load("Textures/Char/icon_mon_" + DataMgr.Inst.m_UserSummonList[m_BallInfo.IDX].Data.strResurceID+"_2") as Texture;
            m_Summon.enabled = true;
        }
        else
        {
            Attri = m_BallInfo.Attri;
            transform.FindChild("Summon").GetComponent<UITexture>().enabled = false;
        }

        switch(Attri)
        {
            case DataMgr.emCardAttribute.emCAB_WA:
                transform.GetComponent<UITexture>().mainTexture = Resources.Load("Textures/Ball_Red") as Texture;
            break;

            case DataMgr.emCardAttribute.emCAB_SU:
                transform.GetComponent<UITexture>().mainTexture = Resources.Load("Textures/Ball_Blue") as Texture;
            break;

            case DataMgr.emCardAttribute.emCAB_YUNG:
                transform.GetComponent<UITexture>().mainTexture = Resources.Load("Textures/Ball_Gold") as Texture;
            break;

            case DataMgr.emCardAttribute.emCAB_ARM:
                transform.GetComponent<UITexture>().mainTexture = Resources.Load("Textures/Ball_Dark") as Texture;
            break;
        }
        
        gameObject.transform.localPosition = new Vector3( a_fX, a_fY);
        gameObject.transform.localRotation = Quaternion.Euler(0, 0, (float)UnityEngine.Random.Range(0, 361 ) );
        gameObject.transform.localScale    = new Vector3( 1f, 1f, 1f);
    }

    
    public void Reset( BattlePuzzleMgr.BallListInfo a_Info)
    {        
        m_BallInfo = a_Info;
        DataMgr.emCardAttribute Attri = DataMgr.emCardAttribute.emCAB_END;
        
        if (m_BallInfo.IDX != -1)
        {            
            Attri = DataMgr.Inst.m_UserSummonList[m_BallInfo.IDX].Data.eAttr;
            transform.FindChild("Summon").GetComponent<UITexture>().mainTexture = 
                Resources.Load("Textures/Char/icon_mon_" + DataMgr.Inst.m_UserSummonList[m_BallInfo.IDX].Data.strResurceID ) as Texture;
            transform.FindChild("Summon").GetComponent<UITexture>().enabled = true;
        }
        else
        {
            Attri = m_BallInfo.Attri;
            transform.FindChild("Summon").GetComponent<UITexture>().enabled = false;
        }        

        switch(Attri)
        {
            case DataMgr.emCardAttribute.emCAB_WA:
                transform.GetComponent<UITexture>().mainTexture = Resources.Load("Textures/Ball_Red") as Texture;
            break;

            case DataMgr.emCardAttribute.emCAB_SU:
                transform.GetComponent<UITexture>().mainTexture = Resources.Load("Textures/Ball_Blue") as Texture;
            break;

            case DataMgr.emCardAttribute.emCAB_YUNG:
                transform.GetComponent<UITexture>().mainTexture = Resources.Load("Textures/Ball_Gold") as Texture;
            break;

            case DataMgr.emCardAttribute.emCAB_ARM:
                transform.GetComponent<UITexture>().mainTexture = Resources.Load("Textures/Ball_Dark") as Texture;
            break;
        }        
    }


    Vector2 m_CheckResult = new Vector2();
    public bool CheckMouseInBall( Vector2 a_Mouse )
    {        
        try
        { 
            m_CheckResult.x = transform.localPosition.x;
            m_CheckResult.y = transform.localPosition.y;
            float fDist = Vector2.Distance(m_CheckResult, a_Mouse);
        
            //충돌.
            if(m_Collider.radius > fDist) // 여기서
            {
                return true;   
            }        
            return false;
        }
        catch(Exception e)
        {
            return false;
        }
    }
    

    //-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    // 없어지는 것 구현.
    private bool m_isExeplotion = false;
    public void Exeplotion()
    {
        if(m_isExeplotion) return;
    //  TweenScale TS = gameObject.AddComponent<TweenScale>();
    //  TS.callWhenFinished = "Call_Exeplotion_End";
    //  TS.duration = 0.7f;
    //  TS.from     = new Vector3(1.0f, 1.0f, 1.0f);
    //  TS.to       = new Vector3(0.0f, 0.0f, 1.0f);
    //  TS.style            = UITweener.Style.Once;
	//	TS.eventReceiver 	= gameObject;
	//	TS.method 			= UITweener.Method.Linear;
    //  m_isExeplotion = true;
        BeginBoom();
        Call_Exeplotion_End();
    }

    void Call_Exeplotion_End()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    //-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    // 반짝반짝 하는 토글 구현.
    public void SetToggle(bool a_Value)
    {
        if( m_bToggle == a_Value ) return;

        m_bToggle = a_Value;
        
        if(a_Value)
        {
            m_sprPoint.enabled = true;
            m_sprToggle.enabled = true;
            TweenAlpha TA       = m_sprToggle.gameObject.AddComponent<TweenAlpha>();
            TA.from             = 0.7f;
            TA.to               = 0.1f;
            TA.duration         = 0.7f;
            TA.style            = UITweener.Style.Once;
		    TA.eventReceiver 	= gameObject;
		    TA.method 			= UITweener.Method.Linear;
            TA.callWhenFinished = "Coll_Toggling1";
        }
        else 
        {
            m_sprPoint.enabled = false;
            m_sprToggle.enabled = false;
            TweenAlpha TA = m_sprToggle.GetComponent<TweenAlpha>();
            if(TA) Destroy( TA );
        }
    }

    void Coll_Toggling1()
    {
        TweenAlpha TA = m_sprToggle.GetComponent<TweenAlpha>();
        if(TA) Destroy(TA);

        if(!m_bToggle) return;

        TA = m_sprToggle.gameObject.AddComponent<TweenAlpha>();
        TA.from             = 0.1f;
        TA.to               = 0.7f;
        TA.duration         = 0.7f;
        TA.style            = UITweener.Style.Once;
		TA.eventReceiver 	= gameObject;
		TA.method 			= UITweener.Method.Linear;
        TA.callWhenFinished = "Coll_Toggling2";        
    }

    void Coll_Toggling2()
    {
        TweenAlpha TA = m_sprToggle.GetComponent<TweenAlpha>();
        if(TA) Destroy(TA);

        if(!m_bToggle) return;

        TA = m_sprToggle.gameObject.AddComponent<TweenAlpha>();
        TA.from             = 0.7f;
        TA.to               = 0.1f;
        TA.duration         = 0.7f;
        TA.style            = UITweener.Style.Once;
		TA.eventReceiver 	= gameObject;
		TA.method 			= UITweener.Method.Linear;
        TA.callWhenFinished = "Coll_Toggling1";        
    }

    void BeginBoom()
    {
        
        GameObject obj = (GameObject)Instantiate((GameObject)Resources.Load("Effects/CFX3_Hit_Misc_B (Yellow)"));
        obj.transform.parent = m_UIRoot.transform;
        obj.gameObject.name = "effect1";
        obj.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -0.05f);

        obj = (GameObject)Instantiate((GameObject)Resources.Load("Effects/CFXM3_Hit_Light_B_Air"));
        obj.transform.parent = m_UIRoot.transform;
        obj.gameObject.name = "effect2";
        obj.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -0.05f);

        obj = (GameObject)Instantiate((GameObject)Resources.Load("Effects/CFXM3_Hit_SmokePuff"));
        obj.transform.parent = m_UIRoot.transform;
        obj.gameObject.name = "effect3";
        obj.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -0.05f);
    }

}
