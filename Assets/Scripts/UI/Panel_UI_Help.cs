using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

public class Panel_UI_Help : MonoBehaviour 
{
    public UISprite m_BlackBG;
    public UIButton m_BtnSend;

    public UILabel m_lbPageTile;
    public UILabel m_lbCatergory;
    public UILabel m_lbTitle;
    public UILabel m_lbContent;
    public UILabel m_lbEmail;
    public UILabel m_lbCellPhone;
    public UILabel m_lbAgree;
    public UILabel m_lbSend;


	public UIPopupList CatList;
	public UILabel Category;
	public UILabel Subject;
	public UILabel Content;
	public UILabel EmailUser;
	public UILabel EmailDomain;
	public UILabel PhoneNumber1;
	public UILabel PhoneNumber2;
	public UILabel PhoneNumber3;	
	public UIToggle Accept;
    
    
    private Deligate    m_LoadingMark;
	private GameObject  m_RootLobby;
    void SetUp(GameObject a_Root)
    { m_RootLobby = a_Root; }
    
    private bool    m_IsSent;
    private int     m_nSentResult;
    private object  m_SentLock;

	//문의 화면 정보를 초기화한다.
    void Awake()
    { 
        m_nSentResult = 0;
        m_IsSent = false;
        m_SentLock = new object();

		//메뉴에 존재하는 탭에 들어갈 정보들을 초기화한다.
        for( int i=0; i<CatList.items.Count; i++ )
        {
           //  if(CatList.items[i] == "게임 문의")
           //  if(CatList.items[i] == "설치 및 실행")
           //  if(CatList.items[i] == "버그 문의")
           //  if(CatList.items[i] == "이벤트 문의")
           //  if(CatList.items[i] == "결제 및 환불(지급)")
           //  if(CatList.items[i] == "건의 및 신고")
           //  if(CatList.items[i] == "계정 문의")
            
            CatList.items[0] = DataMgr.Inst.GetLocal("Game Inquiry");
            CatList.items[1] = DataMgr.Inst.GetLocal("Install or Execute");
            CatList.items[2] = DataMgr.Inst.GetLocal("Bug Inquiry");
            CatList.items[3] = DataMgr.Inst.GetLocal("Event Inquiry");
            CatList.items[4] = DataMgr.Inst.GetLocal("Payment or Refund");
            CatList.items[5] = DataMgr.Inst.GetLocal("Proposal of Report");
            CatList.items[6] = DataMgr.Inst.GetLocal("Account");
        }

		//각각의 메뉴이름 초기화.
        CatList.value       = DataMgr.Inst.GetLocal("Game Inquiry");
        m_lbPageTile.text   = DataMgr.Inst.GetLocal("Customer Center");
        m_lbCatergory.text  = DataMgr.Inst.GetLocal("Section");
        m_lbTitle.text      = DataMgr.Inst.GetLocal("Title");
        m_lbContent.text    = DataMgr.Inst.GetLocal("Content");
        m_lbEmail.text      = DataMgr.Inst.GetLocal("E-mail");
        m_lbCellPhone.text  = DataMgr.Inst.GetLocal("Mobile");            
        m_lbSend.text       = DataMgr.Inst.GetLocal("Send");
        m_lbAgree.text      = DataMgr.Inst.GetLocal("I agree to the collection and use of personal information");

        m_BlackBG.enabled = false;
        onAccept();
        //개인정보 수집 및 이용에 동의합니다.
    }

    public void onAccept()
    {
        if(Accept.value)
        {
            m_BtnSend.GetComponent<BoxCollider>().enabled = true;
            m_BtnSend.SetState(UIButtonColor.State.Normal,true);
        }

        else
        {
            m_BtnSend.GetComponent<BoxCollider>().enabled = false;
            m_BtnSend.SetState(UIButtonColor.State.Disabled ,true);
        }
            
    }

