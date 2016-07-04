using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net; 
using System.Net.Sockets;
using System.Threading;
public struct tagPacketItam
{
	public int uiSize;     // 패캣해더를 포함한 총길이. 
	public int uiCount;    // 받은 순서.
	public int uiType;     // 프로토콜 타입 .
	public byte[] byPacket; // 프로토콜 패킷 .
};

//┌───────────────────────────────────────────────────┐.
//│ 이름 : CNetClient.
//│ 사용법 :.
//│ Connect(주소, 포트, 에러메시지) 접속 .
//│ Send(버퍼,사이즈) 패킷보내기.
//│ Release() 종료처리.
//│ ReadPacket() 받은 패킷 읽기.
//└───────────────────────────────────────────────────┘ .
public class CNetClient
{
	public const int m_CON_MAX_BUFFER_LENGTH = 400000;  // 패킷을 저장해둘 버퍼 m_CON_MAX_PACKET_LENGTH 보다 훨씬 길어야 한다..
	public const int m_CON_MAX_PACKET_LENGTH = 8192;    // 한 프로코콜 패킷의 최대길이. 8192


	public const int m_CON_PacketHeaderSize = 12;       // 12bite가 팩킷해더이다.350 164  
	
	private Socket      m_Socket;
    private string      m_strIP;  
    private int         m_nPort;  
	private IPAddress   m_Ip;    
	private IPEndPoint  m_Endpoint;        	
    private bool        m_bIsDomain;
    private bool        m_IsCompletDomain = false;
    

	private Thread m_Thread;    // 네트워크를 처리할 프로토콜.
	private bool m_bIsConnect; // 연결되었느냐의 여부. 
	public bool GetIsConnect() { return m_bIsConnect; }
	
	private byte[] m_ReadBuffer;
	private int m_nReadBufLength; //현재 Read버퍼에 데이터가 차있는 길이.         
	
	private byte[] m_WriteBuffer;
	private int m_nWriteBufLength; //현재 쓰고싶은 데이터가 차있는 길이.
	
	private Queue<tagPacketItam> m_qReadPacket; // 읽은 패킷이 프로토콜별로 있다.
	private CStream m_Stream_Read;  // 읽기버퍼를 해석할떄 사용한다.
	private System.Object m_ReadLock; // 읽기버퍼의 임계영역에 사용할 동기화 객체.             
	
	
	private ArrayList m_WriteSocket;
	private ArrayList m_ReadSocket;
	private ArrayList m_ExcepSocket;
	private byte[] m_ReadBufferTemp;
	
	private System.Object m_Sync; // 동기화에 사용할 객체. 
	
	public virtual void Close() { } // 네트워크 종료될때 자식에서 호출함.
	public virtual void Read_Timing() { } // 패킷을 받았을때 자식에서 호출할수있게함.
	
	// 종료처리하는데 사용할 변수 .
	private System.Object m_SyneThreadStop; //m_bThreadStop의 동기화에 사용한다.
	private bool m_bThreadStop;   
	
	// 읽기를 페이지 단위로 읽게끔 하는 변수들.
	private int     m_ReadPage_step;
	private int     m_ReadPage_PacketSize;
	private int     m_ReadPage_RemainPacketSize;
	private int     m_ReadPage_StartKey;
	private int     m_ReadPage_EndKey;
	private byte[]  m_ReadPage_Buf;
	private int     m_ReadPage_BufSeek;
	private int     m_ReadPage_iPacketCount;
	private int     m_ReadPage_iPacketType;	

    // 커넥트완료시 호출.
    public virtual void ConnectComplete(bool a_IsConnect) { }
	//┌───────────────────────────────────────────────────┐.
	//│ CNetClient.
	//│ 접속.
	//│
	//└───────────────────────────────────────────────────┘.     

