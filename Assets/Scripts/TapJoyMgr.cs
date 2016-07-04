using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using TapjoyUnity;

public class TapJoyMgr : MonoBehaviour 
{
    public enum emOperWell { AppLaunch=0, StageFailed, InsufficientCurrency, END };
    private TJPlacement [] m_PT;
    

    private int[]           m_ReadyCount;
    private int             m_PaymentGoldApple = 0;

	void Start ()
    {
        DontDestroyOnLoad(gameObject);        
        Tapjoy.OnConnectSuccess += HandleConnectSuccess;        
	}

    public void HandleConnectSuccess() 
    {        
        Tapjoy.OnGetCurrencyBalanceResponse += HandleGetCurrencyBalanceResponse;
	    Tapjoy.OnGetCurrencyBalanceResponseFailure += HandleGetCurrencyBalanceResponseFailure;
        Tapjoy.OnEarnedCurrency += HandleEarnedCurrency;
        Tapjoy.OnSpendCurrencyResponse += HandleSpendCurrencyResponse;
	    Tapjoy.OnSpendCurrencyResponseFailure += HandleSpendCurrencyResponseFailure;
        Tapjoy.OnAwardCurrencyResponse += HandleAwardCurrencyResponse;
	    Tapjoy.OnAwardCurrencyResponseFailure += HandleAwardCurrencyResponseFailure;

        TJPlacement.OnRequestSuccess += RequestSuccess;
        TJPlacement.OnRequestFailure += RequestFailure;
        TJPlacement.OnContentReady   += HandleContentReady;
        TJPlacement.OnContentShow    += HandleContentShow;
        TJPlacement.OnContentDismiss += HandleContentDismiss;
        
        TJPlacement.OnPurchaseRequest   += HandleOnPurchaseRequest;
        TJPlacement.OnRewardRequest     += HandleOnRewardRequest;
        
        m_PT = new TJPlacement[(int)emOperWell.END];     
	    m_PT[(int)emOperWell.AppLaunch]               = TJPlacement.CreatePlacement("AppLaunch");
        m_PT[(int)emOperWell.StageFailed]             = TJPlacement.CreatePlacement("StageFailed");
        m_PT[(int)emOperWell.InsufficientCurrency]    = TJPlacement.CreatePlacement("InsufficientCurrency");        

        m_ReadyCount = new int[(int)emOperWell.END]{0,0,0};                
	}

	void Update () 
    {
        if( m_ReadyCount != null && m_PT != null)
        {
            for(int i=0; i<(int)emOperWell.END; i++)
            {
                if(m_ReadyCount[i] >= 2)
                {
                    if (m_PT[i].IsContentReady())
                    {
                        m_PT[i].ShowContent();
                        Debug.Log("HAN : ShowContent");
                        m_ReadyCount[i] =5;
                    }
                }            
            }
        }
    }


    public void Ready(emOperWell a_Index) // 프로그램 열때.
    {
        #if UNITY_ANDROID && !UNITY_EDITOR//!UNITY_EDITOR
            if( m_ReadyCount[(int)a_Index] == 1 ) return;
            m_PT[(int)a_Index].RequestContent();
            m_ReadyCount[(int)a_Index] = 0;
            Debug.Log("HAN : Ready:"+a_Index.ToString());
        #endif
    }
    
    public void Open(emOperWell a_Index) // 프로그램 열때.
    {
        #if UNITY_ANDROID && !UNITY_EDITOR//!UNITY_EDITOR            
            m_ReadyCount[(int)a_Index]++;
            Debug.Log("HAN : Open:"+a_Index.ToString());
        #endif
    }

    public void DirectOpen(emOperWell a_Index) // 프로그램 열때.
    {
        #if UNITY_ANDROID && !UNITY_EDITOR//!UNITY_EDITOR
            if(m_ReadyCount[(int)a_Index] == 0)             
                m_PT[(int)a_Index].RequestContent();

            m_ReadyCount[(int)a_Index]++;
            Debug.Log("HAN : DirectOpen:"+a_Index.ToString());
        #endif
    }

    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    // callback function
    // Called when the SDK has made contact with Tapjoy's servers. 
    // It does not necessarily mean that any content is available.
    public void RequestSuccess(TJPlacement placement) 
    {        
        for(int i=0; i<(int)emOperWell.END; i++) 
        {
            if(m_PT[i] == placement)            
                Debug.Log("HAN : RequestSuccess:"+((emOperWell)i).ToString());             
        }
    }

