using UnityEngine;
using System.Collections;

public class Hpbar : MonoBehaviour
{
    public UILabel m_Text;
    public UIProgressBar m_Bar;
    private int m_HpMax;
    private int m_Hp;

    public int GetHp() { return m_Hp; }    

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	  if(Input.GetKeyDown(KeyCode.Keypad5)) m_Hp = 0; 
	}

    public void SetUp(int a_nMaxHp)
    {
        m_HpMax = a_nMaxHp;
        m_Hp = a_nMaxHp;
        m_Bar.value = 1.0f;
        m_Text.text = m_Hp.ToString() +" / "+ m_HpMax.ToString();
    }

    public int Minus( int a_Value )
    {
        m_Hp -= a_Value;
        m_Text.text = m_Hp.ToString() +" / "+ m_HpMax.ToString();
        m_Bar.value = (float)m_Hp / (float)m_HpMax;
        return m_Hp;
    }

    public int Plus(int a_Value)
    {
        m_Hp += a_Value;
        if(m_Hp > m_HpMax) m_Hp = m_HpMax;
        m_Text.text = m_Hp.ToString() + " / " + m_HpMax.ToString();
        m_Bar.value = (float)m_Hp / (float)m_HpMax;
        return m_Hp;
    }

    public bool CheckHP_Warning() 
    { 
        int nHp20 = (int)((float)m_HpMax*0.2f);

        if( m_Hp < nHp20 )
        {
            return true;
        }
        
        return false;        
    }

}
