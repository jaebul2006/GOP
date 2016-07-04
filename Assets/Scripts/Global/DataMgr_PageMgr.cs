using UnityEngine;
using System.Collections;

public partial class DataMgr : MonoBehaviour
{
    public enum emMAIN_MODE
    {
        MENU = 0,
        PLAY,
        SEL_STAGE,

        SHOP,

        MISSION,
        COLLECTION,

        FRIEND,
        NOTIFY,

        MYTEAM,
        MYCARD,
        PUSION,
        UPGRADE,

        AURORA,

        OPTION,
        HELP,
        TUTORIAL,
        END,
    };

#if UNITY_ANDROID
    public  TapJoyMgr   m_TapJoyMgr;
#endif
    [System.NonSerialized]public  GameObject  m_TopObj; // 
    // 비공개변수들.

    private Lobby m_Lobby = null;
    private GameObject[] m_Page = new GameObject[(int)(DataMgr.emMAIN_MODE.END)];
    private emMAIN_MODE m_MainState = emMAIN_MODE.MENU;
    private DataMgr.emMAIN_MODE m_backState = emMAIN_MODE.MENU; // 가야할 페이지가 있을때.
    
    // 함수들 
    public emMAIN_MODE GetPageState() { return m_MainState; }

    // Lobby 처음시작할떄 호출되는 함수.
    public void Setup_Lobby(Lobby a_Parent)
    {        
        float fY;
        m_Lobby = a_Parent;
        m_Page[(int)(DataMgr.emMAIN_MODE.MYTEAM)] = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/UI/Panel_UI_Bag_Team_New"));
        fY = m_Page[(int)(DataMgr.emMAIN_MODE.MYTEAM)].transform.localPosition.y;
        m_Page[(int)(DataMgr.emMAIN_MODE.MYTEAM)].transform.parent = a_Parent.transform;
        m_Page[(int)(DataMgr.emMAIN_MODE.MYTEAM)].transform.localPosition = new Vector3(0, fY, 0);
        m_Page[(int)(DataMgr.emMAIN_MODE.MYTEAM)].transform.localScale = new Vector3(1, 1, 1);
        m_Page[(int)(DataMgr.emMAIN_MODE.MYTEAM)].SendMessage("SetUp", a_Parent.gameObject, SendMessageOptions.DontRequireReceiver);
        m_Page[(int)(DataMgr.emMAIN_MODE.MYTEAM)].SetActive(false);

        m_Page[(int)(DataMgr.emMAIN_MODE.UPGRADE)] = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/UI/Panel_UI_Bag_Upgrade"));
        fY = m_Page[(int)(DataMgr.emMAIN_MODE.UPGRADE)].transform.localPosition.y;
        m_Page[(int)(DataMgr.emMAIN_MODE.UPGRADE)].transform.parent = a_Parent.transform;
        m_Page[(int)(DataMgr.emMAIN_MODE.UPGRADE)].transform.localPosition = new Vector3(0, fY, 0);
        m_Page[(int)(DataMgr.emMAIN_MODE.UPGRADE)].transform.localScale = new Vector3(1, 1, 1);
        m_Page[(int)(DataMgr.emMAIN_MODE.UPGRADE)].SendMessage("SetUp", a_Parent.gameObject, SendMessageOptions.DontRequireReceiver);
        m_Page[(int)(DataMgr.emMAIN_MODE.UPGRADE)].SetActive(false);

        m_Page[(int)(DataMgr.emMAIN_MODE.PUSION)] = (GameObject)Instantiate( (GameObject)Resources.Load("Prefabs/UI/Panel_UI_Bag_Sysnthesis") );
        fY = m_Page[(int)(DataMgr.emMAIN_MODE.PUSION)].transform.localPosition.y;
        m_Page[(int)(DataMgr.emMAIN_MODE.PUSION)].transform.parent = a_Parent.transform;
        m_Page[(int)(DataMgr.emMAIN_MODE.PUSION)].transform.localPosition = new Vector3(0, fY, 0);
        m_Page[(int)(DataMgr.emMAIN_MODE.PUSION)].transform.localScale = new Vector3(1, 1, 1);
        m_Page[(int)(DataMgr.emMAIN_MODE.PUSION)].SendMessage("SetUp", a_Parent.gameObject, SendMessageOptions.DontRequireReceiver);
        m_Page[(int)(DataMgr.emMAIN_MODE.PUSION)].SetActive(false);

        m_Page[(int)(DataMgr.emMAIN_MODE.SHOP)] = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/UI/Panel_UI_Shop"));
        fY = m_Page[(int)(DataMgr.emMAIN_MODE.SHOP)].transform.localPosition.y;
        m_Page[(int)(DataMgr.emMAIN_MODE.SHOP)].transform.parent = a_Parent.transform;
        m_Page[(int)(DataMgr.emMAIN_MODE.SHOP)].transform.localPosition = new Vector3(0, fY, 0);
        m_Page[(int)(DataMgr.emMAIN_MODE.SHOP)].transform.localScale = new Vector3(1, 1, 1);
        m_Page[(int)(DataMgr.emMAIN_MODE.SHOP)].SendMessage("SetUp", a_Parent.gameObject, SendMessageOptions.DontRequireReceiver);
        m_Page[(int)(DataMgr.emMAIN_MODE.SHOP)].SetActive(false);

        m_Page[(int)(DataMgr.emMAIN_MODE.AURORA)] = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/UI/Panel_UI_Aurora"));
        fY = m_Page[(int)(DataMgr.emMAIN_MODE.AURORA)].transform.localPosition.y;
        m_Page[(int)(DataMgr.emMAIN_MODE.AURORA)].transform.parent = a_Parent.transform;
        m_Page[(int)(DataMgr.emMAIN_MODE.AURORA)].transform.localPosition = new Vector3(0, fY, 0);
        m_Page[(int)(DataMgr.emMAIN_MODE.AURORA)].transform.localScale = new Vector3(1, 1, 1);
        m_Page[(int)(DataMgr.emMAIN_MODE.AURORA)].SendMessage("SetUp", a_Parent.gameObject, SendMessageOptions.DontRequireReceiver);
        m_Page[(int)(DataMgr.emMAIN_MODE.AURORA)].SetActive(false);

        m_Page[(int)(DataMgr.emMAIN_MODE.SEL_STAGE)] = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/UI/Panel_UI_SelStage"));
        fY = m_Page[(int)(DataMgr.emMAIN_MODE.SEL_STAGE)].transform.localPosition.y;
        m_Page[(int)(DataMgr.emMAIN_MODE.SEL_STAGE)].transform.parent = a_Parent.transform;
        m_Page[(int)(DataMgr.emMAIN_MODE.SEL_STAGE)].transform.localPosition = new Vector3(0, fY, 0);
        m_Page[(int)(DataMgr.emMAIN_MODE.SEL_STAGE)].transform.localScale = new Vector3(1, 1, 1);
        m_Page[(int)(DataMgr.emMAIN_MODE.SEL_STAGE)].SendMessage("SetUp", a_Parent.gameObject, SendMessageOptions.DontRequireReceiver);
        m_Page[(int)(DataMgr.emMAIN_MODE.SEL_STAGE)].SetActive(false);

        m_Page[(int)(DataMgr.emMAIN_MODE.OPTION)] = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/UI/Panel_UI_Option"));
        fY = m_Page[(int)(DataMgr.emMAIN_MODE.OPTION)].transform.localPosition.y;
        m_Page[(int)(DataMgr.emMAIN_MODE.OPTION)].transform.parent = a_Parent.transform;
        m_Page[(int)(DataMgr.emMAIN_MODE.OPTION)].transform.localPosition = new Vector3(0, fY, 0);
        m_Page[(int)(DataMgr.emMAIN_MODE.OPTION)].transform.localScale = new Vector3(1, 1, 1);
        m_Page[(int)(DataMgr.emMAIN_MODE.OPTION)].SendMessage("SetUp", a_Parent.gameObject, SendMessageOptions.DontRequireReceiver);
        m_Page[(int)(DataMgr.emMAIN_MODE.OPTION)].SetActive(false);        

        m_Page[(int)(DataMgr.emMAIN_MODE.HELP)] = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/UI/Panel_UI_Help"));
        fY = m_Page[(int)(DataMgr.emMAIN_MODE.HELP)].transform.localPosition.y;
        m_Page[(int)(DataMgr.emMAIN_MODE.HELP)].transform.parent        = a_Parent.transform;
        m_Page[(int)(DataMgr.emMAIN_MODE.HELP)].transform.localPosition = new Vector3(0, fY, 0);
        m_Page[(int)(DataMgr.emMAIN_MODE.HELP)].transform.localScale    = new Vector3(1, 1, 1);
        m_Page[(int)(DataMgr.emMAIN_MODE.HELP)].SendMessage("SetUp", a_Parent.gameObject, SendMessageOptions.DontRequireReceiver);
        m_Page[(int)(DataMgr.emMAIN_MODE.HELP)].SetActive(false);        
        
        m_Page[(int)(DataMgr.emMAIN_MODE.NOTIFY)] = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/UI/Panel_UI_Notice"));
        fY = m_Page[(int)(DataMgr.emMAIN_MODE.NOTIFY)].transform.localPosition.y;
        m_Page[(int)(DataMgr.emMAIN_MODE.NOTIFY)].transform.parent        = a_Parent.transform;
        m_Page[(int)(DataMgr.emMAIN_MODE.NOTIFY)].transform.localPosition = new Vector3(0, fY, 0);
        m_Page[(int)(DataMgr.emMAIN_MODE.NOTIFY)].transform.localScale    = new Vector3(1, 1, 1);
        m_Page[(int)(DataMgr.emMAIN_MODE.NOTIFY)].SendMessage("SetUp", a_Parent.gameObject, SendMessageOptions.DontRequireReceiver);
        m_Page[(int)(DataMgr.emMAIN_MODE.NOTIFY)].SetActive(false);    
        
        m_Page[(int)(DataMgr.emMAIN_MODE.TUTORIAL)] = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/UI/Panel_UI_Tutorial"));
        fY = m_Page[(int)(DataMgr.emMAIN_MODE.TUTORIAL)].transform.localPosition.y;
        m_Page[(int)(DataMgr.emMAIN_MODE.TUTORIAL)].transform.parent        = a_Parent.transform;
        m_Page[(int)(DataMgr.emMAIN_MODE.TUTORIAL)].transform.localPosition = new Vector3(0, fY, 0);
        m_Page[(int)(DataMgr.emMAIN_MODE.TUTORIAL)].transform.localScale    = new Vector3(1, 1, 1);
        m_Page[(int)(DataMgr.emMAIN_MODE.TUTORIAL)].SetActive(false);            
        //.        
    }

