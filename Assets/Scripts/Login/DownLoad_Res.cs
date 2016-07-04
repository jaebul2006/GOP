using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DownLoad_Res : MonoBehaviour 
{    
    public GameObject   m_UIRoot;
    public UILabel      m_lbAlert;
    private bool        m_bAlert;

    //[PackageSelector]
    //public string       m_Package;

    
    public UIProgressBar    m_bar;    
    private int             m_reTry;
    private bool            m_bTry;


    private int    m_AlertTimeLimit;
    private string [] m_AlertText;
    private int m_AlertIndex;

    // Use this for initialization.
    void Start ()
    {        
        m_bar.value = 0.0f;
        m_AlertTimeLimit = 0;
        m_AlertText = new string[4];
        m_AlertText[0] = "Download";
        m_AlertText[1] = "Download.";
        m_AlertText[2] = "Download..";
        m_AlertText[3] = "Download...";
        m_AlertIndex = 0;
        m_lbAlert.text = "Download";
	}
	
	// Update is called once per frame.
	void Update() 
    {
        if(m_bAlert)
        {
            if( System.Environment.TickCount > m_AlertTimeLimit)
            {
                m_AlertTimeLimit = System.Environment.TickCount + 400;                
                m_AlertIndex++; m_AlertIndex %= m_AlertText.Length;                
                m_lbAlert.text = m_AlertText[m_AlertIndex];
                Debug.Log("aa"+m_AlertIndex);
            }            
        }
        
	    if(m_bTry)
        {
            DataMgr.Inst.m_SerMgr.StaticDB(Result_StaticDB, Alert_DlBar);   
            m_bTry = false;
        }
	}


    
    string  m_strStcDBVer;
    Dictionary<string, object> m_TempLoginData;
    public void BeignDownload(Dictionary<string, object> a_LoginData)
    {        
        m_strStcDBVer = a_LoginData["StcDBVer"].ToString();        
        string strStcDBVer_App  = PlayerPrefs.GetString("StcDBVer", "");                
        m_reTry = 2;
        m_bAlert = false;
	    m_bTry = false;
#if UNITY_EDITOR

#else
/*        RemotePackageManager.Load(m_Package).GetAll<Object>(objs => {
            foreach (Object o in objs)
            {
                if (o.GetType().Name == "")
                {
                    //Debug.Log("Found asset: '" + o.name + "' of type: " + o.GetType().Name);
                }
        
            }   
        }); */
#endif

        m_TempLoginData = a_LoginData;

        DataMgr.Inst.m_UserInfo.ID = PlayerPrefs.GetString("ID", "");
        DataMgr.Inst.m_UserInfo.Session = a_LoginData["Session"].ToString();

        

        // 마지막에 실행.
        if (!DataMgr.Inst.IsStaticDB() || (m_strStcDBVer != strStcDBVer_App) )
        {
            m_bAlert = true;
            DataMgr.Inst.m_SerMgr.StaticDB(Result_StaticDB, Alert_DlBar); // 정적 데이터 요청.            
        }
            
        else
        { 
            m_lbAlert.text = "Lobby Loading";
            DataMgr.Inst.LoadStaticDB();
            EndDownload();
        }
    }

    public void Result_StaticDB(bool a_Result, Dictionary<string, object> a_dicJson, string a_Str, serverManager.ErrorCode a_ErrorCode)
    {
        if(a_Result)
        {
            m_lbAlert.text = "Lobby Loading";
            DataMgr.Inst.SaveStaticDB(a_Str);
            DataMgr.Inst.LoadStaticDB(a_dicJson);
            EndDownload();
            PlayerPrefs.SetString("StcDBVer", m_strStcDBVer);
            m_bAlert = false;
        }
        else
        {
            m_reTry--;
            if(m_reTry > 0)
            {
                m_bTry = true;
            }
            else
            {
                CMessageBox.Create( DataMgr.Inst.GetLocal("Failed to request data.\nWill you try again?"), 2, onCheckver_Eror);
            }
            
        }
        
    }

    public void onCheckver_Eror( bool a_bValue )
    {
        if(a_bValue)
        {
            DataMgr.Inst.m_SerMgr.StaticDB(Result_StaticDB, Alert_DlBar);
        }
        else    
            Application.Quit();
    }

    public void Alert_DlBar(int a_uiRemainSize, int a_uiMaxSize)
    {
        m_bar.value = (float)a_uiRemainSize/(float)a_uiMaxSize;
    }

    void EndDownload()
    {
        DataMgr.Inst.LoginSetting(PlayerPrefs.GetString("ID", ""), m_TempLoginData);        
        Application.LoadLevel("LobbyScene");
    }

}
