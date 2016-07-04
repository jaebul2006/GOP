using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using TapjoyUnity;
#endif

public class Panel_Result : MonoBehaviour 
{    
    public GameObject       m_BG2_BoxOpen;        
    public GameObject       m_BG3;

    public UILabel          m_lbPlayTime;
    public UILabel          m_lbGetGold;
    public GameObject       m_Box;
    public GameObject       m_LightEffect;
    public GameObject       m_Give_Gold;
    public GameObject       m_Give_sum;

    public GameObject       m_BtnNextStage;
    public GameObject       m_BtnReplay;
    public GameObject       m_BtnExit;
    public GameObject       m_Light;

    public AudioSource      m_Sound1;
    public AudioSource      m_Sound2;

    [System.NonSerialized] public GameMgr m_GameMgr;
    [System.NonSerialized] public string m_SendNextStage;
    [System.NonSerialized] public string m_SendRetry;
    [System.NonSerialized] public string m_SendExit;
	
    private DataMgr.tagSaticAdventureStage m_StageInfo;

    private Vector3 m_BoxStartPos;
	void Start () 
    {
        m_BoxStartPos = new Vector3(m_Box.transform.localPosition.x,
            m_Box.transform.localPosition.y, 0.0f);
        m_Box.GetComponent<BoxCollider>().enabled = false;        
        m_LightEffect.SetActive(false);
        m_BtnNextStage.SetActive(false);
        m_BtnReplay.SetActive(false);
        m_BtnExit.SetActive(false);        
        m_Light.SetActive(false);
        m_BG2_BoxOpen.SetActive(false);
        m_BG3.SetActive(false);
	}
	
	
    private float m_fLight = 0f;
	void Update( )
    {
        if(m_Light.activeSelf )
        {
            m_fLight += (15f * Time.deltaTime);
            m_Light.transform.localRotation = Quaternion.Euler(0, 0, m_fLight);
        }
        
	}


    TweenPosition m_TP;
    // 떨어진다.
    public void Begin_Direction( DataMgr.tagSaticAdventureStage a_StageInfo )
    {        
        m_StageInfo = a_StageInfo;
        m_Give_Gold.SetActive(false);
        m_Give_sum.SetActive(false);
        m_BtnNextStage.SetActive(false);
        m_BtnReplay.SetActive(false);
        m_BtnExit.SetActive(false);
        m_Light.SetActive(false);        
        m_BG2_BoxOpen.SetActive(false);
        m_BG3.SetActive(false);

        m_bResult = false;
        DataMgr.Inst.m_SerMgr.AdventureEnd_Succ(a_StageInfo.nIDX, m_GameMgr.m_PlayTime_Second,
            m_GameMgr.m_PlayGetGold,
            m_GameMgr.m_Attri_AttackCount[0],
            m_GameMgr.m_Attri_AttackCount[1],
            m_GameMgr.m_Attri_AttackCount[2],
            m_GameMgr.m_Attri_AttackCount[3],
            AdventureEnd_Result);

        m_lbPlayTime.text = "Clear Time : " + m_GameMgr.m_PlayTime_Second;
        m_lbGetGold.text = "Get Bouns gold : " + m_GameMgr.m_PlayGetGold + "G";        
        
        m_Box.SetActive(true);
        m_Box.transform.localPosition = m_BoxStartPos;
        m_Box.GetComponent<BoxCollider>().enabled = false;
        m_TP                    = m_Box.AddComponent<TweenPosition>();
        m_TP.from               = new Vector3(0, 753.3f, 0f);
        m_TP.to                 = new Vector3(0, -14.1f, 0f);
        m_TP.style              = UITweener.Style.Once;
        m_TP.duration           = 2f;
        m_TP.method             = UITweener.Method.BounceIn; //Linear, EaseIn, EaseOut, EaseInOut, BounceIn, BounceOut,
        m_TP.callWhenFinished   = "Begin2";
        m_TP.eventReceiver      = gameObject;

        m_Sound1.Play();
        m_Sound2.Play();
    }

    // 도착.
    public void Begin2()
    {
        Destroy(m_TP);
        m_Box.GetComponent<Animator>().SetBool("Biv", true);
        m_Box.GetComponent<BoxCollider>().enabled = true;
    }

    // .
    public void Begin3()
    {        
        m_LightEffect.SetActive(false);        
        m_BtnNextStage.SetActive(true);
        m_BtnReplay.SetActive(true);
        m_BtnExit.SetActive(true);     
        m_Light.SetActive(true);
        m_BG3.SetActive(true);
        if(m_NewCardIdx == -1)
        {
            m_Give_Gold.SetActive(true);
            m_Give_Gold.transform.GetChild(0).GetComponent<UILabel>().text = m_GetGold.ToString();
        }
        else
        {
            m_Give_sum.SetActive(true);
            m_Give_sum.GetComponent<UITexture>().mainTexture = 
                Resources.Load( "Textures/Char/icon_mon_" + DataMgr.Inst.m_UserSummonList[m_NewCardIdx].Data.strResurceID ) as Texture;

            m_Give_sum.transform.FindChild("lbName").GetComponent<UILabel>().text = DataMgr.Inst.m_UserSummonList[m_NewCardIdx].Data.strName;
            m_Give_sum.transform.FindChild("Grade").GetComponent<UISprite>().spriteName = "icon_grade_" + DataMgr.Inst.m_UserSummonList[m_NewCardIdx].nGrade.ToString("00");
        }
    }

