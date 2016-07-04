﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_ANDROID
using TapjoyUnity;
#endif

public class CreateMember : MonoBehaviour 
{
    public UIInput      m_lbID;
    public UIInput      m_lbPW;
    public UIInput      m_lbPW_Confirm;
    public UIInput      m_lbNicName;

    public UISprite     m_AlertMsg; 
    public UILabel      m_lbAlertMsg;
    public GameObject   m_AutoLogin;

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
	// Use this for initialization
	void Start () 
    {
	    m_lbAlertMsg = m_AlertMsg.transform.FindChild("Label").GetComponent<UILabel>();
        m_AlertMsg.enabled = false;
        
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void Call_Back()
    {
        gameObject.SetActive(false);
        m_AutoLogin.SetActive(true);
    }

    public void Call_CreateMember()
    {        
        if( m_lbID.value.Length > 10 )
        {
            StartCoroutine(Coroutine_SetAlertText( DataMgr.Inst.GetLocal("Length of ID overrun") ));
            return;
        }
        if( m_lbPW.value.Length > 10 )
        {
            StartCoroutine(Coroutine_SetAlertText( DataMgr.Inst.GetLocal("Length of Passward overrun") ));
            return;
        }

        if( m_lbPW.value != m_lbPW_Confirm.value)
        {
            StartCoroutine(Coroutine_SetAlertText( DataMgr.Inst.GetLocal("Password verification was not properly done.") ));
            return;
        }
        

        DataMgr.Inst.m_SerMgr.CreateMember( 
            m_lbID.value, 
            m_lbPW.value, 
            "", CreateMember_Result );
    }

    public void CreateMember_Result(bool a_Result, Dictionary<string, object> a_dicJson, string a_Str, serverManager.ErrorCode a_ErrorCode)
    { 
        if(a_Result)
        {   
            // 바로 로그인.
#if UNITY_ANDROID
            Tapjoy.SetUserLevel(1); 
            Tapjoy.SetUserID(m_lbID.value);
#endif
            PlayerPrefs.SetString("ID", m_lbID.value);
            PlayerPrefs.SetString("PW", m_lbPW.value);
            gameObject.SetActive(false);
            m_AutoLogin.SetActive(true);
            m_AutoLogin.SendMessage("Call_Login", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            
            if(a_ErrorCode == serverManager.ErrorCode.EC_CreateMember_IsID) // 이미존재하는 아이디.            
                StartCoroutine( Coroutine_SetAlertText( DataMgr.Inst.GetLocal("Wrong ID") ) );
            
            else if(a_ErrorCode == serverManager.ErrorCode.EC_CreateMember_IsNicName)            
                StartCoroutine( Coroutine_SetAlertText( DataMgr.Inst.GetLocal("THE NICNAME IS NOT") ) );
                            
            else if(a_ErrorCode == serverManager.ErrorCode.EC_CreateMember_ID_LangthZoro)   // 아이디 길이0.
                StartCoroutine(Coroutine_SetAlertText( DataMgr.Inst.GetLocal("Length of ID is zero") ));

		    else if(a_ErrorCode == serverManager.ErrorCode.EC_CreateMember_ID_LangthOver)   // 아이디 길이초과	
                StartCoroutine(Coroutine_SetAlertText( DataMgr.Inst.GetLocal("Length of ID overrun") ));

		    else if(a_ErrorCode == serverManager.ErrorCode.EC_CreateMember_ID_Special)      // 아이디에 특수문자포함.		
                StartCoroutine(Coroutine_SetAlertText( DataMgr.Inst.GetLocal("ID include Special") ));

		    else if(a_ErrorCode == serverManager.ErrorCode.EC_CreateMember_PW_LangthZoro)   // 비밀번호 길이0.		
                StartCoroutine(Coroutine_SetAlertText( DataMgr.Inst.GetLocal("Length of Passward is zero") ));

		    else if(a_ErrorCode == serverManager.ErrorCode.EC_CreateMember_PW_LangthOver)   // 비밀번호길이초과	        
                StartCoroutine(Coroutine_SetAlertText( DataMgr.Inst.GetLocal("Length of Passward overrun") ));
            else
            {
                StartCoroutine(Coroutine_SetAlertText( DataMgr.Inst.GetLocal("Do not connect server") ));
            }
        }
    }
}