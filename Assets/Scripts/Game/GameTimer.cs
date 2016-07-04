using UnityEngine;
using System.Collections;

public class GameTimer : MonoBehaviour
{
    public UISprite m_Mark;
    private string[] m_SecondName;
    private string m_WaitName;
    // Use this for initialization
    void Start ()
    {
        m_SecondName = new string[6];        
        m_SecondName[0] = "icon_timer_5";
        m_SecondName[1] = "icon_timer_4";
        m_SecondName[2] = "icon_timer_3";
        m_SecondName[3] = "icon_timer_2";
        m_SecondName[4] = "icon_timer_1";
        m_SecondName[5] = "icon_timer_0";
        m_WaitName = "icon_timer";
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    private GameObject m_SendObj;
    private string  m_SendMsgName;
    private bool m_bTimeEnd;
    private bool m_IsTimeOn = false;

    public bool GetIsTimeOn() { return m_IsTimeOn; }
    public void Begin(GameObject a_SendObj, string a_SendMsgName )
    {
        m_SendObj = a_SendObj;
        m_SendMsgName = a_SendMsgName;
        StartCoroutine(Timer());
        m_bTimeEnd = false;
        m_IsTimeOn = true;
    }     

    IEnumerator Timer(  )
    {
        for(int i=0; i< m_SecondName.Length; i++)
        {
            if (m_bTimeEnd) { m_IsTimeOn = false; m_bTimeEnd = false; yield return null;}
            m_Mark.spriteName = m_SecondName[i];
            yield return new WaitForSeconds(1.0f);
        }

        if (m_bTimeEnd) { m_IsTimeOn = false; m_bTimeEnd = false; yield return null;}
        
        m_SendObj.SendMessage(m_SendMsgName, SendMessageOptions.DontRequireReceiver);        
        m_IsTimeOn = false;
    }

    public void EndTime()
    {
        m_Mark.spriteName = m_WaitName;
        m_bTimeEnd = true;
    }
}
