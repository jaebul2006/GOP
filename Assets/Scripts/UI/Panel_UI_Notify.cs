using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Panel_UI_Notify : MonoBehaviour 
{
    public UILabel m_lbTitleNotice;
    public UILabel m_lbNewNotice;
    public UILabel m_lbEvent;    

	private GameObject  m_PrefabSlot;
	private Transform   m_ListParent;
	private GameObject  m_Popup;
    private List<GameObject> m_List;


    private GameObject m_RootLobby;
    void SetUp(GameObject a_Root)
    { m_RootLobby = a_Root; }

	void Awake()
    {
        m_List = new List<GameObject>();
		m_PrefabSlot = Resources.Load ("Prefabs/EventSlot") as GameObject;
		m_Popup = Resources.Load ("Prefabs/Panel_Event_Popup") as GameObject;
		m_ListParent = transform.FindChild ("Events");

        m_lbTitleNotice.text     = DataMgr.Inst.GetLocal("Notice");
        m_lbNewNotice.text       = DataMgr.Inst.GetLocal("Recent Notice");
        m_lbEvent.text           = DataMgr.Inst.GetLocal("Event");
	}

    public void OnEnable()
    {
        SetNotice();
    }

	//서버에서 전송받은 데이터로 공지 메뉴를 초기화.
	void SetNotice()
    {
        foreach(GameObject obj in m_List)
        {
            Destroy(obj);
        }
        m_List.Clear();

		for(int i=0; i<DataMgr.Inst.m_NoticeList.Count; i++)
        {			
			GameObject Slot = Instantiate(m_PrefabSlot) as GameObject;
			Slot.name = i.ToString();
			Slot.transform.parent = m_ListParent;
			Slot.transform.localScale = Vector3.one;
			Slot.transform.FindChild("Label").GetComponent<UILabel>().text = DataMgr.Inst.m_NoticeList[i]._Title;
			Slot.GetComponent<UIButtonMessage>().target = gameObject;
            m_List.Add(Slot);
		}

        m_ListParent.GetComponent<UIGrid>().repositionNow = true;
	}

	//상세 내용 팝업 출력.
	void ClickEvent(GameObject obj)
    {
		GameObject pop = Instantiate(m_Popup) as GameObject;
		pop.transform.parent = transform;
		pop.transform.localScale = Vector3.one;
		pop.transform.localPosition = Vector3.zero;
		pop.SendMessage("SetPopup", int.Parse(obj.name), SendMessageOptions.DontRequireReceiver);
	}

	public void Back()
    {
		DataMgr.Inst.SetPageState( DataMgr.emMAIN_MODE.MENU );
	}
}