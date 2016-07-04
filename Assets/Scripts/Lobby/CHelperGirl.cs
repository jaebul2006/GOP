using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CHelperGirl : MonoBehaviour 
{
    public UITexture    m_TexEye;
    
    public UISprite     m_SprHelp;
    public UILabel      m_LbHelp;        

    private int m_eyeTimeLimit;
    private List<string> m_HelpText;

	// Use this for initialization
	void Start ()
    {
        m_eyeTimeLimit = System.Environment.TickCount;
        m_TextTiemLimit = System.Environment.TickCount;
        m_HelpText = new List<string>();

        m_HelpText.Add( DataMgr.Inst.GetLocal( "Hi I am Hekate" ));
        m_HelpText.Add( DataMgr.Inst.GetLocal( "You can form a team to depart for advanture by Summon menu." ));
        m_HelpText.Add( DataMgr.Inst.GetLocal( "Please check Summon menue to see Summon ability" ));
        m_HelpText.Add( DataMgr.Inst.GetLocal( "You can check collected Summons by Summon book." ));
        m_HelpText.Add( DataMgr.Inst.GetLocal( "Its good to prepare adventure by using store." ));
        m_HelpText.Add( DataMgr.Inst.GetLocal( "If the Summons are weak, your adventure can be difficult." ));
        m_HelpText.Add( DataMgr.Inst.GetLocal( "If advanture is blocked, Enchant or composite" ));
        m_HelpText.Add( DataMgr.Inst.GetLocal( "By settings menu, configure an environment suitable." ));
        m_HelpText.Add( DataMgr.Inst.GetLocal( "Player skill applied in all battle." ));
        m_HelpText.Add( DataMgr.Inst.GetLocal( "You can get skill in every 5th level." ));
        m_HelpText.Add( DataMgr.Inst.GetLocal( "Shows current stage." ));
        m_HelpText.Add( DataMgr.Inst.GetLocal( "Damage the monsters when 3 or more are attached." ));
        m_HelpText.Add( DataMgr.Inst.GetLocal( "Timer will operate when 2 or more is attached." ));
        m_HelpText.Add( DataMgr.Inst.GetLocal( "When timer stops, attack turn will decrease." ));
        m_HelpText.Add( DataMgr.Inst.GetLocal( "When monster turn is 0, it will attack player once." ));
        m_HelpText.Add( DataMgr.Inst.GetLocal( "Shows gold during play." ));
        m_HelpText.Add( DataMgr.Inst.GetLocal( "Character profile / can use skill when gage is MAX." ));
        m_HelpText.Add( DataMgr.Inst.GetLocal( "When attacking monster with different attribution, damage will increase or decrease." ));

	    m_SprHelp.enabled = false;
        m_LbHelp.enabled = false;
	}
	
	// Update is called once per frame
    // void LateUpdate() 
	void LateUpdate()
    {
	    // 
        if(System.Environment.TickCount > m_eyeTimeLimit)
        {
            StartCoroutine(Begin_EyeAni());
            m_eyeTimeLimit = (UnityEngine.Random.Range(2,6) * 1000) + System.Environment.TickCount;              
        }


        if(m_bIsText)
        {
            if(System.Environment.TickCount > m_TextTiemLimit)
            { 
                m_SprHelp.enabled = false;
                m_LbHelp.enabled = false;
                m_bIsText = false;        
            }
        }
        
	}

    IEnumerator Begin_EyeAni()
    {
        m_TexEye.uvRect = new Rect(0.0f, 0.0f, 1.0f, 0.3333f);
        m_TexEye.Update();
        yield return new WaitForSeconds( 0.1f );

        m_TexEye.uvRect = new Rect(0.0f, 0.3333f, 1.0f, 0.3333f);
        m_TexEye.Update();
        yield return new WaitForSeconds( 0.1f );

        m_TexEye.uvRect = new Rect(0.0f, 0.6666f, 1.0f, 0.3333f);
        m_TexEye.Update();
        yield return new WaitForSeconds( 0.1f );
    }

         
    private bool m_bIsText = false;
    private int  m_TextIndex;
    private int  m_TextTiemLimit = 0;
    void OnSay()
    {
        m_bIsText = true;
        m_SprHelp.enabled = true;
        m_LbHelp.enabled = true;
   
        m_LbHelp.text = m_HelpText[m_TextIndex];
        m_TextIndex++; m_TextIndex%=m_HelpText.Count;
        
        m_TextTiemLimit = System.Environment.TickCount +2000;                         
    }


    

}
