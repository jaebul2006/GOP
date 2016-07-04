using UnityEngine;
using System.Collections;
using System;

public partial class GameMgr : MonoBehaviour 
{
//  public GameObject[]     m_Slot;    
//  private UITexture[]     m_Summon;
//  public Hpbar            m_HpMgr;
    
    public void PlayerDamageEff(int a_MonIdx)
    {
        if(m_DamageEffect.Length  <= a_MonIdx) return;
        m_DamageEffect[a_MonIdx].Begin();
    }

        
    void PlayerAttack( int a_TeamSum, int a_MonIdx, int a_BoolCount)
    {
        // DataMgr.Inst.m_UserInfo.Team[DataMgr.Inst.m_TeamIdx][i]
        // Ball_A(RED), Ball_A(Blue), Ball_A(Gold), Ball_A(Dark)        발사체.    
        // Hit_fire_ATK, Hit_Ice_ATK, Hit_Light_ATK, Hit_Dark_ATK       1단공격.
        // Hit_fire_ATK1, Hit_Ice_ATK1, Hit_Light_ATK1, Hit_Dark_ATK1   2단공격.
        // Hit_fire_ATK2, Hit_Ice_ATK2, Hit_Light_ATK2, Hit_Dark_ATK2   3단공격.
        // HP_Effects                                                   HP 회복.
        
        if(m_SumSlot[a_TeamSum].gameObject.activeSelf == false) return;
        if(m_Monster[a_MonIdx].GetIsAttack() ) return;

        m_SumSlot[a_TeamSum].SlotFireEff();

        // 발사 위치.
        Vector2 FirPos = new Vector2();        
        FirPos.x = m_SumAtkPoint[a_TeamSum].transform.localPosition.x;
        FirPos.y = m_SumAtkPoint[a_TeamSum].transform.localPosition.y;                 
        
        // 도착위치.
        //Vector2 MonPos = m_MonAtkPoint[a_MonIdx].transform.localPosition;
        Vector2 MonPos = m_Monster[a_MonIdx].m_MonCenterPoint;                

        m_EffectBulletCount++;
        EffectBullet.Fire(m_UIRoot, FirPos, MonPos,
            m_SumSlot[a_TeamSum].m_Info.Data.eAttr,
            a_BoolCount, 
            a_TeamSum, 
            a_MonIdx,
            PlayerAttackEff_Boom
            );
    }   

    private int m_EffectBulletCount = 0;
    void PlayerAttackEff_Boom(int a_TeamSum, int a_MonIdx, int a_BoolCount)
    {
        if(m_Timer.GetIsTimeOn()==false) m_TrunDrag++;
        m_Monster[a_MonIdx].Action_Damage( MonsterDamageEnd, m_SumSlot[a_TeamSum].m_Info, a_BoolCount);        
    }

    void MonsterDamageEnd()
    { m_EffectBulletCount--; }

    // 스킬 시전.
    public void onBtn_SKill1() { SkillCasting(0); }
    public void onBtn_SKill2() { SkillCasting(1); }
    public void onBtn_SKill3() { SkillCasting(2); }
    public void onBtn_SKill4() { SkillCasting(3); }

    void SkillCasting(int a_Index)
    {        
        if( m_GameStap == GameState.GamIng && 
            m_Turn == Turn.Player )
        {            
            Delegate_ClickBall(a_Index);
            m_SumSlot[a_Index].onBtnSkill();            
        }        
    }

    public void AlertSkill(int a_TeamSum)
    {        
        m_SumSlot[a_TeamSum].m_isSkilling = false;
        if(m_SumSlot[a_TeamSum].m_Skill.eEffect == DataMgr.emSKILL_TYPE.ATK)
        {
            int nTarketIdx = SkillBrain(a_TeamSum);
            if(nTarketIdx == -1) return;
            SkillAtk(a_TeamSum, nTarketIdx, false);
        }
        else if(m_SumSlot[a_TeamSum].m_Skill.eEffect == DataMgr.emSKILL_TYPE.AtkAll)
        {
            for (int i=0; i<m_Monster.Length; i++)
            {
                if(m_Monster[i] == null) continue;
                SkillAtk(a_TeamSum, i, true);
            }
        }
        else if(m_SumSlot[a_TeamSum].m_Skill.eEffect == DataMgr.emSKILL_TYPE.BallChange)
        {            
            m_PuzzleMgr.ChangeBallCount( a_TeamSum, m_SumSlot[a_TeamSum].m_Skill.nEffectValue );            
        }
        else if(m_SumSlot[a_TeamSum].m_Skill.eEffect == DataMgr.emSKILL_TYPE.Heal)
        {
            // 발사.
            GameObject obj = (GameObject)Instantiate( (GameObject)Resources.Load("Effects/HP_Skill") );
            obj.transform.parent = m_UIRoot.transform;
            obj.gameObject.name = "Hp";  
            obj.transform.localPosition = new Vector3(
                m_HpEffPos.x, 
                m_HpEffPos.y, -0.05f);
                          
            // 힐링.
            float fLastDamage = m_SumSlot[a_TeamSum].m_Info.GetAttack() * (float)(m_SumSlot[a_TeamSum].m_Skill.nEffectValue/100f);
            m_DamageManager.DuplicateHeal(m_HpMgr.gameObject, ((int)fLastDamage).ToString() );
            m_HpMgr.Plus( (int)fLastDamage );
        }
    }
    
