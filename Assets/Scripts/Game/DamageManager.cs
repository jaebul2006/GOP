using UnityEngine;
using System.Collections;

public class DamageManager : MonoBehaviour 
{	   
	GameObject m_DamagePrefab;
//	GameObject m_LevelUpPrefab;
//	GameObject m_BuffTextPrefab;
	Vector3     modify_pos = new Vector3(0f, 0f, 0f);
	Transform   myTrans;
    
    public Camera           m_UiCamara;
    public Camera           m_BattleCamera;   


	void Awake()
    {
		myTrans = transform;
        m_DamagePrefab = Resources.Load("Prefabs/Damage") as GameObject;        

//		m_LevelUpPrefab = Resources.Load("Prefabs/LevelUp") as GameObject;
//		m_BuffTextPrefab = Resources.Load("Prefabs/BuffText") as GameObject;

//        if (DataMgr.Inst.GetLanguage() == DataMgr.emLanguage.emLanguage_Korea)
//        {
//            GameObject Obj  = (GameObject)Resources.Load("Textures/UI_Buff/Pre_UI_Buff");
//            m_BuffTextPrefab.transform.GetChild(0).GetComponent<UISprite>().atlas = Obj.GetComponent<UIAtlas>();
//        }
//        else
//        {
//            GameObject Obj = (GameObject)Resources.Load("Textures/UI_Buff/Pre_UI_Buff_Eng");
//            m_BuffTextPrefab.transform.GetChild(0).GetComponent<UISprite>().atlas = Obj.GetComponent<UIAtlas>();
//        }
	}

	// 데미지를 출력.
    // a_OutputText : 데미지값.
    // a_nFontCode : 0힐, 1데미지.
    Vector3 Interval2 = new Vector3(0.0f, 0.0f, 0.0f);
    public void DuplicateDmg( GameObject a_ObjTarget, string a_OutputText) 
    {
        GameObject DmgObj           = Instantiate(m_DamagePrefab) as GameObject;
		DmgObj.transform.parent     = a_ObjTarget.transform;
		DmgObj.transform.localScale = Vector3.one *2.0f;        

        //HpBar Background.        
        if( a_ObjTarget.transform.localPosition.y > 500.0f )        
            Interval2.y = 500.0f - a_ObjTarget.transform.localPosition.y;        
        else 
            Interval2.y = 0f;

        DmgObj.transform.localPosition = Interval2;		
        DmgObj.SendMessage ("SetDmg", new object[] {a_OutputText, 1}, SendMessageOptions.DontRequireReceiver);
	}

    Vector3 Interval1 = new Vector3(0.0f, 200.0f, 0.0f);
    public void DuplicateHeal( GameObject a_ObjTarget, string a_OutputText) 
    {
        GameObject DmgObj           = Instantiate(m_DamagePrefab) as GameObject;
		DmgObj.transform.parent     = a_ObjTarget.transform;
		DmgObj.transform.localScale = Vector3.one *2.0f;        

        DmgObj.transform.localPosition = Interval1;		
        DmgObj.SendMessage ("SetDmg", new object[] {a_OutputText, 0}, SendMessageOptions.DontRequireReceiver);
	}

}
