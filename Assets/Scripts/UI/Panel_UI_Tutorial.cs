using UnityEngine;
using System.Collections;

public class Panel_UI_Tutorial : MonoBehaviour 
{
    public UITexture    m_BG;
    public UIButton     m_Left;
    public UIButton     m_Right;
    public UISprite     m_End;
    public UILabel      m_LbPage;
    

    private Texture []  m_Tex;
    private int         m_ViewIndex;

	// Use this for initialization
    void Awake()
    {
        m_Tex = new Texture[8];
        for(int i=0; i<m_Tex.Length; i++)
        {
            m_Tex[i] = Resources.Load( "Textures/Tutorial/tuttorial_"+(i+1) ) as Texture;
        }
        m_ViewIndex = 0;
        m_LbPage.text = "1/"+m_Tex.Length;
        m_End.enabled = false;
    }

	void Start () {}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public  void onLeft()
    {
        m_ViewIndex--; if(m_ViewIndex < 0) m_ViewIndex = 0;
        m_LbPage.text = (m_ViewIndex+1).ToString()+"/"+m_Tex.Length;        
        m_BG.mainTexture = m_Tex[m_ViewIndex];
        m_Right.gameObject.SetActive(true);
        m_End.enabled = false;
    }

    public void onRight()
    {
        m_ViewIndex++;
        if(m_ViewIndex >= m_Tex.Length)
            m_ViewIndex = m_Tex.Length-1;
        
        if(m_ViewIndex >= (m_Tex.Length-1))
        {
            m_End.enabled = true;
            m_Right.gameObject.SetActive(false);
        }

        m_LbPage.text = (m_ViewIndex+1).ToString()+"/"+m_Tex.Length;
        m_BG.mainTexture = m_Tex[m_ViewIndex];
    }

    public  void onClose()
    {
        DataMgr.Inst.SetBackPageState();
    }
}
