﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net; 
using System.Net.Sockets;
using System.Threading;
using System.IO;


//┌───────────────────────────────────────────────────┐.
//│ 이름 : CNetClient.
//│ 사용법 : .
//│ Connect(주소, 포트, 에러메시지) 접속.
//│ Send(버퍼,사이즈) 패킷보내기.
//│ Release() 종료처리.
//│ ReadPacket() 받은 패킷 읽기.
//└───────────────────────────────────────────────────┘.
public class CNetClient_V2 : MonoBehaviour
{    
	public const int m_CON_MAX_BUFFER_LENGTH = 400000;  // 패킷을 저장해둘 버퍼 m_CON_MAX_PACKET_LENGTH 보다 훨씬 길어야 한다. 400000
	public const int m_CON_MAX_PACKET_LENGTH = 8192;    // 한 프로코콜 패킷의 최대길이. 8192


	public const int m_CON_PacketHeaderSize = 12;       // 12bite가 팩킷해더이다.350 164
	
	private TcpClient      m_Socket;
    NetworkStream          m_theStream;
	StreamWriter           m_theWriter;
	StreamReader           m_theReader;


    private string      m_strIP;  
    private int         m_nPort;  
	private IPAddress   m_Ip;
    private bool        m_bIsDomain;
    private bool        m_IsCompletDomain = false;
    

	private Thread m_Thread;    // 네트워크를 처리할 프로토콜.
	private bool m_bIsConnect;  // 연결되었느냐의 여부.
	public bool GetIsConnect() { return m_bIsConnect; }
	
	private byte[] m_ReadBuffer;
	private int m_nReadBufLength; //현재 Read버퍼에 데이터가 차있는 길이.         			
	
	private Queue<tagPacketItam>    m_qReadPacket; // 읽은 패킷이 프로토콜별로 있다.
	private CStream                 m_Stream_Read;  // 읽기버퍼를 해석할떄 사용한다.
	private System.Object           m_ReadLock; // 읽기버퍼의 임계영역에 사용할 동기화 객체.		
	private tagPacketItam           m_nowReadPacket; // 현재 패킷    


	protected virtual void Read_Timing( tagPacketItam a_PacketItam ) { }   // 패킷을 받았을때 자식에서 호출할수있게함.
	protected virtual void Packet_Reading( // 값이 오고있다는 신호를 보낸다.
        int uiType, //받는중인 파일. 
        int uiRemainSize, //지금까지 받은 파일.
        int uiMaxSize ) { }  //받아야할 파일 크기.

