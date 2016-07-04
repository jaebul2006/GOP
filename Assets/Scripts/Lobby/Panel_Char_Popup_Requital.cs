using UnityEngine;
using System.Collections;

public class Panel_Char_Popup_Requital : MonoBehaviour
{
    public GameObject  m_Bg;
    public GameObject  m_btnOK;
    public UILabel     m_LbName;
    public UILabel     m_LbUpgrade;
    public GameObject  m_Grade;
    public UITexture   m_texChar;

    public UISprite    m_Eff_Bling; // 번쩍번쩍
    public UISprite    m_Eff_Spark;
    public UISprite    m_Eff_Light;
    public UISprite    m_Eff_Sport;

    // Use this for initialization
    void Start ()
    {
        m_Bg.SetActive(false);
        
        m_btnOK.GetComponent<UISprite>().enabled = false;
        m_btnOK.GetComponent<BoxCollider>().enabled = false;
        m_btnOK.SetActive(false);     
    }
	
	// Update is called once per frame
	void Update ()
    {
        
    }   

    // 이펙트 프레임 돌린다.
    IEnumerator EffectSpark_ani()
    {
        int idx = 1;        
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            idx++;
            if (idx > 3) idx = 1;
            m_Eff_Spark.spriteName = "efferct_streng_0" + idx.ToString();
        }
    }
    IEnumerator EffectLight_ani()
    {
        int idx = 1;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            idx++;
            if (idx > 3) idx = 1;
            m_Eff_Light.spriteName = "effrct_light_0" + idx.ToString();
        }
    }

    public delegate void CollOk();
    private CollOk m_CollOk;
    DataMgr.tagUserSummon m_CharData = null;
    public void Begin(DataMgr.tagUserSummon a_CharData, CollOk a_CollOk)
    {
        m_CharData          = a_CharData;
        m_CollOk            = a_CollOk;

        m_Eff_Bling.enabled = true;
        TweenScale ts       = m_Eff_Bling.gameObject.AddComponent<TweenScale>();
        ts.from             = new Vector3(1, 1, 1);
        ts.to               = new Vector3(15, 15, 15);
        ts.style            = UITweener.Style.Once;
        ts.eventReceiver    = gameObject;
        ts.duration         = 1.0f;
        ts.method           = UITweener.Method.Linear;
        ts.ignoreTimeScale  = false;
        ts.callWhenFinished = "collBack_Begin"; // 트윈모션이 끝나고 호출된 함수명.
    }

    public void collBack_Begin()
    {
        m_Eff_Bling.enabled = false;
        m_btnOK.GetComponent<UISprite>().enabled = true;
        m_btnOK.GetComponent<BoxCollider>().enabled = true;
        m_btnOK.SetActive(true);

        m_Bg.SetActive(true);
        m_LbName.text           = m_CharData.Data.strName;
        m_texChar.mainTexture   = Resources.Load("Textures/Char/icon_mon_" + m_CharData.Data.strResurceID) as Texture;
        //m_sprGrade.spriteName   = "icon_grade_0" + m_CharData.nGrade;        
        m_Grade.transform.FindChild( "Star_"+m_CharData.nGrade.ToString() ).gameObject.SetActive(true);

        if (m_CharData.nUpgrade == 0) m_LbUpgrade.enabled = false;
        else m_LbUpgrade.text = "+" + m_CharData.nUpgrade;

        StartCoroutine(EffectLight_ani());
        StartCoroutine(EffectSpark_ani());
    }

    public void Callback_OK()
    {
        m_CollOk();
        Destroy(gameObject);
    }
}
