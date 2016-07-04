using UnityEngine;
using System.Collections;

public class MyBall : MonoBehaviour 
{    
    public UITexture            m_Summon;   
    public UITexture            m_Summon_1;	    
    
    private int                 m_nSummonIdx;
    public void SetUp(int a_nSummonIdx, ref int a_nCount)
    {        
        DataMgr.emCardAttribute Attri = DataMgr.emCardAttribute.emCAB_END;                
        m_nSummonIdx = a_nSummonIdx; 

        m_Summon_1.enabled = false;
        if (a_nSummonIdx != -1)
        {
            DataMgr.tagUserSummon Info = DataMgr.Inst.m_UserSummonList[a_nSummonIdx];
            Attri = Info.Data.eAttr;
            m_Summon.mainTexture = Resources.Load("Textures/Char/icon_mon_" + Info.Data.strResurceID ) as Texture;
            m_Summon_1.mainTexture = Resources.Load("Textures/Char/icon_mon_" + Info.Data.strResurceID+"_2") as Texture;
            m_Summon.enabled = true;            
        }
        else
        {            
            Attri = DataMgr.Inst.m_liNotUseAttriOrder[a_nCount].Attri;
            a_nCount++;
            m_Summon.enabled = false;
        }

        switch(Attri)
        {
            case DataMgr.emCardAttribute.emCAB_WA:
                transform.GetComponent<UITexture>().mainTexture = Resources.Load("Textures/Ball_Red") as Texture;
            break;

            case DataMgr.emCardAttribute.emCAB_SU:
                transform.GetComponent<UITexture>().mainTexture = Resources.Load("Textures/Ball_Blue") as Texture;
            break;

            case DataMgr.emCardAttribute.emCAB_YUNG:
                transform.GetComponent<UITexture>().mainTexture = Resources.Load("Textures/Ball_Gold") as Texture;
            break;

            case DataMgr.emCardAttribute.emCAB_ARM:
                transform.GetComponent<UITexture>().mainTexture = Resources.Load("Textures/Ball_Dark") as Texture;
            break;
        }
    }

	// Update is called once per frame
	void Update () 
    {

	}

    
    private bool m_IsFaceOff = false;
    public void onClick()
    {         
        if(m_IsFaceOff || m_nSummonIdx == -1) return;
        m_Summon.enabled = false; 
        m_Summon_1.enabled = true;
        m_IsFaceOff = true;
        Invoke("onFaceOffEnd", 1f);
    }

    void onFaceOffEnd()
    {
        m_Summon.enabled = true;
        m_Summon_1.enabled = false;
        m_IsFaceOff = false;
    }

}
