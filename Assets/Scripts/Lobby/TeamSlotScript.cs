using UnityEngine;
using System;
//using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/*
카드 클래스 .
프리팹 Card에 적용된 클래스.
소환수의 공격력,방어력 등급 레벨 강화수치 이름 등을 보여줌 .
3D케릭터 보여줌.

3D 케릭터는 Resources/3D폴더에서 Type_케릭터번호 로 불러옴.  data 변수 안에 케릭터의 모든 데이터가 담겨있음.
*/

public class TeamSlotScript : MonoBehaviour 
{
	public UILabel 		charName;
	public UILabel 		level;
	public UILabel 		upgrade;
	public UISprite 	attr;
	public UISprite		BG;
	public UISprite 	grade;
	public UITexture 	thumb;	
	public GameObject 	isSelected;
	public UISprite	    getNew;

    private DataMgr.tagUserSummon m_varCharData;
    public DataMgr.tagUserSummon m_CharData
    {
        get
        {
            return m_varCharData;
        }
    }

    // Use this for initialization.
    void Awake()
	{
		setEmpty();
	}
	
	void Start () 
	{
	
	}
	
	void Update () 
	{
		
	}

	// 빈 카드로 세팅  팀설정에서 빈 자리일경우.
	void setEmpty()
	{
		charName.text 	= "";
		level.text 		= "";
		
		grade.enabled 	= false;
		upgrade.text 	= "";	
		attr.enabled 	= false;
		
		isSelected.SetActive(false);
		if(getNew != null) getNew.enabled = false;
		
		thumb.enabled = false;
		BG.spriteName = "img_character_back";
        m_varCharData = null;
	}	    
    
    // often change data update.
    public void setReflash()
    {
        if(m_varCharData == null ) return;
        
        level.text = "LV. " + m_varCharData.nLevel;
        grade.spriteName = "icon_grade_"+ m_varCharData.nGrade.ToString("00");
        if(m_varCharData.nUpgrade > 0) upgrade.text = "+"+ m_varCharData.nUpgrade;
		else upgrade.text = "";
    }
    


	//팀 캐릭터로 설정한다.
	void setChar(DataMgr.tagUserSummon a_CharData)
	{
		attr.enabled 	= true;	// 속성 이미지 보이기.
		grade.enabled 	= true; // 등급 이미지 보이기.
        m_varCharData = a_CharData;
		
		// 속성 세팅
		string 	sName = "None";
		string 	bName = "None";
        		     		     
		     if(a_CharData.Data.eAttr == DataMgr.emCardAttribute.emCAB_WA)  { sName = "1"; bName = "_fire"; }
		else if(a_CharData.Data.eAttr == DataMgr.emCardAttribute.emCAB_SU)  { sName = "2"; bName = "_water"; }
		else if(a_CharData.Data.eAttr == DataMgr.emCardAttribute.emCAB_YUNG){ sName = "3"; bName = "_light"; }
		else if(a_CharData.Data.eAttr == DataMgr.emCardAttribute.emCAB_ARM) { sName = "4"; bName = "_dark"; }

		// 2D 케릭터 이미지 로드.
		thumb.enabled = true;		
		thumb.mainTexture = Resources.Load("Textures/Char/icon_mon_" + a_CharData.Data.strResurceID) as Texture;
		//thumb.MakePixelPerfect();

		charName.text 	= DataMgr.Inst.GetLocal( a_CharData.Data.strName );
		level.text 		= "LV. " + a_CharData.nLevel;
		
		grade.spriteName = "icon_grade_"+a_CharData.nGrade.ToString("00");
		if(a_CharData.nUpgrade > 0) upgrade.text = "+" + a_CharData.nUpgrade;
		else upgrade.text = "";

		if(sName == "None") attr.enabled = false;
		else attr.spriteName = "icon_attr_" + sName;

		BG.spriteName = "img_character_back" + bName;

		isSelected.SetActive(false);

		// 리스트중 팀에 설정되어 있는지 체크.
		int index = a_CharData.nKey;        

        for (int i=0; i<Defines.DEF_MAX_TEAM; i++)
        {
			for(int j=0; j<Defines.DEF_MAX_TEAM_ITEM; j++)
            {
				if(index == DataMgr.Inst.GetTempTeam(i, j) )
				{
					if(i == DataMgr.Inst.m_TeamIdx)
                    {
						equiped();
						return;
					}
                    else
                    {
						other_equiped();
					}
				}
			}
		}
	}
	
	///  케릭터 뒤에 라이트 켜짐 여부.
	/////////////////////////////////////////////////	.
	// 팀 교체시 .
	void setTeam()
	{
		isSelected.SetActive(true);
	}

	// 이미 팀에 설정되어 있는 경우.
	void equiped()
	{
		if(getNew == null) return;

		getNew.enabled = true;
		getNew.spriteName = "icon_equip";
		getNew.alpha = 1f;
	}

	// 이미 팀에 설정되어 있는 경우.
	void other_equiped()
	{
		if(getNew == null) return;
		
		getNew.enabled = true;
		getNew.spriteName = "icon_equip";
		getNew.alpha = 0.5f;
	}
	// 리스트에서 선택했을 경우.
	void selected()
	{
		isSelected.SetActive(true);
	}
	// 선택되지 않은 경우.
	void notSelected()
	{
		isSelected.SetActive(false);
	}
}
