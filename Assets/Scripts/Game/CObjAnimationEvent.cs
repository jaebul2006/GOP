using UnityEngine;
using System.Collections;

public class CObjAnimationEvent : MonoBehaviour 
{
	public delegate void Delegate_EndSkill( int a_NumSkill );
	public Delegate_EndSkill m_Delegate_EndSkill = null;

	public delegate void Delegate_Death( );
	public Delegate_Death m_Delegate_Death = null;

	public delegate void Delegate_Damage( );
	public Delegate_Damage m_Delegate_Damage = null;

	public delegate void Delegate_NormalAtteck( );
	public Delegate_NormalAtteck m_Delegate_NormalAtteck = null;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}


	void NormalAtteck()
	{
		if(m_Delegate_NormalAtteck != null) m_Delegate_NormalAtteck();
	}

	//죽었을때 호출하는 함수.
	void Death()
	{
		if(m_Delegate_Death != null) m_Delegate_Death();
	}

	void damage()
	{
		if(m_Delegate_Damage != null) m_Delegate_Damage();
	}

	//스킬쩠을때 호출하는함수.
	void Skill_1()
	{
		if(m_Delegate_EndSkill != null) m_Delegate_EndSkill(0);
	}

	void Skill_2()
	{
		if(m_Delegate_EndSkill != null) m_Delegate_EndSkill(1);		
	}

	void Skill_3()
	{
		if(m_Delegate_EndSkill != null) m_Delegate_EndSkill(2);		
	}

	void Skill_4()
	{
		if(m_Delegate_EndSkill != null) m_Delegate_EndSkill(3);	
	}

	void Skill_5()
	{
		if(m_Delegate_EndSkill != null) m_Delegate_EndSkill(4);
	}

}
