using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallLine : MonoBehaviour 
{
    //[SerializeField]
    public UISprite         m_SprLine;
        
    private GameObject      m_Start;
    private GameObject      m_End;
    


    public void SetUp(GameObject a_Start, GameObject a_End)
    {
        gameObject.transform.localScale    = new Vector3(1.0f, 1.0f, 1.0f);
        m_SprLine.alpha = 1.0f;
        m_Start = a_Start;
        m_End   = a_End;
        m_SprLine.depth = 7;
    }

	// Use this for initialization
	void Start () 
    {
	    
	}
	
	// Update is called once per frame
	void Update () 
    {
        // 각도계산.
        float fDist_l;
        float Radian = (float)BattlePuzzleMgr.getAngle(
            m_Start.transform.localPosition, 
            m_End.transform.localPosition, out fDist_l);
        Radian = Radian/(float)(System.Math.PI/180.0);

        // 거리계산.
        float fDist_w = 0;                                                        
        fDist_w = Vector2.Distance(m_Start.transform.position, m_End.transform.position);

        gameObject.transform.localPosition = m_Start.transform.localPosition;     
        m_SprLine.width = (int)fDist_l;
        gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, Radian);
        
	}
  
}