	//메일을 전송한다.
	void Send()
	{
		MailMessage mail = new MailMessage();

		string user_email = EmailUser.text + "@" + EmailDomain.text;
		//string user_phone = PhoneNumber1.text + "-" + PhoneNumber2.text + "-" + PhoneNumber3.text;
        
        mail.To.Add("hanyouhelp@gmail.com"); // jhhan@solideng.co.kr help@hanyou.kr, hanttl1004@naver.com, hanttl1004@gmail.com
        //mail.To.Add("hanttl1004@gmail.com");
		mail.From = new MailAddress("hanyouhelp@gmail.com");
        //mail.From = new MailAddress("hanttl1004@gmail.com");
		mail.Subject = "("+CatList.value+")"+"("+user_email+") : "+Subject.text;
		mail.Body = Content.text+"\n\nID:"+DataMgr.Inst.m_UserInfo.ID;//+"\n\nPhone:"+user_phone;		        
        mail.BodyEncoding = UTF8Encoding.UTF8;
        mail.SubjectEncoding = UTF8Encoding.UTF8; 
        
        

        // 465, 587
		SmtpClient smtpServer = new SmtpClient("smtp.gmail.com", 587); //mail.solideng.co.kr, smtp.worksmobile.com, smtp.gmail.com, smtp.naver.com
        //SmtpClient smtpServer = new SmtpClient("smtp.gmail.com", 587);
        smtpServer.Timeout = 10;
        smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtpServer.EnableSsl = true;
        //smtpServer.Credentials = new System.Net.NetworkCredential("hanttl1004", "han840627", "gmail.com") as ICredentialsByHost;
        smtpServer.Credentials = new System.Net.NetworkCredential("hanyouhelp", "gksdbrhddb1", "gmail.com") as ICredentialsByHost;        
        //smtpServer.Credentials = new System.Net.NetworkCredential("hanttl1004", "ehdrhkwkd1122", "naver.com") as ICredentialsByHost;
        //smtpServer.Credentials = new System.Net.NetworkCredential("help_sender", "zhzkrpdla1", "hanyou.kr") as ICredentialsByHost;

        smtpServer.SendCompleted += SendCompletedEventHandler;
		ServicePointManager.ServerCertificateValidationCallback = 
			delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{ return true; };
			
		try
        {
			smtpServer.SendAsync(mail, "Help");
            
            lock(m_SentLock) 
            { 
                m_nSentResult = 0;
                m_IsSent = false; 
            }
            
            m_BlackBG.enabled = true;
            m_BtnSend.GetComponent<BoxCollider>().enabled = false;
            m_BtnSend.SetState(UIButtonColor.State.Disabled ,true);
            if( m_LoadingMark != null) m_LoadingMark.UpCount();
            else m_LoadingMark = Deligate.Create();
		}
		catch (Exception e)
        {			
            CMessageBox.Create( DataMgr.Inst.GetLocal("Failed sending email."), 1, null ); 
            DataMgr.Inst.Log("Help:Send : " + e.Message );
		}
	}
    
    void SendCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        // Get the unique identifier for this asynchronous operation.
        String token = (string) e.UserState;
        lock(m_SentLock) 
        {         
            if (e.Error != null || e.Cancelled) // 에러.
            {
                m_nSentResult = -1;
                Console.WriteLine("Send Error : " + token + " : "+ e.Error.ToString());            
            }
            else // 썽공.
            {
                m_nSentResult = 1;
                Console.WriteLine("Message sent : " + token);
            }

            m_IsSent = true; 
        }
    }

    void Update()
    {
        lock(m_SentLock)
        {
            if(m_IsSent)
            {
                if(m_nSentResult == 1)
                {                     
                    CMessageBox.Create( DataMgr.Inst.GetLocal("Successfully sent."), 1, onDlgRelust);
                }  
                else if(m_nSentResult == -1)
                {
                    CMessageBox.Create( DataMgr.Inst.GetLocal("Failed sending email."), 1, null );
                }                
                m_BlackBG.enabled = false;            
                m_IsSent = false;
                if(m_LoadingMark != null)        
                    m_LoadingMark.DownCount();
            }
        }                
    }


    public void onDlgRelust(bool a_Value)
    {
        Subject.text ="";
        Content.text ="";
        Back();
    }

	void Back()
    {
		DataMgr.Inst.SetBackPageState();
	}

    
}