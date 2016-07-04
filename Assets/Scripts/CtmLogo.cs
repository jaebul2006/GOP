using UnityEngine;
using System.Collections;

public class CtmLogo : MonoBehaviour 
{           
    public  UITexture[]     m_TitleLogo;    
    private int             m_Index;
    private float           m_Time;
    private float           m_nLimitTime;
    private bool            m_Is;

    void Awake()
    {        
       if(PlayerPrefs.GetInt("Gra",1) == 1)       		
           Screen.SetResolution(Defines.DEF_DEGINE_SCREEN_WIDHT, Defines.DEF_DEGINE_SCREEN_HEIGHT, true);		
       else 
           Screen.SetResolution(Defines.DEF_DEGINE_SCREEN_WIDHT/2, Defines.DEF_DEGINE_SCREEN_HEIGHT/2, true);		
    }

    // Use this for initialization
	void Start()
    {
        for(int i=0; i<m_TitleLogo.Length; i++)
            m_TitleLogo[i].enabled = false;
        
        m_Is            = false;
        m_Index         = 0;
        m_Time          = 3f;
    	m_nLimitTime    = 0;
    }
    	
    // Update is called once per frame
	void Update ()
    {
    	if( Time.time > m_nLimitTime)
        {
            if(m_TitleLogo.Length <= m_Index && !m_Is)
            {      
                m_Is = true;
                Application.LoadLevel("LoginScene");
                return;
            }
            m_nLimitTime = Time.time + m_Time;
            m_TitleLogo[m_Index].enabled = true;
            m_Index++;
        }
    }
}
