using UnityEngine;
using System.Collections;

public class CSummonSlot : MonoBehaviour 
{    
    public  int                     m_TeamIdx;
    public  GameObject              m_OverMark;
    public  UISprite                m_sprBG;
    public  UISprite                m_Attri;
    [System.NonSerialized] public DataMgr.tagUserSummon         m_Info;
    [System.NonSerialized] public DataMgr.tagSaticSummonSkill   m_Skill;
    [System.NonSerialized] public Defines.Delegate_Int          m_Delegate_AlertSkill;
    private UITexture               m_Face;
    private UIProgressBar           m_SkillGaugeBar;
    private int                     m_SkillGaugeValue;
    private bool                    m_bMaxGauge;
    public  bool                    m_isSkilling;

    public void OnOver()
    {
        if(m_bMaxGauge) m_OverMark.SetActive(true);
    }
    public void OffOver()
    {
        m_OverMark.SetActive(false);
    }

    public void ResetData()
    {
        m_Info = DataMgr.Inst.m_UserSummonList[ DataMgr.Inst.m_UserInfo.Team[DataMgr.Inst.m_TeamIdx][m_TeamIdx]];
        m_Skill = DataMgr.Inst.m_DB_SummonSkill[ m_Info.Data.nSkill ];
    }

	// Use this for initialization.
	void Awake() 
    {
        m_Face = transform.FindChild("Summon").GetComponent<UITexture>();            
        m_SkillGaugeBar = transform.FindChild("Skill").GetComponent<UIProgressBar>();

        if (DataMgr.Inst.m_UserInfo.Team[DataMgr.Inst.m_TeamIdx][m_TeamIdx] == -1)
        {
            gameObject.SetActive(false);
        }
        else
        {
            m_Info = DataMgr.Inst.m_UserSummonList[ DataMgr.Inst.m_UserInfo.Team[DataMgr.Inst.m_TeamIdx][m_TeamIdx]];
            m_Skill = DataMgr.Inst.m_DB_SummonSkill[ m_Info.Data.nSkill ];
            m_Face.mainTexture = Resources.Load("Textures/Char/icon_mon_" + m_Info.Data.strResurceID ) as Texture;
            m_SkillGaugeBar.value = 0.0f;
            m_SkillGaugeValue = 0;
            
            switch(m_Info.Data.eAttr)
            {
                case DataMgr.emCardAttribute.emCAB_WA: m_Attri.spriteName = "Small_icon_attr_Fire"; break;
                case DataMgr.emCardAttribute.emCAB_SU: m_Attri.spriteName = "Small_icon_attr_ICE"; break;
                case DataMgr.emCardAttribute.emCAB_YUNG: m_Attri.spriteName = "Small_icon_attr_light"; break;
                case DataMgr.emCardAttribute.emCAB_ARM: m_Attri.spriteName = "Small_icon_attr_dark"; break;
            }
        }

        m_isSkilling = false;
        m_bMaxGauge = false;
        m_OverMark.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
    {	    
	}

    public void ResetGauge()
    {
        m_SkillGaugeValue   = 0;
        m_bMaxGauge         = false;
        m_OverMark.SetActive(false);            
        m_SkillGaugeBar.value = 0;

        // Debug 조치. 
    //    m_SkillGaugeValue = m_Info.Data.nMaxGuage;
    //    m_SkillGaugeBar.value = 1f;
    //    m_bMaxGauge = true;
    }
    public void AddGauge()
    {        
        m_SkillGaugeValue += Defines.DEF_ADD_GAUGE;
        
        if( m_SkillGaugeValue >= m_Info.Data.nMaxGuage && 
            m_bMaxGauge == false )
        {
            m_SkillGaugeValue = m_Info.Data.nMaxGuage;
            m_bMaxGauge = true;
        }            

        m_SkillGaugeBar.value = (float)m_SkillGaugeValue / (float)m_Info.Data.nMaxGuage;
    }

    // 스킬 클릭했을때.
    public bool onBtnSkill()
    {
        if(m_SkillGaugeBar.value < 1f) return false;
        if(m_bMaxGauge == false) return false;

        m_SkillGaugeValue = 0;
        m_SkillGaugeBar.value = 0f;
        m_bMaxGauge = false;
        m_isSkilling = true;
        SlotSkillFff();        
        return true;
    }

    // 스킬 이펙트.
    private TweenScale m_SkillEff_TS;    
    public void SlotSkillFff(  )
    {
        m_SkillEff_TS        = gameObject.AddComponent<TweenScale>();
        m_SkillEff_TS.from   = transform.localScale;

        Vector3 vTemp = new Vector3();
        vTemp.x = transform.localScale.x+1;
        vTemp.y = transform.localScale.y+1;
        vTemp.z = transform.localScale.z;

        m_SkillEff_TS.to                 = vTemp;
        m_SkillEff_TS.style              = UITweener.Style.Once;
        m_SkillEff_TS.duration           = 0.1f;
        m_SkillEff_TS.method             = UITweener.Method.Linear;
        m_SkillEff_TS.callWhenFinished   = "SlotSkillFff_1";
        m_SkillEff_TS.eventReceiver      = gameObject;
    }

    void SlotSkillFff_1()
    {
        Destroy(m_SkillEff_TS);
        m_SkillEff_TS        = gameObject.AddComponent<TweenScale>();
        m_SkillEff_TS.from   = transform.localScale;

        Vector3 vTemp = new Vector3();
        vTemp.x = transform.localScale.x-1f;
        vTemp.y = transform.localScale.y-1f;
        vTemp.z = transform.localScale.z;

        m_SkillEff_TS.to                 = vTemp;
        m_SkillEff_TS.style              = UITweener.Style.Once;
        m_SkillEff_TS.duration           = 0.1f;
        m_SkillEff_TS.method             = UITweener.Method.Linear;
        m_SkillEff_TS.callWhenFinished   = "SlotSkillFff_2";
        m_SkillEff_TS.eventReceiver      = gameObject;
    }

    void SlotSkillFff_2()
    {
        Destroy(m_SkillEff_TS);
        m_Delegate_AlertSkill(m_TeamIdx);
        m_OverMark.SetActive(false);
    }    

    //발사 이펙트.
    TweenPosition m_FireEff_TP;
    public void SlotFireEff()
    {
        m_FireEff_TP        = gameObject.AddComponent<TweenPosition>();
        m_FireEff_TP.from   = transform.localPosition;

        Vector3 vTemp = new Vector3();
        vTemp.x = transform.localPosition.x;
        vTemp.y = transform.localPosition.y-10;
        vTemp.z = transform.localPosition.z; 

        m_FireEff_TP.to                 = vTemp;
        m_FireEff_TP.style              = UITweener.Style.Once;
        m_FireEff_TP.duration           = 0.1f;
        m_FireEff_TP.method             = UITweener.Method.Linear;
        m_FireEff_TP.callWhenFinished   = "SlotFireEff_2";
        m_FireEff_TP.eventReceiver      = gameObject;
    }

    void SlotFireEff_2()
    {
        Destroy(m_FireEff_TP);

        m_FireEff_TP        = gameObject.AddComponent<TweenPosition>();
        m_FireEff_TP.from   = transform.localPosition;

        Vector3 vTemp = new Vector3();
        vTemp.x = transform.localPosition.x;
        vTemp.y = transform.localPosition.y+10;
        vTemp.z = transform.localPosition.z;

        m_FireEff_TP.to                 = vTemp;
        m_FireEff_TP.style              = UITweener.Style.Once;
        m_FireEff_TP.duration           = 0.3f;
        m_FireEff_TP.method             = UITweener.Method.Linear;
        m_FireEff_TP.callWhenFinished   = "SlotFireEff_End";
        m_FireEff_TP.eventReceiver      = gameObject;
    }

    void SlotFireEff_End()
    {
        Destroy(m_FireEff_TP);
    }

    //속성상성인 아군 속성마크표시.
    public bool m_isAttrimark;
    public TweenScale m_Attrimark_TS;
    public void SetAttrimark(bool a_is)
    {
        m_isAttrimark = a_is;
        if(m_isAttrimark)
        {
            m_Attri.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            AttrimarkEff();
        }
        else
        { 
            Destroy(m_Attrimark_TS);
            m_Attri.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }


    public void AttrimarkEff()
    {
        m_Attrimark_TS                  = m_Attri.gameObject.AddComponent<TweenScale>();
        m_Attrimark_TS.from             = new Vector3(1f, 1f, 1f);
        m_Attrimark_TS.to               = new Vector3(1.4f, 1.4f, 1.4f);
        m_Attrimark_TS.style            = UITweener.Style.PingPong;
        m_Attrimark_TS.duration         = 0.6f;
        m_Attrimark_TS.method           = UITweener.Method.Linear;        
    }

}