	public CNetClient(String a_strIP, int a_nPort, bool a_IsDomain)
	{
		m_ReadBuffer        = new byte[m_CON_MAX_BUFFER_LENGTH]; // 버퍼사이즈를 잡는다.
		m_ReadBufferTemp    = new byte[m_CON_MAX_BUFFER_LENGTH]; // 버퍼사이즈를 잡는다.            
		m_WriteBuffer       = new byte[m_CON_MAX_BUFFER_LENGTH]; // 버퍼사이즈를 잡는다.
		
		m_qReadPacket       = new Queue<tagPacketItam>();
		m_Stream_Read       = new CStream();
		m_ReadLock          = new System.Object();
		
		m_Sync              = new System.Object();
		m_WriteSocket       = new ArrayList();
		m_ReadSocket        = new ArrayList();
		m_ExcepSocket       = new ArrayList();
		m_nReadBufLength    = 0;
		m_nWriteBufLength   = 0;
		m_bIsDomain         = a_IsDomain;
        m_nPort             = a_nPort;

		// 쓰레드 스탑할 객체 
		m_SyneThreadStop    = new System.Object();
		m_bThreadStop       = false;
		m_bIsConnect        = false;            
		m_strIP = a_strIP;
		m_ReadPage_step             = 0;
		m_ReadPage_PacketSize       = 0;
		m_ReadPage_RemainPacketSize = 0;
		m_ReadPage_StartKey         = 0;
		m_ReadPage_EndKey           = 0;
		m_ReadPage_Buf              = null;
		m_ReadPage_iPacketCount     = 0;
		m_ReadPage_iPacketType      = 0;
		m_ReadPage_BufSeek          = 0;     
	}
	
	//┌───────────────────────────────────────────────────┐.
	//│ Connect : 클라이언트에 연결한다.                
	//│ Release : 쓰래드를 종료하고, 소켓을 닫는다.
    //│ 
    //│  Policy Server(유니티에서만 사용한다.).
	//│	  이구문을 실행하고 서버에서 폴리시 서버를 가동하지 않으면. 
	//│	  c# unable to connect as no valid cross....  에러가난다.
	//│	  주소는 서버 주소, 포트 843고정이다.
    //│
	//└───────────────────────────────────────────────────┘.    
	public bool Connect()
	{
		// 연결
		try
		{
			if(m_bIsConnect)
			{
				System.Console.WriteLine("Connect() : socket connect!!");
				return false;
			}
 
            // 연결 설정
            if(m_IsCompletDomain == false)
            {
                if(m_bIsDomain)
                {
                    System.Net.IPHostEntry hostInfo = System.Net.Dns.GetHostEntry(m_strIP);  		                
                    if(hostInfo.AddressList.Length > 0)                        
                    {
                        m_strIP = hostInfo.AddressList[0].ToString();
                        m_Ip = IPAddress.Parse(m_strIP);                                                
                        Security.PrefetchSocketPolicy(m_strIP, 843);
                        m_IsCompletDomain = true;
                    }
                }
                else
                {
                    m_Ip = IPAddress.Parse(m_strIP);
                    Security.PrefetchSocketPolicy(m_strIP, 843);
                    m_IsCompletDomain = true;                    
                }
            }
        		       
		    // 서버연결정보 객체
		    m_Endpoint = new IPEndPoint(m_Ip, m_nPort);
            
			// 소켓만들기. 
			m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			
			
			// Send operations will time-out if confirmation 
			// is not received within 1000 milliseconds.
			// m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 2000);
			// The socket will linger for 10 seconds after Socket.Close is called.
			LingerOption lingerOption = new LingerOption(true, 0);
			m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, lingerOption);
            
            // 비동기연결.
            SocketAsyncEventArgs Saea = new SocketAsyncEventArgs();            
            Saea.RemoteEndPoint = m_Endpoint;
            Saea.Completed += this.Connect_Completed;                        
			if(m_Socket.ConnectAsync(Saea) == false)
            {
                Connect_Completed(this, Saea );
            }            

			m_nReadBufLength = 0;
			m_nWriteBufLength = 0;            
		}
		catch (Exception e)
		{
			System.Console.WriteLine("Connect Failed ({0})", e.Message);
            Debug.Log("Connect Failed " + e.Message);
			m_bIsConnect = false;
			return false;
		}
	
