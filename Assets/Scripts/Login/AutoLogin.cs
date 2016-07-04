using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using System;

public class AutoLogin : MonoBehaviour 
{
    public GameObject   m_UIRoot;
    public UISprite 	m_Logo_FULL;
    public UISprite 	m_Logo;
    public UISprite 	m_Logo_Text;
    public UISprite 	m_Logo_Text_Eng;
    private UISlider    m_Logo_Slider;
    
    public UISprite 	m_BtnTouch;
    public GameObject   m_BtnChange;
    public GameObject   m_BtnCreateMember;
    public GameObject   m_LoginRegi;
    public GameObject   m_CreateMember;
    public GameObject   m_DownLoadRes;
    


    //==============================================================================.
    // 로고 관련 
    public IEnumerator StartLogo()
    { 
        yield return new WaitForSeconds(0.5f);

        m_Logo.enabled = true;
        m_Logo.transform.localScale = new Vector3(0.0f, 0.0f, 1.0f);
    
		TweenScale ts 		    = m_Logo.gameObject.AddComponent<TweenScale> ();
		ts.from 			    = new Vector3 (1.5f, 1.5f, 1.5f);
		ts.to 				    = new Vector3 (1.0f, 1.0f, 1.0f);
		ts.style 			    = UITweener.Style.Once;
		ts.eventReceiver 	    = gameObject;
		ts.duration 		    = 0.6f;
		ts.method 			    = UITweener.Method.Linear;
		ts.callWhenFinished     = null;        

        TweenAlpha ta1 		    = m_Logo.gameObject.AddComponent<TweenAlpha> ();
		ta1.from 			    = 0.0f;
		ta1.to 				    = 1.0f;
		ta1.style 			    = UITweener.Style.Once;
		ta1.eventReceiver 	    = gameObject;
		ta1.duration 		    = 0.2f;
		ta1.method 			    = UITweener.Method.Linear;
		ta1.callWhenFinished    = null;
        yield return new WaitForSeconds(0.6f);          
    //    DataMgr.Inst.Log("StartLogo 2");

        m_Logo_FULL.enabled = true;
        m_Logo_FULL.color   = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        TweenAlpha ta 		= m_Logo_FULL.gameObject.AddComponent<TweenAlpha> ();
		ta.from 			= 0.0f;
		ta.to 				= 1.0f;
		ta.style 			= UITweener.Style.Once;
		ta.eventReceiver 	= gameObject;
		ta.duration 		= 2.0f;
		ta.method 			= UITweener.Method.Linear;
		ta.callWhenFinished = null;        
          
        TweenAlpha ta3 		= m_Logo.gameObject.AddComponent<TweenAlpha> ();
		ta3.from 			= 1.0f;
		ta3.to 				= 0.0f;
		ta3.style 			= UITweener.Style.Once;
		ta3.eventReceiver 	= gameObject;
		ta3.duration 		= 2.0f;
		ta3.method 			= UITweener.Method.Linear;
		ta3.callWhenFinished = null;        

        yield return new WaitForSeconds(2.0f);        

        if (DataMgr.Inst.GetLanguage() == DataMgr.emLanguage.emLanguage_Korea)
        {
            m_Logo_Text.gameObject.SetActive(true);
            m_Logo_Slider       = m_Logo_Text.transform.GetComponent<UISlider>();
            m_Logo_Slider.value = 0.0f;
            
        }
        else
        {
            m_Logo_Text_Eng.gameObject.SetActive(true);
            m_Logo_Slider           = m_Logo_Text_Eng.transform.GetComponent<UISlider>();
            m_Logo_Slider.value     = 0.0f;
        }
        
        TweenNumber _twn = TweenNumber.Begin(this.gameObject, 0.8f, 1.0f);
        _twn.from = 0.0f;
        _twn.eventReceiver = this.gameObject;
        _twn.callWhenTween = "OnTweenValue";

        yield return new WaitForSeconds(0.8f);    
        
    
        string ID   = PlayerPrefs.GetString("ID", "");
        string PW   = PlayerPrefs.GetString("PW", "");        

        m_BtnCreateMember.GetComponent<BoxCollider>().enabled = true;
        m_BtnCreateMember.GetComponent<UIButton>().SetState( UIButtonColor.State.Normal, true );
        m_BtnChange.GetComponent<BoxCollider>().enabled = true;
        m_BtnChange.GetComponent<UIButton>().SetState( UIButtonColor.State.Normal, true );

        // 값이 이상하면 직접 입력하게한다.
        if(ID == "" || PW == "")
        {            
            gameObject.SetActive(false);
            m_LoginRegi.SetActive(true);   
            yield return null;
        }
        else
        {
            m_BtnTouch.color            = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            m_BtnTouch.enabled          = true;
            m_BtnTouch.GetComponent<Collider>().enabled = true; 
            m_BtnTouch.gameObject.GetComponent<TweenAlpha>().enabled = true;
        }
                
//        FaidOutTouch();
    }

