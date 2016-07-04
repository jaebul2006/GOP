using UnityEngine;
using System.Collections;

public class Map10X : MonoBehaviour
{    
    //   5
    //  3 4
    // 0 1 2
    private GameObject []   m_MonsterPos;
    private GameObject      m_Parent;
    private GameObject      m_Sky;
    private GameObject      m_MapRoundPoint;
    private Vector3         m_vSkyRot;

    // 접근자.
    public GameObject GetMonPos( int a_IDX )
    {
        return m_MonsterPos[a_IDX];
    }

    // 생성.
    public static Map10X Create(GameObject a_Parent )
    {
        GameObject Obj = Instantiate(Resources.Load("Prefabs/MAP/MAP_" + DataMgr.Inst.m_DB_AdventureStage[DataMgr.Inst.m_SelectStageIndex].strMapID) as GameObject);
        Obj.transform.parent = a_Parent.transform;

        Map10X Script = Obj.GetComponent<Map10X>();
        Script.m_MonsterPos = new GameObject[6];
        Script.m_Parent = a_Parent;
        
        Script.m_MapRoundPoint = Instantiate(Resources.Load("Prefabs/MAP/MAP_ROUND_POINT") as GameObject);
        Script.m_MapRoundPoint.transform.parent = a_Parent.transform;        

        //Sky_101
        Script.m_Sky = Instantiate(Resources.Load("Prefabs/MAP/Sky_" + DataMgr.Inst.m_DB_AdventureStage[DataMgr.Inst.m_SelectStageIndex].strMapID) as GameObject);
        //Obj.transform.parent = a_Parent.transform;        
        Script.m_vSkyRot = new Vector3(0f, 0f, 0f);

        for(int i=0 ;i<Script.m_MonsterPos.Length; i++)
        {
            Script.m_MonsterPos[i] = Script.m_MapRoundPoint.transform.FindChild("Mon_" + (i+1) ).gameObject;            
        }
                
        return Script;
    }

    public void Close()
    {
        Destroy(m_MapRoundPoint);
        Destroy(m_Sky);
        Destroy(gameObject);
    }


    // Use this for initialization
    void Start ()
    {           
        StartPos();
    }

    public void StartPos()
    {
        m_Parent.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        m_Parent.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

        transform.localPosition = new Vector3(0.02645869f, 0.38f+0.06f, 0.3805113f);
        transform.localRotation = Quaternion.Euler(0.0f, -12.1f, 0.0f);

        m_MapRoundPoint.transform.localPosition = new Vector3(0.0f, 0.15f+0.06f, 0.0f);
        m_MapRoundPoint.transform.localRotation = Quaternion.Euler(0.0f, 57.2f, 0.0f);
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_vSkyRot.y-=0.1f;
	    m_Sky.transform.localRotation = Quaternion.Euler(m_vSkyRot);
	}
    

    private GameObject  m_MoveTo_EventObj;
    private string      m_MoveTo_Event;
    private TweenRotation m_TR;
    public void MoveTo(float a_Rot, GameObject a_EventObj, string a_Event )
    {        
        Vector3 vTemp;        
        m_TR    = m_Parent.AddComponent<TweenRotation>();
        if (m_TR == null) return;
        m_TR.from             = m_Parent.transform.localRotation.eulerAngles;
        vTemp               = m_Parent.transform.localRotation.eulerAngles;
        vTemp.y             += a_Rot;
        m_TR.to               = vTemp;
        m_TR.style            = UITweener.Style.Once;
        m_TR.duration         = 4;
        m_TR.method           = UITweener.Method.Linear;
        m_TR.callWhenFinished = "MoveToEND";
        m_TR.eventReceiver    = gameObject;
        m_MoveTo_Event      = a_Event;
        m_MoveTo_EventObj   = a_EventObj;
        BeginWarking();
    }

    private void MoveToEND()
    {
        m_MoveTo_EventObj.SendMessage(m_MoveTo_Event, SendMessageOptions.DontRequireReceiver);
        Destroy(m_TR);
        EndWarking();
    }

    //위아래로 딸깍딸깍 해준다.
    public bool m_IsWarking = false;

    private void BeginWarking()
    {
        m_IsWarking = true;
        UpWarking();        
    }
    private void EndWarking()
    {
        m_IsWarking = false;
    }

    TweenPosition m_WarkingTP;
    private void UpWarking()
    {
        if(m_IsWarking == false)
        {
            if(m_WarkingTP) Destroy(m_WarkingTP);
            return;
        }
            
        Vector3 vTemp;
        m_WarkingTP    = m_Parent.AddComponent<TweenPosition>();
        if (m_WarkingTP == null) return;

        m_WarkingTP.from             = m_Parent.transform.localPosition;
        vTemp               = m_Parent.transform.localPosition;
        vTemp.y            += 0.4f;
        m_WarkingTP.to               = vTemp;
        m_WarkingTP.style            = UITweener.Style.Once;
        m_WarkingTP.duration         = 0.5f;
        m_WarkingTP.method           = UITweener.Method.Linear;
        m_WarkingTP.callWhenFinished = "DownWarking";
        m_WarkingTP.eventReceiver    = gameObject;
    }

    private void DownWarking()
    {
        if(m_WarkingTP) Destroy(m_WarkingTP);
        Vector3 vTemp;
        m_WarkingTP    = m_Parent.AddComponent<TweenPosition>();
        if (m_WarkingTP == null) return;
        m_WarkingTP.from             = m_Parent.transform.localPosition;
        vTemp               = m_Parent.transform.localPosition;
        vTemp.y            -= 0.4f;
        m_WarkingTP.to               = vTemp;                    
        m_WarkingTP.style            = UITweener.Style.Once;
        m_WarkingTP.duration         = 0.5f;
        m_WarkingTP.method           = UITweener.Method.Linear;
        m_WarkingTP.callWhenFinished = "UpWarking";
        m_WarkingTP.eventReceiver    = gameObject;
    }    
}
