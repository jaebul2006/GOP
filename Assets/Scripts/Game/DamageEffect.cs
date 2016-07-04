using UnityEngine;
using System.Collections;

public class DamageEffect : MonoBehaviour 
{
    UITexture m_Tex;
	// Use this for initialization
	void Start () 
    {
        m_Tex = GetComponent<UITexture>();
	    m_Tex.alpha = 0.0f;
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    TweenAlpha m_TR;
    public void Begin()
    {
        m_TR                    = gameObject.AddComponent<TweenAlpha>();        
        m_TR.from               = 0.0f;        
        m_TR.to                 = 255.0f;                    
        m_TR.style              = UITweener.Style.Once;
        m_TR.duration           = 0.5f;
        m_TR.method             = UITweener.Method.Linear;
        m_TR.callWhenFinished   = "EffDown";
        m_TR.eventReceiver      = gameObject;
    }

    void EffDown()
    {
        m_TR                    = gameObject.AddComponent<TweenAlpha>();        
        m_TR.from               = 255.0f;
        m_TR.to                 = 0.0f;
        m_TR.style              = UITweener.Style.Once;
        m_TR.duration           = 0.3f;
        m_TR.method             = UITweener.Method.Linear;
        m_TR.callWhenFinished   = "EffrectEnd";
        m_TR.eventReceiver      = gameObject;
    }

    void EffrectEnd()
    {        
        m_Tex.alpha = 0.0f;
        Destroy(m_TR);
    }

}
