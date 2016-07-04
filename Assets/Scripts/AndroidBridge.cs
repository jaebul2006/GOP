using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID 
    using TapjoyUnity;
#endif
using MiniJSON;

public class AndroidBridge : MonoBehaviour
{   
    public Transform m_Lobby;        
    private Dictionary<string, string>  m_ItemCode;
    private Dictionary<string, string>  m_GoogleSKU;
    private string      m_cpSEQ = "1225"; 
    private string      m_appSEQ = "127";
    private Defines.Delegate_BoolStrStr     m_Delegate;

#if UNITY_ANDROID && !UNITY_EDITOR
    AndroidJavaObject AndroidPlugin= null;
#endif

	string strLabelPayment = "";
	
    struct Box
    {
        public string strValue;
        public float fValue;
    }

	// Use this for initialization
	void Start () 
    {        
        DontDestroyOnLoad(gameObject);
        List<Box> PayList = new List<Box>();

        foreach( KeyValuePair<int, DataMgr.tagSaticShop> Obj in DataMgr.Inst.m_DB_Shop )
        {
            if(Obj.Value.strPayCode == "Cash")
            {
                Box BOX;
                BOX.strValue = Obj.Value.strPayCount;
                BOX.fValue = float.Parse(Obj.Value.strPayCount);
                PayList.Add(BOX);
            }            
        }

        PayList.Sort(delegate (Box x, Box y)
        {            
            return x.fValue.CompareTo(y.fValue);                        
        });        

        m_ItemCode = new Dictionary<string, string>();
        m_ItemCode.Add(PayList[5].strValue, "9064374EF3DC1E665AF2942A23F4860341");
        m_ItemCode.Add(PayList[4].strValue, "9086B7FBCF63B1F0558A367348E2B32583");
        m_ItemCode.Add(PayList[3].strValue, "906B60592B802066F05E9108FDAED3E598");
        m_ItemCode.Add(PayList[2].strValue, "9081E06049129D978CF6795A036D810A46");
        m_ItemCode.Add(PayList[1].strValue, "90622B879D8D656BAAE10B621E73F58993");
        m_ItemCode.Add(PayList[0].strValue, "901A057738C9518D5E78D7F74AD6D01DA2");
        
        m_GoogleSKU = new Dictionary<string, string>();
        m_GoogleSKU.Add(PayList[5].strValue, "gop1100ruby");
        m_GoogleSKU.Add(PayList[4].strValue, "gop520ruby");
        m_GoogleSKU.Add(PayList[3].strValue, "gop300ruby");
        m_GoogleSKU.Add(PayList[2].strValue, "gop90ruby");
        m_GoogleSKU.Add(PayList[1].strValue, "gop45ruby");
        m_GoogleSKU.Add(PayList[0].strValue, "27_ruby");                
        
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJNI.AttachCurrentThread();
		Initiate();
#endif
	}

	// Destroy function
	void Destroy() 
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		if(AndroidPlugin!= null)
			AndroidPlugin.Dispose();
#endif
	}
	
	// Update is called once per frame
	void Update () {}

    	// 초기화 함수
	int Initiate() 
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		// Unity 3D Player 클래스를 가져온다.
		AndroidJavaClass androidClass = new AndroidJavaClass("kr.hanyou.google.godofpuzzle.MainUnityActivity");

		print(androidClass);
		
		if(androidClass != null) 
        {
			AndroidPlugin = androidClass.GetStatic<AndroidJavaObject>("instance");
			androidClass.Dispose();
			
			if(AndroidPlugin== null) 
                return -1;
			return 0;
		}
