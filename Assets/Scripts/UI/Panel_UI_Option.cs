using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class Panel_UI_Option : MonoBehaviour 
{    
//    public UISprite m_BtnRestore;
    public UILabel      m_lbTapGameOption;
    public UILabel      m_lbTapGeneralOption;
    
    public UILabel m_lbEff_On;
    public UILabel m_lbEff_Off;
    public UILabel m_lbEff_Title;

    public UILabel m_lbScr_On;
    public UILabel m_lbScr_Off;
    public UILabel m_lbScr_Title;

    public UILabel m_lbGra_On;
    public UILabel m_lbGra_Off;
    public UILabel m_lbGra_Title;

    public UILabel m_lbPar_On;
    public UILabel m_lbPar_Off;
    public UILabel m_lbPar_Title;

    public UILabel m_lbSeesion;
    public UILabel m_lbNotice;
    public UILabel m_CustomerCenter;

	public GameObject[] Category    = new GameObject[2];
	public UIToggle[]   Snd_Btn     = new UIToggle[2];
	public UIToggle[]   Scr_Btn     = new UIToggle[2];
	public UIToggle[]   Gra_Btn     = new UIToggle[2];
	public UIToggle[]   Par_Btn     = new UIToggle[2];
	public UILabel UID;

    private GameObject m_RootLobby;
    void SetUp(GameObject a_Root)
    { m_RootLobby = a_Root; CheckOption(); }

    private Camera m_LobbyCamera;

	void Start()
    {
        m_LobbyCamera = GameObject.FindWithTag("LobbyCamera").GetComponent<Camera>();        
        m_lbTapGameOption.text = DataMgr.Inst.GetLocal("Settings");
        m_lbTapGeneralOption.text = DataMgr.Inst.GetLocal("Info");

        m_lbEff_On.text     = DataMgr.Inst.GetLocal("On");
        m_lbEff_Off.text    = DataMgr.Inst.GetLocal("Off");
        m_lbEff_Title.text  = DataMgr.Inst.GetLocal("Sound");

        m_lbScr_On.text     = DataMgr.Inst.GetLocal("On");
        m_lbScr_Off.text    = DataMgr.Inst.GetLocal("Off");
        m_lbScr_Title.text  = DataMgr.Inst.GetLocal("Power saving mode");

        m_lbGra_On.text     = DataMgr.Inst.GetLocal("High");
        m_lbGra_Off.text    = DataMgr.Inst.GetLocal("Low");
        m_lbGra_Title.text  = DataMgr.Inst.GetLocal("Resolution");

        m_lbPar_On.text     = DataMgr.Inst.GetLocal("High");
        m_lbPar_Off.text    = DataMgr.Inst.GetLocal("Low");
        m_lbPar_Title.text  = DataMgr.Inst.GetLocal("Effect");

        m_lbSeesion.text        = DataMgr.Inst.GetLocal("Game ID");
        m_lbNotice.text         = DataMgr.Inst.GetLocal("Game Tips");
        m_CustomerCenter.text   = DataMgr.Inst.GetLocal("Customer Center");
		
		UID.text = "Game ID : " + DataMgr.Inst.m_UserInfo.ID;
        // m_BtnRestore.transform.FindChild("Label").GetComponent<UILabel>().text = DataMgr.Inst.GetLocal("Restore Btn");
        // m_BtnRestore.GetComponent<BoxCollider>().enabled = true;
	}

    void CheckOption()
    {        
		if(PlayerPrefs.GetInt("Snd",1) == 1)
        {
			Snd_Btn[0].startsActive = true;
            AudioListener.volume = 1f;
		}
        else
        {
			Snd_Btn[1].startsActive = true;
            AudioListener.volume = 0f;
		}

		if(PlayerPrefs.GetInt("Scr",1) == 1)
        {
			Scr_Btn[0].startsActive = true;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}
        else
        {
			Scr_Btn[1].startsActive = true;
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
		}

		if(PlayerPrefs.GetInt("Gra",1) == 1)
        {
			Gra_Btn[0].startsActive = true;
            StartCoroutine(coSetResolution(1));
		}
        else
        {
			Gra_Btn[1].startsActive = true;
            StartCoroutine(coSetResolution(0));
		}

		if(PlayerPrefs.GetInt("Par",1) == 1)
        {
			Par_Btn[0].startsActive = true;
		}
        else
        {
			Par_Btn[1].startsActive = true;
		}

    }

	void btn_back()
    {
		DataMgr.Inst.SetPageState(DataMgr.emMAIN_MODE.MENU);
	}

	//각 설정 버튼에대한 처리 및 변경내용 저장.
	void sndOn()
    {
		PlayerPrefs.SetInt ("Snd", 1);
		AudioListener.volume = 1f;
	}
	
	void sndOff()
    {
		PlayerPrefs.SetInt ("Snd", 0);
		AudioListener.volume = 0f;
	}

	void scrOn()
    {   
        // 절전모드 없애기.
		PlayerPrefs.SetInt ("Scr", 1);
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}
	
	void scrOff()
    {
        // 절전모드 가동.
		PlayerPrefs.SetInt ("Scr", 0);
		Screen.sleepTimeout = SleepTimeout.SystemSetting;
	}

	void graOn()
    {
        StartCoroutine(coSetResolution(1));		
	}

	void graOff()
    {
        StartCoroutine(coSetResolution(0));		
	}

	void parOn()
    {
		PlayerPrefs.SetInt ("Par", 1);
	}
	
	void parOff()
    {
		PlayerPrefs.SetInt ("Par", 0);
	}
    
    string[] m_Layer_NO = new string[]{"Nothing"}; 
    string[] m_Layer_UI = new string[]{"UI"}; 

    IEnumerator coSetResolution( int a_nGraphic )
    { 
        if(a_nGraphic == 1)
        {
            if(PlayerPrefs.GetInt("Gra", -1) == 1) yield break;
            PlayerPrefs.SetInt ("Gra", 1);
                        
            m_LobbyCamera.cullingMask = LayerMask.GetMask(m_Layer_NO);
            yield return new WaitForSeconds(0.2f);

		    Screen.SetResolution(Defines.DEF_DEGINE_SCREEN_WIDHT, Defines.DEF_DEGINE_SCREEN_HEIGHT, true);
            yield return new WaitForSeconds(0.8f);
             
            m_LobbyCamera.cullingMask = LayerMask.GetMask(m_Layer_UI);            
        }
        else if(a_nGraphic == 0)
        {
            if(PlayerPrefs.GetInt("Gra", -1) == 0) yield break;
            PlayerPrefs.SetInt ("Gra", 0);            
            m_LobbyCamera.cullingMask = LayerMask.GetMask(m_Layer_NO);
            yield return new WaitForSeconds(0.2f);

		    Screen.SetResolution(Defines.DEF_DEGINE_SCREEN_WIDHT/2, Defines.DEF_DEGINE_SCREEN_HEIGHT/2, true);
            yield return new WaitForSeconds(0.8f);

            m_LobbyCamera.cullingMask = LayerMask.GetMask(m_Layer_UI);
        }
    }

	//옵션탭 클릭.
	void ClickCat1()
    {
		for(int i=0; i<Category.Length; i++)
        {
			if(i==0) Category[i].SetActive(true);
			else Category[i].SetActive(false);
		}
	}

	//공지탭 클릭.
	void ClickCat2()
    {
		for(int i=0; i<Category.Length; i++)
        {
			if(i==1) Category[i].SetActive(true);
			else Category[i].SetActive(false);
		}
	}

	void clickNotice()
    {
	//	GameObject obj = Instantiate(Resources.Load("Prefabs/UI/Panel_UI_Notice") as GameObject) as GameObject;
	//	obj.transform.parent = m_RootLobby.transform;
	//	obj.transform.localScale = Vector3.one;
	//	obj.transform.localPosition = Vector3.zero;
	}

	void clickHelp() 
    {
        DataMgr.Inst.SetPageState( DataMgr.emMAIN_MODE.HELP, DataMgr.emMAIN_MODE.OPTION );
	}

    void clickMenual()
    {
	    DataMgr.Inst.SetPageState( DataMgr.emMAIN_MODE.TUTORIAL, DataMgr.emMAIN_MODE.OPTION );
	}

    void ClickLogOut()
    {
        CMessageBox.Create("Do you logout?", 2, MsgLogout);
    }

    void MsgLogout(bool a_bOK )
    {
        if(a_bOK)
        {
            DataMgr.Inst.m_SerMgr.LogOut();
            Destroy( GameObject.Find("UI Root") );
            Destroy( GameObject.Find("AndroidBridge") );
#if UNITY_IOS && !UNITY_EDITOR
            Destroy( GameObject.Find("IOSPayment") );
#elif UNITY_ANDROID && !UNITY_EDITOR
            Destroy( GameObject.Find("Tapjoy") );
            Destroy( GameObject.Find("TapjoyUnity") );
#endif
            Destroy( GameObject.Find("DataMgr") );
            PlayerPrefs.SetString("ID", "");
            PlayerPrefs.SetString("PW", "");
            Application.LoadLevel("LogoScene");
        }
    }
    /*/복구관련.
    public void clickRestore() 
    {
        DataMgr.Inst.m_PanelManager.BeginPopup( DataMgr.emPOPUP_STATE.emRestoreAppley, gameObject );
    }

    void PanelPopup_CallBackOK()
    {
        DataMgr.Inst.m_PanelManager.Loading();
        DataMgr.Inst.m_ServerMgr.SEND_RESTORE_NOTIFICATION( ESTORE_NOTIFICATION_RESULT );        
        m_BtnRestore.GetComponent<UIButton>().GetComponent<Collider>().enabled = false;       
    }
    
    
    public void ESTORE_NOTIFICATION_RESULT(Dictionary<string, object> a_dicJson)
    { 
        DataMgr.Inst.m_PanelManager.DeleteLoadingNow();

        if(a_dicJson != null)
        { 
            // 값이 제대로 있는지 검사한다.             
            string _Info            = a_dicJson["Info"].ToString();
            string _CardList        = a_dicJson["CardList"].ToString();
            string _DiyCardList     = a_dicJson["DiyCardList"].ToString();                        
            string _Stage           = a_dicJson["StageInfo"].ToString();            
            string _Mission         = a_dicJson["MissionInfo"].ToString();                        

            if( _Info == null || _DiyCardList == null || _CardList == null ||
                DataMgr.Inst.m_SQLManager.CheckJson_GameData(_Info ) == false ||
                DataMgr.Inst.m_SQLManager.CheckJson_CardData(_CardList ) == false ||
                DataMgr.Inst.m_SQLManager.CheckJson_DiyCardData(_DiyCardList ) == false ||
                DataMgr.Inst.m_SQLManager.CheckJson_StageData(_Stage ) == false ||
                DataMgr.Inst.m_SQLManager.CheckJson_MissionData(_Mission ) == false  )
            {                        
                DataMgr.Inst.m_PanelManager.BeginPopup( DataMgr.emPOPUP_STATE.emRestoreFAILED, null );                         
                return;
            }                      

            DataMgr.Inst.m_SQLManager.SetUserData( _Info, _CardList, _DiyCardList, _Stage, _Mission );
            DataMgr.Inst.m_UI_TOP.Reflesh();
            DataMgr.Inst.m_ServerMgr.SEND_RESTORE_COMPLET(null);            
            DataMgr.Inst.m_PanelManager.BeginPopup( DataMgr.emPOPUP_STATE.emRestoreSUCC, null );
            DataMgr.Inst.SetIsRestore(false);            
        }
        else
        {            
            DataMgr.Inst.m_PanelManager.BeginPopup( DataMgr.emPOPUP_STATE.emRestoreFAILED, null );
        }        
    }*/
    

}