    private bool    m_bResult = false;
    private int     m_NewCardIdx = -1;
    private int     m_GetGold;
    public void AdventureEnd_Result(bool a_Result, Dictionary<string, object> a_dicJson, string a_Str, serverManager.ErrorCode a_ErrorCode)
    {
        if(a_Result == true)
        {
            int AffterGold = DataMgr.Inst.m_UserInfo.Gold;
            DataMgr.Inst.m_UserInfo.Gold = DataMgr.Inst.CastInt(a_dicJson["Gold"]);
            //m_GetGold = DataMgr.Inst.m_UserInfo.Gold - (m_StageInfo.nReward_Gold + AffterGold + m_GameMgr.m_PlayGetGold);
            m_GetGold = DataMgr.Inst.m_UserInfo.Gold - (AffterGold + m_GameMgr.m_PlayGetGold);
            

            DataMgr.Inst.SetCardList( a_dicJson["CardList"].ToString() );
            
            for (int i = 0; i < Defines.DEF_MAX_TEAM_ITEM; i++)
            {            
                if (m_GameMgr.m_SumSlot[i].gameObject.activeSelf)
                {
                    m_GameMgr.m_SumSlot[i].ResetData();
                }
            }

            
            string advenGrade = a_dicJson["AdvenGrade"].ToString();
            
            string OutCode = a_dicJson["OutCode"].ToString();

            if (OutCode == "S" || OutCode == "D")
                m_NewCardIdx = int.Parse( a_dicJson["S"].ToString() );             
           
            DataMgr.Inst.m_UserInfo.Crown = DataMgr.Inst.CastInt( a_dicJson["Crown"] );

            if( a_dicJson.ContainsKey("AdvenStage") )
            {
                string [] strAdvenStage = a_dicJson["AdvenStage"].ToString().Split('_');
                DataMgr.Inst.m_UserInfo.nAdven_difficulty = int.Parse(strAdvenStage[0]);
                DataMgr.Inst.m_UserInfo.nAdven_Stage      = int.Parse(strAdvenStage[1]);
                DataMgr.Inst.m_UserInfo.nAdven_Floor      = int.Parse(strAdvenStage[2]);
            }

            DataMgr.Inst.m_Accrue._AdventureClear++;


            if(a_dicJson.ContainsKey("pLevel"))
            {
                DataMgr.Inst.m_UserInfo.Level =  DataMgr.Inst.CastInt(a_dicJson["pLevel"]);
#if UNITY_ANDROID
                Tapjoy.SetUserLevel(DataMgr.Inst.m_UserInfo.Level); 
#endif
            }
            

            m_bResult = true;
        }
        else
        {
             CMessageBox.Create( DataMgr.Inst.GetLocal("Error occurred."), 1, onAdven_Eror);
        }
    }    

    // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=.
    // 버튼콜백 함수들.
    TweenScale m_TS;
    public void onBtnBox()
    {
        if(!m_bResult) return;        
        m_Box.SetActive(false);   
        m_BG2_BoxOpen.SetActive(true);        

        m_LightEffect.SetActive(true);
        m_TS                    = m_LightEffect.AddComponent<TweenScale>();
        m_TS.from               = new Vector3(0.0f, 0.0f, 0.0f);
        m_TS.to                 = new Vector3(50.0f, 50.0f, 0f);
        m_TS.style              = UITweener.Style.Once;
        m_TS.duration           = 1;
        m_TS.method             = UITweener.Method.Linear;
        m_TS.callWhenFinished   = "Begin3";
        m_TS.eventReceiver      = gameObject;
    }

    public void onBtnBox2()
    {        
        
    }

    
    public void onAdven_Eror(bool a_Value)
    {
        Exit();
    }

    public void Exit()
    {
        //m_GameMgr.gameObject.SendMessage( m_SendExit, SendMessageOptions.DontRequireReceiver);
        DataMgr.Inst.GetLobby_On();
        Destroy(m_GameMgr.gameObject);
    }

    public void NextStage()
    {
        m_GameMgr.gameObject.SendMessage( m_SendNextStage, SendMessageOptions.DontRequireReceiver);
    }

    public void Retry()
    {
        m_GameMgr.gameObject.SendMessage( m_SendRetry, SendMessageOptions.DontRequireReceiver);
    }
}