        // 쓰레드.
		return true;
	}
        
    private void Connect_Completed(object sender, SocketAsyncEventArgs e)
    {   
        if(e.SocketError == SocketError.Success)
        {
            m_bIsConnect = true;
            ConnectComplete(true);
            m_bThreadStop = false;
		    m_Thread = new System.Threading.Thread(this.ThreadCallBack);
		    m_Thread.Start();
            Debug.Log("연결완료.");
        }
        else
        {
            m_bIsConnect = false;
            ConnectComplete(false);
            Debug.Log("연결실패.");
        }
        
        
    }
    

    public bool Connect_Direct()
	{
		// 연결
		try
		{
			if(m_bIsConnect)
			{
				System.Console.WriteLine("Connect() : socket connect!!");
				return false;
			}
 
            // 연결 설정
            if(m_IsCompletDomain == false)
            {
                if(m_bIsDomain)
                {
                    System.Net.IPHostEntry hostInfo = System.Net.Dns.GetHostEntry(m_strIP);  		                
                    if(hostInfo.AddressList.Length > 0)                        
                    {
                        m_strIP = hostInfo.AddressList[0].ToString();
                        m_Ip = IPAddress.Parse(m_strIP);                                                
                        Security.PrefetchSocketPolicy(m_strIP, 843);
                        m_IsCompletDomain = true;
                    }
                }
                else
                {
                    m_Ip = IPAddress.Parse(m_strIP);
                    Security.PrefetchSocketPolicy(m_strIP, 843);
                    m_IsCompletDomain = true;                    
                }
            }
        		       
		    // 서버연결정보 객체
		    m_Endpoint = new IPEndPoint(m_Ip, m_nPort);
            
			// 소켓만들기. 
			m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			
			
			// Send operations will time-out if confirmation 
			// is not received within 1000 milliseconds.
			// m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 2000);
			// The socket will linger for 10 seconds after Socket.Close is called.
			LingerOption lingerOption = new LingerOption(true, 0);
			m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, lingerOption);                        


			m_Socket.Connect(m_Endpoint);
            
            m_nReadBufLength    = 0;
			m_nWriteBufLength   = 0;
            m_bIsConnect        = true;            
            ConnectComplete(true);

            m_bThreadStop = false;
		    m_Thread = new System.Threading.Thread(this.ThreadCallBack);
		    m_Thread.Start();                                                        
			
            Debug.Log("Connect .");
		}
		catch (Exception e)
		{
			System.Console.WriteLine("Connect Failed ({0})", e.Message);
            Debug.Log("Connect Failed " + e.Message);
			m_bIsConnect = false;
            ConnectComplete(false);            
			return false;
		}
	
        // 쓰레드.
		return true;
	}


    public string GetMyIp()
    { 
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        string ClientIP = string.Empty;
        for (int i = 0; i < host.AddressList.Length; i++)
        {
            if (host.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
            {
                ClientIP = host.AddressList[i].ToString();
            }
        }
        return ClientIP;
    }

	// 연결해제 ( 외부에서 호출할떄!! 쓰레드 종료할때까지 대기  ).
	public void Release()
	{
		if (!m_bIsConnect) return;
		if (m_Socket == null) return;
		
		Thread TempThread = m_Thread;
		m_bIsConnect = false;

		lock (m_SyneThreadStop)
		{
			m_bThreadStop = true;
		}
		if (TempThread != null)
		{
			TempThread.Join();
			TempThread = null;
		}
	}
	
	
	// 내부에서 사용할때. 
	public void Release_InThread()
	{
		if (!m_bIsConnect) return;
		if (m_Socket == null) return;            
		m_bIsConnect = false;

		lock (m_SyneThreadStop)
		{
			m_bThreadStop = true;
		}
	}
	
	
	//┌───────────────────────────────────────────────────┐.
	//│ Send.
	//│ 패킷을 보내느함수 .
	//│.
	//└───────────────────────────────────────────────────┘.
	public bool Send(byte[] a_buf, int a_length)
	{
		lock (m_Sync)
		{
			if (m_bIsConnect == false)
			{
				System.Console.WriteLine("Send : socket == null");
                Debug.Log("Send : socket == null");
				return false;
			}
			
			if ((m_nWriteBufLength + a_length) > m_WriteBuffer.Length )
			{
				System.Console.WriteLine("Send : Max Buffer");
                Debug.Log("Send : Max Buffer");
				return false;
			}
			
			//                CCrypt.Encrypt(a_buf, 4, a_buf, 4, a_length - 4);
			
			Buffer.BlockCopy(a_buf, 0, m_WriteBuffer, m_nWriteBufLength, a_length);
			
			m_nWriteBufLength += a_length;
			
			return true;
		}
	}
	
	//┌───────────────────────────────────────────────────┐.
	//│ ThreadCallBack.
	//│ 쓰레드 .
	//│.
	//└───────────────────────────────────────────────────┘.
	public void ThreadCallBack()
	{
		while (true)
		{
			lock (m_SyneThreadStop)
			{
				if (m_bThreadStop == true)
				{
					if (m_nWriteBufLength <= 0 && m_nReadBufLength <= 0) // 보낼것이 있으면 대기해볼까.                            
						break;                                                    
				}
			}
			
			try
			{
				m_WriteSocket.Clear();
				m_ReadSocket.Clear();
				m_ExcepSocket.Clear();
				
				m_ReadSocket.Add(m_Socket);
				m_ExcepSocket.Add(m_Socket);
				if (m_nWriteBufLength > 0) m_WriteSocket.Add(m_Socket);
				
				
				// 신호가 있을때 까지 무한정 대기 한다.
				Socket.Select(m_ReadSocket, m_WriteSocket, m_ExcepSocket, 1000);
				
				// 읽을것이 있다. 
				if (m_ReadSocket.Count > 0)
				{
					int nSize = m_ReadBuffer.Length - m_nReadBufLength;
					
					// 데이터를 읽는다 .그러나 m_ReadBuffer가 비어있는 만큼만 받아야한다.
					nSize = m_Socket.Receive(m_ReadBufferTemp, nSize, 0);
					
					if (nSize == 0)
					{
						System.Console.WriteLine("recvive : signal shutdown");
                        Debug.Log("recvive : signal shutdown");
						break;// 이부분 다시 
					}
					if (nSize < 0)
					{
						System.Console.WriteLine("recvive : Error {0}", nSize);
                        Debug.Log("recvive : Error " + nSize);
						break;// 이부분 다시 
					}

					// 템프에 있는데이터를 실제 데이터스트림에 복사한다.  
					Buffer.BlockCopy(m_ReadBufferTemp, 0, m_ReadBuffer, m_nReadBufLength, nSize);
					m_nReadBufLength += nSize;					
					Parser();
				}
				
				// 썻어... 
				if (m_WriteSocket.Count > 0)
				{
					lock (m_Sync)
					{
						//m_Socket.Send( buffer );.
						//byte [] buffer = Encoding.UTF8.GetBytes(a_str);.
						int nSize = m_Socket.Send(m_WriteBuffer, m_nWriteBufLength, 0);
						
						if (nSize < 0)
						{
							System.Console.WriteLine("Send : nSize error {0}", nSize);
						}
						else if (nSize > 0)
						{
							while (true)
							{
								m_nWriteBufLength -= nSize;
								
								if (m_nWriteBufLength > 0)
								{
									Buffer.BlockCopy(m_WriteBuffer, nSize, m_WriteBuffer, 0, m_nWriteBufLength);
								}
								else
								{
									m_nWriteBufLength = 0;
									break;
								}
								
							}
						}//if.
					}//lock.
				}//if.
			}
			catch( Exception ez )
			{
				System.Console.WriteLine("Exception exit Loop!! : {0} ", ez.Message);
                Debug.Log("Exception exit Loop!! : " + ez.Message);
				break;
			}
		}//while.
		        
		System.Console.WriteLine("exit Loop!!");
		Debug.Log("exit Loop!!");

		// 종료 처리 코드들.                  
		m_Socket.Shutdown(SocketShutdown.Send);
		m_Socket = null;		
		m_Thread = null;				
		m_bIsConnect = false;
        Close(); // 자식에서 호출할 종료 이벤트함수 .
	}//function.
	
	//┌───────────────────────────────────────────────────┐.
	//│ Parser.
	//│.
	//│ 데이터를 해석한다.         
	//└───────────────────────────────────────────────────┘.
	private void Parser()
	{            
		int nTempReadSize = 0;
		while (true)
		{
			if ( m_ReadPage_step == 0 && m_CON_PacketHeaderSize > m_nReadBufLength) break;  // 패킷해더만큼 패킷이 왔는가를 검사한다.
			
			
			if (m_ReadPage_step == 0)
			{
				m_ReadPage_BufSeek = 0;
				m_ReadPage_StartKey = 0;
				m_ReadPage_EndKey = 0;
				//                    CCrypt.Decrypt(m_ReadBuffer, sizeof(int), m_ReadBuffer, sizeof(int), sizeof(int)*2, m_ReadPage_StartKey, ref m_ReadPage_EndKey);.
				m_Stream_Read.SetReadBuffer(m_ReadBuffer);
				m_ReadPage_PacketSize   = m_Stream_Read.Read_int();
				m_ReadPage_iPacketCount = m_Stream_Read.Read_int();
				m_ReadPage_iPacketType  = m_Stream_Read.Read_int();
				
				
				m_nReadBufLength -= m_CON_PacketHeaderSize;
				m_ReadPage_RemainPacketSize = m_ReadPage_PacketSize - m_CON_PacketHeaderSize;
				
				Buffer.BlockCopy(m_ReadBuffer, m_CON_PacketHeaderSize, m_ReadBuffer, 0, m_ReadBuffer.Length - m_CON_PacketHeaderSize);
				
				
				
				if (m_ReadPage_Buf == null || m_ReadPage_Buf.Length < (m_ReadPage_PacketSize - m_CON_PacketHeaderSize))
					m_ReadPage_Buf = new byte[((m_ReadPage_PacketSize - m_CON_PacketHeaderSize))];
				
				m_ReadPage_step = 1;
			}
			
			//읽을수 있는 사이즈계산.
			if (m_ReadPage_RemainPacketSize > m_ReadBuffer.Length)
				nTempReadSize = m_ReadBuffer.Length;
			else if (m_ReadPage_RemainPacketSize <= m_ReadBuffer.Length)
			{
				nTempReadSize = m_ReadPage_RemainPacketSize;
				if (m_ReadPage_step == 1) m_ReadPage_step = 2;
			}
			
			// 해더 멋번째의 변수의 크기보다 패킷더 많이 왔어야한다.
			if (nTempReadSize > m_nReadBufLength) break;
			
			m_ReadPage_StartKey = m_ReadPage_EndKey;
			//                CCrypt.Decrypt(m_ReadBuffer, 0, m_ReadBuffer, 0, nTempReadSize, m_ReadPage_StartKey, ref m_ReadPage_EndKey);.
			
			if (m_ReadPage_step == 1)
			{
				//     m_Stream_Read.SetReadBuffer(m_ReadBuffer);.
				//     for (int i=0; i<nTempReadSize; i++).
				//     {.
				//         m_ReadPage_Buf[ m_ReadPage_BufSeek ] = m_Stream_Read.Read_byte();.
				//         m_ReadPage_BufSeek++;.
				//     }.
				Buffer.BlockCopy(m_ReadBuffer, 0, m_ReadPage_Buf, m_ReadPage_BufSeek, nTempReadSize);
				m_ReadPage_BufSeek += nTempReadSize;
				
				m_nReadBufLength -= nTempReadSize;
				m_ReadPage_RemainPacketSize -= nTempReadSize;
			}
			else if (m_ReadPage_step == 2)
			{
				//    m_Stream_Read.SetReadBuffer(m_ReadBuffer);.
				//    for (int i=0; i<nTempReadSize; i++).
				//    {.
				//        m_ReadPage_Buf[ m_ReadPage_BufSeek ] = m_Stream_Read.Read_byte();.
				//        m_ReadPage_BufSeek++;.
				//    }.
				Buffer.BlockCopy(m_ReadBuffer, 0, m_ReadPage_Buf, m_ReadPage_BufSeek, nTempReadSize);
				m_ReadPage_BufSeek += nTempReadSize;
				
				Buffer.BlockCopy(m_ReadBuffer, nTempReadSize, m_ReadBuffer, 0, m_ReadBuffer.Length - nTempReadSize);
				m_nReadBufLength -= nTempReadSize;
				m_ReadPage_RemainPacketSize -= nTempReadSize;
				
				Monitor.Enter(m_ReadLock);
				try
				{
					tagPacketItam temp;
					temp.uiSize     = m_ReadPage_PacketSize;
					temp.uiCount    = m_ReadPage_iPacketCount;
					temp.uiType     = m_ReadPage_iPacketType;
					temp.byPacket   = new byte[ (m_ReadPage_PacketSize - m_CON_PacketHeaderSize) ];
					Buffer.BlockCopy(m_ReadPage_Buf, 0, temp.byPacket, 0, (m_ReadPage_PacketSize - m_CON_PacketHeaderSize));
					m_qReadPacket.Enqueue(temp);
				}
				finally
				{
					Monitor.Exit(m_ReadLock);
				}
				
				Read_Timing();
				m_ReadPage_step = 0;
			}
		}
	} //function end.
	
	//┌───────────────────────────────────────────────────┐.
	//│ ReadPacket.
	//│.
	//│ 메인루브에서 실행해서 패킷을 얻어온다.
	//└───────────────────────────────────────────────────┘.        
	public bool ReadPacket(out tagPacketItam a_Item)
	{
		if (Monitor.TryEnter(m_ReadLock))
		{
			try
			{
				if( m_qReadPacket.Count > 0 )
				{
					a_Item = m_qReadPacket.Dequeue();
					return true;
				}                    
				else
				{
					a_Item.uiSize = 0;
					a_Item.uiCount = 0;
					a_Item.uiType = 0;
					a_Item.byPacket = null;
					return false;
				}
			}
			finally
			{
				Monitor.Exit(m_ReadLock);                    
			}
		}            
		
		a_Item.uiSize = 0;
		a_Item.uiCount = 0;
		a_Item.uiType = 0;
		a_Item.byPacket = null;            
		return false;
	}
}// clase end.
