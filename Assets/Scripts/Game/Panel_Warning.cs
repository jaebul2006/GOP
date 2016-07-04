using UnityEngine;
using System.Collections;

public class Panel_Warning : MonoBehaviour 
{
    public UISprite     m_sprText;
    public UISprite     m_sprEff;
    public Vector3      m_Text_TP_from      = new Vector3(603.43f, -29.31f, 0f);        
    public Vector3      m_Text_TP_to        = new Vector3(0f, -29.31f, 0f);        
    public Vector3      m_Text_TP_to_End    = new Vector3(-603.43f, -29.31f, 0f);
	// Use this for initialization
	void Start () 
    {
	    m_sprText.enabled = false;
        if(m_sprEff) m_sprEff.enabled = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    TweenPosition m_sprText_TP;    
    public void Begin()
    {
        if(m_sprEff) m_sprEff.enabled       = true;
        m_sprText.enabled       = true;
        m_sprText_TP            = m_sprText.gameObject.AddComponent<TweenPosition>();
        m_sprText_TP.from       = m_Text_TP_from;        
        m_sprText_TP.to         = m_Text_TP_to;
        m_sprText_TP.style      = UITweener.Style.Once;
        m_sprText_TP.duration   = 1.5f;
        m_sprText_TP.method     = UITweener.Method.Linear;
        m_sprText_TP.callWhenFinished   = "MoveWait";
        m_sprText_TP.eventReceiver      = gameObject;        
    }

    void MoveWait()
    {
        Destroy(m_sprText_TP);
        Invoke( "MoveEnd", 2f );       
    }
    
    void MoveEnd()
    {        
        m_sprText_TP            = m_sprText.gameObject.AddComponent<TweenPosition>();
        m_sprText_TP.from       = m_Text_TP_to;        
        m_sprText_TP.to         = m_Text_TP_to_End;
        m_sprText_TP.style      = UITweener.Style.Once;
        m_sprText_TP.duration   = 1.5f;
        m_sprText_TP.method     = UITweener.Method.Linear;
        m_sprText_TP.callWhenFinished   = "End";
        m_sprText_TP.eventReceiver      = gameObject;  
    }

    public void End()
    {
        Destroy(m_sprText_TP);
        m_sprText.enabled = false;
        if(m_sprEff) m_sprEff.enabled = false;
    }
}
