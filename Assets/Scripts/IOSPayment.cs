using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_IOS
using Prime31;
#endif

public class IOSPayment : MonoBehaviour
{   
	public Transform                        m_Lobby;        
	private Dictionary<string, string>      m_ItemCode;
	private Defines.Delegate_BoolStrStr     m_Delegate;
	private string m_strBuyID = "";

	#if UNITY_IOS
	private List<StoreKitProduct>   m_RecvProducts; // 에플로부터 받은 인앱리스트.
	private bool m_canMakePayments;
	#endif   

	struct Box
	{
		public string strValue;
		public float fValue;
	}

	// Use this for initialization.
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
		m_ItemCode.Add( PayList[5].strValue, "gop006" );
		m_ItemCode.Add( PayList[4].strValue, "gop005" );
		m_ItemCode.Add( PayList[3].strValue, "gop004" );
		m_ItemCode.Add( PayList[2].strValue, "gop003" );
		m_ItemCode.Add( PayList[1].strValue, "gop002" );
		m_ItemCode.Add( PayList[0].strValue, "gop001" );                                

		#if UNITY_IOS && !UNITY_EDITOR  
		IosSetup();
		m_canMakePayments = StoreKitBinding.canMakePayments();
		StoreKitBinding.requestProductData( new string[] { m_ItemCode[PayList[0].strValue], m_ItemCode[PayList[1].strValue], m_ItemCode[PayList[2].strValue], m_ItemCode[PayList[3].strValue], m_ItemCode[PayList[4].strValue], m_ItemCode[PayList[5].strValue] } );
		#endif        
	}

	// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=.
	// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=.
	// 구매 함수.
	// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=.
	// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=.
	#if( (UNITY_IOS || UNITY_TVOS ) && !UNITY_EDITOR)	
	public bool Payment_PayRaiz(string a_PayCount, Defines.Delegate_BoolStrStr a_Delegate)
	{
		//MonoBehaviourGUI.endColumn( true );
		if(!StoreKitBinding.canMakePayments()) return false;
		if( m_RecvProducts == null || m_RecvProducts.Count == 0 ) return false;

		m_strBuyID = "";
		for(int i=0; i<m_RecvProducts.Count; i++)
		{
			if(m_RecvProducts[i].productIdentifier == m_ItemCode[a_PayCount])
			{
				m_strBuyID = m_RecvProducts[i].productIdentifier;
				break;
			}
		}
		m_Delegate = a_Delegate;
		StoreKitBinding.purchaseProduct( m_strBuyID, 1);
		return true;
	}

	void IosSetup()
	{
		// Listens to all the StoreKit events. All event listeners MUST be removed before this object is disposed!
		StoreKitManager.refreshReceiptSucceededEvent += refreshReceiptSucceededEvent;
		StoreKitManager.refreshReceiptFailedEvent += refreshReceiptFailedEvent;
		StoreKitManager.transactionUpdatedEvent += transactionUpdatedEvent;
		StoreKitManager.productPurchaseAwaitingConfirmationEvent += productPurchaseAwaitingConfirmationEvent;
		StoreKitManager.purchaseSuccessfulEvent += purchaseSuccessfulEvent;
		StoreKitManager.purchaseCancelledEvent += purchaseCancelledEvent;
		StoreKitManager.purchaseFailedEvent += purchaseFailedEvent;
		StoreKitManager.productListReceivedEvent += productListReceivedEvent;
		StoreKitManager.productListRequestFailedEvent += productListRequestFailedEvent;
		StoreKitManager.restoreTransactionsFailedEvent += restoreTransactionsFailedEvent;
		StoreKitManager.restoreTransactionsFinishedEvent += restoreTransactionsFinishedEvent;
		StoreKitManager.paymentQueueUpdatedDownloadsEvent += paymentQueueUpdatedDownloadsEvent;
		StoreKitBinding.enableHighDetailLogs (true);
	}

	// StoreKitBinding.refreshReceipt(); 성공하면.
	void refreshReceiptSucceededEvent()
	{
		Debug.Log( "refreshReceiptSucceededEvent" );
	}

	// StoreKitBinding.refreshReceipt(); 실패하면.
	void refreshReceiptFailedEvent( string error )
	{
		Debug.Log( "refreshReceiptFailedEvent: " + error );
	}

	// Fired anytime Apple updates a transaction if you called setShouldSendTransactionUpdateEvents with true. 
	// 언제든지 시작합니다. setShouldSendTransactionUpdateEvents를 true라고 하면  애플른 트렌잭션을 업데이트합니다.
	// Check the transaction.transactionState to know what state the transaction is currently in.
	// 트렌젝션의 현재 상태를 알기위해 확인한다. transaction.transactionState
	void transactionUpdatedEvent( StoreKitTransaction transaction )
	{        
		Debug.Log( "transactionUpdatedEvent: " + transaction );
	}

	// StoreKitBinding.requestProductData(); 성공할때.
	void productListReceivedEvent( List<StoreKitProduct> productList )
	{
		m_RecvProducts = productList;
		Debug.Log( "productListReceivedEvent. total products received: " + productList.Count );

		// 출력한다 인앱상품.
		foreach( StoreKitProduct product in productList )
		Debug.Log( product.ToString() + "\n" );
	}

	// StoreKitBinding.requestProductData(); 실패할때.
	void productListRequestFailedEvent( string error )
	{        
		Debug.Log( "productListRequestFailedEvent: " + error );
	}


	// StoreKitBinding.purchaseProduct() 취소.
	void purchaseFailedEvent( string error )
	{
		if(m_Delegate != null) m_Delegate(false, null, null);
		Debug.Log( "purchaseFailedEvent: " + error );
	}

	// 취소.
	// 시스템 또는 유저에 의해 호출 된다. 
	void purchaseCancelledEvent( string error )
	{
		if(m_Delegate != null) m_Delegate(false, null, null);
		Debug.Log( "purchaseCancelledEvent: " + error );
	}


	// 인앱 상품 구매가 애플서버에서 반환 및 완료를 기다리고 있을때 호출된다.                     (Fired when a product purchase has returned from Apple\'s servers and is awaiting completion. 
	// 기본적으로 플러그인은 당신을 위해 거래를 완료합니다.                                     (By default the plugin will finish transactions for you.
	// 너는 바꿀수있다 that behavior를 autoConfirmTransactions함수를 false로 셋팅하거나.
	// 구매완료를 위해 StoreKitBinding.finishPendingTransaction 호출을 요구합니다.            (You can change that behavior by setting autoConfirmTransactions to false which then requires that you call StoreKitBinding.finishPendingTransaction to complete a purchase.)            
	void productPurchaseAwaitingConfirmationEvent( StoreKitTransaction transaction )
	{
		StoreKitBinding.finishPendingTransaction(transaction.transactionIdentifier);
		Debug.Log( "productPurchaseAwaitingConfirmationEvent: " + transaction );
	}

	// 인앱상품이 성공적으로 지불됬을때 시작됩니다. (Fired when a product is successfully paid for.)
	// 이 이벤트는 제공한다 StoreKitTransaction object를 이것은 인앱상품의 식별자와 영수증(receipt)를 제공한다.   (The event will provide a StoreKitTransaction object that holds the productIdentifer and receipt of the purchased product.)        
	void purchaseSuccessfulEvent( StoreKitTransaction transaction )
	{        
		if (m_strBuyID == transaction.productIdentifier) 
		{
			System.Console.WriteLine("Succ {0}/", transaction.base64EncodedTransactionReceipt);
			string Str1 = transaction.productIdentifier + "_" + transaction.base64EncodedTransactionReceipt;
			if (m_Delegate != null) m_Delegate (true, Str1, null);
			m_strBuyID = "";
		}
	}


	// Fired when an error is encountered while adding transactions from the user\'s purchase history back to the queue.
	// 에러가 발생했다 이에러는 유저의 구매히스토리를 큐에 back에 추가하는 중에 발생하는 트랜젝션 중에 발생할수 있다.
	void restoreTransactionsFailedEvent( string error )
	{
		Debug.Log( "restoreTransactionsFailedEvent: " + error );
	}

	//사용자의 구매 내역에서 모든 트랜잭션이 성공적으로 큐의 뒤에 추가되었을 때 시작됩니다.        (Fired when all transactions from the user\'s purchase history have successfully been added back to the queue. )        
	//주이! 이 이벤트는 거의 상항 발동할것입니다.     (Note that this event will almost always.)
	// 실행된후에 각 개별 트랜젝션이 처리된다.        (fire before each individual transaction is processed.)
	void restoreTransactionsFinishedEvent()
	{
		Debug.Log( "restoreTransactionsFinished" );
	}

	// 시작된다 어떤 StoreKitDownload objects가 애플에 의해 업데이트 됬을 때.      (Fired when any SKDownload objects are updated by Apple.)
	// 모든 다운로드가 완료 될 때까지 호스팅 콘텐츠를 사용하는 경우에는 거래를 확인 할 수 없습니다.    (If using hosted content you should not be confirming the transaction until all downloads are complete.)
	void paymentQueueUpdatedDownloadsEvent( List<StoreKitDownload> downloads )
	{
		Debug.Log( "paymentQueueUpdatedDownloadsEvent: " );
		foreach( var dl in downloads )
		Debug.Log( dl );
	}
	#else 
	public bool Payment_PayRaiz(string a_PayCount, Defines.Delegate_BoolStrStr a_Delegate)
	{
		m_Delegate = a_Delegate;
		Invoke ("CollTest", 0.1f);
		return true;
	}

	void CollTest()
	{
		m_Delegate(true, "Test_IOS_ewoJInNpZ25hdHVyZSIgPSAiQXlBcVZkTnNBVnIxYjJNUkkxdHB4MktkOFp5c3dXOUw3VlhVSGh2L0p3djlKVGV3VFg2alAwUlBlS2Vvak9IQWVFU0xLc1drSW5wbENHT0RBbzZUYXVZcFFhT2JDSThWNXFUV2hXczdYaGdVaGVnbEw4RHdnYTBFT2pHY1RSVlFiZ1VqdmdBbWkzc21XQlZ1T0REcmZqRFlacmsyYWY2MTUyUDhseVBiWVNKL1JHSkRCSGg0ZjgzOGZacXJUNXJIRVo0QUlnbW1hWDFjbk5lVEJKSFpSWTBMQWU1em1jeVpiNzJ0aWkzWWZ3a1p0blM5MGpYZ0Q5bE4rejdnQzgyR0F5aWJDWGZ2NENSd1NtcXczYzNZZTBpRi9CeUFUak13d1ZaQmg4WXh5RlJRS3FIZjhRNmhyNDFNU2NNTEdhSXdUNGNmNW42aWQ4TkNGbW1sbEtGOUE1b0FBQVdBTUlJRmZEQ0NCR1NnQXdJQkFnSUlEdXRYaCtlZUNZMHdEUVlKS29aSWh2Y05BUUVGQlFBd2daWXhDekFKQmdOVkJBWVRBbFZUTVJNd0VRWURWUVFLREFwQmNIQnNaU0JKYm1NdU1Td3dLZ1lEVlFRTERDTkJjSEJzWlNCWGIzSnNaSGRwWkdVZ1JHVjJaV3h2Y0dWeUlGSmxiR0YwYVc5dWN6RkVNRUlHQTFVRUF3dzdRWEJ3YkdVZ1YyOXliR1IzYVdSbElFUmxkbVZzYjNCbGNpQlNaV3hoZEdsdmJuTWdRMlZ5ZEdsbWFXTmhkR2x2YmlCQmRYUm9iM0pwZEhrd0hoY05NVFV4TVRFek1ESXhOVEE1V2hjTk1qTXdNakEzTWpFME9EUTNXakNCaVRFM01EVUdBMVVFQXd3dVRXRmpJRUZ3Y0NCVGRHOXlaU0JoYm1RZ2FWUjFibVZ6SUZOMGIzSmxJRkpsWTJWcGNIUWdVMmxuYm1sdVp6RXNNQ29HQTFVRUN3d2pRWEJ3YkdVZ1YyOXliR1IzYVdSbElFUmxkbVZzYjNCbGNpQlNaV3hoZEdsdmJuTXhFekFSQmdOVkJBb01Da0Z3Y0d4bElFbHVZeTR4Q3pBSkJnTlZCQVlUQWxWVE1JSUJJakFOQmdrcWhraUc5dzBCQVFFRkFBT0NBUThBTUlJQkNnS0NBUUVBcGMrQi9TV2lnVnZXaCswajJqTWNqdUlqd0tYRUpzczl4cC9zU2cxVmh2K2tBdGVYeWpsVWJYMS9zbFFZbmNRc1VuR09aSHVDem9tNlNkWUk1YlNJY2M4L1cwWXV4c1FkdUFPcFdLSUVQaUY0MWR1MzBJNFNqWU5NV3lwb041UEM4cjBleE5LaERFcFlVcXNTNCszZEg1Z1ZrRFV0d3N3U3lvMUlnZmRZZUZScjZJd3hOaDlLQmd4SFZQTTNrTGl5a29sOVg2U0ZTdUhBbk9DNnBMdUNsMlAwSzVQQi9UNXZ5c0gxUEttUFVockFKUXAyRHQ3K21mNy93bXYxVzE2c2MxRkpDRmFKekVPUXpJNkJBdENnbDdaY3NhRnBhWWVRRUdnbUpqbTRIUkJ6c0FwZHhYUFEzM1k3MkMzWmlCN2o3QWZQNG83UTAvb21WWUh2NGdOSkl3SURBUUFCbzRJQjF6Q0NBZE13UHdZSUt3WUJCUVVIQVFFRU16QXhNQzhHQ0NzR0FRVUZCekFCaGlOb2RIUndPaTh2YjJOemNDNWhjSEJzWlM1amIyMHZiMk56Y0RBekxYZDNaSEl3TkRBZEJnTlZIUTRFRmdRVWthU2MvTVIydDUrZ2l2Uk45WTgyWGUwckJJVXdEQVlEVlIwVEFRSC9CQUl3QURBZkJnTlZIU01FR0RBV2dCU0lKeGNKcWJZWVlJdnM2N3IyUjFuRlVsU2p0ekNDQVI0R0ExVWRJQVNDQVJVd2dnRVJNSUlCRFFZS0tvWklodmRqWkFVR0FUQ0IvakNCd3dZSUt3WUJCUVVIQWdJd2diWU1nYk5TWld4cFlXNWpaU0J2YmlCMGFHbHpJR05sY25ScFptbGpZWFJsSUdKNUlHRnVlU0J3WVhKMGVTQmhjM04xYldWeklHRmpZMlZ3ZEdGdVkyVWdiMllnZEdobElIUm9aVzRnWVhCd2JHbGpZV0pzWlNCemRHRnVaR0Z5WkNCMFpYSnRjeUJoYm1RZ1kyOXVaR2wwYVc5dWN5QnZaaUIxYzJVc0lHTmxjblJwWm1sallYUmxJSEJ2YkdsamVTQmhibVFnWTJWeWRHbG1hV05oZEdsdmJpQndjbUZqZEdsalpTQnpkR0YwWlcxbGJuUnpMakEyQmdnckJnRUZCUWNDQVJZcWFIUjBjRG92TDNkM2R5NWhjSEJzWlM1amIyMHZZMlZ5ZEdsbWFXTmhkR1ZoZFhSb2IzSnBkSGt2TUE0R0ExVWREd0VCL3dRRUF3SUhnREFRQmdvcWhraUc5Mk5rQmdzQkJBSUZBREFOQmdrcWhraUc5dzBCQVFVRkFBT0NBUUVBRGFZYjB5NDk0MXNyQjI1Q2xtelQ2SXhETUlKZjRGelJqYjY5RDcwYS9DV1MyNHlGdzRCWjMrUGkxeTRGRkt3TjI3YTQvdncxTG56THJSZHJqbjhmNUhlNXNXZVZ0Qk5lcGhtR2R2aGFJSlhuWTR3UGMvem83Y1lmcnBuNFpVaGNvT0FvT3NBUU55MjVvQVE1SDNPNXlBWDk4dDUvR2lvcWJpc0IvS0FnWE5ucmZTZW1NL2oxbU9DK1JOdXhUR2Y4YmdwUHllSUdxTktYODZlT2ExR2lXb1IxWmRFV0JHTGp3Vi8xQ0tuUGFObVNBTW5CakxQNGpRQmt1bGhnd0h5dmozWEthYmxiS3RZZGFHNllRdlZNcHpjWm04dzdISG9aUS9PamJiOUlZQVlNTnBJcjdONFl0UkhhTFNQUWp2eWdhWndYRzU2QWV6bEhSVEJoTDhjVHFBPT0iOwoJInB1cmNoYXNlLWluZm8iID0gImV3b0pJbTl5YVdkcGJtRnNMWEIxY21Ob1lYTmxMV1JoZEdVdGNITjBJaUE5SUNJeU1ERTJMVEExTFRFM0lEQTBPakkyT2pBMklFRnRaWEpwWTJFdlRHOXpYMEZ1WjJWc1pYTWlPd29KSW5WdWFYRjFaUzFwWkdWdWRHbG1hV1Z5SWlBOUlDSTVaamN3TVdFNVpUa3daV1kyTXpVeE5UZGxPVGcwWWpWbVptRm1Oall6WlRrell6azFaamc0SWpzS0NTSnZjbWxuYVc1aGJDMTBjbUZ1YzJGamRHbHZiaTFwWkNJZ1BTQWlNVEF3TURBd01ESXhNVGd3TnpVNU5TSTdDZ2tpWW5aeWN5SWdQU0FpTUNJN0Nna2lkSEpoYm5OaFkzUnBiMjR0YVdRaUlEMGdJakV3TURBd01EQXlNVEU0TURjMU9UVWlPd29KSW5GMVlXNTBhWFI1SWlBOUlDSXhJanNLQ1NKdmNtbG5hVzVoYkMxd2RYSmphR0Z6WlMxa1lYUmxMVzF6SWlBOUlDSXhORFl6TkRnME16WTJORGsxSWpzS0NTSjFibWx4ZFdVdGRtVnVaRzl5TFdsa1pXNTBhV1pwWlhJaUlEMGdJa016UmpGR01qRXdMVEk1TkRFdE5ESkVOaTFDUVRRNUxVVkZRME14TVRjNVEwUTNRU0k3Q2draWNISnZaSFZqZEMxcFpDSWdQU0FpWjI5d01EQXhJanNLQ1NKcGRHVnRMV2xrSWlBOUlDSXhNRGcwTXpRNE5qRTNJanNLQ1NKaWFXUWlJRDBnSW10eUxtaGhibmx2ZFM1cGIzTXVaMjlrYjJad2RYcDZiR1VpT3dvSkluQjFjbU5vWVhObExXUmhkR1V0YlhNaUlEMGdJakUwTmpNME9EUXpOalkwT1RVaU93b0pJbkIxY21Ob1lYTmxMV1JoZEdVaUlEMGdJakl3TVRZdE1EVXRNVGNnTVRFNk1qWTZNRFlnUlhSakwwZE5WQ0k3Q2draWNIVnlZMmhoYzJVdFpHRjBaUzF3YzNRaUlEMGdJakl3TVRZdE1EVXRNVGNnTURRNk1qWTZNRFlnUVcxbGNtbGpZUzlNYjNOZlFXNW5aV3hsY3lJN0Nna2liM0pwWjJsdVlXd3RjSFZ5WTJoaGMyVXRaR0YwWlNJZ1BTQWlNakF4Tmkwd05TMHhOeUF4TVRveU5qb3dOaUJGZEdNdlIwMVVJanNLZlE9PSI7CgkiZW52aXJvbm1lbnQiID0gIlNhbmRib3giOwoJInBvZCIgPSAiMTAwIjsKCSJzaWduaW5nLXN0YXR1cyIgPSAiMCI7Cn0=", null);
	}
	#endif
}


