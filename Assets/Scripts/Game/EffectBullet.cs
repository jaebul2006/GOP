using UnityEngine;
using System.Collections;

public class EffectBullet : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public delegate void Dal_MonDamage(int a_TeamSum, int a_MonIdx, int a_BoolCount);

    private GameObject  m_UIRoot;
    private string  m_strBoom      = "Hit_fire_ATK";
    private int     m_MonIdx;
    private int     m_TeamSum;
    private Dal_MonDamage m_Del;
    private int     m_BoolCount;

    public static void Fire( GameObject a_UIRoot, Vector2 a_StartPos, Vector2 a_EndPos, 
        DataMgr.emCardAttribute a_SumeAttr, int a_BoolCount, int a_TeamSum, int a_MonIdx, Dal_MonDamage a_Del )
    {   
        string strBallet    = "Ball_A(RED)";
        string strBoom      = "Hit_fire_ATK";
        // 탄구분.
        switch(a_SumeAttr)
        {            
            case DataMgr.emCardAttribute.emCAB_WA: 
                strBallet = "Ball_A (RED)"; 
                if(a_BoolCount == 3 || a_BoolCount == 4) strBoom = "Hit_fire_ATK";
                else if(a_BoolCount == 5 || a_BoolCount == 6) strBoom = "Hit_fire_ATK1";
                else if(a_BoolCount > 6) strBoom = "Hit_fire_ATK2";                
            break;
		    
            case DataMgr.emCardAttribute.emCAB_SU: 
                strBallet = "Ball_A (Blue)"; 
                if(a_BoolCount == 3 || a_BoolCount == 4) strBoom = "Hit_Ice_ATK";
                else if(a_BoolCount == 5 || a_BoolCount == 6) strBoom = "Hit_Ice_ATK1";
                else if(a_BoolCount > 6) strBoom = "Hit_Ice_ATK2";
            break;
		    
            case DataMgr.emCardAttribute.emCAB_YUNG: 
                strBallet = "Ball_A (Gold)";
                if(a_BoolCount == 3 || a_BoolCount == 4) strBoom = "Hit_Light_ATK";
                else if(a_BoolCount == 5 || a_BoolCount == 6) strBoom = "Hit_Light_ATK1";
                else if(a_BoolCount > 6) strBoom = "Hit_Light_ATK2";
            break;
		    
            case DataMgr.emCardAttribute.emCAB_ARM: 
                strBallet = "Ball_A (Dark)"; 
                if(a_BoolCount == 3 || a_BoolCount == 4) strBoom = "Hit_Dark_ATK";
                else if(a_BoolCount == 5 || a_BoolCount == 6) strBoom = "Hit_Dark_ATK1";
                else if(a_BoolCount > 6) strBoom = "Hit_Dark_ATK2";
            break;
        }

        // 발사.
        GameObject obj = (GameObject)Instantiate( (GameObject)Resources.Load("Effects/"+strBallet) );
        obj.transform.parent = a_UIRoot.transform;
        obj.gameObject.name = "Bullet";  
        obj.transform.localPosition = new Vector3(a_StartPos.x, a_StartPos.y, -0.05f);
        EffectBullet EB = obj.AddComponent<EffectBullet>();
        EB.m_UIRoot     = a_UIRoot;
        EB.m_MonIdx     = a_MonIdx;
        EB.m_Del        = a_Del;
        EB.m_strBoom    = strBoom;
        EB.m_TeamSum    = a_TeamSum;
        EB.m_BoolCount  = a_BoolCount; 

 
        TweenPosition TP      = obj.AddComponent<TweenPosition>();
        TP.from               = a_StartPos;
        TP.to                 = a_EndPos;
        TP.style              = UITweener.Style.Once;
        TP.duration           = 0.7f;
        TP.method             = UITweener.Method.Linear;
        TP.callWhenFinished   = "PlayerAttackEff_Boom";
        TP.eventReceiver      = obj;        
    }

    void PlayerAttackEff_Boom()
    {
        Vector2 vPos = transform.localPosition;
        // 발사.
        GameObject obj = (GameObject)Instantiate( (GameObject)Resources.Load("Effects/"+m_strBoom ) );
        obj.transform.parent = m_UIRoot.transform;
        obj.gameObject.name = "Boom_";
        obj.transform.localPosition = new Vector3(vPos.x, vPos.y, -0.05f);                 
        m_Del(m_TeamSum, m_MonIdx, m_BoolCount);   
        Destroy(gameObject);
    }


    private int     m_Skill_Damage;
    public static void Skill( GameObject a_UIRoot, Vector2 a_StartPos, Vector2 a_EndPos, 
        DataMgr.emCardAttribute a_SumeAttr, int a_Damage, int a_TeamSum, int a_MonIdx, Dal_MonDamage a_Del, bool a_bAtkAll )
    {   
        string strBallet    = "Ball_A(RED)";
        string strBoom      = "Hit_fire_ATK";
        // 탄구분.
        switch(a_SumeAttr)
        {            
            case DataMgr.emCardAttribute.emCAB_WA: 
                strBallet   = "Ball_A (RED)";                
                if(!a_bAtkAll) strBoom = "Fire_Skill_1";
                else strBoom = "Fire_Skill_2";
            break;
		    
            case DataMgr.emCardAttribute.emCAB_SU: 
                strBallet   = "Ball_A (Blue)";             
                if(!a_bAtkAll) strBoom = "ICE_Skill_1";
                else strBoom = "ICE_Skill_2";
            break;
		    
            case DataMgr.emCardAttribute.emCAB_YUNG: 
                strBallet   = "Ball_A (Gold)";
                if(!a_bAtkAll) strBoom = "Light_Skill_1";
                else strBoom = "Light_Skill_2";
            break;
		    
            case DataMgr.emCardAttribute.emCAB_ARM: 
                strBallet   = "Ball_A (Dark)";
                if(!a_bAtkAll) strBoom = "Dark_Skill_1";
                else strBoom = "Dark_Skill_2";                
            break;
        }

        // 발사.
        GameObject obj = (GameObject)Instantiate( (GameObject)Resources.Load("Effects/"+strBallet) );
        obj.transform.parent = a_UIRoot.transform;
        obj.gameObject.name = "Bullet";  
        obj.transform.localPosition = new Vector3(a_StartPos.x, a_StartPos.y, -0.05f);
        EffectBullet EB = obj.AddComponent<EffectBullet>();
        EB.m_UIRoot     = a_UIRoot;
        EB.m_MonIdx     = a_MonIdx;
        EB.m_Del        = a_Del;
        EB.m_strBoom    = strBoom;
        EB.m_TeamSum    = a_TeamSum;
        EB.m_BoolCount  = a_Damage; 

 
        TweenPosition TP      = obj.AddComponent<TweenPosition>();
        TP.from               = a_StartPos;
        TP.to                 = a_EndPos;
        TP.style              = UITweener.Style.Once;
        TP.duration           = 0.8f;
        TP.method             = UITweener.Method.Linear;
        TP.callWhenFinished   = "PlayerAttackEff_Boom";
        TP.eventReceiver      = obj;
    }
}