    // Called when there was a problem during connecting Tapjoy servers.
    public void RequestFailure(TJPlacement placement, string error) 
    {
        for(int i=0; i<(int)emOperWell.END; i++) 
        {
            if(m_PT[i] == placement)            
                Debug.Log("HAN : RequestFailure:"+((emOperWell)i).ToString());             
        }
    }

    // Called when the content is actually available to display.
    // 콘텐츠를 실제로 출력할수 있을때 불른다.
    public void HandleContentReady(TJPlacement placement) 
    {        
        for(int i=0; i<(int)emOperWell.END; i++) 
        {
            if(m_PT[i] == placement)
            {
                m_ReadyCount[i]++;
                Debug.Log("HAN : HandleContentReady:"+((emOperWell)i).ToString());
            }
        }
    }

    // Called when the content is shown.
    public void HandleContentShow(TJPlacement placement) 
    {        
        for(int i=0; i<(int)emOperWell.END; i++) 
        {
            if(m_PT[i] == placement) Debug.Log("HAN : HandleContentShow:"+((emOperWell)i).ToString()); 
        }        
    }

    // Called when the content is dismissed.
    public void HandleContentDismiss(TJPlacement placement) 
    {
        for(int i=0; i<(int)emOperWell.END; i++) 
        {
            if(m_PT[i] == placement)
            {
                Debug.Log("HAN : HandleContentShow:"+((emOperWell)i).ToString()); 
                m_ReadyCount[i] =0;
            }
        }
        RefleshGoldApple();
    }

    public void HandleOnPurchaseRequest(TJPlacement placement, 
        TJActionRequest request, string productId)
    {
        for(int i=0; i<(int)emOperWell.END; i++) 
        {
            if(m_PT[i] == placement)            
                Debug.Log("HAN : HandleOnPurchaseRequest:"+((emOperWell)i).ToString());             
        }
        
    }

    public void HandleOnRewardRequest(TJPlacement placement, 
        TJActionRequest request, string itemId, int quantity) 
    {
        for(int i=0; i<(int)emOperWell.END; i++) 
        {
            if(m_PT[i] == placement)            
                Debug.Log("HAN : HandleOnRewardRequest:"+((emOperWell)i).ToString());             
        }
    }


    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    // 매번 콘텐츠를 요청해야함.
    // virtual money
    // 03-02 17:30:59.421: I/Unity(27258): HAN : HandleGetCurrencyBalanceResponse: currencyName: GoldApple, balance: 23
    // ...
    // 03-02 17:31:53.118: D/ConnectivityService(782): notifyType CAP_CHANGED for NetworkAgentInfo [WIFI () - 104]
    // 03-02 17:31:59.163: D/ConnectivityService(782): notifyType CAP_CHANGED for NetworkAgentInfo [WIFI () - 104]
    // 03-02 17:32:59.467: D/ConnectivityService(782): notifyType CAP_CHANGED for NetworkAgentInfo [WIFI () - 104]
    // 03-02 17:33:02.474: D/ConnectivityService(782): notifyType CAP_CHANGED for NetworkAgentInfo [WIFI () - 104]
    // 03-02 17:34:03.102: I/Unity(27258): HAN : HandleContentReady
    // 03-02 17:34:03.163: I/Unity(27258): HAN : HandleContentDismiss
    // 03-02 17:34:04.346: I/Unity(27258): HAN : HandleEarnedCurrency: currencyName: GoldApple, amount: 1
    // 03-02 17:34:04.362: I/Unity(27258): HAN : HandleGetCurrencyBalanceResponse: currencyName: GoldApple, balance: 24
    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
    // 03-02 19:20:14.426: I/Unity(30097): HAN : OpenLaunch
    // 03-02 19:20:14.821: I/Unity(30097): HAN : HandleGetCurrencyBalanceResponse: currencyName: GoldApple, balance: 24
    // 03-02 19:20:16.422: I/Unity(30097): HAN : ShowContent
    // 03-02 19:20:16.436: I/Unity(30097): HAN : RequestSuccess : m_PT_AppLaunch
    // 03-02 19:20:16.437: I/Unity(30097): HAN : HandleContentReady : m_PT_AppLaunch

    // 03-02 19:20:16.437: I/Unity(30097): HAN : RequestSuccess : m_PT_InsufficientCurrency
    // 03-02 19:20:16.437: I/Unity(30097): HAN : HandleContentReady : m_PT_InsufficientCurrency
    // 03-02 19:20:16.561: I/Unity(30097): HAN : HandleContentShow : m_PT_AppLaunch
    // 03-02 19:20:16.562: I/Unity(30097): HAN : RequestSuccess : m_PT_StageFailed
    // 03-02 19:20:16.562: I/Unity(30097): HAN : HandleContentReady : m_PT_StageFailed
    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=