    int SkillBrain(int a_TeamSum)
    {        
        m_TempMonAtkList.Clear();
        int Stap1 = 0; // 0:속성, 1:레벨, 3:체력
        int rand = UnityEngine.Random.Range(0, 10000);
        Debug.Log("랜덤수:"+rand);
        if(rand < 5000) Stap1 = 0;
        else if(rand < 8000) Stap1 = 1;
        else Stap1 = 2;

        int nSelectMonster = -1;

        if(Stap1 == 0)
        {
            // 속성 검색.         
            for (int i=0; i<m_Monster.Length; i++) 
            {
                if(m_Monster[i] == null) continue;
                if(m_Monster[i].GetIsNotAtackState()) continue;
                float fCon = DataMgr.Inst.GetAtti_DamageCon(m_SumSlot[a_TeamSum].m_Info.Data.eAttr ,m_Monster[i].m_Info.eAttr );

                if (fCon > 1.4f)                
                    m_TempMonAtkList.Add(i);                
            }

            if(m_TempMonAtkList.Count > 0)
            {                
                nSelectMonster = m_TempMonAtkList[UnityEngine.Random.Range(0, m_TempMonAtkList.Count)];
            }
        }
        
        if(Stap1 == 1)
        {
            // 가장 낫은 레벨 검색.
            int LowLevel = int.MaxValue;
            int nMonCount = -1;// 나중에 -1하느니 지금 -1부터시작.
            for (int i=0; i<m_Monster.Length; i++) 
            {
                if( m_Monster[i] == null) continue;
                if(m_Monster[i].GetIsNotAtackState()) continue;
                if( m_Monster[i].m_Info.nLevel < LowLevel)
                {
                    LowLevel = m_Monster[i].m_Info.nLevel;
                    nSelectMonster = i;
                }
                else if(LowLevel == m_Monster[i].m_Info.nLevel)
                {
                    m_TempMonAtkList.Add(i);
                }                
                nMonCount++;
            }

            if(m_TempMonAtkList.Count == nMonCount &&  m_TempMonAtkList.Count > 0)
            {
                nSelectMonster = m_TempMonAtkList[UnityEngine.Random.Range(0, m_TempMonAtkList.Count)];
            }

        }

        if(Stap1 == 2)
        { 
            // 체력이 낯은 몬스터 검색.
            int LowHP = int.MaxValue;
            int nMonCount = -1;// 나중에 -1하느니 지금 -1부터시작.
            for (int i=0; i<m_Monster.Length; i++) 
            {
                if( m_Monster[i] == null ) continue;
                if(m_Monster[i].GetIsNotAtackState()) continue;
                if( m_Monster[i].m_Info.nHp < LowHP )
                {
                    LowHP = m_Monster[i].m_Info.nHp;
                    nSelectMonster = i;
                }
                else if(LowHP == m_Monster[i].m_Info.nHp)
                {
                    m_TempMonAtkList.Add(i);
                }                
                nMonCount++;
            }

            if(m_TempMonAtkList.Count == nMonCount &&  m_TempMonAtkList.Count > 0)
            {
                nSelectMonster = m_TempMonAtkList[UnityEngine.Random.Range(0, m_TempMonAtkList.Count)];
            }
        }

        if(nSelectMonster == -1)
        {            
            // 그냥 랜덤.
            m_TempMonAtkList.Clear();
            for (int i=0; i<m_Monster.Length; i++) 
            {
                if( m_Monster[i] == null ) continue;
                if(m_Monster[i].GetIsNotAtackState()) continue;
                
                m_TempMonAtkList.Add(i);                
            }
             
            if(m_TempMonAtkList.Count > 0)
                nSelectMonster = m_TempMonAtkList[UnityEngine.Random.Range(0, m_TempMonAtkList.Count)];
        }        

        return nSelectMonster;
    }

    void SkillAtk( int a_TeamSum, int a_MonIdx, bool a_bAtkAll )
    {
        if(m_SumSlot[a_TeamSum].gameObject.activeSelf == false) return;        
        //m_SumSlot[a_TeamSum].SlotFireEff();.

        // 발사 위치.
        Vector2 FirPos = new Vector2();        
        FirPos.x = m_SumAtkPoint[a_TeamSum].transform.localPosition.x;
        FirPos.y = m_SumAtkPoint[a_TeamSum].transform.localPosition.y;                 
        
        // 도착위치.
        //Vector2 MonPos = m_MonAtkPoint[a_MonIdx].transform.localPosition;.
        Vector2 MonPos = m_Monster[a_MonIdx].m_MonCenterPoint;
        
        m_EffectBulletCount++;
        EffectBullet.Skill(m_UIRoot, FirPos, MonPos,
            m_SumSlot[a_TeamSum].m_Info.Data.eAttr,
            m_SumSlot[a_TeamSum].m_Skill.nEffectValue,
            a_TeamSum,
            a_MonIdx,
            PlayerSkillEff_Boom,
            a_bAtkAll
            );
    }

    void PlayerSkillEff_Boom(int a_TeamSum, int a_MonIdx, int a_Skilldamage)
    {
        m_Monster[a_MonIdx].Skill_Damage( MonsterDamageEnd, m_SumSlot[a_TeamSum].m_Info, a_Skilldamage);
    }
}