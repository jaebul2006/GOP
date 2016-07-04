using UnityEngine;
using System.Collections;

public class Panel_Sell_popup : MonoBehaviour
{
    public UILabel      m_lbName;
    public UILabel      m_lbPlace;
    public UISprite     m_sprCardBack;
    public GameObject   m_Grade;
    public UITexture    m_texChar;
    private DataMgr.tagUserSummon m_CardData;

    // Use this for initialization
    void Start ()
    {        
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}


    public void Begin( DataMgr.tagUserSummon a_CardData )
    {
        m_CardData = a_CardData;
        m_lbName.text = a_CardData.Data.strName;

        int nAttr = 0;
             if (a_CardData.Data.eAttr == DataMgr.emCardAttribute.emCAB_WA)     nAttr = 0;
        else if (a_CardData.Data.eAttr == DataMgr.emCardAttribute.emCAB_SU)     nAttr = 1;
        else if (a_CardData.Data.eAttr == DataMgr.emCardAttribute.emCAB_YUNG)   nAttr = 2;
        else if (a_CardData.Data.eAttr == DataMgr.emCardAttribute.emCAB_ARM)    nAttr = 3;
        m_sprCardBack.spriteName = "CardBack_" + nAttr;
        
        m_texChar.mainTexture = Resources.Load("Textures/char/icon_mon_" + a_CardData.Data.strResurceID) as Texture;

        for(int i=1; i<=Defines.DEF_MAX_GRADE; i++)
        {
            if(a_CardData.Data.nGrade == i)
                m_Grade.transform.FindChild("Star_" + a_CardData.Data.nGrade).gameObject.SetActive(true);
            else 
                m_Grade.transform.FindChild("Star_" + i).gameObject.SetActive(false);
        }
        

        m_lbPlace.text = DataMgr.Inst.m_DB_SummonPriceSeed[a_CardData.Data.nGrade].GetSell(a_CardData).ToString() + "G";
    }

    public void CollBack_OK()
    {
        DataMgr.Inst.GetPage(DataMgr.emMAIN_MODE.MYTEAM).SendMessage("sellCard", m_CardData, SendMessageOptions.DontRequireReceiver);
        gameObject.SetActive(false);
    }

    public void CollBack_Cancel()
    {
        gameObject.SetActive(false);
    }
}
