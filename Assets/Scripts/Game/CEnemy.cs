using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 0.7035036 2.369972 -11.47389
// 32.44 -51.7 4.577637e-05

public class CEnemy : MonoBehaviour 
{
    [System.NonSerialized] public GameObject                m_Parent;
    [System.NonSerialized] public DataMgr.tagSaticMonster   m_Info;
    [System.NonSerialized] public int                       m_IndexPos;
    [System.NonSerialized] public Animation                 m_Ani;
    
    private AudioSource     m_AtkEff;
    private int             m_MaxHp;
    private int             m_Hp;
    private int             m_WaitTurn;
    private GameMgr         m_GameMgr;

    private GameObject m_DamagePos;
    private GameObject m_Gauge;
        
    private UISlider    m_HpGauge;
    private UILabel     m_lbTurn;

    private GameObject  m_ObjUIPoint3D;
    [System.NonSerialized] public Vector2     m_UIPoint = new Vector2();    
    [System.NonSerialized] public Vector2     m_MonCenterPoint = new Vector2();

    
    public bool GetIsNotAtackState() { return m_Hp <= 0 ? true:false; } // 공격받으면 안되는 상태의 적.

    public static CEnemy Create( GameMgr a_GameMgr, UISlider a_HpGauge, int a_IndexPos, GameObject a_Parent, DataMgr.tagSaticMonster a_Monster , bool a_bBoss)
    {
        GameObject Monster              = Instantiate( Resources.Load("Prefabs/3D/OBJ_" + a_Monster.strResurceID ) as GameObject );
        Monster.transform.parent        = a_Parent.transform;
        Monster.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        Monster.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        Monster.transform.FindChild("SelectTurn").gameObject.SetActive(false);
        Monster.transform.FindChild("Shadow").gameObject.SetActive(false);        

        if (a_bBoss)
        {
            Monster.transform.localScale    = new Vector3(
            Monster.transform.localScale.x+0.01f, 
            Monster.transform.localScale.y+0.01f, 
            Monster.transform.localScale.z+0.01f);
        }
        
        
        CEnemy Script                   = Monster.AddComponent<CEnemy>();
        Script.m_AtkEff                 = Monster.GetComponent<AudioSource>();
        Script.m_Parent                 = a_Parent;
        Script.m_Info                   = a_Monster;
        Script.m_IndexPos               = a_IndexPos;
        Script.m_Ani    	            = Monster.GetComponent<Animation>();
        Script.m_MaxHp                  = a_Monster.nHp;
        Script.m_Hp                     = a_Monster.nHp;  
        Script.m_WaitTurn               = a_Monster.nWaitTurn;
        Script.m_GameMgr                = a_GameMgr;
        Script.m_ObjUIPoint3D           = Monster.transform.FindChild("UI_Gizmo").gameObject;
        Script.m_HpGauge                = a_HpGauge;
        Script.m_HpGauge.value          = 1.0f;
        Script.m_lbTurn                 = a_HpGauge.transform.FindChild("LbTurn").GetComponent<UILabel>();
        
        string Name = "Small_icon_attr_Fire";
        if(a_Monster.eAttr == DataMgr.emCardAttribute.emCAB_WA)   // 화 상극 : 명.
            Name = "Small_icon_attr_Fire";
		if(a_Monster.eAttr == DataMgr.emCardAttribute.emCAB_SU)    	// 수 상극 : 화.
            Name = "Small_icon_attr_ICE";
		if(a_Monster.eAttr == DataMgr.emCardAttribute.emCAB_YUNG)     // 명 상극 : 암.
            Name = "Small_icon_attr_light";
		if(a_Monster.eAttr == DataMgr.emCardAttribute.emCAB_ARM)    	// 암 상극 : 명.
            Name = "Small_icon_attr_dark";
            
        a_HpGauge.transform.FindChild("Attri").GetComponent<UISprite>().spriteName = Name;
        Script.m_lbTurn.text = a_Monster.nWaitTurn.ToString();

        CObjAnimationEvent Event        = Monster.GetComponent<CObjAnimationEvent>();
        Event.m_Delegate_EndSkill       = Script.Call_EndSkill;
        Event.m_Delegate_Death          = Script.Call_AniDeathEnd;
        Event.m_Delegate_Damage         = Script.Call_AniDamageEnd;
        Event.m_Delegate_NormalAtteck   = Script.Call_NormalAtteck;
        
        Script.m_DamagePos = new GameObject();
        Script.m_DamagePos.name = "DamagePos_"+Script.m_IndexPos;
        Script.m_DamagePos.transform.parent        = Script.m_GameMgr.m_UIRoot.transform;
        Script.m_DamagePos.transform.localPosition = new Vector3(0, 0, 0);
        Script.m_DamagePos.transform.localScale = new Vector3(1, 1, 1);
 //       Script.m_DamagePos.transform = ;
       
        Script.m_Gauge = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/MonsterGauge"));
        Script.m_Gauge.transform.parent        = Script.m_GameMgr.m_UIRoot.transform;           
        Script.m_Gauge.transform.localPosition = new Vector3(0, 0, 0);
//        Script.m_Gauge.name = "MonsterGauge_" + a_IndexPos;

//        Script.m_TargetMark = Script.m_Gauge.transform.FindChild("TargetMark").GetComponent<UISprite>();
//        Script.m_TargetMark.enabled = false;
//        Script.m_HpGauge   = Script.m_Gauge.transform.FindChild( "HpBar" ).GetComponent<UISlider>();
//        Script.m_TurnGauge = Script.m_Gauge.transform.FindChild( "TurnBar" ).GetComponent<UISlider>();
//        Script.m_TurnGauge.value = 0.0f;
         
        return Script;
    }	
	
