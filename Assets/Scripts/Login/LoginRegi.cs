using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using System;

public class LoginRegi : MonoBehaviour 
{    
    public  GameObject  m_AutoLogin;
    public  GameObject  m_DownLoadRes;
    public  UIInput     m_lbID;
    public  UIInput     m_lbPW;
    public  UISprite    m_AlertMsg;
    private UILabel     m_lbAlertMsg;
	
    void Start ()
    {
	    m_lbAlertMsg = m_AlertMsg.transform.FindChild("Label").GetComponent<UILabel>();
        m_AlertMsg.enabled = false;
	}
	

	void Update ()
    {
	
	}

    private IEnumerator Coroutine_SetAlertText(string a_Value)
    { 
        m_AlertMsg.enabled = true;        
        m_lbAlertMsg.text = a_Value;
        
		TweenScale ts 		= m_AlertMsg.gameObject.AddComponent<TweenScale> ();
		ts.from 			= new Vector3 (0, 1, 1);
		ts.to 				= new Vector3 (1, 1, 1);
		ts.style 			= UITweener.Style.Once;
		ts.eventReceiver 	= gameObject;
		ts.duration 		= 0.2f;
		ts.method 			= UITweener.Method.Linear;
		ts.callWhenFinished = null;        

        yield return new WaitForSeconds( 2.0f );
        m_lbAlertMsg.text = "";
        m_AlertMsg.enabled = false;        
    }

    public void Call_Back()
    {
        gameObject.SetActive(false);
        m_AutoLogin.SetActive(true);
    }

    bool m_bLogin = false;
    public void Call_Login()
    {           
        if(m_bLogin)return;        
        DataMgr.Inst.m_SerMgr.Login( m_lbID.value, m_lbPW.value, Login_Result );
        m_bLogin = true;
    }

    //로그인 결과.
    void Login_Result(bool a_Result, Dictionary<string, object> a_dicJson, string a_Str, serverManager.ErrorCode a_ErrorCode)
    { 
        if(a_Result)
        {
            PlayerPrefs.SetString("ID", m_lbID.value);
            PlayerPrefs.SetString("PW", m_lbPW.value);                        
            gameObject.SetActive(false);
            m_DownLoadRes.SetActive(true);            
            m_DownLoadRes.GetComponent<DownLoad_Res>().BeignDownload(a_dicJson);
        }
        else        
        {
            if(a_ErrorCode == serverManager.ErrorCode.EC_ARGUMENT)
            {                   
                StartCoroutine(Coroutine_SetAlertText( DataMgr.Inst.GetLocal("Passwrd or ID is incorrect") ));
                m_bLogin = false;
            }
            else if(a_ErrorCode == serverManager.ErrorCode.EC_NoneUser)
            {
                StartCoroutine(Coroutine_SetAlertText( DataMgr.Inst.GetLocal("The ID is not avaliable") ));
                m_bLogin = false;
            }
		    else if(a_ErrorCode == serverManager.ErrorCode.EC_LOGIN_PwNot)
            {
                StartCoroutine(Coroutine_SetAlertText( DataMgr.Inst.GetLocal("the Password is incorrect") ));
                m_bLogin = false;
            }
		    else if(a_ErrorCode == serverManager.ErrorCode.EC_LOGIN_HVersion)
            {
                CMessageBox.Create( "You need to download upper version.", 1, onCheckver_Varsion);                
            }
		    else
            {
               CMessageBox.Create( "Failed to connect to server.", 1, onCheckver_Eror);  
            }
            

            return;
        }
    }

    public void onCheckver_Eror( bool a_bValue )
    {
         Application.Quit();
    }

    public void onCheckver_Varsion( bool a_bValue )
    {
        Application.OpenURL("market://details?id=kr.hanyou.google.godofpuzzle");    
    }
}