#endif	
		return -1;
	}

	//02-25 17:25:58.434: I/Unity(30632): Han : {"PARAM_PAYRAIZ_UTIL_TYPE":1,"PARAM_PAYRAIZ_UTIL_RESULT_GOOGLE_ITEM_INFO":"[{\"productId\":\"27_ruby\",\"type\":\"inapp\",\"price\":\"₩3,300\",\"price_amount_micros\":3300000000,\"price_currency_code\":\"KRW\",\"title\":\"27 ruby (God of Puzzle)\",\"description\":\"27 ruby\"},{\"productId\":\"gop45ruby\",\"type\":\"inapp\",\"price\":\"₩5,500\",\"price_amount_micros\":5500000000,\"price_currency_code\":\"KRW\",\"title\":\"45 Ruby (God of Puzzle)\",\"description\":\"45 ruby\"},{\"productId\":\"gop90ruby\",\"type\":\"inapp\",\"price\":\"₩11,000\",\"price_amount_micros\":11000000000,\"price_currency_code\":\"KRW\",\"title\":\"90 Ruby (God of Puzzle)\",\"description\":\"90 Ruby\"},{\"productId\":\"gop300ruby\",\"type\":\"inapp\",\"price\":\"₩33,000\",\"price_amount_micros\":33000000000,\"price_currency_code\":\"KRW\",\"title\":\"300 Ruby (God of Puzzle)\",\"description\":\"300 Ruby\"},{\"productId\":\"gop520ruby\",\"type\":\"inapp\",\"price\":\"₩55,000\",\"price_amount_micros\":55000000000,\"price.
	// 메시지를 받은 후 결제 유효성을 확인 후 아이템을 지급한다.
    // 결과는 Json.
	public void onPurchaseResult(string strParam) 
    {        
        Dictionary<string, object> dicJson = Json.Deserialize(strParam) as Dictionary<string, object>;                
                        
        Debug.Log( "Han : "+ strParam);
        
        if( dicJson.ContainsKey("PARAM_PAYRAIZ_UTIL_TYPE") )
        {
            // PARAM_PAYRAIZ_UTIL_TYPE : 페이레이즈 유틸 타입. 
            // PARAM_RAYRAIZ_UTIL_RESULT_GOOGLE_ITEM_INFO.
            //List<string> list = dicJson["PARAM_RAYRAIZ_UTIL_RESULT_GOOGLE_ITEM_INFO"] as List<string>;
            
            // productId : 제품 ID.
            // type : 상품타입.
            // price : 상품가격.
            // price_amount_micros : 마이크로 단위의 가격.
            // price_currency_code : 가격 통화 코드.
            // title : 상품명.
            // description : 상품 설명.            
        }
        else
        {
            //RESULT_CODE : 결제 결과 100 성공.
            //ORDER_NO : 결제 번호.
            //APP_PARAM : 앱에서 보낸 파라미터.		
            //{"RESULT_CODE":"100","ORDER_NO":"GPA.1324-5526-2729-79888","APP_PARAM":""}

            m_Delegate( dicJson["RESULT_CODE"].ToString() == "100" ? true: false, 
            dicJson["ORDER_NO"].ToString(), 
            dicJson["APP_PARAM"].ToString() );
        }
	}

    public void Payment_PayRaiz(string a_PayCount, Defines.Delegate_BoolStrStr a_Func)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // 안드로이드 라이브러리의 결제 요청 메소드를 호출한다.
		// PayBasic : PayRaiz 결제호출 메서드.
		// 90BDDFE973735AF594C5DB76B924DFDDC4 : PARAM VALUE.
		// PayBasic : APP PARAM.
		// 2 : CP SEQ.
		// 35 : APP SEQ.
		// KR : PARAM COUNTRY.
		
        if(AndroidPlugin == null) return;        
        
	    m_Delegate = a_Func;
        AndroidPlugin.Call("PayBasic",
            m_ItemCode[a_PayCount], 
            "(ID:"+DataMgr.Inst.m_UserInfo.ID + ")(UID:" + SystemInfo.deviceUniqueIdentifier+")",
            m_cpSEQ,
            m_appSEQ,
            "");

        //파라미터 설명:
        //02-25 17:25:58.434: I/Unity(30632): Han : {"PARAM_PAYRAIZ_UTIL_TYPE":1,"PARAM_PAYRAIZ_UTIL_RESULT_GOOGLE_ITEM_INFO":"[{\"productId\":\"27_ruby\",\"type\":\"inapp\",\"price\":\"₩3,300\",\"price_amount_micros\":3300000000,\"price_currency_code\":\"KRW\",\"title\":\"27 ruby (God of Puzzle)\",\"description\":\"27 ruby\"},{\"productId\":\"gop45ruby\",\"type\":\"inapp\",\"price\":\"₩5,500\",\"price_amount_micros\":5500000000,\"price_currency_code\":\"KRW\",\"title\":\"45 Ruby (God of Puzzle)\",\"description\":\"45 ruby\"},{\"productId\":\"gop90ruby\",\"type\":\"inapp\",\"price\":\"₩11,000\",\"price_amount_micros\":11000000000,\"price_currency_code\":\"KRW\",\"title\":\"90 Ruby (God of Puzzle)\",\"description\":\"90 Ruby\"},{\"productId\":\"gop300ruby\",\"type\":\"inapp\",\"price\":\"₩33,000\",\"price_amount_micros\":33000000000,\"price_currency_code\":\"KRW\",\"title\":\"300 Ruby (God of Puzzle)\",\"description\":\"300 Ruby\"},{\"productId\":\"gop520ruby\",\"type\":\"inapp\",\"price\":\"₩55,000\",\"price_amount_micros\":55000000000,\"price.
        //skuDetails – String, 상품 아이템의 상세내역을 포함하는 JSON형식의 문자열(Google Play 스펙을 참조하세요.)
        //purchaseData – String, 구매 상세 내역을 포함하는 JSON형식의 문자열. 이 값은 구매요청 후 안드로이드에서 반환되는 결과값입니다. null값을 넘기면 구매유요성 검증과정은 생략됩니다.
        //dataSignature – String, 개발자의 개인키로 서명한 구매 데이타에 대한 서명값. 이 값은 구매요청 후 안드로이드에서 반환되는 결과값입니다. null값을 넘기면 구매유요성 검증과정은 생략됩니다.
        //campaignId – String, 해당 구매를 일으킨 캠페인의 아이디. 구매와 캠패인을 연관 추적하고 싶지않으면 null을 넘기면 됩니다.
        Tapjoy.TrackPurchase(m_GoogleSKU[a_PayCount], "KRW", double.Parse(a_PayCount), null);
        //Tapjoy.TrackPurchaseInGooglePlayStore(m_GoogleSKU[a_PayCount], "KRW", a_PayCount.ToString(), null);