    public GameObject GetPage(emMAIN_MODE a_Index)
    {
        return m_Page[(int)(a_Index)];
    }

    public void GetLobby_On()
    {
        if (m_Lobby == null) return;
        m_TopObj = m_Lobby.gameObject;
        m_Lobby.gameObject.SetActive(true);

        m_Page[(int)m_MainState].SendMessage("Begin", SendMessageOptions.DontRequireReceiver);
    }
    public void GetLobby_Off()
    {
        if (m_Lobby == null) return;
        m_Lobby.m_Panel_Load.gameObject.SetActive(false);
        m_Lobby.gameObject.SetActive(false);
    }
    public void GetLodingOn()
    {
        if (m_Lobby == null) return;
        m_Lobby.m_Panel_Load.gameObject.SetActive(true);
    }
    
    public void GetPayRaiz(string a_PayCount, Defines.Delegate_BoolStrStr a_Func)
    {
        if (m_Lobby == null) return;
#if UNITY_ANDROID
        m_Lobby.m_Payment.Payment_PayRaiz(a_PayCount, a_Func);
#endif
    }
    
    public void TestPayRaiz(string a_PayCount)
    {
        if (m_Lobby == null) return;
#if UNITY_ANDROID
        m_Lobby.m_Payment.Test_Payment(a_PayCount);
#endif
    }

