using UnityEngine;
using System.Collections;

public class Panel_Load : MonoBehaviour 
{
    public UISprite[]   m_Cycle;

    private Vector3     m_vRot = new Vector3();
 
    private int m_OnIdx = 0;
    private int m_hafeIdx = 3;

    private int m_LimitTime=0;

	// Use this for initialization
	void Start () 
    {
	    for(int i=0; i<m_Cycle.Length; i++)
        {
            if(i==m_OnIdx) m_Cycle[i].alpha = 1f;
            else if(i==m_hafeIdx) m_Cycle[i].alpha = 0.5f;
            else m_Cycle[i].alpha = 0;            
        }
	}
	
    

	// Update is called once per frame.
	void Update () 
    {        
        if( System.Environment.TickCount >  m_LimitTime)
        {
            m_OnIdx++;
            m_hafeIdx = m_OnIdx-1;
            if(m_OnIdx >= m_Cycle.Length )
            {
                m_OnIdx = 0;
                m_hafeIdx = m_Cycle.Length-1;
            }
            m_LimitTime = System.Environment.TickCount+ 500;
        }
        

        for(int i=0; i<m_Cycle.Length; i++)
        {
            if(i==m_OnIdx) m_Cycle[i].alpha = 1f;
            else if(i==m_hafeIdx) m_Cycle[i].alpha = 0.5f;
            else m_Cycle[i].alpha = 0;            
        }
	}
    
    TweenAlpha m_TA;
    public void FadeOn( )
    {        
        m_TA = gameObject.AddComponent<TweenAlpha>();        
        m_TA.from               = 0.0f;        
        m_TA.to                 = 1.0f;                    
        m_TA.style              = UITweener.Style.Once;
        m_TA.duration           = 1;
        m_TA.method             = UITweener.Method.Linear;
        m_TA.callWhenFinished   = "Destroy_TA";
        m_TA.eventReceiver      = gameObject;
    }

    public void FadeOut( )
    {        
        m_TA = gameObject.AddComponent<TweenAlpha>();        
        m_TA.from               = 1.0f;        
        m_TA.to                 = 0.0f;                    
        m_TA.style              = UITweener.Style.Once;
        m_TA.duration           = 2;
        m_TA.method             = UITweener.Method.Linear;                
        m_TA.callWhenFinished   = "Destroy_TA";
        m_TA.eventReceiver      = gameObject;
    }

    void Destroy_TA()
    {
        Destroy(m_TA);        
    }
}
