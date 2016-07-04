
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSlot : MonoBehaviour
{
    public  UILabel                     m_Product_name;
    public  UILabel                     m_Product_explain;
    public  UILabel                     m_lbPrice;
    public  UISprite                    m_IconPrice;
    public  UISprite                    m_IconProduct;
    public  GameObject                  m_BuyBtn;
    private DataMgr.tagSaticShop        m_ShopData;
    private GameObject                  m_RootLobby;

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void SetUp(DataMgr.tagSaticShop a_Data, GameObject a_RootLobby)
    {
        if( a_Data.strGetCode.IndexOf('K', 0)  != -1)
        {
            if( DataMgr.Inst.m_UserInfo.CrownSlot >= Defines.DEF_MAX_SUMMON_EX_SLOT)
            {                
                Destroy(gameObject);
                return;
            }
        }

        m_RootLobby = a_RootLobby;
        m_ShopData = a_Data;
        string TempName, TempComment;
        TempName = DataMgr.Inst.GetLocal(a_Data.strName);
        TempComment = DataMgr.Inst.GetLocal(a_Data.strToolTip);

        // 얻는 갯수 계산한다..        
        if (TempName.IndexOf("{0}") != -1)
        {
            string GetCount = a_Data.strGetCode;
            GetCount = GetCount.Remove(GetCount.Length - 1);
            m_Product_name.text = string.Format(TempName, GetCount);
        }            
        else
            m_Product_name.text = TempName;

        if (TempComment.IndexOf("{0}") != -1)
        {
            string GetCount = a_Data.strGetCode;
            GetCount = GetCount.Remove(GetCount.Length - 1);
            m_Product_explain.text = string.Format(TempComment, GetCount);
        }            
        else
            m_Product_explain.text = TempComment;

        m_lbPrice.text = a_Data.strPayCount;
        
        if (a_Data.strPayCode == "Cash")
        {                        
            m_lbPrice.text = System.Math.Round(System.Math.Abs(float.Parse(a_Data.strPayCount) - 0.01f), 2).ToString();
            m_IconPrice.spriteName = "UI_top_dollar";
        }
        else if (a_Data.strPayCode == "Ruby")
            m_IconPrice.spriteName = "UI_top_gem";        
        else if (a_Data.strPayCode == "FriendPoint")
            m_IconPrice.spriteName = "UI_top_Apple";
        else
            m_IconPrice.spriteName = "UI_top_coin";

        m_IconProduct.spriteName = "IconShop_" + a_Data.strImgID;
    }
    
    public void BtnClick()
    {
        
        string name="";
        if(m_ShopData.strName.IndexOf("{0}") != -1)
            name = string.Format(m_ShopData.strName, m_ShopData.strGetCode.Remove( m_ShopData.strGetCode.Length-1, 1) );

        CMessageBox.Create( string.Format("Do you want to buy {0}?", name), 2, onQuestionBuy);
    }

    public void onQuestionBuy(bool a_Is)
    { 
        if(a_Is)
        {
            BtnClick2();
        }
    }

    public void BtnClick2()
    {   
        // 금액체크.
        if(m_ShopData.strPayCode == "Ruby")
        {
            if( int.Parse(m_ShopData.strPayCount) > DataMgr.Inst.m_UserInfo.Ruby)
            {                
                CMessageBox.Create( DataMgr.Inst.GetLocal("You have not enough ruby."), 1, null);
                return;
            }
        }
        else if(m_ShopData.strPayCode == "Gold")
        {
            if( int.Parse(m_ShopData.strPayCount) > DataMgr.Inst.m_UserInfo.Gold)
            {                
                CMessageBox.Create( DataMgr.Inst.GetLocal("You have not enough gold."), 1, null);
                return;
            }
        }
        else if(m_ShopData.strPayCode == "FriendPoint")
        {
            if( int.Parse(m_ShopData.strPayCount) > DataMgr.Inst.m_UserInfo.FriendPoint)
            {                
                CMessageBox.Create( DataMgr.Inst.GetLocal("Golden apple is not enough."), 1, null);
                return;
            }
        }
        
        // 가격 체크.
        if(m_ShopData.strGetCode.IndexOf('S',0) != -1)
        { 
            if (DataMgr.Inst.m_UserInfo.SummonSlot <= DataMgr.Inst.m_UserSummonList.Count)
            {
                CMessageBox.Create( DataMgr.Inst.GetLocal("Hero slot is not enough."), 1, null);
                return;
            }
        }

        if( m_ShopData.strGetCode.IndexOf('K', 0)  != -1)
        {
            if( DataMgr.Inst.m_UserInfo.CrownSlot >= Defines.DEF_MAX_SUMMON_EX_SLOT)
            {
                CMessageBox.Create( DataMgr.Inst.GetLocal("Reach a maximun laurel !"), 1, null);
                return;
            }
        }

        if (m_ShopData.strPayCode == "Cash")
        {             
            DataMgr.Inst.GetPayRaiz( m_ShopData.strPayCount, PayRaiz_Result);            
        }
        else
        {
            m_BuyBtn.GetComponent<UIButton>().enabled = false;
            m_BuyBtn.GetComponent<BoxCollider>().enabled = false;        
            DataMgr.Inst.m_SerMgr.ShopBuy(m_ShopData.nIDX, null, Buy_Result);
        }
    }

    void PayRaiz_Result(bool a_Result, string a_strPaymentCode, string a_PayCount )
    {
        if(a_Result)
        {            
            m_BuyBtn.GetComponent<UIButton>().enabled = false;
            m_BuyBtn.GetComponent<BoxCollider>().enabled = false;
            DataMgr.Inst.m_SerMgr.ShopBuy(m_ShopData.nIDX, a_strPaymentCode, Buy_Result);
        }
    }

    public void Buy_Result(bool a_Result, Dictionary<string, object> a_dicJson, string a_Str, serverManager.ErrorCode a_ErrorCode)
    {
        Debug.Log("구입 메시지");
        if(a_Result)
        {
            if(a_dicJson["PayCode"].ToString() != "H")
                DataMgr.Inst.DataPasing( a_dicJson["PayCode"].ToString(), a_dicJson["PayValue"].ToString());

            DataMgr.Inst.DataPasing( a_dicJson["OutCode"].ToString(), a_dicJson["OutValue"].ToString());

            if(m_ShopData.eCategory == DataMgr.emSHOP_TAB.em_CARD)
            {                
                // 팝업창.
                GameObject ObjRtn               = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/Panel_Char_Popup_Requital"));
                ObjRtn.transform.parent         = m_RootLobby.transform;
                ObjRtn.transform.localScale     = new Vector3(1.0f, 1.0f, 1.0f);
                ObjRtn.transform.localPosition  = new Vector3(0.0f, 0.0f, 0.0f);
                ObjRtn.GetComponent<Panel_Char_Popup_Requital>().Begin(DataMgr.Inst.m_UserSummonList[int.Parse(a_dicJson["S"].ToString())], onBuy_PopupEnd);
            }
            else
            {
                CMessageBox.Create( DataMgr.Inst.GetLocal("Purchase succeeded."), 1, onBuy_Eror);
            }
        }
        else
        {
            if(a_ErrorCode == serverManager.ErrorCode.EC_DeficitMoney)
            {
                CMessageBox.Create( DataMgr.Inst.GetLocal("You have not enough Moneys."), 1, onBuy_Eror);
            }
            else 
            {
                CMessageBox.Create( DataMgr.Inst.GetLocal("Failed to buy! Call to Manager. (ErrorCode:"+(int)a_ErrorCode+")"), 1, onBuy_Eror);
            }
        }
    }

    public void onBuy_PopupEnd()
    {
        m_BuyBtn.GetComponent<UIButton>().enabled = true;
        m_BuyBtn.GetComponent<BoxCollider>().enabled = true;
    }

    public void onBuy_Eror(bool b)
    {
        if( m_ShopData.strGetCode.IndexOf('K', 0)  != -1)
        {
            if( DataMgr.Inst.m_UserInfo.CrownSlot >= Defines.DEF_MAX_SUMMON_EX_SLOT)
            {                
                Destroy(gameObject);
                return;
            }
        }
        
        m_BuyBtn.GetComponent<UIButton>().enabled = true;
        m_BuyBtn.GetComponent<BoxCollider>().enabled = true;
    }

}