    public void Msg_OK( string a_Con)
    {
        if (m_Lobby == null) return;
        CMessageBox.Create(a_Con, 1, null);
    }


    // 페이지 설정 
    public void SetPageState(DataMgr.emMAIN_MODE a_State, DataMgr.emMAIN_MODE a_backState = emMAIN_MODE.MENU)
    {
        m_backState = a_backState;
        int nState = (int)a_State;

        if (a_State == emMAIN_MODE.UPGRADE)
        {
            for (int i = 0; i < m_Page.Length; i++)
            {
                if (m_Page[i] != null)
                {
                    if (i == (int)emMAIN_MODE.UPGRADE)
                    {
                        m_Page[i].SetActive(true);
                        m_Page[i].GetComponent<UIPanel>().depth = 7;
                        m_MainState = a_State;
                    }
                    else if (i == (int)emMAIN_MODE.MYTEAM)
                    {
                        if (!m_Page[i].activeSelf)
                        {
                            m_Page[i].SetActive(true);
                            m_Page[i].GetComponent<UIPanel>().depth = 6;
                            m_MainState = a_State;
                        }
                    }
                    else m_Page[i].SetActive(false);
                }
            }
        }
        else if (a_State == emMAIN_MODE.PUSION)
        {
            for (int i = 0; i < m_Page.Length; i++)
            {
                if (m_Page[i] != null)
                {
                    if (i == (int)emMAIN_MODE.PUSION)
                    {
                        m_Page[i].SetActive(true);
                        m_Page[i].GetComponent<UIPanel>().depth = 7;                        
                        m_MainState = a_State;
                    }
                    else if (i == (int)emMAIN_MODE.MYTEAM)
                    {
                        if (!m_Page[i].activeSelf)
                        {
                            m_Page[i].SetActive(true);
                            m_Page[i].GetComponent<UIPanel>().depth = 6;
                            m_MainState = a_State;
                        }
                    }
                    else m_Page[i].SetActive(false);
                }
            }
        }
        else if (a_State == emMAIN_MODE.MENU)
        {            
            for (int i = 0; i < m_Page.Length; i++)
                if (m_Page[i] != null) m_Page[i].SetActive(false);            
        }
        else             
        {
            for (int i = 0; i < m_Page.Length; i++)
            { if (m_Page[i] != null) m_Page[i].SetActive(false); }

            if (m_Page[nState] != null)
            {
                m_Page[nState].SetActive(true);
                m_Page[nState].SendMessage("Begin", SendMessageOptions.DontRequireReceiver);
                
                if (a_State == emMAIN_MODE.TUTORIAL) m_Page[nState].GetComponent<UIPanel>().depth = 8;
                else m_Page[nState].GetComponent<UIPanel>().depth = 6;

                m_MainState = a_State;
            }
        }
    }

    public void SetBackPageState()
    {
        SetPageState(m_backState);        
    }//


}