using UnityEngine;
using System.Collections;

public class DamageScript : MonoBehaviour 
{

    public UILabel  m_Label_Damage;
    public UILabel  m_Label_Heal;
    public float    m_DestTime;    	

	void Start()
    {
        Destroy( gameObject, m_DestTime );
	}

	//상태에 따라 데미지나 힐로 구분하여 문자를 출력한다.
	void SetDmg(object[] info)
    {

        if( (int)info[1] == 1 )
        {

            m_Label_Damage.text = (string)info[0];
            m_Label_Heal.enabled = false;
        }
        else 
        {
            m_Label_Heal.text = (string)info[0];
            m_Label_Damage.enabled = false;
        }
	}
}
