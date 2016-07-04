using UnityEngine;
using System.Collections;

public class Panel_Event_Popup : MonoBehaviour 
{
	public UILabel LabelTitle;
	public UILabel LabelContent;

	// 이벤트 정보를 받아 팝업에 나타낸다.
	void SetPopup (int idx) 
    {
		LabelTitle.text = DataMgr.Inst.m_NoticeList[idx]._Title;
		Debug.Log(DataMgr.Inst.m_NoticeList[idx]._Content);
        LabelContent.text = DataMgr.Inst.m_NoticeList[idx]._Content;
	//	StartCoroutine(DownloadImage(DataMgr.Inst.m_NoticeData [idx]._Content));
	}

    /*
	IEnumerator DownloadImage(string url)
    {
		WWW downloader = new WWW (url);

		yield return downloader;

		if(downloader.error == null && downloader.texture != null)
        {
			EventImage.mainTexture = downloader.texture;
			EventImage.MakePixelPerfect();
		}
	}*/

	void ClickClose () 
    {
		Destroy(gameObject);
	}
}