    void Get2DPoint()
    {            
        if(m_Gauge == null) return;
        Vector3 Temp =  m_GameMgr.m_2DCamara.ViewportToWorldPoint(
                    m_GameMgr.m_3DCamara.WorldToViewportPoint(
                        transform.position));
                
        m_Gauge.transform.position = Temp;
        m_MonCenterPoint.x = m_Gauge.transform.localPosition.x;
        m_MonCenterPoint.y = m_Gauge.transform.localPosition.y;


        Temp =  m_GameMgr.m_2DCamara.ViewportToWorldPoint(
                            m_GameMgr.m_3DCamara.WorldToViewportPoint(
                                m_ObjUIPoint3D.transform.position));

        m_DamagePos.transform.position = Temp;        

    //    m_UIPoint.x = Temp.x;
    //    m_UIPoint.y = Temp.y;

    //    m_Gauge.transform.position = m_UIPoint;     
        
    //    float nHarfWidth = 0.0f;        
    //    nHarfWidth = (float)m_HpGauge.transform.FindChild("Background").GetComponent<UISprite>().width;
        
    //    Temp.x = -(nHarfWidth/2) + 30;
	//	Temp.y = 30;
    //    Temp.z = 0;   

	//	m_Gauge.transform.localPosition = m_Gauge.transform.localPosition + Temp;
        
    }
    

	void Update()
    {
	    Get2DPoint();
	}        

    // 턴을 계산 한다.
    public bool Count_Turn()
    {
        m_WaitTurn--;   
        m_lbTurn.text = m_WaitTurn.ToString();
        return IsMyTurn();
    }

    public bool IsMyTurn()
    {
        if(m_WaitTurn <= 0) return true;        
        return false;
    }

    // 전투준비 입장. 
    public void Action_Entrance() 
    { 
        m_HpGauge.gameObject.SetActive(true);
        m_Ani.Play(m_Info.strEntranceAni); 
        Invoke("Action_Entrance_AniEnd",  m_Ani.GetClip(m_Info.strEntranceAni).length );
    }
    public void Action_Entrance_AniEnd()
    {
        m_Ani.Play("stand");
    }

    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    // 공격한다.         
    private bool m_IsAttack = false;
    public bool GetIsAttack() { return m_IsAttack; }
    public void Attack()
    {
        m_IsAttack = true;        
        m_AtkEff.Play();        
        m_Ani.Play( DataMgr.Inst.m_DB_MonsterSkill[ m_Info.nAttackSkill ].strAni);         
    }