#endif
    }
    
    public void Test_Payment(string a_PayCount)
    { 
#if UNITY_ANDROID && !UNITY_EDITOR
        Tapjoy.TrackPurchaseInGooglePlayStore(m_GoogleSKU[a_PayCount], "KRW", a_PayCount.ToString(), null); 
#endif
    }

    public void Payment_Google(string a_PayCount, Defines.Delegate_BoolStrStr a_Func)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        //안드로이드 라이브러리의 결제 요청 메소드를 호출한다.
		// PayGoogle 결제호출 메서드 : PayGoogle.
		// PARAM VALUE : 9087C0E62ACDBD2DD9093149A1E4B1623B.
		// APP PARAM : PayGoogle.
		// CP SEQ : 2.
		// APP SEQ : 34.
		// GOOGLE SKU : com.dual.test1.
		// GOOGLE BASE64KEY : MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAywpI2rcRTOA9zYk4u0R2YO2vvWe2TYGTxywoiOOCgkjgNp1Blkc2JTYTB5lpU0+be2atQf2zQWpFtSI+tDb8tDWMegWGus1gbNJq1+XT62vGiQZtOYLWF+PKOXiEpHpSTp8Hv86AQMi+OjvsihkSIFZpabk7yRBBBUS7cUNSb7Wj1TVwXtdjXQsize8zWldYNMBm8SQWYA4RFrUGqvkZx2komVeVqnYVU+1kEJYaeIIiYlRIF9EiyhsFrZS76oqtgmfg5CzBvyL91S0/K1glyTNQ0PeqghQuNvFZT7NET4KwbA10bNqLdiKQlBWlsgG5PgU6QB49BB9uNBoID/qbfQIDAQAB.
			
		if(AndroidPlugin!= null) 
		{
            m_Delegate = a_Func;
		    AndroidPlugin.Call(
                "PayGoogle", 
                m_ItemCode[a_PayCount],
                a_PayCount.ToString(),
                m_cpSEQ, 
                m_appSEQ,
                "com.dual.test1", 
                "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAgYmYLVTuNLvCRH/BXVFHzyagQNP76iJeoxVoG+LpzqDYcyl5nRwZ9xSTQnZA4Chvd2ZVcU/brzhRk/u+QDEQSUcRD8rX18VzPgdNclnmKXpJScBDDHqpaenResCaSLV7E8mWwW2ouj0Qt8wUwJZkC8Ki+/vZk5yaU4xNhpGnIK8B5X80qNHCZDr0j0gZFTjM9uEScACDAFaEs37MPAhi/hKeydje61cKGCrfB4DErIHNz9hZf1mzTV7w8dALkwPGgBULjrBJ52o9sluuOTocyW6HeryC7rUIW/UEEOva/IUN0fln65Yx3/4VP8DzuxTr3BxFtvHztViPoRyeyft3iQIDAQAB");
            Tapjoy.TrackPurchaseInGooglePlayStore(m_GoogleSKU[a_PayCount], "KRW", a_PayCount, null);
		}