    //황금사과 정보요청하기.
    public void RefleshGoldApple()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        Tapjoy.GetCurrencyBalance();
        #endif
    }
    // 황금사과 정보요청 성공.
    public void HandleGetCurrencyBalanceResponse(string currencyName, int balance)
    {
	    Debug.Log("HAN : HandleGetCurrencyBalanceResponse: currencyName: " + currencyName + ", balance: " + balance);
        m_PaymentGoldApple = balance;
        if(balance > 0)
        {
            UseGoldApple(m_PaymentGoldApple);
        }
            
    }
	// 황금사과 정보요청 실패.
    public void HandleGetCurrencyBalanceResponseFailure(string error) 
    {
	    Debug.Log("HAN : HandleGetCurrencyBalanceResponseFailure: " + error);
    }

    // 최근에 머니를 벌었을때 호출된다.
    // 흭득한 머니만 온다. 
    public void HandleEarnedCurrency(string currencyName, int amount) 
    { Debug.Log("HAN : HandleEarnedCurrency: currencyName: " + currencyName + ", amount: " + amount); }

    // 황금사과를 소비한다.
    public void UseGoldApple(int a_Value)
    {
        Tapjoy.SpendCurrency(a_Value);
    }
    // 소비성공.
    public void HandleSpendCurrencyResponse(string currencyName, int balance) 
    {
	    Debug.Log("HAN : HandleSpendCurrencyResponse: currencyName: " + currencyName + ", balance: " + balance);        
        DataMgr.Inst.m_SerMgr.ADD_FriendPoint(m_PaymentGoldApple, GoldApple_Result);
    }
	// 소비실패.
    public void HandleSpendCurrencyResponseFailure(string error) 
    {
	    Debug.Log("HAN : HandleSpendCurrencyResponseFailure: " + error);
    }

    // 황금사과를 지금한다.
    public void GetGoldApple(int a_Value)
    {
        Tapjoy.AwardCurrency(a_Value);
    }

    // 황금사과를 지금한다.
    public void HandleAwardCurrencyResponse(string currencyName, int balance) 
    {
	    Debug.Log("HAN : HandleAwardCurrencySucceeded: currencyName: " + currencyName + ", balance: " + balance);        
    }
	
    public void HandleAwardCurrencyResponseFailure(string error) 
    {
	    Debug.Log("HAN : HandleAwardCurrencyResponseFailure: " + error);
    }

    // 황금사과 지급성공(FriendPoint).
    void GoldApple_Result(bool a_Result, Dictionary<string, object> a_dicJson, string a_Str, serverManager.ErrorCode a_ErrorCode)
    {   
        if( a_Result)
        {   
            DataMgr.Inst.DataPasing( a_dicJson["OutCode"].ToString(), a_dicJson["OutValue"].ToString() );
            UseGoldApple( int.Parse(a_dicJson["OutValue"].ToString()) );
        }   
        else
        {   
            GetGoldApple(m_PaymentGoldApple); //다시.
        }   
    }   
}
#elif UNITY_IOS
public class TapJoyMgr : MonoBehaviour 
{
	void Start () 
    {  
	}

    public void HandleConnectSuccess() 
    {               
	}

	void Update () 
    {
    }


  
    //황금사과 정보요청하기.
    public void RefleshGoldApple()
    {        
    }
    // 황금사과 정보요청 성공.
    public void HandleGetCurrencyBalanceResponse(string currencyName, int balance)
    {	                
    }

	// 황금사과 정보요청 실패.
    public void HandleGetCurrencyBalanceResponseFailure(string error) 
    {	    
    }

    // 최근에 머니를 벌었을때 호출된다.
    // 흭득한 머니만 온다. 
    public void HandleEarnedCurrency(string currencyName, int amount) 
    {     
    }

    // 황금사과를 소비한다.
    public void UseGoldApple(int a_Value)
    {        
    }

    // 소비성공.
    public void HandleSpendCurrencyResponse(string currencyName, int balance) 
    {	    
    }
	// 소비실패.
    public void HandleSpendCurrencyResponseFailure(string error) 
    {	
    }

    // 황금사과를 지금한다.
    public void GetGoldApple(int a_Value)
    {     
    }

    // 황금사과를 지금한다.
    public void HandleAwardCurrencyResponse(string currencyName, int balance) 
    {	    
    }
	
    public void HandleAwardCurrencyResponseFailure(string error) 
    {
    }

    // 황금사과 지급성공(FriendPoint).
    void GoldApple_Result(bool a_Result, Dictionary<string, object> a_dicJson, string a_Str, serverManager.ErrorCode a_ErrorCode)
    {   
    }   
}
#endif