    public void Call_EndSkill( int a_NumSkill )
    {
        if(m_IsAttack)
        {
            m_GameMgr.m_HpMgr.Minus(m_Info.nAttack);
            m_GameMgr.PlayerDamageEff(m_IndexPos==5 ? 2:m_IndexPos );            
            m_WaitTurn = m_Info.nWaitTurn;
            m_lbTurn.text = m_Info.nWaitTurn.ToString();
            m_IsAttack = false;            
            m_Ani.Play("stand");
        }        
    }
    public void Call_NormalAtteck( )
    {        
        if(m_IsAttack)
        {
            m_GameMgr.m_HpMgr.Minus(m_Info.nAttack);
            m_GameMgr.PlayerDamageEff(m_IndexPos==5 ? 2:m_IndexPos);            
            m_WaitTurn      = m_Info.nWaitTurn;
            m_lbTurn.text   = m_Info.nWaitTurn.ToString();
            m_IsAttack      = false;            
            m_Ani.Play("stand");
        }
    }



    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    // 공격을 받는다.
    private bool m_IsDeath = false;    
    public bool GetDeath()      { return m_IsDeath; }    
    private Defines.Delegate_None m_delDamageAlert;
    
    // 공격실행
    public void Action_Damage( Defines.Delegate_None a_delDamageAlert, DataMgr.tagUserSummon a_Info, int a_BoolCount)
    {             
        if(m_IsAttack){ a_delDamageAlert(); return; }

        m_delDamageAlert = a_delDamageAlert;        
        float fCon = DataMgr.Inst.GetAtti_DamageCon( a_Info.Data.eAttr, m_Info.eAttr);
        int nLastDamage = (int)((a_Info.GetAttack()*fCon)*(1f+(0.25f*a_BoolCount)));
        m_Hp -= nLastDamage;
        m_HpGauge.value = (float)m_Hp/(float)m_Info.nHp;            
        
        m_GameMgr.m_DamageManager.DuplicateDmg(m_DamagePos, nLastDamage.ToString());

        if(m_Hp <= 0)        
            ExeDeath();
        else
            ExeDamage();
    }
    
    public void Skill_Damage( Defines.Delegate_None a_delDamageAlert, DataMgr.tagUserSummon a_Info, int a_SkillDamage)
    {                          
        m_delDamageAlert = a_delDamageAlert;        
        float fCon = DataMgr.Inst.GetAtti_DamageCon( a_Info.Data.eAttr, m_Info.eAttr);

        int nLastDamage = (int)((a_Info.GetAttack()*fCon) * (float)(a_SkillDamage/100f));
        m_Hp -= nLastDamage;
        m_HpGauge.value = (float)m_Hp/(float)m_Info.nHp;
        m_GameMgr.m_DamageManager.DuplicateDmg(m_DamagePos, nLastDamage.ToString());        

        if(m_IsAttack)
        {
             if(m_Hp <= 0)
            {
                m_IsDeath = true;
                m_HpGauge.gameObject.SetActive(false);
            }
                                             
            if(m_delDamageAlert != null) m_delDamageAlert();
        }
        else
        {
            if(m_Hp <= 0)        
                ExeDeath();
            else
                ExeDamage();
        }
        
    }        
    
    void ExeDamage()
    {
        m_Ani.Play("damage");
        Invoke("end_Death_Damage", m_Ani.GetClip("damage").length );
    }
    void ExeDeath()
    {        
        m_Ani.Play("death");
        Invoke("end_Death_Damage", m_Ani.GetClip("death").length );
    }

    void end_Death_Damage()
    {
        if(m_Hp <= 0)
        {
            m_IsDeath = true;
            m_HpGauge.gameObject.SetActive(false);
        }
        else m_Ani.Play("stand");
                                     
        if(m_delDamageAlert != null) m_delDamageAlert();
    }

    public void Call_AniDamageEnd( ) {}
    public void Call_AniDeathEnd( ) {}	
}
