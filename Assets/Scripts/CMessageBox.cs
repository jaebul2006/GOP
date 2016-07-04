using UnityEngine;
using System.Collections;

public class CMessageBox : MonoBehaviour 
{   
    public enum emMSG_MARK{ Warning ,Error };
    public GameObject   m_OneOk;
    public GameObject   m_TwoOk;
    public GameObject   m_TwoCancel;
    public UILabel      m_lbMessage;    
    public UISprite     m_Mark;

    [System.NonSerialized]
    public int     m_Style; // 1:원버튼, 2:투버튼.

    
    public delegate void CollBackFunc( bool a_Result );

    [System.NonSerialized]
    public CollBackFunc m_CollBackFunc;

    public void onClick_Ok()
    {
        if(m_CollBackFunc != null)
            m_CollBackFunc(true);

        Destroy(gameObject);
    }

    public void onClick_Cancel()
    {
        if(m_CollBackFunc != null)
            m_CollBackFunc(false);

        Destroy(gameObject);
    }

    static public GameObject Create(string a_strMessage, 
        int a_Style , CollBackFunc a_CollBackFunc, emMSG_MARK a_MSG_MARK=emMSG_MARK.Warning)
    {
        GameObject ObjRtn = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/MessageBox"));
        ObjRtn.transform.parent = DataMgr.Inst.m_TopObj.transform;
        ObjRtn.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        ObjRtn.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        CMessageBox Msg         = ObjRtn.GetComponent<CMessageBox>();
        Msg.m_lbMessage.text    = a_strMessage;        
        Msg.m_Style = a_Style;
        Msg.m_CollBackFunc = a_CollBackFunc;

        if(a_Style == 1)
        {
            Msg.m_OneOk.SetActive(true);
            Msg.m_TwoOk.SetActive(false);
            Msg.m_TwoCancel.SetActive(false);
        }
        else if(a_Style == 2)
        {
            Msg.m_OneOk.SetActive(false);
            Msg.m_TwoOk.SetActive(true);
            Msg.m_TwoCancel.SetActive(true);
        }

        if(a_MSG_MARK==emMSG_MARK.Warning)
        {
            Msg.m_Mark.spriteName = "MsgMark_warring";
        }
        else 
        {
            Msg.m_Mark.spriteName = "MsgMark_error";
        }        
        return ObjRtn;
    }
}