	// 종료처리하는데 사용할 변수.
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
	public void SetUp(String a_strIP, int a_nPort, bool a_IsDomain)
	{
        // 버퍼사이즈를 잡는다.
		m_ReadBuffer        = new byte[m_CON_MAX_BUFFER_LENGTH]; 
        if(m_ReadBuffer == null) m_ReadBuffer = new byte[m_CON_MAX_BUFFER_LENGTH/2];
		
		m_qReadPacket       = new Queue<tagPacketItam>();
		m_Stream_Read       = new CStream();
		m_ReadLock          = new System.Object();
				
		m_nReadBufLength    = 0;		
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

        m_nowReadPacket = new tagPacketItam();		
		m_nowReadPacket.byPacket   = new byte[ m_CON_MAX_BUFFER_LENGTH ];        
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
            m_Socket = new TcpClient();
            m_Socket.BeginConnect(m_Ip, m_nPort, Connect_Completed, m_Socket);
			m_nReadBufLength = 0;
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
        
    private void Connect_Completed(IAsyncResult ar)
    {           
        try
        {
            m_theStream = m_Socket.GetStream();
        //    m_theWriter = new StreamWriter(m_theStream);
            m_theReader = new StreamReader(m_theStream);
            Debug.Log("연결완료.");
            m_bIsConnect = true;                        		    
            ConnectComplete(true);
        }
        catch (Exception e)
        {
            Debug.Log("연결실패.");
            m_bIsConnect = false;
            ConnectComplete(false);
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
            m_Socket = new TcpClient();
            m_Socket.Connect(m_Ip, m_nPort);
			m_nReadBufLength = 0;

            Debug.Log("연결완료.");
            m_theStream = m_Socket.GetStream();        
            m_theReader = new StreamReader(m_theStream);            
            m_bIsConnect = true;                        		    
            ConnectComplete(true);
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


    //disconnect from the socket
	public void CloseSocket() 
    {
        if(!m_bIsConnect) return;
		
        m_bIsConnect = false;
		m_theStream.Close();
//		m_theWriter.Close();
		m_theReader.Close();
		
	}

    //┌───────────────────────────────────────────────────┐.
	//│ Send.
	//│ 패킷을 보내느함수.	
	//└───────────────────────────────────────────────────┘.
	public bool Send(byte[] a_buf, int a_length)
	{
	    if (m_bIsConnect == false)
	    {
		    System.Console.WriteLine("Send : socket == null");
            Debug.Log("Send : socket == null");
		    return false;
	    }	
	    //CCrypt.Encrypt(a_buf, 4, a_buf, 4, a_length - 4);
	    //m_theWriter.Write(a_buf, 0, a_length);
	    //m_theWriter.Flush();
        
        m_theStream.Write(a_buf, 0, a_length);
	    return true;
	}    

    //┌───────────────────────────────────────────────────┐.
	//│ read message from server.
	//│ 패킷을 보내느함수 .
	//│.
	//└───────────────────────────────────────────────────┘.    
	public void CheckReadPacket()
    {		
        if(!m_bIsConnect) return; 

		if (m_theStream.DataAvailable) 
        {
            int nSize = m_ReadBuffer.Length - m_nReadBufLength; // 읽을수이쓴데이터 계산.
			// Byte[] inStream = new Byte[mySocket.SendBufferSize];.
			nSize = m_theStream.Read(m_ReadBuffer, m_nReadBufLength, nSize);
			
			if (nSize == 0)
			{
			    System.Console.WriteLine("recvive : signal shutdown");
                Debug.Log("recvive : signal shutdown");
				return; // 이부분 다시.
			}
			if (nSize < 0)
			{
				System.Console.WriteLine("recvive : Error {0}", nSize);
                Debug.Log("recvive : Error " + nSize);
				return; // 이부분 다시.
			}

            // 템프에 있는데이터를 실제 데이터스트림에 복사한다.
			// Buffer.BlockCopy(m_ReadBufferTemp, 0, m_ReadBuffer, m_nReadBufLength, nSize);			
            m_nReadBufLength += nSize;
			Parser();
		}

        
	}
    

	//┌───────────────────────────────────────────────────┐.
	//│ Parser.
	//│ .
	//│ 데이터를 해석한다.
	//└───────────────────────────────────────────────────┘.
	private void Parser()
	{
		int nTempReadSize = 0;
		while (true)
		{ 
			if ( m_ReadPage_step == 0 && (m_CON_PacketHeaderSize > m_nReadBufLength) ) break;  // 패킷해더만큼 패킷이 왔는가를 검사한다.

			if ( m_ReadPage_step == 0)
			{
				m_ReadPage_BufSeek  = 0;
				m_ReadPage_StartKey = 0;
				m_ReadPage_EndKey   = 0;
                
                m_Stream_Read.SetReadBuffer(m_ReadBuffer);                				                
				m_nowReadPacket.uiSize     = m_Stream_Read.Read_int();
				m_nowReadPacket.uiCount    = m_Stream_Read.Read_int();
				m_nowReadPacket.uiType     = m_Stream_Read.Read_int();
                if( m_nowReadPacket.byPacket.Length < (m_nowReadPacket.uiSize - m_CON_PacketHeaderSize) )
				    m_nowReadPacket.byPacket   = new byte[ (m_nowReadPacket.uiSize - m_CON_PacketHeaderSize) ];
                
                //퍼버를 읽은 만큼 댕긴다.
				m_nReadBufLength -= m_CON_PacketHeaderSize;
				m_ReadPage_RemainPacketSize = m_nowReadPacket.uiSize - m_CON_PacketHeaderSize;		
				Buffer.BlockCopy(m_ReadBuffer, m_CON_PacketHeaderSize, m_ReadBuffer, 0, m_ReadBuffer.Length - m_CON_PacketHeaderSize);				
				
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
            
			// 읽어야할 수 > 실제 버퍼에 패킷 사이즈.
			if (nTempReadSize > m_nReadBufLength)
            {
                Packet_Reading( m_nowReadPacket.uiType, m_ReadPage_BufSeek, m_ReadPage_RemainPacketSize);
                break;
            }                
			
			m_ReadPage_StartKey = m_ReadPage_EndKey;
			//CCrypt.Decrypt(m_ReadBuffer, 0, m_ReadBuffer, 0, nTempReadSize, m_ReadPage_StartKey, ref m_ReadPage_EndKey);.
			
			if (m_ReadPage_step == 1)
			{
				Buffer.BlockCopy(m_ReadBuffer, 0, m_nowReadPacket.byPacket, m_ReadPage_BufSeek, nTempReadSize);
				m_ReadPage_BufSeek += nTempReadSize;
				
				m_nReadBufLength -= nTempReadSize;
				m_ReadPage_RemainPacketSize -= nTempReadSize;
			}
			else if (m_ReadPage_step == 2)
			{
				Buffer.BlockCopy(m_ReadBuffer, 0, m_nowReadPacket.byPacket, m_ReadPage_BufSeek, nTempReadSize);
				m_ReadPage_BufSeek += nTempReadSize;
				
                //퍼버를 읽은 만큼 댕긴다.
				Buffer.BlockCopy(m_ReadBuffer, nTempReadSize, m_ReadBuffer, 0, m_ReadBuffer.Length - nTempReadSize);
				m_nReadBufLength -= nTempReadSize;
				m_ReadPage_RemainPacketSize -= nTempReadSize;
				
				Monitor.Enter(m_ReadLock);
                
				try
				{
                    for(int i=0; i<3; i++)
                    {
                        if( m_nowReadPacket.byPacket == null || m_nowReadPacket.byPacket.Length < (m_nowReadPacket.uiSize - m_CON_PacketHeaderSize) ) continue;
				        m_qReadPacket.Enqueue(m_nowReadPacket);
                        break;
                    }
				}
				finally
				{
				    Monitor.Exit(m_ReadLock);                        
				}
				
				Read_Timing( m_nowReadPacket);
				m_ReadPage_step = 0;
			}

            // 값이 오고있다는 신호를 보낸다.
            Packet_Reading( m_nowReadPacket.uiType, m_ReadPage_BufSeek, m_ReadPage_RemainPacketSize);
		}
	} // function end.

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