#endif
    }

    public void Payment_PayRaizUtil()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // 안드로이드 라이브러리의 페이레이즈 유틸 메소드를 호출한다.
		// PayRaizUtil : PayRaizUtil 결제호출 메서드.
		// MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAywpI2rcRTOA9zYk4u0R2YO2vvWe2TYGTxywoiOOCgkjgNp1Blkc2JTYTB5lpU0+be2atQf2zQWpFtSI+tDb8tDWMegWGus1gbNJq1+XT62vGiQZtOYLWF+PKOXiEpHpSTp8Hv86AQMi+OjvsihkSIFZpabk7yRBBBUS7cUNSb7Wj1TVwXtdjXQsize8zWldYNMBm8SQWYA4RFrUGqvkZx2komVeVqnYVU+1kEJYaeIIiYlRIF9EiyhsFrZS76oqtgmfg5CzBvyL91S0/K1glyTNQ0PeqghQuNvFZT7NET4KwbA10bNqLdiKQlBWlsgG5PgU6QB49BB9uNBoID/qbfQIDAQAB.
		// com.dual.test1 : GOOGLE SKU.
		if(AndroidPlugin!= null) 
		{	
            string []   googleSKU = { m_GoogleSKU["2.99"], m_GoogleSKU["4.99"], m_GoogleSKU["9.99"], m_GoogleSKU["29.99"], m_GoogleSKU["49.99"], m_GoogleSKU["99.99"] };

			AndroidPlugin.Call(
                "PayRaizUtil", 
                1,
                "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAgYmYLVTuNLvCRH/BXVFHzyagQNP76iJeoxVoG+LpzqDYcyl5nRwZ9xSTQnZA4Chvd2ZVcU/brzhRk/u+QDEQSUcRD8rX18VzPgdNclnmKXpJScBDDHqpaenResCaSLV7E8mWwW2ouj0Qt8wUwJZkC8Ki+/vZk5yaU4xNhpGnIK8B5X80qNHCZDr0j0gZFTjM9uEScACDAFaEs37MPAhi/hKeydje61cKGCrfB4DErIHNz9hZf1mzTV7w8dALkwPGgBULjrBJ52o9sluuOTocyW6HeryC7rUIW/UEEOva/IUN0fln65Yx3/4VP8DzuxTr3BxFtvHztViPoRyeyft3iQIDAQAB", 
                googleSKU);
		}
#endif
     }

    /*
    void Payment_NStore()
    {
        // 안드로이드 라이브러리의 결제 요청 메소드를 호출한다.
		// PayNStore : PayNStore 결제호출 메서드.
		// 900A542C210B6177A5A631CC655F521D1E : PARAM VALUE
		// PayNStore : APP PARAM
		// 2 : CP SEQ
		// 65 : APP SEQ
		// LSFL213841367830710136 : NHN AID
		// t1ZLkM1jDX : NHN KEY
		// 1000002631 : NHN PID
		// 100 : NHN PRICE
		
		if(AndroidPlugin!= null) 
		{
		    AndroidPlugin.Call("PayNStore", "900A542C210B6177A5A631CC655F521D1E", "PayNStore", "2", "65", "LSFL213841367830710136", "t1ZLkM1jDX", "1000002631", "100");
        }
    }

    void Payment_TStore()
    {
        // 안드로이드 라이브러리의 결제 요청 메소드를 호출한다.
		// PayTStore : PayTStore 결제호출 메서드.
		// 903A4695821C145FE24EEE2FE43D854AFD : PARAM VALUE
		// PayTStore : APP PARAM
		// 2 : CP SEQ
		// 66 : APP SEQ
		// OA00378362 : TSTORE AID
		// 0901248658 : TSTORE PID
		 
		if(AndroidPlugin!= null) 
		{
		    AndroidPlugin.Call("PayTStore", "903A4695821C145FE24EEE2FE43D854AFD", "PayTStore", "2", "66", "OA00378362", "0901248658");
		}
    }

    void Payment_MOLStore()
    {
        // 안드로이드 라이브러리의 결제 요청 메소드를 호출한다.
		// PayMolStore : PayMolStore 결제호출 메서드.
		// 90C19A0EF11F3AA9252FA61F5C84261C6E : PARAM VALUE
		// PayMolStore : APP PARAM
		// 2 : CP SEQ
		// 93 : APP SEQ
		// 921C1B748743BA6AF7586194E56F266A6F : APP CODE (PIN PAY)
		
		if(AndroidPlugin!= null) 
		{
			AndroidPlugin.Call("PayMolStore", "90C19A0EF11F3AA9252FA61F5C84261C6E", "PayMolStore", "2", "93", "921C1B748743BA6AF7586194E56F266A6F");
		}
    }


    void Payment_Coupone()
    {
        // 안드로이드 라이브러리의 결제 요청 메소드를 호출한다.
		// PayCoupon : PayCoupon 결제호출 메서드.
		// 90BDDFE973735AF594C5DB76B924DFDDC4 : PARAM VALUE
		// PayCoupon : APP PARAM
		// 2 : CP SEQ
		// 35 : APP SEQ
		
		if(AndroidPlugin!= null) 
		{
		    AndroidPlugin.Call("PayCoupon", "90998C1C0D7DD24E0FD001CA5AA8B36A97", "PayCoupon", "1219", "114");
		}
    }

    //*/
	


}