    public void FaidInTouch()
    {           
        TweenAlpha ta = m_BtnTouch.gameObject.AddComponent<TweenAlpha>();                
		ta.from 			= 1.0f;
		ta.to 				= 0.0f;
		ta.style 			= UITweener.Style.Once;
		ta.eventReceiver 	= gameObject;
		ta.duration 		= 1.0f;
		ta.method 			= UITweener.Method.Linear;
		ta.callWhenFinished = "FaidOutTouch";
    }

    public void FaidOutTouch()
    {
        TweenAlpha ta;
        if ( UnityEngine.SystemLanguage.Korean == UnityEngine.Application.systemLanguage )    
            ta = m_BtnTouch.gameObject.AddComponent<TweenAlpha> ();
        else
            ta = m_BtnTouch.gameObject.AddComponent<TweenAlpha> ();             
        
		ta.from 			= 0.0f;
		ta.to 				= 1.0f;
		ta.style 			= UITweener.Style.Once;
		ta.eventReceiver 	= gameObject;
		ta.duration 		= 1.0f;
		ta.method 			= UITweener.Method.Linear;
		ta.callWhenFinished = "FaidInTouch";  
    }

    public void OnTweenValue(float value) 
    {
        m_Logo_Slider.value = value;        
    }

	// Use this for initialization
	void Start () 
    {
        Input.multiTouchEnabled     = false;
        Application.runInBackground = true;
        DataMgr.Inst.m_TopObj       = m_UIRoot;
        m_Logo_FULL.enabled         = false;
        m_Logo.enabled              = false;
        m_Logo_Text.gameObject.SetActive(false);
        m_Logo_Text_Eng.gameObject.SetActive(false);        

        m_BtnTouch.enabled = false;
        m_BtnTouch.GetComponent<Collider>().enabled = false;
        m_BtnChange.GetComponent<BoxCollider>().enabled = false;
        m_BtnChange.GetComponent<UIButton>().SetState( UIButtonColor.State.Disabled, true );
        m_BtnCreateMember.GetComponent<BoxCollider>().enabled = false;
        m_BtnCreateMember.GetComponent<UIButton>().SetState( UIButtonColor.State.Disabled, true );        
        StartCoroutine(StartLogo());        
    }	

	// Update is called once per frame
	void Update () 
    {        
    } 

    public void onBtnLoginPage()
    {
        gameObject.SetActive(false);
        m_LoginRegi.SetActive(true);
    }

    public void onBtnCreateMember()
    {
        gameObject.SetActive(false);
        m_CreateMember.SetActive(true);
    }

    //================================================================.
    // 자동 로그인 관련.
    private bool m_bLogin=false;
    public void Call_Login()
    {
        if(m_bLogin) return;
        string ID   = PlayerPrefs.GetString("ID", "");
        string PW   = PlayerPrefs.GetString("PW", "");
     
        // 값이 이상하면 직접 입력하게한다.
        if(ID == "" || PW == "")
        {
            gameObject.SetActive(false);
            m_LoginRegi.SetActive(true);
            return;
        }

        DataMgr.Inst.m_SerMgr.Login( ID, PW, Login_Result);
        m_bLogin = true;
    }

    //로그인 결과.
    void Login_Result(bool a_Result, Dictionary<string, object> a_dicJson, string a_Str, serverManager.ErrorCode a_ErrorCode)
    { 
        if (a_Result)
        {
            gameObject.SetActive(false);
            m_DownLoadRes.SetActive(true);
            m_DownLoadRes.GetComponent<DownLoad_Res>().BeignDownload(a_dicJson);
        }
        else
        {
            if( a_ErrorCode == serverManager.ErrorCode.EC_ARGUMENT || 
                a_ErrorCode == serverManager.ErrorCode.EC_NoneUser || 
                a_ErrorCode == serverManager.ErrorCode.EC_LOGIN_PwNot )
            {
                m_bLogin = false;
                gameObject.SetActive(false);
                m_LoginRegi.SetActive(true);             
            }            
		    else if (a_ErrorCode == serverManager.ErrorCode.EC_LOGIN_HVersion)
            {
                CMessageBox.Create( DataMgr.Inst.GetLocal("You need to download upper version."), 1, onCheckver_Varsion);
                return;
            }
		    else
            {
                CMessageBox.Create( DataMgr.Inst.GetLocal("Cannot connect to server."), 1, onCheckver_Eror);   
            }            
        }
    }

    public void onCheckver_Varsion( bool a_bValue )
    {
        Application.OpenURL("market://details?id=kr.hanyou.google.godofpuzzle");        
    }

    public void onCheckver_Eror( bool a_bValue )
    {
        Application.Quit();        
    }
}
